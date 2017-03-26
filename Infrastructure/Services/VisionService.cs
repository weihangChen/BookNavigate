using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;


namespace Infrastructure.Services
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

