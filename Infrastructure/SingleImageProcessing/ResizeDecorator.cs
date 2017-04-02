using Infrastructure.Models;
using Infrastructure.Extensions;

namespace Infrastructure.SingleImageProcessing
{


    public class ResizeDecorator : ImgDecorator
    {
        int resizeWidth;
        int resizeHeight;

        public ResizeDecorator(
        int resizeWidth,
        int resizeHeight)
        {
            this.resizeWidth = resizeWidth;
            this.resizeHeight = resizeHeight;

        }

        public void ProcessImage(ImageData image)
        {
            image.bitmap = image.bitmap.ResizeImage(resizeWidth, resizeHeight);


        }
    }
}
