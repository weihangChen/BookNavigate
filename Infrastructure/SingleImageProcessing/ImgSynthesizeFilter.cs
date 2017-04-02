using System.Drawing;

namespace Infrastructure.SingleImageProcessing
{
    //different methods for image bluring options
    //http://docs.opencv.org/trunk/dc/dd3/tutorial_gausian_median_blur_bilateral_filter.html
    //sample code
    //https://github.com/opencv/opencv/blob/master/samples/cpp/tutorial_code/ImgProc/Smoothing.cpp
    //http://docs.opencv.org/trunk/d7/da8/tutorial_table_of_content_imgproc.html
    //https://github.com/opencv/opencv/tree/master/samples/cpp/tutorial_code
    //todo replace all Image<frame,color> with Mat 
    //ex. Mat _frameArrayBuffer = CvInvoke.Imread("", LoadImageType.AnyColor);

    /// <summary>
    /// all impl should following the guidelines from 3.3 Synthetic Data Generation at this paper
    /// https://www.robots.ox.ac.uk/~vgg/publications/2014/Jaderberg14/jaderberg14.pdf
    /// </summary>
    public interface ImgSynthesizeFilter
    {
        Bitmap Process(Bitmap inputImg);
    }
}
