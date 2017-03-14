using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SingleCharacterCollect.Models;
using log4net;

namespace SingleCharacterCollect.Services
{
    public interface IDriverService
    {
        void GatherResource();
    }

    public class DriverService : IDriverService
    {
        FirefoxDriver driver;
        IImageService imageService;
        ISpellService spellService;
        log4net.ILog Logger;

        public DriverService()
        {
            driver = new FirefoxDriver();
            //wait 10 seconds for browser to load a url
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            imageService = new ImageService();
            spellService = new SpellService();
            Logger = LogManager.GetLogger("test");
        }

        public void GatherResource()
        {
            try
            {
                foreach (var bookData in Books.MyBooks)
                {
                    int errorCount = 0;
                    NavigateThrougOneBook(bookData.StartUrl, errorCount, true, bookData.History);

                    Logger.Info($"finish processing book {bookData.BookName}, total pages {bookData.History.ProcessedTotalPage} ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /// <summary>
        /// go through each page from a book, resursive, as long as consecutiveErrorCount is lower than 10, continue
        /// when consecutiveErrorCount is larger than 10, it means that clicking next page no longer goes to next page
        /// </summary>
        /// <param name="bookUrl"></param>
        public void NavigateThrougOneBook(string pageUrl, int consecutiveErrorCount, bool firstPage, ProcessHistory history, IWebElement nextButton = null)
        {

            //IWebElement nextButton = default(IWebElement);
            if (consecutiveErrorCount > 10)
                return;
            try
            {
                if (firstPage)
                {
                    driver.Navigate().GoToUrl(pageUrl);
                    driver.SwitchTo().Frame(":0.reader");
                    //change the displaymode
                    driver.FindElementByXPath("//div[@aria-label='Visningsalternativ']").Click();
                    driver.FindElementByXPath("//div[contains(@id, 'readingMode')]").Click();

                    driver.FindElementByXPath("//div[contains(., 'Ursprungliga sidor')]").Click();
                    System.Threading.Thread.Sleep(500);
                    driver.FindElementByXPath("//div[@title='Stäng']").Click();

                    //get the pagianation button
                    nextButton = driver.FindElementByXPath("//span[contains(., 'navigate_next')]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("not able to set up the displaymode or find the paging button");
                Console.WriteLine(e);
            }
            try
            {
                System.Threading.Thread.Sleep(1000);
                nextButton.Click();
                var newPageUrl = driver.Url;
                if (!newPageUrl.Equals(pageUrl))
                {
                    consecutiveErrorCount = 0;
                    ProcessOnePage();
                    history.ProcessedTotalPage++;
                }
                else
                {
                    consecutiveErrorCount++;
                }
                NavigateThrougOneBook(newPageUrl, consecutiveErrorCount, false, history, nextButton);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// process each page
        /// </summary>
        public void ProcessOnePage()
        {
            imageService.ProcessImage();
        }


    }
}
