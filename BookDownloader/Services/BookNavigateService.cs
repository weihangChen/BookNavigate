using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using BookDownloader.Models;
using log4net;
using System.Configuration;
using Infrastructure.Extensions;
using System.IO;

namespace BookDownloader.Services
{
    public interface IBookNavigateService
    {
        void GatherResource();
    }

    public abstract class BookNavigateService : IBookNavigateService
    {
        protected FirefoxDriver driver;
        protected log4net.ILog Logger;
        protected readonly string BookDataPath = ConfigurationManager.AppSettings.Get("BookDataPath");

        public BookNavigateService()
        {
            driver = new FirefoxDriver();
            //set up webdriver to wait 10 seconds for browser to load a url
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            Logger = LogManager.GetLogger("BookNavigation_Log");
        }



        public void GatherResource()
        {
            foreach (var bookData in Books.MyBooks)
            {
                int errorCount = 0;
                var navigateButton = InitialSetup(bookData.StartUrl);
                var bookName = bookData.BookName;
                var bookPath = string.Format(BookDataPath, bookName);
                Directory.CreateDirectory(bookPath);

                Func<string> getPageImagePathFunc = () =>
                {
                    return bookPath + Guid.NewGuid() + ".jpg";
                };

                NavigateThrougOneBook(bookData.StartUrl, navigateButton, errorCount, bookData.History, getPageImagePathFunc);

                Logger.Info($"finish processing book {bookData.BookName}, total pages {bookData.History.ProcessedTotalPage} ");
            }
        }

        protected IWebElement InitialSetup(string firstPageUrl)
        {
            IWebElement nextButton = default(IWebElement);
            try
            {
                driver.Navigate().GoToUrl(firstPageUrl);
                //first start of browser takes long time
                System.Threading.Thread.Sleep(3000);
                //need to go to the correct iframe
                driver.SwitchTo().Frame(":0.reader");

                //setup the displaymode
                DisplayModeSetup();

                //get the pagianation button
                nextButton = driver.FindElementByXPath("//span[contains(., 'navigate_next')]");
            }
            catch (Exception e)
            {
                Logger.Error("not able to set up the displaymode or find the paging button", e);
                throw e;
            }
            return nextButton;
        }

        protected void SavePageImageToBookFolder(byte[] bytes, string bookPath)
        {
            bytes.FromBytesToFile(bookPath + Guid.NewGuid() + ".jpg");
        }

        // go through each page from a book, resursive, as long as consecutiveErrorCount is lower than 10, continue
        // when consecutiveErrorCount is larger than 10, it means that clicking next page no longer goes to next page
        protected void NavigateThrougOneBook(string pageUrl, IWebElement nextButton,
            int consecutiveErrorCount, ProcessHistory history, Func<string> getPageImagePathFunc)
        {
            if (consecutiveErrorCount > 10)
                return;


            try
            {
                System.Threading.Thread.Sleep(1000);
                nextButton.Click();
                var newPageUrl = driver.Url;
                if (!newPageUrl.Equals(pageUrl))
                {
                    consecutiveErrorCount = 0;
                    ProcessOnePage(getPageImagePathFunc);
                    history.ProcessedTotalPage++;
                }
                else
                {
                    consecutiveErrorCount++;
                }
                NavigateThrougOneBook(newPageUrl, nextButton, consecutiveErrorCount, history, getPageImagePathFunc);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }

        }


        protected abstract void DisplayModeSetup();

        protected abstract void ProcessOnePage(Func<string> getPageImagePathFunc);



    }
}
