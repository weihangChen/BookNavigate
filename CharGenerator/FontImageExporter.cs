using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Configuration;
using System.IO;
using Infrastructure.Extensions;

namespace CharGenerator
{

    public class FontImageExporter
    {

        public static DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["ExportDir"]);
        public static string BigString = ConfigurationManager.AppSettings["GeneratedChars"];


        public void ExportFontData(List<FontData> fontDatas, bool removeOffset, int offsetX, int offsetY)
        {
            var decorators = new List<ImageDecorator>();
            if (removeOffset)
            {
                decorators.Add(new PeelDecorator(offsetX, offsetY));
            }
            foreach (var fontdata in fontDatas)
            {
                var imagePath = Path.Combine(dir.ToString(), fontdata.FontName);
                Directory.CreateDirectory(imagePath);


                BigString.ToList().ForEach(letter =>
                {
                    var txt = letter.ToString();
                    var fileName = StringResources.LetterMapping[txt];
                    SaveOneImage(txt, fileName, fontdata, imagePath, decorators);

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
                    foreach(var dec in imgDecorators)
                    {
                        image.bitmap = dec.DecorateImage(image.bitmap);
                    }
                }
                    
                //if (resize)
                //{
                //    image.bitmap = image.bitmap.ResizeImage(newwidth, newheight);
                //}

                //if (peelOffset)
                //{
                //    var features = image.bitmap.ConvertImageToTwoDimensionArray();
                //    var features_new = features.PeelOffset(peelOffsetSize);
                //    image.bitmap = features_new.ConvertTwoDimensionArrayToImage();
                //}
                image.bitmap.Save($"{imagePath}/{fileName}.jpg");
            }
        }







    }

}
