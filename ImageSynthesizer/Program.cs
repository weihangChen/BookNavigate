using Infrastructure.SingleImageProcessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using Infrastructure.Services;
using Infrastructure.Models;

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

        //not used, generated char quality is low
        //static Func<Bitmap, int, Bitmap> medianBlurFilterFunc = (img, ksize) =>
        //{
        //    var filter = new MedianBlurFilter(ksize);
        //    return filter.Process(img);
        //};

        //sigma
        static List<int> oddSigma = Enumerable.Range(1, 5).Where(x => x % 2 == 1).ToList();
        //kernal dimension
        static List<int> oddKernalSize = Enumerable.Range(1, 5).Where(x => x % 2 == 1).ToList();
        //random int calculator
        static Random cal = new Random();
        static string FontDataDirDest = ConfigurationManager.AppSettings["FontDataDirDest"];
        static string FontDataDir = ConfigurationManager.AppSettings["FontDataDir"];
        static int synthesizeCount = 5;

        /// <summary>
        /// for single chars being redered in 2000 different fonts, 
        /// in order to make the number from 2000 to 5000 training image for each char
        /// synthesize them using different methods 
        /// 
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //create 62 way directories
            PreSynthesize();


            foreach (var folder in Directory.GetDirectories(FontDataDir))
            {

                var files = Directory.GetFiles(folder);

                foreach (var file in files)
                {
                    try
                    {
                        var img = Image.FromFile(file) as Bitmap;
                        var charIdentity = Path.GetFileNameWithoutExtension(file);
                        var charFolder = Path.Combine(FontDataDirDest, charIdentity);

                        //for every single image, generate 5 synthesized ones
                        var one2five = Enumerable.Range(1, synthesizeCount).ToList();
                        one2five.ForEach(index =>
                        {
                            var tossNum = cal.Next(1, 3);
                            SynthesizeOneImage(img, tossNum, charFolder);
                        });
                        PostProcessingOriginalImg(img, charFolder);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        public static void PreSynthesize()
        {
            //create folders for each char
            StringResources.SixtyTwoChars.ToList().ForEach(x =>
            {
                var folderName = StringResources.LetterMapping[x.ToString()];
                var folder = Path.Combine(FontDataDirDest, folderName);
                Directory.CreateDirectory(folder);
            });

        }


        public static void SynthesizeOneImage(Bitmap img, int tossNum, string charFolder)
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

        public static void PostProcessingOriginalImg(Bitmap img, string charFolder)
        {
            if (img == null)
                return;

            var path = Path.Combine(charFolder, IDGenerator.GetBase36(5) + ".jpg");
            img.Save(path);
        }

        public static void PostProcessingSynthesizedImg(Bitmap img, string charFolder)
        {
            if (img == null)
                return;

            var path = Path.Combine(charFolder, IDGenerator.GetBase36(5) + ".jpg");
            img.Save(path);
        }

    }



}
