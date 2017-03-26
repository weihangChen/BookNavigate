using System.Drawing;

namespace Infrastructure.Models
{
    public class ImageDataMerging : ImageData
    {
        public ImageDataMerging(Rectangle MergingRectangle)
        {
            this.MergingRectangle = MergingRectangle;
        }
        public Rectangle MergingRectangle { get; set; }
    }
}
