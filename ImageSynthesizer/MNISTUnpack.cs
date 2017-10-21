using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Infrastructure.Extensions;
using System.Linq;


namespace ImageSynthesizer
{
    public class MNISTUnpack
    {

        public static void UnpackByteFileToImages(string lablePath, string imagePath, string dest, bool resaveByteFiles, bool saveImages, bool orderbylabel)
        {
            //create 10 directories under the specific root dir
            DirGenerator.CreateDirs(StringResources.Digits.Select(x=>x.ToString()).ToList(), dest);
            //create stream and reader for label and image byte file reading
            var labelStream = new FileStream(lablePath, FileMode.Open, FileAccess.Read);
            var labelReader = new BinaryReader(labelStream);
            var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            var imageReader = new BinaryReader(imageStream);
            //verify label file
            var labelFileLength = labelStream.Length;
            var lmagic = labelReader.ReadInt32BigEndian();
            var lnum = labelReader.ReadInt32BigEndian();
            if (labelFileLength != 60008 || lmagic != 2049 || lnum != 60000)
            {
                throw new ArgumentException("label byte file corrupted");
            }
            //verify image file
            int magic = imageReader.ReadInt32BigEndian();
            int numImages = imageReader.ReadInt32BigEndian();
            int numRows = imageReader.ReadInt32BigEndian();
            int numCols = imageReader.ReadInt32BigEndian();
            if (magic != 2051 || numImages != 60000 || numRows != 28 || numCols != 28)
                throw new ArgumentException("image byte file corrupted");
            //read and save images to hard disk
            var imageDatas = new List<ImageData>();
            try
            {
                while (true)
                {
                    var imageData = imageReader.ReadAsImage(28, 28);
                    int label = labelReader.ReadByte();
                    imageData.Label = label.ToString();
                    imageDatas.Add(imageData);


                }
            }
            catch (EndOfStreamException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            labelReader.Close();
            imageReader.Close();
            if (saveImages)
            {
                foreach (var imageData in imageDatas)
                {
                    var labelDir = Path.Combine(dest, imageData.Label);
                    var imgPath = $"{labelDir}\\{Guid.NewGuid().ToString()}.jpg";
                    imageData.bitmap.Save(imgPath);
                }
            }

            if (resaveByteFiles)
            {
                //write bytes to label file
                var labelWriter = new BinaryWriter(new FileStream("mnistlabel", FileMode.Create));
                var imageDataWriter = new BinaryWriter(new FileStream("mnistimage", FileMode.Create));
                if (orderbylabel)
                {
                    imageDatas = imageDatas.OrderBy(x => Convert.ToInt32(x.Label)).ToList();
                }

                ByteFileGenerator.WriteDataToFile(imageDatas, labelWriter, imageDataWriter, 60000, 28);
            }
        }
    }
}
