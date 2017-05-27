﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        
        public void ExportFontData(List<FontData> fontDatas)
        {
            foreach (var fontdata in fontDatas)
            {
                var imagePath = Path.Combine(dir.ToString(), fontdata.FontName);
                Directory.CreateDirectory(imagePath);


                BigString.ToList().ForEach(letter =>
                {
                    var txt = letter.ToString();
                    var fileName = StringResources.LetterMapping[txt];
                    SaveOneImage(txt, fileName, fontdata, imagePath);
                    
                });

            }
        }

        public void SaveOneImage(string txt, string fileName, FontData fontdata, string imagePath)
        {
            var image = new ImageData { Text = txt };
            var drawer = new TextDrawer();
            var font = fontdata.GetFont();
            using (image.bitmap = drawer.DrawTextOnImage(txt, font, Color.Black, Color.White) as Bitmap)
            //using (image.bitmap = drawer.DrawTextOnImageNoOffset(txt, font, Color.Black, Color.White) as Bitmap)
            {
                image.bitmap.Save($"{imagePath}/{fileName}.jpg");
            }
        }






    }

}
