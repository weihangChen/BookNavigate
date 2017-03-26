using BookImageProcessor.Services;
using System;
using System.Threading.Tasks;

namespace BookImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("-----------START---------");
            IImageExtractionService service = new ImageExtractionService();
            Task.Run(() => service.ExtractDataFromImage()).Wait();
            Console.WriteLine("----------- END ---------");
            Console.ReadLine();
        }
    }
}
