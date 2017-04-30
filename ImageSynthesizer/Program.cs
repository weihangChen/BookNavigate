using System;
using System.Configuration;
using Infrastructure.Models;
using System.IO;

namespace ImageSynthesizer
{
    class Program
    {
        static int size = 28;

        //random int calculator
        static string _fontDataDirDest = ConfigurationManager.AppSettings["FontDataDirDest"];
        static string _fontDataDir = ConfigurationManager.AppSettings["FontDataDir"];
        static string _mnistDir = ConfigurationManager.AppSettings["MNISTDest"];
        static int synthesizeCount = 5;


        static void Main(string[] args)
        {
            Console.WriteLine("generate a byte file containing all image byte data by pointing to an image source - 1");
            Console.WriteLine("synthesize images and make copies of original ones - 2");
            Console.WriteLine("unpack mnist byte files into images - 3");

            var command = Console.ReadLine();
            if (command.Equals("1"))
            {
                Console.WriteLine("shuffle the dataset?");
                var shuffle = Convert.ToBoolean(Console.ReadLine());
                ByteFileGenerator.GenerateByteFile(_fontDataDirDest, size, shuffle);
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
                DirGenerator.CreateDirs(tobeGeneratedChars, _fontDataDirDest);

                Directory.CreateDirectory(_fontDataDir);
                Directory.CreateDirectory(_fontDataDirDest);
                ImageGenerator.GenerateImages(_fontDataDir,
                                              _fontDataDirDest,
                                              tobeGeneratedChars,
                                              shouldSynthesize,
                                              synthesizeCount,
                                              copy,
                                              colorInvert,
                                              size);
            }
            else if (command.Equals("3"))
            {
                Console.WriteLine("read byte data from MNIST files, then output new byte files?");
                bool resaveByteFile = Convert.ToBoolean(Console.ReadLine());
                Console.WriteLine("save images to hard disk?");
                bool saveImages = Convert.ToBoolean(Console.ReadLine());
                bool orderbylabel = false;
                if (resaveByteFile)
                {
                    Console.WriteLine("order by label?");
                    orderbylabel = Convert.ToBoolean(Console.ReadLine());

                }

                MNISTUnpack.UnpackByteFileToImages("..\\..\\Data\\train-labels-idx1-ubyte",
                                                   "..\\..\\Data\\train-images-idx3-ubyte",
                                                   _mnistDir,
                                                   resaveByteFile,
                                                   saveImages,
                                                   orderbylabel);

            }
        }

    }



}
