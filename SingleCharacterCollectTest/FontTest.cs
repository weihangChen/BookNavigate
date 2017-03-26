using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Infrastructure.Services;
using Infrastructure.Extensions;
using Infrastructure.Models;
using System.IO;
using CharGenerator;

namespace SingleCharacterCollectTest
{
    public class FontEntity
    {
        public int FontSize { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public FontEntity(int FontSize, int Height, int Width)
        {
            this.FontSize = FontSize;
            this.Height = Height;
            this.Width = Width;
        }

        public override string ToString()
        {
            return FontSize.ToString() + " - " + Height.ToString() + " - " + Width.ToString();
        }
    }

    [TestClass]
    public class FontTest
    {
        /// <summary>
        /// this test does not assert anything, but to write the fontsize paring with pixel height width data to file
        /// </summary>
        //[TestMethod]
        //public void CroppedToEdgeCharSizeMeasureTest()
        //{
        //    var path = "../../Files/FontCharSizeData.txt";
        //    var fontEntities = new List<FontEntity>();
        //    //var allFontSizes = new List<int> { 14, 16, 18, 20, 22, 24, 26, 28 };
        //    var allFontSizes = new List<int> { 16 };
        //    var textDrawer = new TextDrawer();
        //    var imageProcessor = new ImageProcessor();
        //    int maxH = 0;
        //    int maxW = 0;


        //    allFontSizes.ForEach(fontSize =>
        //    {
        //        var croppedImages = new List<Bitmap>();
        //        foreach (var fontName in FontResource.Fonts_Small)
        //        {
        //            var font = new Font(fontName, fontSize);
        //            //go through all letters

        //            StringResource.AllString_OperatorIncluded.ToList().ForEach(letter =>
        //            {
        //                //draw image
        //                var txt = letter.ToString();
        //                var bmp = (Bitmap)textDrawer.DrawTextOnImage(txt, font, Color.Black, Color.White);
        //                var bmpCrop = imageProcessor.CropImageToEdge(bmp, txt);
        //                //bmp.Save(SimpleHelper.GetPathForImage());
        //                bmpCrop.Save(SimpleHelper.GetPathForImage());

        //                croppedImages.Add(bmpCrop);
        //                //CropEdgeAssert(bmp, bmpCrop, fontName, txt);

        //            });
        //        }

        //        maxH = croppedImages.Max(x => x.Height);
        //        maxW = croppedImages.Max(x => x.Width);
        //        fontEntities.Add(new FontEntity(fontSize, maxH, maxW));
        //    });

        //    var fService = new FileService();
        //    fontEntities.ForEach(x => fService.WriteToFile(x.ToString(), path));
        //}

        //when CropToEdgeMultiTest fails during the loop, write down the font name and character down, run this test instead
        //the fontName and letter need to be filled

        //not used
        [TestMethod]
        public void CropToEdgeSingleTest()
        {
            var fontName = "Adobe Devanagari";
            var letter = "7";
            int fontSize = 50;
            var textDrawer = new TextDrawer();
            var font = new Font(fontName, fontSize);

            //draw image
            var bmp = (Bitmap)textDrawer.DrawTextOnImage(letter, font, Color.Black, Color.White);
            //crop to edge - the main method under test here 
            var bmpCrop = bmp.CropImageToEdge(letter);
            bmp.Save(Guid.NewGuid()+".jpg");
            bmpCrop.Save(Guid.NewGuid() + ".jpg");

            CropEdgeAssert(bmp, bmpCrop, fontName, letter, true);

        }

        //protected TrainingDataDTOWindowsFont LoadFontImages(string BigString)
        //{
        //    var exporter = new FontImageExporter(BigString);
        //    var config = new GraphicConfig();
        //    var dir = new DirectoryInfo(@"..\..\Files\image\font\all");
        //    //var dto = exporter.ExportFontImages(dir, FontResource.Fonts_All, true, fontSize: FontResource.HoneyFontSize, CropStrategy: CropStrategy.CropWidthOnly, resize: true, resizeWidth: FontResource.HoneyWidth, resizeHeight: FontResource.HoneyHeight);


        //    //var dto = exporter.ExportFontImages(dir, FontResource.Fonts_All, saveImage: true, fontSize: FontResource.HoneyFontSize, CropStrategy: CropStrategy.CropToEdge, resize: true, resizeWidth: FontResource.HoneyWidth, resizeHeight: FontResource.HoneyHeight, NormalizeImages: false);

        //    var dto = exporter.ExportFontImages(dir, FontResource.Fonts_Small, saveImage: true, fontSize: FontResource.HoneyFontSize, CropStrategy: CropStrategy.CropToEdge, resize: false, resizeWidth: FontResource.HoneyWidth, resizeHeight: FontResource.HoneyHeight, NormalizeImages: false);


        //    return dto;
        //}






        protected void CropEdgeAssert(Bitmap origin, Bitmap cropped, string fontname = "", string txt = "", bool saveImage = false)
        {

            //if (saveImage)
            //{
            //    origin.Save(SimpleHelper.GetPathForImage());
            //    cropped.Save(SimpleHelper.GetPathForImage());
            //}

            var whitePixels = new List<Color>();
            var diffY = origin.Height - cropped.Height;
            var diffX = origin.Width - cropped.Width;

            //todo this is wrong, prime number /2 what about remainder
            var minX = diffX / 2;
            var maxX = cropped.Width + diffX / 2 - 1;

            var minY = diffY / 2;
            var maxY = cropped.Height + diffY / 2 - 1;


            //be sure that everything that get cropped are white
            for (int i = 0; i < origin.Height; i++)
            {
                for (int j = 0; j < origin.Width; j++)
                {
                    var pixel = origin.GetPixel(j, i);
                    if (ShouldbeWhiteX(i, minY, maxY) && ShouldbeWhiteX(j, minX, maxX))
                        whitePixels.Add(pixel);
                }
                //imageRawData.Add(i, rowRawData);
            }
            //r:0 g:0 b:0 -- pure black (all values 0)
            //r:255 g:255 b:255 -- pure white (all values 255)
            var anynonwhite = whitePixels.Any(x => x.R != 255);
            var error = string.Format("all pixel should be white for {0} - {1}", fontname, txt);


            if (anynonwhite == true)
            {
                var nonwhiteData = whitePixels.Where(x => x.R != 255).ToList();
                //save images somewhere to take a look 


            }

            //Assert.IsTrue(anynonwhite == false, error);

        }
        protected bool ShouldbeWhiteX(int position, int min, int max)
        {
            if (position >= min && position <= max)
                return false;
            return true;
        }




        [TestMethod]
        public void ShouldbeWhiteXTest()
        {
            int min = 3;
            int max = 6;
            Assert.IsTrue(ShouldbeWhiteX(0, min, max) == true);
            Assert.IsTrue(ShouldbeWhiteX(2, min, max) == true);
            Assert.IsTrue(ShouldbeWhiteX(3, min, max) == false);
            Assert.IsTrue(ShouldbeWhiteX(6, min, max) == false);
            Assert.IsTrue(ShouldbeWhiteX(7, min, max) == true);


        }






    }
}
