using Infrastructure.Models;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

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

            Console.WriteLine("END");

        }
    }
}
