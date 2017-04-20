using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Drawing;
using Infrastructure.Services;
using System.Collections.Generic;
using Infrastructure.Extensions;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Models;

namespace SingleCharacterCollectTest
{
    [TestClass]
    public class ByteDateTest
    {
        /// <summary>
        /// create a file, write integers to the file, save it, read it, check the binary data
        /// </summary>
        /// 
        [TestMethod]
        public void Test1()
        {
            File.Delete("test");
            var writer = new BinaryWriter(new FileStream("test", FileMode.CreateNew));

            writer.Write(1);
            writer.Write(9);

            writer.Flush();
            writer.Close();

            //verify total bytes
            var info = new FileInfo("test");
            //one integer is 4 bytes
            Assert.IsTrue(info.Length == 8);
            //convert the bytes back to integer see if it is 1
            var reader = new BinaryReader(new FileStream("test", FileMode.Open));
            var bytes = reader.ReadBytes(4);
            var digit = BitConverter.ToInt32(bytes, 0);
            Assert.IsTrue(digit == 1);

            var bytes1 = reader.ReadBytes(4);
            var digit1 = BitConverter.ToInt32(bytes1, 0);
            Assert.IsTrue(digit1 == 9);
        }



        /// <summary>
        /// the MNIST dataset is 100% correct, but since we are going to use sythentic data, and we wish to save the synthetic data in the same way as 
        /// MNIST, therefore, it is good to verify things like 
        /// - how many bytes it is
        /// - big littel endianess
        /// - should image bytes be saved row-by-row or column-to-column 
        /// </summary>
        [TestMethod]
        public void MNISTDataAssert()
        {
            var labelPath = "../../Data/train-labels-idx1-ubyte";

            var labels = new List<int>();

            try
            {
                using (var stream = new FileStream(labelPath, FileMode.Open, FileAccess.Read))
                {
                    Assert.IsTrue(stream.Length == 60008);
                    using (var reader = new BinaryReader(stream))
                    {
                        var magic = reader.ReadInt32BigEndian();
                        var num = reader.ReadInt32BigEndian();
                        while (true)
                        {
                            int label = reader.ReadByte();
                            labels.Add(label);

                        }

                    }
                }
            }
            catch (EndOfStreamException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Assert.IsTrue(labels.Count == 60000);




            //--------------IMAGE BYTE DATA TEST
            var imageDatas = new List<ImageData>();
            var dataPath = "../../Data/train-images-idx3-ubyte";
            try
            {
                using (var stream = new FileStream(dataPath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        int magic = reader.ReadInt32BigEndian(); // discard
                        int numImages = reader.ReadInt32BigEndian();
                        Assert.IsTrue(numImages == 60000);
                        int numRows = reader.ReadInt32BigEndian();

                        int numCols = reader.ReadInt32BigEndian();
                        Assert.IsTrue(numRows == 28 && numCols == 28);

                        while (true)
                        {
                            var imageData = reader.ReadAsImage(28, 28);
                            imageDatas.Add(imageData);
                            var convertedColors = imageData.bitmap.GetPixelsForOneImage(28, 28);
                            for(int i =0;i< imageData.colors.Count; i++)
                            {
                                var actualColor = imageData.colors[i];
                                var convertedColor = convertedColors[i];
                                Assert.IsTrue(actualColor == convertedColor);
                            }
                          
                            //bitmap.Save($"..\\..\\TMP1\\{IDGenerator.GetGuid()}.jpg");
                        }
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                //it is supposed to be throw this 
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
            
        }


    }
}
