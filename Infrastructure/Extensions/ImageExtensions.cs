using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Infrastructure.Extensions
{
    public static class ImageExtensions
    {
        public static void FromBytesToFile(this byte[] bytes, string path)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image i = Image.FromStream(ms);
                i.Save(path);
                //always recycle bitmap manually
                i.Dispose();
            }
        }

        public static Image FromFileToImage(this string path)
        {
            return FromFileToBytes(path).FromBytesToImage();
        }

        public static byte[] FromFileToBytes(this string path)
        {
            return File.ReadAllBytes(path);
        }

       

        public static byte[] ImageToByte(this Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Image ByteToImage(this byte[] bytes)
        {
            return Image.FromStream(new MemoryStream(bytes));
        }


        public static Bitmap Rezie(this Bitmap image, int width, int height)
        {
            Image<Bgr, byte> img = new Image<Bgr, byte>(image);
            Image<Bgr, byte> cpimg = img.Resize(width, height, Inter.Linear);
            return cpimg.Bitmap;
        }

        /// <summary>
        /// kowning only size of one dimension, scale the other dimension by comparing with origianl image
        /// </summary>
        /// <param name="image">to be resized image</param>
        /// <param name="size">known new size</param>
        /// <param name="XOrY">known size is at X-axis or Y-axis</param>
        /// <returns></returns>
        public static Bitmap RezieWithOneKnowAxis(this Bitmap image, int size, Axis axis)
        {
            int width = 0;
            int height = 0;
            if (axis == Axis.X)
            {
                width = size;
                height = Convert.ToInt32(image.Height * (width / image.Width));
            }
            else if (axis == Axis.Y)
            {
                height = size;
                width = Convert.ToInt32(image.Width * (height / image.Height));
            }

            return Rezie(image, width, height);
        }



        public static Image FromBytesToImage(this byte[] bytes)
        {
            Image i;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                i = Image.FromStream(ms);
            }
            return i;
        }

        public static Image CropImage(this Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }


        public static void DrawContoursOnImage(this Image img, List<Rectangle> recs)
        {
            Font font = new Font("Times New Roman", 13.0f);
            Brush bgBrush = new SolidBrush(Color.Goldenrod);

            using (Graphics g = Graphics.FromImage(img))
            {
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#ff6bb5"), 1))
                {

                    recs.ForEach(rec =>
                    {
                        Point p = new Point((rec.Left + rec.Right) / 2, rec.Top);
                        g.DrawRectangle(pen, rec);
                        //g.DrawString(imageData.Text, font, bgBrush, new PointF(p.X + 1 - font.Height / 3, p.Y + 1 - font.Height));
                        //g.DrawString(imageData.label.ToString(), font, Brushes.Black, 5, 5);
                    });
                }
            }
        }

        public static double[] ConvertImageToOneDimensionArray(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            double[] features = new double[width * height];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    var pixel = bmp.GetPixel(j, i);
                    features[i * width + j] = (pixel.R == 255) ? 0 : 1;
                }
            return features;
        }


        public static int[][] ConvertImageToTwoDimensionArray(this Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int[][] features = new int[width][];
            for (int i = 0; i < width; i++)
            {
                features[i] = new int[height];
                for (int j = 0; j < height; j++)
                {
                    var pixel = bmp.GetPixel(i, j);
                    if (pixel.R != 255)
                    {
                        var t = pixel.R;
                    }
                    features[i][j] = (pixel.R == 255) ? 0 : 1;
                }
            }
            return features;
        }

        public static Bitmap CropFromBitmap(this Bitmap source, Rectangle rec, GraphicsUnit graphicsUnit = GraphicsUnit.Pixel)
        {
            if (rec.Width == 0 || rec.Height == 0)
                throw new ArgumentException("the to be crop rec width or height is 0");
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(rec.Width, rec.Height);
                var graphics = Graphics.FromImage(bmp);
                //graphics.SetupGraphic();
                graphics.DrawImage(source, 0, 0, rec, graphicsUnit);
            }
            catch (Exception e)
            {
                throw e;
            }


            return bmp;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.SetupGraphic();
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void SetupGraphic(this Graphics graphics)
        {
            //sourcecopy is kind of important to get higher predication rate
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }
        public static Bitmap CropImageToEdge(this Bitmap bmp, string txt = "", string font = "")
        {

            var imageRawData = new Dictionary<int, List<double>>();
            Rectangle edgeRetangle = new Rectangle();

            try
            {
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

                if (darkRows.Count() == 0)
                {
                    throw new Exception("stop the porgram since no dark color row is found");
                }


                var firstDarkRowIndex = darkRows.First().Key;
                var lastDarkRowIndex = darkRows.Last().Key;

                //rightMostIndex - leftMostIndex
                var letterWidth = rightMostIndex - leftMostIndex + 1;
                var letterHeight = lastDarkRowIndex - firstDarkRowIndex + 1;
                //the right index and left index is same, set the rectangle width to 1
                letterWidth = letterWidth == 0 ? 1 : letterWidth;
                letterHeight = letterHeight == 0 ? 1 : letterHeight;
                //modify the leftMostIndex and firstDarkRowIndex
                leftMostIndex = leftMostIndex > 1 ? leftMostIndex : leftMostIndex - 1;
                firstDarkRowIndex = firstDarkRowIndex > 1 ? firstDarkRowIndex : firstDarkRowIndex - 1;


                edgeRetangle = new Rectangle(leftMostIndex, firstDarkRowIndex, letterWidth, letterHeight);
            }
            catch (Exception e)
            {
                throw e;
            }
            var newBmp = bmp.CropFromBitmap(edgeRetangle);
            return newBmp;

        }


        /// <summary>
        /// convert pixel data + height + width into bitmap
        /// </summary>
        /// <param name="widthOrigin"></param>
        /// <param name="heightOrigin"></param>
        /// <param name="pixels"></param>
        /// <param name="mag">magnify</param>
        /// <returns></returns>
        public static Bitmap GetBitmap(this byte[][] pixels, int widthOrigin, int heightOrigin, int mag)
        {
            // create a C# Bitmap suitable for display in a PictureBox control
            int width = widthOrigin * mag;
            int height = heightOrigin * mag;
            Bitmap result = new Bitmap(width, height);
            Graphics gr = Graphics.FromImage(result);
            for (int i = 0; i < heightOrigin; ++i)
            {
                for (int j = 0; j < widthOrigin; ++j)
                {
                    int pixelColor = 255 - pixels[i][j]; // white background, black digits
                    //int pixelColor = dImage.pixels[i][j]; // black background, white digits
                    Color c = Color.FromArgb(pixelColor, pixelColor, pixelColor); // gray scale
                    //Color c = Color.FromArgb(pixelColor, 0, 0); // red scale
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * mag, i * mag, mag, mag); // fills bitmap via Graphics object
                }
            }
            return result;
        }


        public static List<byte> GetPixelsForOneImage(this Bitmap img, int xmax, int ymax)
        {
            var pixels = new List<byte>();
            //var imgResized = (Bitmap)img.ResizeImage(size, size);

            //vertical pixel iteration first
            //for (int x = 0; x < size; x++)
            //{
            //    for (int y = 0; y < size; y++)
            //    {
            //        Color pixel = imgResized.GetPixel(x, y);
            //        var pixelValue = Convert.ToInt32((pixel.R + pixel.G + pixel.B) / 3);
            //        pixels.Add(pixelValue);
            //    }
            //}

            //horizontal pixel iteration first
            for (int y = 0; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    var pixelValue = Convert.ToInt32((pixel.R + pixel.G + pixel.B) / 3);
                    pixels.Add((byte)pixelValue);
                }
            }

            return pixels;
        }
    }
}
