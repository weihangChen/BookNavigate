using Infrastructure.Models;
using Infrastructure.Services;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CharGenerator
{
    class Program
    {
        
        /// <summary>
        /// in total windows + google font 1951
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            FontImageExporter exporter = new FontImageExporter();

            Console.WriteLine("export one test image in font 'Arial' (mostly for testing purpose), or export char level images in various fonts?  (1 / 2) ");
            if (Console.ReadLine().Equals("1"))
            {
                Console.WriteLine("what is the word you want to generate?");
                var word = Console.ReadLine();
                //always append empty lines for testing purpose
                var strbuilder = new StringBuilder();
                strbuilder.AppendLine();
                strbuilder.Append(' ', 3).Append(word).Append(' ', 3).Append(".");
                strbuilder.AppendLine();
                strbuilder.Append(' ', 6).Append(".");

                //17 size is good size to produce height of 28
                Console.WriteLine("what is the font size");
                var fontSize = Convert.ToInt32(Console.ReadLine());
                var path = ConfigurationManager.AppSettings["ExportWordDir"];
                Directory.CreateDirectory(path);
                exporter.SaveOneImage(strbuilder.ToString(), Guid.NewGuid().ToString(), new WindowsFont("Arial", fontSize), path);


            }
            else
            {

                Console.WriteLine("export 14 windows font or 1900 google font. '1' or '2'");
                var command = Console.ReadLine();
                int fontSize = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultExportFontSize"]);
                if (command.Equals("2"))
                {
                    var googleFontDir = ConfigurationManager.AppSettings["GoogleFontDir"];
                    var googleFontDirs = Directory.GetDirectories(googleFontDir)
                                            .SelectMany(x => Directory.GetFiles(x)
                                            .Where(m => m.Contains(".ttf")))
                                            .ToList();
                    var googleFontDatas = googleFontDirs.Select(x =>
                    {
                        var tmp = x.Split('\\');
                        var fontName = tmp[tmp.Length - 1].Replace(".ttf", "").Trim();
                        var e = new GoogleFont(fontName: fontName, fontSize: fontSize, fontPath: x);
                        return (FontData)e;
                    }).ToList();

                    exporter.ExportFontData(googleFontDatas);
                }
                else if (command.Equals("1"))
                {

                    var windowsFont = FontResource.Fonts_Small.Select(x =>
                    {
                        var font = new WindowsFont(x, fontSize);
                        return (FontData)font;
                    }).ToList();
                    exporter.ExportFontData(windowsFont);
                }
            }

            Console.WriteLine("END");

        }
    }
}
