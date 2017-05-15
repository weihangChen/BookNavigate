using System;
using System.Drawing;

namespace Infrastructure.Services
{
    public interface ITextDrawer
    {
        Image DrawTextOnImage(String text, Font font, Color textColor, Color backColor);
    }


    public class TextDrawer : ITextDrawer
    {
        public Image DrawTextOnImage(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            //the only way to remove offset background is to use StringFormat
            //https://weblogs.asp.net/israelio/DrawString-_2F00_-MeasureString-Offset-Problem-Solved-_2100_
            StringFormat sFormat = new StringFormat(StringFormat.GenericTypographic);
            PointF origin = new PointF(0, 0);
            SizeF textSize = drawing.MeasureString(text, font, origin, sFormat);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            //high quality set up - start, else drawn text looks like crap
            //https://msdn.microsoft.com/en-us/library/a619zh6z%28v=vs.110%29.aspx
            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //drawing.SmoothingMode = SmoothingMode.AntiAlias;
            //drawing.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //high quality set up - end
            drawing.DrawString(text, font, textBrush, 0, 0, sFormat);



            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }


    }
}
