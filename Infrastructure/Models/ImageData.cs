using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Infrastructure.Models
{
    public class ImageData
    {
        public Bitmap bitmap { get; set; }
        public Rectangle BoundingRectangle { get; set; }
        public DirectoryInfo HardDiskPath { get; set; }
        public Rectangle EdgeRetangle { get; set; }
        public string Label { get; set; }
        public string Path { get; set; }
   

        public double[] featureVector { get; set; }
        
        public string Text { get; set; }



        public byte[][] pixels;
        public List<byte> colors;
       

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
            colors = new List<byte>();
        }











    }

}
