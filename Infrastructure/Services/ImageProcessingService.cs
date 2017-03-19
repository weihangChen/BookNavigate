using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace infrastructure.Services
{

    public interface IOpenCVService
    {
        List<Contour<Point>> ExtractContours(Bitmap inputImg);
    }

    public class ImageProcessingService : IOpenCVService
    {
        public ImageProcessingService()
        {
        }


        public List<Contour<Point>> ExtractContours(Bitmap inputImg)
        {
            var contours = new List<Contour<Point>>();
            using (var frame = new Image<Bgr, byte>(inputImg))
            {
                using (var grayFrame = frame.Convert<Gray, Byte>())
                {
                    var c = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE,
                        Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
                    //while (c != null)
                    //{
                    //    contours.Add(c);
                    //    c = c.HNext;
                    //}
                    contours = FilterContours(c, grayFrame, filterContoursBySize: true, noiseFilter: true);
          
                }
            }


            return contours;
        }


        protected bool IsIntersectX(Contour<Point> Current, Contour<Point> Next)
        {
            bool IsIntersectedX = false;
            var currentLeftBound = Current.BoundingRectangle.X;
            var currentRightBound = currentLeftBound + Current.BoundingRectangle.Width;
            var nextLeftBound = Next.BoundingRectangle.X;
            var nextRightBound = nextLeftBound + Next.BoundingRectangle.Width;


            var test1 = currentLeftBound > nextLeftBound && currentLeftBound < nextRightBound;
            var test2 = currentRightBound > nextLeftBound && currentLeftBound < nextRightBound;
            var test3 = nextLeftBound > currentLeftBound && nextLeftBound < currentRightBound;
            var test4 = nextRightBound > currentLeftBound && nextRightBound < currentRightBound;


            if (test1 || test2 || test3 || test4)
                IsIntersectedX = true;
            return IsIntersectedX;
        }

        private List<Contour<Point>> FilterContours(Contour<Point> contours, Image<Gray, byte> frame,
             bool filterContoursBySize = false, int minContourLength = 20,
            int minContourArea = 10, double minFormFactor = 0.5, bool noiseFilter = false)
        {
            var frameWidth = frame.Width;
            var frameHeight = frame.Height;
            int maxArea = frameWidth * frameHeight / 5;
            var c = contours;
            List<Contour<Point>> result = new List<Contour<Point>>();
            while (c != null)
            {
                if (filterContoursBySize)
                    if (c.Total < minContourLength ||
                        c.Area < minContourArea || c.Area > maxArea ||
                        c.Area / c.Total <= minFormFactor)
                        goto next;

                if (noiseFilter)
                {
                    Point p1 = c[0];
                    Point p2 = c[(c.Total / 2) % c.Total];
                    if (frame[p1].Intensity <= double.Epsilon && frame[p2].Intensity <= double.Epsilon)
                        goto next;
                }
                result.Add(c);

                next:
                c = c.HNext;
            }

            return result;
        }


    }
}
