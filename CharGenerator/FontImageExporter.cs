using System.Collections.Generic;
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
                var tmp = Path.Combine(dir.ToString(), fontdata.FontName);
                Directory.CreateDirectory(tmp);


                BigString.ToList().ForEach(letter =>
                {
                    var txt = letter.ToString();
                    var fileName = StringResources.LetterMapping[txt];
                    var image = new ImageData { Text = txt };
                    var drawer = new TextDrawer();
                    var font = fontdata.GetFont();
                    using (image.bitmap = drawer.DrawTextOnImage(txt, font, Color.Black, Color.White) as Bitmap)
                    {
                        image.bitmap.Save($"{tmp}/{fileName}.jpg");
                    }

                });

            }
        }






    }

}
