using Infrastructure.Models;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.Extensions;
using System.Collections.Generic;
using Infrastructure.Services;

namespace CharGenerator
{
    class Program
    {

        /// <summary>
        /// 1. generate imgages for windows fonts then google ones
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //init google fonts from ttf file

            var googleFontDirs = Directory.GetDirectories(ConfigurationManager.AppSettings["GoogleFontDir"])
                                    .SelectMany(x => Directory.GetFiles(x)
                                    .Where(m => m.Contains(".ttf")))
                                    .ToList();

            var googleFonts = googleFontDirs.Select(x =>
            {
                var tmp = x.Split('\\');
                var fontName = tmp[tmp.Length - 1].Replace(".ttf", "").Trim();
                return new { fontName = fontName, path = x };

            }).ToList();

            //----------
            FontImageExporter exporter = new FontImageExporter();

            Console.WriteLine("export one test image (mostly for testing purpose), or export char level images in various fonts?  (1 / 2) ");
            if (Console.ReadLine().Equals("1"))
            {
                //font name
                Console.WriteLine("which font?");
                var fontName = Console.ReadLine();
                //17 size is good size to produce height of 28
                Console.WriteLine("what is the font size");
                var fontSize = Convert.ToInt32(Console.ReadLine());
                //create fontdata, check against windows/google fonts
                //lets assume that if it is not windows font then it is google font
                var fontData = default(FontData);
                if (FontResource.Fonts_Small.Any(x => x.Equals(fontName)))
                    fontData = new WindowsFont(fontName, fontSize);
                else
                {
                    var tmp = googleFonts.FirstOrDefault(x => x.fontName.Equals(fontName));

                    fontData = new GoogleFont(fontName, fontSize, tmp.path);
                }


                //content
                Console.WriteLine("what is the word you want to generate?");
                var word = Console.ReadLine();

                var strbuilder = new StringBuilder();
                //where the word consists of multi chars, append some space around it
                if (word.Length > 1)
                {
                    strbuilder.AppendLine();
                    strbuilder.Append(' ', 3).Append(word).Append(' ', 3).Append(".");
                    strbuilder.AppendLine();
                    strbuilder.Append(' ', 6).Append(".");
                }
                else
                {
                    strbuilder.Append(word);
                }
                var path = ConfigurationManager.AppSettings["ExportWordDir"];
                Directory.CreateDirectory(path);
                var name = Guid.NewGuid().ToString();

                //resize and save new images
                Console.WriteLine("resize to 28*28?");
                var resize = Convert.ToBoolean(Console.ReadLine());
                if (resize)
                {
                    exporter.SaveOneImage(strbuilder.ToString(), name, fontData, path, new List<ImageDecorator> { new ResizeDecorator(28, 28) });
                }
                else
                {
                    exporter.SaveOneImage(strbuilder.ToString(), name, fontData, path);
                }
            }
            else
            {

                Console.WriteLine("export windows font images or google font images. (1 / 2)");
                var command = Console.ReadLine();
                int fontSize = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultExportFontSize"]);




                var fontDatas = new List<FontData>();
                if (command.Equals("2"))
                {
                    var googleFontDatas = googleFonts.Select(x =>
                    {
                        var e = new GoogleFont(fontName: x.fontName, fontSize: fontSize, fontPath: x.path);
                        return (FontData)e;
                    }).ToList();

                    //google fonts are many, some are too unnormal, manually create a list with all normal font names
                    string[] googleFontNameList = File.ReadAllLines(@"..\..\fonts_small.txt");
                    fontDatas = googleFontDatas.Where(x => googleFontNameList.Contains(x.FontName)).ToList();
                }
                else if (command.Equals("1"))
                {

                    fontDatas = FontResource.Fonts_Small.Select(x =>
                    {
                        var font = new WindowsFont(x, fontSize);
                        return (FontData)font;
                    }).ToList();

                }



                //---------------auto adjust offset



                //X,Y - 3,6 for font 40 is good, remove some offset manually
                Console.WriteLine("remove white pixel as offsets? (true/false)");
                var removeOffset = Convert.ToBoolean(Console.ReadLine());

                var offsetRemoveOption = -1;
                var offsetX = 0;
                var offsetY = 0;
                if (removeOffset)
                {
                    Console.WriteLine("specify offset removal option - remove offsets (0) / even and remove offsets (1) / even and keep offset remains (2)? (0,1,2)");
                    offsetRemoveOption = Convert.ToInt32(Console.ReadLine());


                    Console.WriteLine("specify offsets - 'X,Y'");
                    var offsets = Console.ReadLine();
                    offsetX = Convert.ToInt32(offsets.Split(',')[0]);
                    offsetY = Convert.ToInt32(offsets.Split(',')[1]);



                }

                var decorators = new List<ImageDecorator>();

                if (removeOffset)
                {
                    if (offsetRemoveOption == 0)
                    {
                        decorators.Add(new PeelDecorator(offsetX, offsetX, offsetY, offsetY));

                    }
                    else if (offsetRemoveOption == 1)
                    {
                        decorators.Add(new EvenAndPeelDecorator(offsetX, offsetX, offsetY, offsetY));
                    }
                    else if (offsetRemoveOption == 2)
                    {
                        decorators.Add(new AutoPeelDecorator(offsetX, offsetX, offsetY, offsetY));
                    }
                }
                exporter.ExportFontData(fontDatas, decorators);
            }

            Console.WriteLine("END");

            //following code not used in the main program, some of the google fonts are bad, manually remove them, keep a list of google fonts that are good
            //System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\googlefonts.txt");

            //var allfonts = Directory.GetDirectories(@"D:\Test\FontData_After_Filter_stag");
            //allfonts.ToList().ForEach(x => {
            //    var dir = new DirectoryInfo(x);
            //    var dirName = dir.Name;
            //    file.WriteLine(dirName);
            //});
            //file.Close();
        }
    }
}
