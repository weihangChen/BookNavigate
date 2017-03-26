using Infrastructure.Models;
using Infrastructure.SingleImageProcessing;
using System.Collections.Generic;
namespace CharGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var imageDecorators = new List<IImageDecorator>();
            imageDecorators.Add(new ResizeDecorator(32, 32));
            //imageDecorators.Add(new NormalizeDecorator(CropStrategy.CropToEdge));


            var exporter = new FontImagePersistentExporter();

            var dto = exporter.ExportFontImages(FontResource.Fonts_Small, imageDecorators);
        }
    }
}
