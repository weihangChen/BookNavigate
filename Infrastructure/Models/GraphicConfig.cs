using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class GraphicConfig
    {
        //default value constructor
        public GraphicConfig()
        {
            this.Font = new Font("Arial", 16);
            this.Brush = new SolidBrush(Color.Black);
            this.Point = new PointF(150.0F, 150.0F);

            this.Format = new StringFormat();
            this.Format.Alignment = StringAlignment.Center;
            this.ImageHeight = 32;
            this.ImageWidth = 32;
        }

        public GraphicConfig(Font Font, SolidBrush Brush, PointF Point, StringFormat Format, int ImageHeight, int ImageWidth)
        {
            this.Font = Font;
            this.Brush = Brush;
            this.Point = Point;
            this.ImageHeight = ImageHeight;
            this.ImageWidth = ImageWidth;
            this.Format = Format;


        }
        public Font Font { get; set; }
        public SolidBrush Brush { get; set; }
        public PointF Point { get; set; }

        public StringFormat Format { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
    }
}
