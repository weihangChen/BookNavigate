using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Infrastructure.SingleImageProcessing
{
    public class MedianBlurFilter : ImgSynthesizeFilter
    {
        int Ksize; // odd number . ex 3
        public MedianBlurFilter()
        {
            Ksize = 3;
        }

        public MedianBlurFilter(int ksize)
        {
            Ksize = ksize;
        }

        public Bitmap Process(Bitmap inputImg)
        {
            var result = new Image<Bgr, byte>(inputImg.Width, inputImg.Height);
            using (var frame = new Image<Bgr, byte>(inputImg))
            {
                CvInvoke.MedianBlur(frame, result, Ksize);

            }
            return result.Bitmap;
        }
    }
}
