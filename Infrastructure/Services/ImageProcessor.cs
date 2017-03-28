using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Infrastructure.Extensions;
using log4net;
using Infrastructure.Models;

namespace Infrastructure.Services
{

    public class ImageProcessor
    {


        private ILog Logger;
        //settings
        public bool equalizeHist = false;
        public bool noiseFilter = false;
        public int cannyThreshold = 50;
        public bool blur = true;
        public int adaptiveThresholdBlockSize = 4;
        public double adaptiveThresholdParameter = 1.2d;
        public bool addCanny = true;
        public bool filterContoursBySize = true;
        public bool onlyFindContours = false;
        public int minContourLength = 20;
        public int minContourArea = 10;
        public double minFormFactor = 0.5;
        //
        public List<Contour<Point>> contours;
        //public Templates templates = new Templates();
        //public Templates samples = new Templates();
        //public List<FoundTemplateDesc> foundTemplates = new List<FoundTemplateDesc>();
        //public TemplateFinder finder = new TemplateFinder();
        //public Image<Gray, byte> binarizedFrame;
        public Image<Gray, byte> frame;

        //
        private Image<Gray, byte> grayFrame;
        private Image<Gray, byte> smoothedGrayFrame;
        private Image<Gray, byte> cannyFrame;

        public ImageProcessor()
        {
            Logger = LogManager.GetLogger(this.GetType());
            // Callback = new GenericCallbacks();
        }





        public Rectangle FindCropRectangle(Bitmap bmp, CropStrategy CropStrategy, string txt = "")
        {
            var imageRawData = new Dictionary<int, List<double>>();

            int width = bmp.Width;
            int height = bmp.Height;

            //read in bitmpa data row by row into imageRawData
            var leftMostIndex = 0;
            var rightMostIndex = 0;

            for (int i = 0; i < height; i++)
            {
                var rowRawData = new List<double>();
                for (int j = 0; j < width; j++)
                {
                    var pixel = bmp.GetPixel(j, i);
                    var tmp = (pixel.R == 255) ? 0 : 1;

                    if (tmp == 1)
                    {
                        if (leftMostIndex > j || leftMostIndex == 0)
                            leftMostIndex = j;

                        if (rightMostIndex < j || rightMostIndex == 0)
                            rightMostIndex = j;
                    }
                    rowRawData.Add(tmp);
                }
                imageRawData.Add(i, rowRawData);
            }

            //find the rectangle
            var darkRows = imageRawData.Where(x => x.Value.Contains(1));
            var firstDarkRowIndex = darkRows.First().Key;
            var lastDarkRowIndex = darkRows.Last().Key;

            //rightMostIndex - leftMostIndex
            var letterWidth = rightMostIndex - leftMostIndex;
            var letterHeight = lastDarkRowIndex - firstDarkRowIndex;
            //the right index and left index is same, set the rectangle width to 1
            letterWidth = letterWidth == 0 ? 1 : letterWidth + 1;
            letterHeight = letterHeight == 0 ? 1 : letterHeight + 1;
            //modify the leftMostIndex and firstDarkRowIndex
            leftMostIndex = leftMostIndex > 1 ? leftMostIndex : leftMostIndex - 1;
            firstDarkRowIndex = firstDarkRowIndex > 1 ? firstDarkRowIndex : firstDarkRowIndex - 1;


            Rectangle edgeRetangle = new Rectangle();
            if (CropStrategy == CropStrategy.CropToEdge)
                edgeRetangle = new Rectangle(leftMostIndex, firstDarkRowIndex, letterWidth, letterHeight);
            else if (CropStrategy == CropStrategy.CropHeightOnly)
                edgeRetangle = new Rectangle(0, firstDarkRowIndex, width, letterHeight);
            else if (CropStrategy == CropStrategy.CropWidthOnly)
                edgeRetangle = new Rectangle(leftMostIndex, 0, letterWidth, height);

            return edgeRetangle;

        }




        public List<Contour<Point>> GenerateContours(Image<Bgr, byte> frame)
        {

            //frame is memory leak 1, dispose somewhere else 
            //memory leak 2
            grayFrame = frame.Convert<Gray, Byte>();
            //memory leak 3
            this.frame = grayFrame.Clone();

            if (equalizeHist)
                grayFrame._EqualizeHist();//autocontrast

            //smoothed
            //memory leak 4
            smoothedGrayFrame = grayFrame.PyrDown();
            //memory leak 5
            smoothedGrayFrame = smoothedGrayFrame.PyrUp();
            //canny 

            if (noiseFilter)
                //memory leak 6
                //cannyFrame = smoothedGrayFrame.Canny(new Gray(cannyThreshold), new Gray(cannyThreshold));
                cannyFrame = smoothedGrayFrame.Canny(cannyThreshold, cannyThreshold);
            //smoothing
            if (blur)
                grayFrame = smoothedGrayFrame;
            //binarize
            CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255, Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, adaptiveThresholdBlockSize + adaptiveThresholdBlockSize % 2 + 1, adaptiveThresholdParameter);
            //
            grayFrame._Not();
            //
            if (addCanny)
                if (cannyFrame != null)
                    grayFrame._Or(cannyFrame);
            //
            //this.binarizedFrame = grayFrame;

            //dilate canny contours for filtering
            //memory leak 7
            if (cannyFrame != null)
                cannyFrame = cannyFrame.Dilate(3);

            //find contours
            var sourceContours = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
            //filter contours
            contours = FilterContours(sourceContours, cannyFrame, grayFrame.Width, grayFrame.Height);

            //TODO 1206
            //keep the grayframe, because binaryframe is same as grayframe, the contours to images part still require the grayframe, using the original frame
            //have problems like background color etc. revert the black and white here for binaryframe
            //remove one memory leak works
            //ClearEMGUCVMemory(cannyFrame);
            //remove one memory leak works
            grayFrame._Not();
            //ClearEMGUCVMemory(grayFrame);
            //remove one memory leak not working
            //ClearEMGUCVMemory(smoothedGrayFrame);



            return contours;

        }







        private List<Contour<Point>> FilterContours(Contour<Point> contours, Image<Gray, byte> cannyFrame, int frameWidth, int frameHeight)
        {
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
                    if (cannyFrame[p1].Intensity <= double.Epsilon && cannyFrame[p2].Intensity <= double.Epsilon)
                        goto next;
                }
                result.Add(c);

                next:
                c = c.HNext;
            }

            return result;
        }






        #region divide data into rows + perform merging of contours

        /// <summary>
        /// contours that lies closed to each other, are grouped together, form a new convex hull
        /// then get classified again 
        ///
        /// TODO
        /// we assume that the all imageDatas are already sorted by rectangle-X
        /// </summary>
        /// <param name="ImageDatasPerRow"></param>
        /// <param name="BigString"></param>


        //public void MergeContours(List<ImageDatasPerRow> ImageDatasPerRow, bool cropFromOriginBitmap, Func<int, bool> HaltCallback = null, Func<Bitmap, Bitmap> ImagePostProcessingCallback = null)
        //{
        //    foreach (var row in ImageDatasPerRow)
        //    {
        //        var imageDatas = row.ImageDatas;
        //        var total = imageDatas.Count();


        //        //check two digits every time
        //        var ImageDatasMerging = new List<ImageDataMerging>();
        //        for (var i = 0; i < total; i++)
        //        {
        //            var current = imageDatas[i];
        //            if (current.IsBadData)
        //                continue;


        //            ImageData right1 = null;
        //            ImageData right2 = null;
        //            bool IsCurrentIntersectedWithRight1 = false;
        //            bool IsCurrentIntersectedWithRight2 = false;


        //            //use try catch to bypass the outofindex exception
        //            try
        //            {
        //                right1 = imageDatas[i + 1];
        //                right2 = imageDatas[i + 2];
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //            var mergingImageDatas = new List<ImageData>();


        //            if (right1 != null)
        //                IsCurrentIntersectedWithRight1 = IsIntersectX(current, right1);

        //            if (right2 != null)
        //                IsCurrentIntersectedWithRight2 = IsIntersectX(current, right2);

        //            //after checking intersectionX for two position, start the merging, only keep the first one, abamdon the next ones

        //            if (IsCurrentIntersectedWithRight1 || IsCurrentIntersectedWithRight2)
        //            {
        //                mergingImageDatas.Add(current);
        //                if (IsCurrentIntersectedWithRight1)
        //                {
        //                    right1.IsBadData = true;
        //                    mergingImageDatas.Add(right1);
        //                }
        //                if (IsCurrentIntersectedWithRight2)
        //                {
        //                    right2.IsBadData = true;
        //                    mergingImageDatas.Add(right2);
        //                }
        //            }

        //            if (mergingImageDatas.Count == 0)
        //                continue;
        //            var newImageData = GetNewImageDataFormMergingData(mergingImageDatas);
        //            ImageDatasMerging.Add(newImageData);

        //        }
        //        //remove the bad data , the ones that get merged, assign the ImageDatasMerging to row
        //        //good pratice to dipose bitmap for baddata every time
        //        DisposeBitmaps(imageDatas.Where(x => x.IsBadData).ToList());

        //        imageDatas.RemoveAll(x => x.IsBadData);
        //        var imageDatasMerging = FromRetangelsToImageDatas(ImageDatasMerging, cropFromOriginBitmap, HaltCallback, ImagePostProcessingCallback);
        //        row.ImageDatasMerging = imageDatasMerging;
        //    }
        //}





        //public List<ImageData> FromRetangelsToImageDatas(List<ImageDataMerging> newMergingImageDatas, bool cropFromOriginBitmap, Func<int, bool> HaltCallback = null, Func<Bitmap, Bitmap> ImagePostProcessingCallback = null)
        //{
        //    var ImageDatas = new List<ImageData>();
        //    try
        //    {
        //        int count = 0;
        //        foreach (var newMergingImageData in newMergingImageDatas)
        //        {
        //            //Bitmap bmp = null;
        //            //if (cropFromOriginBitmap)
        //            //{
        //            //    bmp = frame.CropFromImageEMGUCVGray(newMergingImageData.MergingRectangle);
        //            //}
        //            //else
        //            //    bmp = binarizedFrame.CropFromImageEMGUCVGray(newMergingImageData.MergingRectangle);
        //            //bmp = ImagePostProcessingCallback != null ? ImagePostProcessingCallback(bmp) : bmp;

        //            Bitmap bmp = CropAndGetBmp(newMergingImageData.MergingRectangle, cropFromOriginBitmap, HaltCallback, ImagePostProcessingCallback);
        //            var imageData = new ImageData { BoundingRectangle = newMergingImageData.MergingRectangle, bitmap = bmp };
        //            ImageDatas.Add(imageData);
        //            if (HaltCallback != null && HaltCallback(count++) == true)
        //                break;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Error("Reanalyze of merging ImageDatas Fails", exception);
        //        throw exception;

        //    }
        //    return ImageDatas;
        //}


        //public void FromRetangelsToImageDatas1(List<ImageData> ImageDatas, bool cropFromOriginBitmap, Func<int, bool> HaltCallback = null, Func<Bitmap, Bitmap> ImagePostProcessingCallback = null)
        //{
        //    try
        //    {
        //        int count = 0;
        //        foreach (var imageData in ImageDatas)
        //        {
        //            imageData.bitmap = CropAndGetBmp(imageData.RectangleFinal, cropFromOriginBitmap, HaltCallback, ImagePostProcessingCallback);
        //            if (HaltCallback != null && HaltCallback(count++) == true)
        //                break;
        //        }
        //    }catch(Exception exception) { 

        //           Logger.Error("Fail to crop images by Rectangels", exception);
        //           throw exception;
        //       }

        //}


        //public Bitmap CropAndGetBmp(Rectangle rect, bool cropFromOriginBitmap, Func<int, bool> HaltCallback = null, Func<Bitmap, Bitmap> ImagePostProcessingCallback = null)
        //{
        //    Bitmap bmp = null;
        //    if (cropFromOriginBitmap)
        //    {
        //        bmp = frame.CropFromImageEMGUCVGray(rect);
        //    }
        //    else
        //        bmp = grayFrame.CropFromImageEMGUCVGray(rect);
        //    bmp = ImagePostProcessingCallback != null ? ImagePostProcessingCallback(bmp) : bmp;
        //    return bmp;
        //}

        /// <summary>
        /// http://www.emgu.com/forum/viewtopic.php?t=118
        /// return a totally new ImageDatamerging with nothing but a new rectangel
        /// </summary>
        /// <param name="mergingImageDatas"></param>
        /// <returns></returns>
        public ImageDataMerging GetNewImageDataFormMergingData(List<ImageData> mergingImageDatas)
        {
            if (mergingImageDatas == null || mergingImageDatas.Count < 2)
                throw new ArgumentException("at least 2 contours for merging");
            //take first ImageData as base

            Rectangle newRect = new Rectangle();
            using (MemStorage stor = new MemStorage())
            {
                Contour<System.Drawing.Point> contour = new Contour<Point>(stor);
                Enumerable.Range(0, mergingImageDatas.Count).ToList().ForEach(index =>
                {
                    var imageData = mergingImageDatas[index];
                    imageData.IsBadData = true;
                    foreach (var p in imageData.Contour)
                    {
                        contour.Push(p);
                    }
                });
                Seq<Point> convexHull = contour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                newRect = convexHull.GetMinAreaRect().MinAreaRect();
            }

            return new ImageDataMerging(newRect);
        }


        //public List<ImageDatasPerRow> SplitImageDatasIntoRows(List<ImageData> imageDatas, string BigString)
        //{
        //    var ImageDataRows = new List<ImageDatasPerRow>();
        //    imageDatas = imageDatas.OrderBy(x => x.Contour.BoundingRectangle.Y).ToList();


        //    var total = imageDatas.Count();

        //    var ImageDatasRowCache = new ImageDatasPerRow();

        //    var Row = 1;
        //    for (var i = 0; i < total; i++)
        //    {
        //        var current = imageDatas[i];
        //        //use ImageDatasLastRow as cache, clear it at row break, so the last row data is cached at the end
        //        ImageDatasRowCache.ImageDatas.Add(current);
        //        if (i == total - 1)
        //            break;
        //        //if index is already total -1, jump out of the loop
        //        var next = imageDatas[i + 1];

        //        //all letters such as i, j, = are identified as two contours instead of one, therefore 
        //        //perform extra IsRowBreak for the previous as well, if two rowbreak or more are true then it is true
        //        bool isRowBreak = false;
        //        if (i == 0)
        //            isRowBreak = IsRowBreak(current, next, i);
        //        else
        //        {
        //            var previous = imageDatas[i - 1];
        //            isRowBreak = IsRowBreak(previous, next, i) && IsRowBreak(current, next, i);
        //        }
        //        if (isRowBreak)
        //        {
        //            var ImageDatasPerRow = new ImageDatasPerRow { Row = Row };
        //            var dataPerRow = ImageDatasRowCache.ImageDatas.OrderBy(x => x.Contour.BoundingRectangle.X).ToList();
        //            ImageDatasPerRow.ImageDatas.AddRange(dataPerRow);
        //            ImageDataRows.Add(ImageDatasPerRow);

        //            //clear the cache row
        //            ImageDatasRowCache.ImageDatas.Clear();
        //            Row++;
        //        }
        //    }
        //    //dont forget to sort the last row data
        //    ImageDatasRowCache.ImageDatas = ImageDatasRowCache.ImageDatas.OrderBy(x => x.Contour.BoundingRectangle.X).ToList();
        //    ImageDatasRowCache.Row = Row;
        //    //add the ImageDatasRowCache as last row data
        //    ImageDataRows.Add(ImageDatasRowCache);
        //    return ImageDataRows;
        //}

        protected bool IsRowBreak(ImageData Current, ImageData Next, int index)
        {


            var currentUpBound = Current.Contour.BoundingRectangle.Y;
            var currentBottomBound = currentUpBound + Current.Contour.BoundingRectangle.Height;

            var nextUpBound = Next.Contour.BoundingRectangle.Y;
            var nextBottomBound = nextUpBound + Next.Contour.BoundingRectangle.Height;


            //if next contour Y is not intersected with current contour Y, it is a row break

            var IsRowBreak = false;
            //if the difference is smaller than 20, let's assume the gap is too small 
            var diffUpBottom = nextUpBound - currentBottomBound;
            if (currentUpBound == nextUpBound || diffUpBottom < 20)
                return IsRowBreak;

            //current contour Y is intersected with next contour Y axis
            var testCase1 = currentUpBound > nextUpBound && currentUpBound < nextBottomBound;
            var testCase2 = currentBottomBound > nextUpBound && currentBottomBound < nextBottomBound;

            //next contour Y is intersected with current contour Y axis
            var testCase3 = nextUpBound > currentUpBound && nextUpBound < currentBottomBound;
            var testCase4 = nextBottomBound > currentUpBound && nextBottomBound < currentBottomBound;
            if (!testCase1 && !testCase2 && !testCase3 && !testCase4)
            {


                IsRowBreak = true;
            }
            return IsRowBreak;
        }

        protected bool IsIntersectX(ImageData Current, ImageData Next)
        {
            bool IsIntersectedX = false;
            var currentLeftBound = Current.Contour.BoundingRectangle.X;
            var currentRightBound = currentLeftBound + Current.Contour.BoundingRectangle.Width;
            var nextLeftBound = Next.Contour.BoundingRectangle.X;
            var nextRightBound = nextLeftBound + Next.Contour.BoundingRectangle.Width;


            var test1 = currentLeftBound > nextLeftBound && currentLeftBound < nextRightBound;
            var test2 = currentRightBound > nextLeftBound && currentLeftBound < nextRightBound;
            var test3 = nextLeftBound > currentLeftBound && nextLeftBound < currentRightBound;
            var test4 = nextRightBound > currentLeftBound && nextRightBound < currentRightBound;



            if (test1 || test2 || test3 || test4)
                IsIntersectedX = true;
            return IsIntersectedX;
        }

        #endregion



        #region memery dispose
        public void DisposeResource()
        {
            grayFrame.Dispose();
            frame.Dispose();
        }

        public void DisposeResourceAfterContoursToImages()
        {
            ClearEMGUCVMemory(cannyFrame);
            //ClearEMGUCVMemory(grayFrame);
            ClearEMGUCVMemory(smoothedGrayFrame);
        }

        protected void DisposeBitmaps(List<ImageData> ImageDatas)
        {
            foreach (var data in ImageDatas)
            {
                data.bitmap.Dispose();
                data.featureVector = null;
            }
        }

        public void ClearEMGUCVMemory(Image<Gray, byte> image)
        {
            if (image != null)
                image.Dispose();
        }

        #endregion


























        public List<Contour<Point>> ExtractContours(Bitmap inputImg)
        {
            var contours = new List<Contour<Point>>();
            using (var frame = new Image<Bgr, byte>(inputImg))
            {
                using (var grayFrame = frame.Convert<Gray, Byte>())
                {
                    //autocontrast
                    //grayFrame._EqualizeHist();
                    ////binarize
                    //CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255,
                    //    Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C,
                    //    Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, 3 + 3 % 2 + 1, 1.2d);

                    try
                    {

                        CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255,
                            Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,
                            Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, 3 + 3 % 2 + 1, 0.2d);


                        grayFrame.Bitmap.Save(@"C:\Users\weihang\Documents\visual studio 2015\Projects\SingleCharacterCollect\SingleCharacterCollectTest\Files\ok.jpg");
                        //revert
                        //grayFrame._Not();
                        //var c = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE,
                        //    Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);


                        var c = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE,
                           Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
                        var contourList = c.AsContourList();

                        contours = FilterContours(contourList,
                                                  grayFrame,
                                                  filterContoursBySize: true,
                                                  noiseFilter: true);
                    }
                    catch (Exception e)
                    {
                        var ttt = e.ToString();
                    }

                }
            }


            return contours;
        }





        private List<Contour<Point>> FilterContours(List<Contour<Point>> contours, Image<Gray, byte> frame,
             bool filterContoursBySize = false, int minContourLength = 20,
            int minContourArea = 10, double minFormFactor = 0.5, bool noiseFilter = false)
        {
            var frameWidth = frame.Width;
            var frameHeight = frame.Height;
            int maxArea = frameWidth * frameHeight / 5;

            List<Contour<Point>> result = new List<Contour<Point>>();

            foreach (var c in contours)
            {
                if (filterContoursBySize)
                    if (c.Total < minContourLength ||
                        c.Area < minContourArea || c.Area > maxArea ||
                        c.Area / c.Total <= minFormFactor)
                        continue;

                if (noiseFilter)
                {
                    Point p1 = c[0];
                    Point p2 = c[(c.Total / 2) % c.Total];
                    if (frame[p1].Intensity <= double.Epsilon && frame[p2].Intensity <= double.Epsilon)
                        continue;
                }
                result.Add(c);

            }

            return result;
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




        public void Test(Bitmap inputImg)
        {
            using (var frame = new Image<Bgr, byte>(inputImg))
            {
                using (var grayFrame = frame.Convert<Gray, Byte>())
                {
                    //autocontrast
                    grayFrame._EqualizeHist();
                    grayFrame._GammaCorrect(1.8d);
                    ////binarize
                    //CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255,
                    //    Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C,
                    //    Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, 3 + 3 % 2 + 1, 1.2d);

                    try
                    {

                        CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255,
                            Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,
                            Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, 3 + 3 % 2 + 1, 0.2d);


                        grayFrame.Bitmap.Save(@"D:\Test\ok1.jpg");
                        //revert
                        //grayFrame._Not();
                        //var c = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE,
                        //    Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);


                    }
                    catch (Exception e)
                    {
                        var ttt = e.ToString();
                    }

                }
            }


        }





    }
}
