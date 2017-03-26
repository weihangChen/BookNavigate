using Infrastructure.Models;


namespace Infrastructure.SingleImageProcessing
{
    public interface IImageDecorator
    {
        void ProcessImage(ImageData image);
    }
}
