using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImageSynthesizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = ConfigurationManager.AppSettings["DataDir"];
            var folders = Directory.GetDirectories(path);

            var synthesizer = new Synthesizer();
            //foreach (var folder in folders)
            //{
            //    var dto = new TrainingDataDTOWindowsFont();
            //    var imageDatas = Directory.GetFiles(folder)
            //        .Select(x => new ImageData {
            //            bitmap = Image.FromFile(x) as Bitmap,
            //            HardDiskPath = new DirectoryInfo(folder.ToString())
            //        })
            //        .ToList();
            //    dto.ImageDatas = imageDatas;
            //    synthesizer.ProcessImages(dto);

            //}

        }
    }
}
