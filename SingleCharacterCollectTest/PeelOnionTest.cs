using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.Models;
using CharGenerator;
using Infrastructure.Extensions;
using System.Drawing;

namespace SingleCharacterCollectTest
{
    //remove the offset around a char and see if classification rate increases
    [TestClass]
    public class PeelOnionTest
    {
        [TestMethod]
        public void PeelTestOnRealImage()
        {
            //prepare
            var fileName = "a";
            var path = "../../Data";
            var path1 = "../../Data/a.jpg";
            var path2 = "../../Data/a1.jpg";

            var font = new WindowsFont("Arial", 40);
            var service = new FontImageExporter();
            service.SaveOneImage("a", fileName, font, path);
            //peel the offset whitespace
            var bitmap = Image.FromFile(path1) as Bitmap;
            var features = bitmap.ConvertImageToTwoDimensionArray();
            var features_new = features.PeelOffset(7, 12);
            var bitmap_new = features_new.ConvertTwoDimensionArrayToImage();
            bitmap_new.Save(path2);
        }


        //two dimention matrix , check if converted matrix data is correct
        [TestMethod]
        public void PeelTest()
        {
            int[][] features = new int[4][];
            for (int i = 0; i < features.Length; i++)
            {
                //dark pixel has value 1, therefore do not populate matrix with 1
                features[i] = new int[4] { (i + 1) * 10 + 1, (i + 1) * 10 + 2, (i + 1) * 10 + 3, (i + 1) * 10 + 4 };
            }
            var features_new = features.PeelOffset(1, 0);
            Assert.IsTrue(features_new[0][0] == 21);
            Assert.IsTrue(features_new[0][1] == 22);
            Assert.IsTrue(features_new[0][2] == 23);
            Assert.IsTrue(features_new[0][3] == 24);
            Assert.IsTrue(features_new[1][0] == 31);
            Assert.IsTrue(features_new[1][1] == 32);
            Assert.IsTrue(features_new[1][2] == 33);
            Assert.IsTrue(features_new[1][3] == 34);

            var features_new1 = features.PeelOffset(0, 1);
            Assert.IsTrue(features_new1[0][0] == 12);
            Assert.IsTrue(features_new1[0][1] == 13);
            Assert.IsTrue(features_new1[1][0] == 22);
            Assert.IsTrue(features_new1[1][1] == 23);
            Assert.IsTrue(features_new1[2][0] == 32);
            Assert.IsTrue(features_new1[2][1] == 33);
            Assert.IsTrue(features_new1[3][0] == 42);
            Assert.IsTrue(features_new1[3][1] == 43);


        }
    }
}
