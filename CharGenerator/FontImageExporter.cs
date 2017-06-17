using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Configuration;
using System.IO;
using System;

namespace CharGenerator
{

    public class FontImageExporter
    {

        public static DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["ExportDir"]);
        public static string BigString = ConfigurationManager.AppSettings["GeneratedChars"];


        public void ExportFontData(List<FontData> fontDatas, List<ImageDecorator> decorators)
        {
            foreach (var fontdata in fontDatas)
            {
                var imagePath = Path.Combine(dir.ToString(), fontdata.FontName);
                Directory.CreateDirectory(imagePath);


                BigString.ToList().ForEach(letter =>
                {
                    try
                    {
                        var txt = letter.ToString();
                        var fileName = StringResources.LetterMapping[txt];
                        SaveOneImage(txt, fileName, fontdata, imagePath, decorators);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                });

            }
        }

        //public void SaveOneImage(string txt, string fileName, FontData fontdata, string imagePath, bool resize = false, int newwidth = 0, int newheight = 0, bool peelOffset = false, int peelOffsetSize = 0)
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
