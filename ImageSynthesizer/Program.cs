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
            foreach (var folder in folders)
            {
                var images = Directory.GetFiles(folder)
                    .Select(x => Image.FromFile(x))
                    .ToList();
                synthesizer.ProcessImages(images);

            }

        }
    }
}
