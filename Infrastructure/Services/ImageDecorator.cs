using System.Drawing;
using Infrastructure.Extensions;

namespace Infrastructure.Services
{
    public abstract class ImageDecorator
    {

        public abstract Bitmap DecorateImage(Bitmap img);
    }

    public class ResizeDecorator : ImageDecorator
    {
        int _newwidth;
        int _newheight;
        public ResizeDecorator(int newwidth, int newheight)
        {
            _newwidth = newwidth;
            _newheight = newheight;
        }



        public override Bitmap DecorateImage(Bitmap img)
        {
            return img.ResizeImage(_newwidth, _newheight);
        }
    }

    public class PeelDecorator : ImageDecorator
    {
        int _offsetX;
        int _offsetY;

        public PeelDecorator(int offsetX, int offsetY)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        public override Bitmap DecorateImage(Bitmap img)
        {
            var features = img.ConvertImageToTwoDimensionArray();
            var features_new = features.PeelOffset(_offsetX, _offsetY);
            var img_new = features_new.ConvertTwoDimensionArrayToImage();
            return img_new;
        }

    }
}
