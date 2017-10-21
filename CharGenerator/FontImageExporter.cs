using System.Collections.Generic;
using System.Drawing;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Configuration;
using System.IO;

namespace CharGenerator
{

    public class FontImageExporter
    {
        public static DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["ExportDir"]);
        public static string BigString = ConfigurationManager.AppSettings["GeneratedChars"];
        public static string _twoLetters = ConfigurationManager.AppSettings["TwoLetters"];

        public void ExportFontData(List<FontData> fontDatas, List<ImageDecorator> decorators)
        {
            LabelConfig labelConfig = null;
            if (_twoLetters.Equals("1"))
            {
                labelConfig = new TwoLetterConfig();
            }
            else
            {
                labelConfig = new SingleLetterConfig(BigString);
            }

            foreach (var fontdata in fontDatas)
            {
                var imagePath = Path.Combine(dir.ToString(), fontdata.FontName);
                Directory.CreateDirectory(imagePath);
                labelConfig.LabelDatas.ForEach(ld =>
                {
                    SaveOneImage(ld.Label, ld.LabelAsFolderName, fontdata, imagePath, decorators);
                });
            }
        }

        public void SaveOneImage(string txt, string fileName, FontData fontdata, string imagePath, List<ImageDecorator> imgDecorators = null)
        {
            var image = new ImageData { Text = txt };
            var drawer = new TextDrawer();
            var font = fontdata.GetFont();
            using (image.bitmap = drawer.DrawTextOnImage(txt, font, Color.Black, Color.White) as Bitmap)
            {
                if (imgDecorators != null)
                {
                    foreach (var dec in imgDecorators)
                    {
                        image.bitmap = dec.DecorateImage(image.bitmap);
                    }
                }
                image.bitmap.Save($"{imagePath}/{fileName}.jpg");
            }
        }

    }

}
