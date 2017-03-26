using Emgu.CV;
using System;
using System.Drawing;
using System.IO;

namespace Infrastructure.Models
{
    public class ImageData
    {
        public Bitmap bitmap { get; set; }
        public Contour<Point> Contour { get; set; }
        public Rectangle BoundingRectangle { get; set; }
        public DirectoryInfo HardDiskPath { get; set; }
        public Rectangle EdgeRetangle { get; set; }

        /// <summary>
        /// use this method to get rectangle, the ones in contours come from original imagedatas, while the boudingrectangle comes from merging imagedata
        /// </summary>
        /// <returns></returns>
        public Rectangle RectangleFinal
        {
            get
            {
                if (BoundingRectangle == null || (BoundingRectangle.X == 0 && BoundingRectangle.Y == 0 && BoundingRectangle.Height == 0 && BoundingRectangle.Width == 0))
                    return Contour.BoundingRectangle;
                return BoundingRectangle;
            }
        }

        public double[] featureVector { get; set; }
        
        public string Text { get; set; }



        public byte[][] pixels;

        //public int label { get; set; }
        public int orderInCollection { get; set; }
        public int width; // 28
        public int height; // 28
        public bool IsBadData { get; set; }
        //public double Probability { get; set; }
        public BigStringType BigstringType { get; set; }
        public ImageData(string text)
        {
            Text = text;
        }
        public ImageData()
        {

        }











    }

}
