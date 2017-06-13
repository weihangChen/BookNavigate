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
        protected int _offsetXLeft;
        protected int _offsetXRight;

        protected int _offsetYTop;
        protected int _offsetYBottom;

        public PeelDecorator(int offsetXLeft, int offsetXRight, int offsetYTop, int offsetYBottom)
        {
            _offsetXLeft = offsetXLeft;
            _offsetXRight = offsetXRight;
            _offsetYTop = offsetYTop;
            _offsetYBottom = offsetYBottom;
        }

        public override Bitmap DecorateImage(Bitmap img)
        {
            var features = img.ConvertImageToTwoDimensionArray();
            var features_new = features.PeelOffset(_offsetXLeft, _offsetXRight, _offsetYTop, _offsetYBottom);
            var img_new = features_new.ConvertTwoDimensionArrayToImage();
            return img_new;
        }

    }

    /// <summary>
    /// two stpes
    /// 1. calculate the offset diff in X and Y
    /// 2. mix the offset diff with manual offset removal amount, so offsets are removed, but at the same time, the offset are X and Y are even
    /// </summary>
    public class EvenAndPeelDecorator : PeelDecorator
    {
        public EvenAndPeelDecorator(int offsetXLeft, int offsetXRight, int offsetYTop, int offsetYBottom) :
            base(offsetXLeft, offsetXRight, offsetYTop, offsetYBottom)
        {
        }

        public override Bitmap DecorateImage(Bitmap img)
        {
            var features = img.ConvertImageToTwoDimensionArray();
            var offsets = img.CalculateOffsetDiffs();
            var features_new = features.PeelOffset(_offsetXLeft + offsets[0], _offsetXRight + offsets[1], _offsetYTop + offsets[2], _offsetYBottom + offsets[3]);
            var img_new = features_new.ConvertTwoDimensionArrayToImage();
            return img_new;
        }
    }

    /// <summary>
    /// this is different from EvenAndPeelDecorator at the second step, the manual input of offset data is the remain number, not the removal number
    /// 
    /// </summary>
    public class AutoPeelDecorator : PeelDecorator
    {
        public AutoPeelDecorator(int offsetXLeft, int offsetXRight, int offsetYTop, int offsetYBottom) :
            base(offsetXLeft, offsetXRight, offsetYTop, offsetYBottom)
        {
        }

        public override Bitmap DecorateImage(Bitmap img)
        {
            var features = img.ConvertImageToTwoDimensionArray();
            var offsets = img.CalculateOffsets();
            var features_new = features.PeelOffset(
                offsets[0] - _offsetXLeft,
                offsets[1] - _offsetXRight,
                offsets[2] - _offsetYTop,
                offsets[3] - _offsetYBottom);
            var img_new = features_new.ConvertTwoDimensionArrayToImage();
            return img_new;
        }
    }
}
