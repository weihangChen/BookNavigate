using System.Collections.Generic;
using System.Drawing;

namespace Infrastructure.Models
{
    public class TrainingDataDTO
    {
        public string dataFilePath { get; set; }

        public int Count { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int FeatureVectorSize { get; set; }
    }

    public class TrainingDataDTODigit : TrainingDataDTO
    {
        public TrainingDataDTODigit(string dataFilePath)
        {
            this.dataFilePath = dataFilePath;
            this.ImageDatas = new List<ImageData>();
        }
        public List<ImageData> ImageDatas { get; set; }
    }
    public class TrainingFontDataSet : TrainingDataDTO
    {
        public List<ImageData> ImageDatas { get; set; }
        public Rectangle containerRect { get; set; }

        public TrainingFontDataSet()
        {
            ImageDatas =  new List<ImageData>();
        }
    }
}
