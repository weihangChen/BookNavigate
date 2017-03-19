using System;


namespace SingleCharacterCollect
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----------START---------");
            
            var service = new BookDownloader.Services.BookNavigateServiceSinglePage();
            service.GatherResource();

            Console.WriteLine("----------- END ---------");
            Console.ReadLine();
        }



       
    }
}
