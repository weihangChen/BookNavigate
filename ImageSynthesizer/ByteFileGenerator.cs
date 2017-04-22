using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using Infrastructure.Extensions;

namespace ImageSynthesizer
{
    public class ByteFileGenerator
    {
        /// <summary>
        /// normalize size of images, put all images and its identity into two byte files as for MNIST
        /// one file with image data
        /// second file with image label data
        /// 
        /// shuffle will make sure that the imagedata are organized in random order, so machine learning training don't need to shuffle it 
        /// </summary>
        public static void GenerateByteFile(string fontDataDirDest, int size, bool shuffle)
        {
            var imageData = ConfigurationManager.AppSettings["ImageData"];
            var imageLabel = ConfigurationManager.AppSettings["ImageLabel"];
            File.Delete(imageData);
            File.Delete(imageLabel);




            //get the total count
            var total = 0;
            foreach (var folder in Directory.GetDirectories(fontDataDirDest))
            {
                var tmp = folder.Split('\\');
                var charIdentity = StringResources.FolderToLetterMapping[tmp[tmp.Length - 1]];
                total += Directory.GetFiles(folder).Count();

            }






            var imageDatas = new List<ImageData>();

            foreach (var folder in Directory.GetDirectories(fontDataDirDest))
            {
                var tmp = folder.Split('\\');
                var charIdentity = StringResources.FolderToLetterMapping[tmp[tmp.Length - 1]];

                foreach (var file in Directory.GetFiles(folder))
                {
                    //var img = Image.FromFile(file) as Bitmap;
                    imageDatas.Add(new ImageData { Path = file, Label = charIdentity });
                    
                }
            }
            //write data to file
            var labelWriter = new BinaryWriter(new FileStream(imageLabel, FileMode.CreateNew));
            var imageDataWriter = new BinaryWriter(new FileStream(imageData, FileMode.CreateNew));
            WriteDataToFile(imageDatas, labelWriter, imageDataWriter, total, size, shuffle);

            //data integrity check
            var labels = imageDatas.Select(x => Convert.ToInt32(x.Label)).ToList();
            VerifyLabelByteFile(imageLabel, labels);
            VerifyImageDataByteFile(imageData, imageDatas.Count(), size);

        }

        public static void WriteDataToFile(List<ImageData> imageDatas, BinaryWriter labelWriter, BinaryWriter imageDataWriter, int total, int size, bool shuffle)
        {
            if (shuffle)
            {
                imageDatas.Shuffle();
            }


            //label byte file 
            labelWriter.WriteInt32BigEndian(2049);
            labelWriter.WriteInt32BigEndian(total);
            foreach (var data in imageDatas)
            {
                var label = Convert.ToInt32(data.Label);
                labelWriter.Write((byte)label);
            }
            labelWriter.Flush();
            labelWriter.Close();

            //image byte file
            imageDataWriter.WriteInt32BigEndian(2051);
            imageDataWriter.WriteInt32BigEndian(total);
            imageDataWriter.WriteInt32BigEndian(size);
            imageDataWriter.WriteInt32BigEndian(size);
            foreach (var data in imageDatas)
            {
                var img = Image.FromFile(data.Path) as Bitmap;
                var bytes = img.GetPixelsForOneImage(size, size).ToArray();
                imageDataWriter.Write(bytes);
            }
            imageDataWriter.Flush();
            imageDataWriter.Close();
        }


        private static void VerifyLabelByteFile(string imageLabel, List<int> labels)
        {
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
        }
        private static void VerifyImageDataByteFile(string imageData, int count, int size)
        {
            var info1 = new FileInfo(imageData);
            //12 is three digits, 4 bytes for image count, 4 bytes for X, 4 bytes for Y
            if (info1.Length != count * size * size + 12)
                throw new ArgumentException("total byte count not match");


        }
    }
}
