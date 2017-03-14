using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace SingleCharacterCollect
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----------START---------");
            var service = new SingleCharacterCollect.Services.DriverService();
            service.GatherResource();

            Console.WriteLine("----------- END ---------");
            Console.ReadLine();
        }
    }
}
