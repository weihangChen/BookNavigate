using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SingleCharacterCollect.Models;
using log4net;
using SingleCharacterCollect.Extensions;
using System.Drawing;
using System.Net.Http;

namespace SingleCharacterCollect.Services
{
    public interface IDriverService
    {
        void GatherResource();
    }

    public class DriverService : IDriverService
    {
        FirefoxDriver driver;
        ISpellService spellService;
        log4net.ILog Logger;

        public DriverService()
        {
            driver = new FirefoxDriver();
            //wait 10 seconds for browser to load a url
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);

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
            if (consecutiveErrorCount > 10)
                return;
            try
            {
                if (firstPage)
                {
                    driver.Navigate().GoToUrl(pageUrl);
                    //first start of browser takes long time
                    System.Threading.Thread.Sleep(3000);
                    //need to go to the correct iframe
                    driver.SwitchTo().Frame(":0.reader");

                    
                    //change the displaymode
                    driver.FindElementByXPath("//div[@aria-label='Visningsalternativ']").Click();
                    System.Threading.Thread.Sleep(1000);
                    driver.FindElementByXPath("//div[contains(@id, 'readingMode')]").Click();
                    System.Threading.Thread.Sleep(1000);
                    driver.FindElementByXPath("//div[contains(., 'Ursprungliga sidor')]").Click();
                    System.Threading.Thread.Sleep(1000);
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
        /// take a screen shot, crop the reading session, save it without scaling
        /// if view book in templte2 this method is adopted
        /// </summary>
        public void ProcessOnePage2()
        {
            var shot = driver.GetScreenshot();
            var fullImg = shot.AsByteArray.FromBytesToImage();
            var ele = driver.FindElementByXPath("//div[@role='main']");
            var point = ele.Location;

            // Get width and height of the element
            int eleWidth = ele.Size.Width;
            int eleHeight = ele.Size.Height;

            // Crop the entire page screenshot to get only element screenshot
            var eleScreenshot = fullImg.CropImage(new Rectangle(point.X, point.Y,
                eleWidth, eleHeight));
            eleScreenshot.Save($"C:/Users/weihang/Desktop/0315/{Guid.NewGuid()}.jpg");
        }

        /// <summary>
        /// if view booking in template1 this method is adopted
        /// just download the image
        /// https://books.googleusercontent.com/books/content/reader?id=nH15DQAAQBAJ&hl=sv&pg=PT8&img=1&zoom=3&sig=ACfU3U2QmuDH1GCB7qFCLvk4YDieaZyqhg&source=ge-web-app&w=673
        /// </summary>
        public void ProcessOnePage()
        {
            var imgs = driver.FindElementsByTagName("img");
            string url = "";
            foreach (var img in imgs)
            {
                var location = img.Location;
                var src = img.GetAttribute("src");
                if (location.X != 0 && location.Y != 0 && !string.IsNullOrEmpty(src))
                {
                    url = src;
                    break;
                }
            }
            if (string.IsNullOrEmpty(url))
                throw new Exception("book image not found");
            using (var client = new HttpClient())
            {
                var data = client.GetByteArrayAsync(new Uri(url)).Result;
                data.FromBytesToFile($"C:/Users/weihang/Desktop/0315/{Guid.NewGuid()}.jpg");
            }

        }


    }
}
