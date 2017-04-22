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
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageSynthesizer
{
    class Program
    {
        static int size = 28;

        //random int calculator
        static Random cal = new Random();
        static string FontDataDirDest = ConfigurationManager.AppSettings["FontDataDirDest"];
        static string FontDataDir = ConfigurationManager.AppSettings["FontDataDir"];
        static int synthesizeCount = 5;
        

        static void Main(string[] args)
        {
            Console.WriteLine("generate a byte file containing all image byte data - 1");
            Console.WriteLine("synthesize images and make copies of original ones - 2");
            Console.WriteLine("unpack mnist byte files into images - 3");

            var command = Console.ReadLine();
            if (command.Equals("1"))
            {
                ByteFileGenerator.GenerateByteFile();
            }
            else if (command.Equals("2"))
            {
                Console.WriteLine("we are going to normalize the data and put them into folder with char name, do you want to spawn 5 synthesized images per image? (false/true)");
                var shouldSynthesize = Convert.ToBoolean(Console.ReadLine());

                Console.WriteLine("generate more copies of original image? type of number, if 1 no copy is saved, if larger than 1 then N-1 more copies will be saved");
                var copy = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("which string set do you want to generate? (1 - digits, 2 - 62 ways chars)");
                var stringSetChoice = Console.ReadLine();

                string tobeGeneratedChars = "";
                if (stringSetChoice.Equals("1"))
                    tobeGeneratedChars = StringResources.Digits;
                else if (stringSetChoice.Equals("2"))
                    tobeGeneratedChars = StringResources.SixtyTwoChars;

                Console.WriteLine("should make background black? (true/false)");
                var colorInvert = Convert.ToBoolean(Console.ReadLine());
                DirGenerator.CreateDirs(tobeGeneratedChars, FontDataDirDest);
                ImageGenerator.GenerateImages(FontDataDir,
                                           FontDataDirDest,
                                           tobeGeneratedChars,
                                           shouldSynthesize,
                                           synthesizeCount,
                                           copy,
                                           colorInvert,
                                           size);
            }
            else if (command.Equals("3"))
            {

            }
        }

    }



}
