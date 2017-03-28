using Infrastructure.Models;
using Infrastructure.Services;
using Infrastructure.SingleImageProcessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSynthesizer
{
    public class Synthesizer
    {

        private ImageProcessor ImageProcessor;
        List<IImageDecorator> imageDecorators;

        public Synthesizer()
        {
            ImageProcessor = new ImageProcessor();
        }

        //public void ProcessImages(TrainingDataDTOWindowsFont model)
        //{
        //    foreach (var img in model.ImageDatas)
        //    {
        //        ImageProcessor.Test(img.bitmap);
        //        //imageDecorators.ForEach(x => x.ProcessImage(img));
        //    }
        //}



        
    }


}
