using System;
using System.Net.Http;
using Infrastructure.Extensions;

namespace BookDownloader.Services
{
    public class BookNavigateServiceSinglePage : BookNavigateService
    {
        protected override void DisplayModeSetup()
        {
            driver.FindElementByXPath("//div[@aria-label='Visningsalternativ']").Click();
            System.Threading.Thread.Sleep(1000);
            driver.FindElementByXPath("//div[contains(@id, 'readingMode')]").Click();
            System.Threading.Thread.Sleep(1000);
            driver.FindElementByXPath("//div[contains(., 'Ursprungliga sidor')]").Click();
            System.Threading.Thread.Sleep(1000);
            driver.FindElementByXPath("//div[@title='Stäng']").Click();
        }


        protected override void ProcessOnePage(Func<string> getPageImagePathFunc)
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
                data.FromBytesToFile(getPageImagePathFunc());
            }

        }
    }
}
