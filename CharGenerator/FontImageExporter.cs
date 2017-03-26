using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Configuration;
using Infrastructure.SingleImageProcessing;

namespace CharGenerator
{
    /// <summary>
    /// contains the core functions generating images using windows font data
    /// </summary>
    public class FontImageExporter
    {
        private ImageProcessor ImageProcessor;

        public TextDrawer Drawer { get; set; }
        public static string BigString = ConfigurationManager.AppSettings["GeneratedChars"];
        public FontImageExporter()
        {
            ImageProcessor = new ImageProcessor();
            Drawer = new TextDrawer();
        }





        /// <summary>
        /// load bitmap by using drawstring + windows font, this method create one folder for each digit, populate each folder with digit images in multiple fonts
        /// </summary>
        /// <param name="ExportDirectoryInfo"></param>
        /// <param name="BigString">ex. abcdef, then 6 folders will be created</param>
        /// <param name="Fonts">the extracted fonts</param>
        /// <param name="saveImage">loaded images saved to hard disk</param>
        /// <param name="fontSize">by default 50, find it as a fit for identification</param>
        /// <returns></returns>
        public virtual TrainingDataDTOWindowsFont ExportFontImages(
            List<string> fonts,
            List<IImageDecorator> imageDecorators,
            int fontSize = 50)
        {
            var dto = new TrainingDataDTOWindowsFont();
            var imageDatas = dto.ImageDatas;
            //process each font
            foreach (var f in fonts)
            {
                var font = new Font(f, fontSize);
                //go through all letters
                BigString.ToList().ForEach(letter =>
                {
                    var txt = letter.ToString();

                    var image = new ImageData { Text = txt };
                    image.bitmap = Drawer.DrawTextOnImage(txt, font, Color.Black, Color.White) as Bitmap;

                    imageDecorators.ForEach(x => x.ProcessImage(image));

                    imageDatas.Add(image);

                });
            }

            return dto;
        }






    }

}
