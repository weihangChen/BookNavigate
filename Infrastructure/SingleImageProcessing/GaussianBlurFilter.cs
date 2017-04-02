using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Infrastructure.SingleImageProcessing
{
    //GaussianBlur
    public class GaussianBlurFilter : ImgSynthesizeFilter
    {
        Size Ksize; // ex. 15
        double SigmaX;//odd number. ex. 3


        public GaussianBlurFilter()
        {
            Ksize = new Size(15, 15);
            SigmaX = 3;
        }

        public GaussianBlurFilter(Size ksize, double sigmaX)
        {
            Ksize = ksize;
            SigmaX = sigmaX;
        }

        public Bitmap Process(Bitmap inputImg)
        {
            var result = new Image<Bgr, byte>(inputImg.Width, inputImg.Height);
            using (var frame = new Image<Bgr, byte>(inputImg))
            {
                CvInvoke.GaussianBlur(frame, result, Ksize, SigmaX);
            }
            return result.Bitmap;
        }
    }
}
