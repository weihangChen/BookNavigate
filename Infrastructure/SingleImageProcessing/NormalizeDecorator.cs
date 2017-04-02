using Infrastructure.Models;
using Infrastructure.Services;
using Infrastructure.Extensions;
using System.Drawing;

namespace Infrastructure.SingleImageProcessing
{
    public class NormalizeDecorator : ImgDecorator
    {

        CropStrategy CropStrategy;
        ImageProcessor ImageProcessor;
        public NormalizeDecorator(CropStrategy cropStrategy)
        {
           CropStrategy = cropStrategy;
           ImageProcessor = new ImageProcessor();
        }
        public void ProcessImage(ImageData image)
        {
            image.EdgeRetangle = ImageProcessor.FindCropRectangle(image.bitmap, CropStrategy);
            image.bitmap = image.bitmap.CropImage(image.EdgeRetangle) as Bitmap;            
        }
    }
}
