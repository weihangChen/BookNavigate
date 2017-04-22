using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Infrastructure.Extensions
{
    public static class ByteExtension
    {
        public static int ReadInt32BigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            var num = BitConverter.ToInt32(bytes, 0);
            return num;
        }

        public static void WriteInt32BigEndian(this BinaryWriter writer, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            writer.Write(bytes);
            
        }

        public static ImageData ReadAsImage(this BinaryReader reader, int xmax, int ymax)
        {
            var data = new ImageData();
            var bitmap = new Bitmap(xmax, ymax);
            var colors = new List<byte>();
            for (int y = 0; y < ymax; ++y)
            {
                for (int x = 0; x < xmax; ++x)
                {

                    byte b = reader.ReadByte();
                    colors.Add(b);
                    var color = (int)b;
                    bitmap.SetPixel(x, y, Color.FromArgb(color, color, color));

                }
            }
            data.bitmap = bitmap;
            data.colors = colors;
            return data;

        }
    }
}
