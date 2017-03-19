using System;
using Infrastructure.Extensions;
using System.Drawing;

namespace BookDownloader.Services
{
    class BookNavigateServiceDoublePage : BookNavigateService
    {
        protected override void DisplayModeSetup()
        {

        }


        protected override void ProcessOnePage(Func<string> getPageImagePathFunc)
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

            eleScreenshot.Save(getPageImagePathFunc());

            //always recycle bitmap manually
            fullImg.Dispose();
            eleScreenshot.Dispose();
        }
    }
}
