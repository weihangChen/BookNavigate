using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Infrastructure.SingleImageProcessing
{
    public class AutoContrastFilter : ImgSynthesizeFilter
    {
        public AutoContrastFilter()
        {

        }

        public Bitmap Process(Bitmap inputImg)
        {
            Bitmap bitmap = default(Bitmap);
            using (var frame = new Image<Bgr, byte>(inputImg))
            {
                frame._EqualizeHist();
                bitmap = frame.Bitmap;
            }
            return bitmap;
        }
    }
}
