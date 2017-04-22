using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSynthesizer
{
    public class ByteFileGenerator
    {
        /// <summary>
        /// normalize size of images, put all images and its identity into two byte files as for MNIST
        /// one file with image data
        /// second file with image label data
        /// </summary>
        public static void GenerateByteFile()
        {
            var imageData = ConfigurationManager.AppSettings["ImageData"];
            var imageLabel = ConfigurationManager.AppSettings["ImageLabel"];
            File.Delete(imageData);
            File.Delete(imageLabel);


            var imageDataWriter = new BinaryWriter(new FileStream(imageData, FileMode.CreateNew));
            var labelWriter = new BinaryWriter(new FileStream(imageLabel, FileMode.CreateNew));
            //get the total count
            var total = 0;
            foreach (var folder in Directory.GetDirectories(FontDataDirDest))
            {
                var tmp = folder.Split('\\');
                var charIdentity = StringResources.FolderToLetterMapping[tmp[tmp.Length - 1]];
                total += Directory.GetFiles(folder).Count();

            }

            labelWriter.Write(total);
            imageDataWriter.Write(total);
            imageDataWriter.Write(size);
            imageDataWriter.Write(size);






            List<int> labels = new List<int>();
            foreach (var folder in Directory.GetDirectories(FontDataDirDest))
            {
                var tmp = folder.Split('\\');
                var charIdentity = StringResources.FolderToLetterMapping[tmp[tmp.Length - 1]];

                foreach (var file in Directory.GetFiles(folder))
                {
                    var img = Image.FromFile(file) as Bitmap;
                    var pixels = img.GetPixelsForOneImage(size, size);
                    //one pixel's color one byte writes to data writer
                    pixels.ForEach(pixel => imageDataWriter.Write(pixel));
                    //one integer as one byte writes to label writer
                    labelWriter.Write((byte)(Convert.ToInt32(charIdentity)));
                    //add identity as record
                    labels.Add(Convert.ToInt32(charIdentity));
                }
            }
            imageDataWriter.Flush();
            imageDataWriter.Close();
            labelWriter.Flush();
            labelWriter.Close();

            //----------------------------
            //----------------------------

            //label binary file verify
            var info = new FileInfo(imageLabel);
            if (info.Length != labels.Count + 4)
                throw new ArgumentException("total byte count not match");

            var reader = new BinaryReader(new FileStream(imageLabel, FileMode.Open));
            if (labels.Count != reader.ReadInt32())
                throw new Exception("total incorrect");
            int count = 0;
            while (true)
            {

                var label = (int)(reader.ReadByte());
                if (label != labels[count])
                    throw new ArgumentException("saved byte data not match the content");

                count++;
                if (count == labels.Count)
                    break;
            }




            //-----------------------------
            //verify image binary data length
            var info1 = new FileInfo(imageData);
            //12 is three digits, 4 bytes for image count, 4 bytes for X, 4 bytes for Y
            if (info1.Length != labels.Count * size * size + 12)
                throw new ArgumentException("total byte count not match");


        }

    }
}
