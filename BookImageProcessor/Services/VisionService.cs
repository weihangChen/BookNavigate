using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using BookImageProcessor.Models;

namespace BookImageProcessor.Services
{
    public interface IVisionService
    {
        Task<OcrResults> RecognizeText(byte[] imageArrayData, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect);
    }



    public class VisionService : IVisionService
    {
        private readonly IVisionServiceClient visionClient;

        public VisionService(string subscriptionKey)
        {
            this.visionClient = new VisionServiceClient(subscriptionKey);
        }
     


        //protected void ConvertOCRResultToPageData(OcrResults results, PageData PageData)
        //{
        //    int LineNumber = 1;
        //    if (results != null && results.Regions != null)
        //    {
        //        foreach (var item in results.Regions)
        //        {
        //            foreach (var line in item.Lines)
        //            {
        //                var newLine = new LineData(LineNumber++);

        //                newLine.WordDatas = line.Words.Select(x => new WordData { Content = x.Text }).ToList();
        //                PageData.LineDatas.Add(newLine);
        //            }
        //        }
        //    }
        //}

        //protected string GetRetrieveText(OcrResults results)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();

        //    if (results != null && results.Regions != null)
        //    {
        //        stringBuilder.Append("Text: ");
        //        stringBuilder.AppendLine();
        //        foreach (var item in results.Regions)
        //        {
        //            foreach (var line in item.Lines)
        //            {
        //                foreach (var word in line.Words)
        //                {
        //                    stringBuilder.Append(word.Text);
        //                    stringBuilder.Append(" ");
        //                }

        //                stringBuilder.AppendLine();
        //            }

        //            stringBuilder.AppendLine();
        //        }
        //    }

        //    return stringBuilder.ToString();
        //}



        public async Task<OcrResults> RecognizeText(byte[] imageArrayData, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
        {
            OcrResults ocrResult = null;
            string resultStr = string.Empty;

            try
            {
                ocrResult = await this.visionClient.RecognizeTextAsync(new MemoryStream(imageArrayData), languageCode, detectOrientation);

            }

            catch (Exception ex)
            {
                throw ex;
            }

            return ocrResult;
        }
    }
}
