using Infrastructure.SingleImageProcessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using Infrastructure.Services;
using Infrastructure.Models;
using Infrastructure.Extensions;

namespace ImageSynthesizer
{
    class Program
    {

        //definen three callback funtions for filters 
        static Func<Bitmap, Bitmap> autoContrastFilterFunc = (img) =>
        {
            var filter = new AutoContrastFilter();
            return filter.Process(img);
        };

        static Func<Bitmap, Size, double, Bitmap> gaussianBlurFilterFunc = (img, size, sigmax) =>
        {
            var filter = new GaussianBlurFilter(size, sigmax);
            return filter.Process(img);
        };
        static int size = 28;


        //sigma
        static List<int> oddSigma = Enumerable.Range(1, 5).Where(x => x % 2 == 1).ToList();
        //kernal dimension
        static List<int> oddKernalSize = Enumerable.Range(1, 5).Where(x => x % 2 == 1).ToList();
        //random int calculator
        static Random cal = new Random();
        static string FontDataDirDest = ConfigurationManager.AppSettings["FontDataDirDest"];
        static string FontDataDir = ConfigurationManager.AppSettings["FontDataDir"];
        static int synthesizeCount = 5;
        static bool shouldSynthesize;
        static int copy = 1;
        static string TobeGeneratedChars;
        static bool GenerateByteFileContainingAllImages;
        static void Main(string[] args)
        {
            Console.WriteLine("generate a byte file containing all image byte data and identity as MNIST OR do you want to synthesize images? (false/true)");
            GenerateByteFileContainingAllImages = Convert.ToBoolean(Console.ReadLine());
            if (GenerateByteFileContainingAllImages)
            {
                NormalizeAndGenerateByteFile();
            }
            else
            {
                Console.WriteLine("we are going to normalize the data and put them into folder with char name, do you want to spawn 5 synthesized images per image? (false/true)");
                shouldSynthesize = Convert.ToBoolean(Console.ReadLine());

                Console.WriteLine("generate more copies of original image? type of number, if 1 no copy is saved, if larger than 1 then N-1 more copies will be saved");
                copy = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("which string set do you want to generate? (1 - digits, 2 - 62 ways chars)");
                var stringSetChoice = Console.ReadLine();
                if (stringSetChoice.Equals("1"))
                    TobeGeneratedChars = StringResources.Digits;
                else if (stringSetChoice.Equals("2"))
                    TobeGeneratedChars = StringResources.SixtyTwoChars;


                CreateDirs();
                GenerateImages();
            }

        }


        /// <summary>
        /// normalize size of images, put all images and its identity into two byte files as for MNIST
        /// one file with image data
        /// second file with image label data
        /// </summary>
        protected static void NormalizeAndGenerateByteFile()
        {
            var imageData = ConfigurationManager.AppSettings["ImageData"];
            var imageLabel = ConfigurationManager.AppSettings["ImageLabel"];


            var imageDataWriter = new BinaryWriter(new FileStream(imageData, FileMode.CreateNew));
            var labelWriter = new BinaryWriter(new FileStream(imageLabel, FileMode.CreateNew));

            List<int> labels = new List<int>();
            foreach (var folder in Directory.GetDirectories(FontDataDirDest))
            {
                var tmp = folder.Split('\\');
                var charIdentity = StringResources.FolderToLetterMapping[tmp[tmp.Length - 1]];
                foreach (var file in Directory.GetFiles(folder))
                {
                    var img = Image.FromFile(file) as Bitmap;
                    var bytes = GetBytesForOneImage(img);
                    imageDataWriter.Write(bytes);
                    labelWriter.Write(Convert.ToInt32(charIdentity));
                    labels.Add(Convert.ToInt32(charIdentity));
                }
            }
            imageDataWriter.Flush();
            imageDataWriter.Close();
            labelWriter.Flush();
            labelWriter.Close();

            //----------------------------
            //----------------------------

            //verify label binary data length is N * 4
            var info = new FileInfo(imageLabel);
            if (info.Length != labels.Count * 4)
                throw new ArgumentException("total byte count not match");
            
            var reader = new BinaryReader(new FileStream(imageLabel, FileMode.Open));
            int count = 0;
            while (true)
            {
                var bytes = reader.ReadBytes(4);
                var digit = BitConverter.ToInt32(bytes, 0);
                if (digit != labels[count])
                    throw new ArgumentException("saved byte data not match the content");

                count++;
                if (count == labels.Count)
                    break;
            }
            //verify image binary data length
            var info1 = new FileInfo(imageData);
            if (info1.Length != labels.Count * 28 * 28)
                throw new ArgumentException("total byte count not match");


        }



        protected static byte[] GetBytesForOneImage(Bitmap img)
        {
            byte[] bytes = new byte[size * size];
            var imgResized = (Bitmap)img.ResizeImage(size, size);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Color pixel = imgResized.GetPixel(x, y);
                    var pixelValue = Convert.ToInt32((pixel.R + pixel.G + pixel.B) / 3);
                    var pixelValueAsByte = Convert.ToByte(pixelValue);
                    bytes[size * x + y] = Convert.ToByte(pixelValueAsByte);
                }
            }
            return bytes;
        }


        protected static void GenerateImages()
        {

            foreach (var folder in Directory.GetDirectories(FontDataDir))
            {

                var files = Directory.GetFiles(folder);

                foreach (var file in files)
                {
                    try
                    {
                        var img = Image.FromFile(file) as Bitmap;
                        var charIdentity = Path.GetFileNameWithoutExtension(file);
                        if (!TobeGeneratedChars.Contains(charIdentity))
                            continue;
                        var charFolder = Path.Combine(FontDataDirDest, charIdentity);

                        //for every single image, generate 5 synthesized ones
                        var one2five = Enumerable.Range(1, synthesizeCount).ToList();

                        if (shouldSynthesize)
                        {
                            one2five.ForEach(index =>
                            {
                                var tossNum = cal.Next(1, 3);
                                SynthesizeOneImage(img, tossNum, charFolder);
                            });
                        }
                        for (int i = 0; i < copy; i++)
                        {
                            PostProcessingOriginalImg(img, charFolder);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        protected static void CreateDirs()
        {
            //create folders for each char
            TobeGeneratedChars.ToList().ForEach(x =>
            {
                var folderName = StringResources.LetterMapping[x.ToString()];
                var folder = Path.Combine(FontDataDirDest, folderName);
                Directory.CreateDirectory(folder);
            });

        }


        protected static void SynthesizeOneImage(Bitmap img, int tossNum, string charFolder)
        {
            var synthesizedImg = default(Bitmap);


            try
            {
                if (tossNum == 1)
                {
                    synthesizedImg = autoContrastFilterFunc(img);
                }
                //else if (tossNum == 2)
                //{
                //    var index = cal.Next(0, oddSigma.Count);
                //    var randomKsize = oddSigma[index];
                //    synthesizedImg = medianBlurFilterFunc(img, randomKsize);
                //}
                else if (tossNum == 2)
                {
                    var indexSigma = cal.Next(0, oddSigma.Count);
                    var indexKernal = cal.Next(0, oddKernalSize.Count);
                    var randomSigmaSize = oddSigma[indexSigma];
                    var randomKernalsize = oddKernalSize[indexKernal];
                    synthesizedImg = gaussianBlurFilterFunc(img,
                                                            new Size(randomKernalsize, randomKernalsize),
                                                            randomSigmaSize);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            PostProcessingSynthesizedImg(synthesizedImg, charFolder);

        }

        protected static void PostProcessingOriginalImg(Bitmap img, string charFolder)
        {
            if (img == null)
                return;

            var path = Path.Combine(charFolder, IDGenerator.GetBase36(5) + ".jpg");
            img.Save(path);
        }

        protected static void PostProcessingSynthesizedImg(Bitmap img, string charFolder)
        {
            if (img == null)
                return;

            var path = Path.Combine(charFolder, IDGenerator.GetBase36(5) + ".jpg");
            img.Save(path);
        }

    }



}
