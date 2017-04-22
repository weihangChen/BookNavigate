using Infrastructure.Models;
using Infrastructure.SingleImageProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Infrastructure.Extensions;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageSynthesizer
{

    public class ImageGenerator
    {
        static Random cal = new Random();
        //sigma
        static List<int> oddSigma = Enumerable.Range(1, 5).Where(x => x % 2 == 1).ToList();
        //kernal dimension
        static List<int> oddKernalSize = Enumerable.Range(1, 5).Where(x => x % 2 == 1).ToList();
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

        public static void GenerateImages(string FontDataDir,
                                          string FontDataDirDest,
                                          string TobeGeneratedChars,
                                          bool shouldSynthesize,
                                          int synthesizeCount,
                                          int copy,
                                          bool colorInvert,
                                          int size)
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
                                SynthesizeOneImage(img, tossNum, charFolder, colorInvert, size);
                            });
                        }
                        for (int i = 0; i < copy; i++)
                        {
                            SaveImageToHardDisk(img, charFolder, colorInvert, size);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

     


        protected static void SynthesizeOneImage(Bitmap img, int tossNum, string charFolder, bool colorInvert, int size)
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
            SaveImageToHardDisk(synthesizedImg, charFolder, colorInvert, size);

        }

        protected static void SaveImageToHardDisk(Bitmap img, string charFolder, bool colorInvert, int size)
        {
            if (img == null)
                return;
            var path = Path.Combine(charFolder, Guid.NewGuid().ToString() + ".jpg");
            Bitmap imgTobeSave = img.ResizeImage(size, size);
            if (colorInvert)
            {
                using (var frame = new Image<Bgr, byte>(imgTobeSave))
                {
                    frame._Not();
                    frame.Bitmap.Save(path);
                }
            }
            else
            {
                img.Save(path);
            }
        }

    }
}
