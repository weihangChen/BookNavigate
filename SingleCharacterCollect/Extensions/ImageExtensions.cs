using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SingleCharacterCollect.Extensions
{
    public static class ImageExtensions
    {
        public static void FromBytesToFile(this byte[] bytes, string path)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image i = Image.FromStream(ms);
                i.Save(path);
            }
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


        public async static Task<byte[]> LoadImage(Uri uri)
        {
            var httpClient = new HttpClient();
            var data = await httpClient.GetByteArrayAsync(uri);
            return data;
        }
    }
}
