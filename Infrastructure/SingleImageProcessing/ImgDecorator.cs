using Infrastructure.Models;


namespace Infrastructure.SingleImageProcessing
{
    public interface ImgDecorator
    {
        void ProcessImage(ImageData image);
    }
}
