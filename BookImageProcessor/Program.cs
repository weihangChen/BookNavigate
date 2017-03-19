using BookImageProcessor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----------START---------");

            IImageExtractionService service = new ImageExtractionService();
            Action test =()=> service.ExtractChars();
            
            Task.Run(()=>service.ExtractChars()).Wait();
            Console.WriteLine("----------- END ---------");
            Console.ReadLine();
        }
    }
}
