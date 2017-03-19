using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Extensions;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Drawing;
using Infrastructure.Models;
using infrastructure.Services;
using Emgu.CV;
using System;

namespace BookImageProcessor.Services
{
    public interface IImageExtractionService
    {
        Task ExtractChars();

    }
    public class ImageExtractionService : IImageExtractionService
    {
        static readonly string BookBaseDir = ConfigurationManager.AppSettings.Get("BookDataPath");
        const int ValidWordDigitCount = 4;
        const int WordLimit = 30;

        ISpellService SpellService;
        IVisionService VisionService;
        IOpenCVService OpenCVService;

        public ImageExtractionService()
        {
            VisionService = new VisionService(ConfigurationManager.AppSettings["OxfordKey"]);
            SpellService = new SpellService();
            OpenCVService = new ImageProcessingService();
        }

        protected void CreateFolders(List<string> imagePathsWithinOneBook)
        {
            foreach (var imagePath in imagePathsWithinOneBook)
            {
                Directory.CreateDirectory(imagePath);
            }
        }

        public async Task ExtractChars()
        {
            //one folder per book 
            foreach (var bookDir in Directory.GetDirectories(BookBaseDir))
            {
                foreach (var pageImage in Directory.GetFiles(bookDir))
                {
                    var pageImageBytes = pageImage.FromFileToBytes();
                    var words = await OCROnePage(pageImageBytes);
                    //if there are too few words at a page, don't process it
                    if (words.Count() <= WordLimit)
                        continue;


                    var pageImagePath = pageImage.Replace(".jpg", "");
                    Directory.CreateDirectory(pageImagePath);
                    ExtractWordIntoSingileChar(pageImageBytes, words, pageImagePath);



                }
            }
        }


        protected async Task<List<Word>> OCROnePage(byte[] pageImageBytes)
        {
            //send image to oxford, obtain coordinate and content
            var ocrResults = await VisionService.RecognizeText(pageImageBytes);
            //get all valid words with more than 4 characters
            var allLines = ocrResults.Regions.SelectMany(x => x.Lines).ToList();
            var allWords = allLines.SelectMany(p => p.Words).ToList();

            //be in valid length
            var allWordsValidLength = allWords.Where(x => x.Text.Count() > ValidWordDigitCount).ToList();
            //be in 62 char set
            var allValidWordsWithSixtyTwoChars = allWordsValidLength.Where(x => x.Text.ContainsOnlySpecificSetOfChars(StringResources.SixtyTwoChars)).ToList();
            //exists in ditionary
            var allValidWords = allValidWordsWithSixtyTwoChars.Where(x => SpellService.IsWordCorrect(x.Text)).ToList();
            return allValidWords;

        }


        //crop the bounding box from original image and save to disk
        protected void ExtractWordIntoSingileChar(byte[] pageBytes, IEnumerable<Word> words, string pageImagePath)
        {
            var fullImg = pageBytes.FromBytesToImage();
            foreach (var word in words)
            {
                var convertedWordRec = new System.Drawing.Rectangle(word.Rectangle.Left, word.Rectangle.Top,
                                                   word.Rectangle.Width, word.Rectangle.Height);
                using (var wordImg = fullImg.CropImage(convertedWordRec) as Bitmap)
                {
                    //generated contours should be ordrered from left to right, and been through merging 
                    //List<Contour<Point>> allContours = OpenCVService.ExtractContours(wordImg);
                    //only proceed if the count is the same
                    //if (allContours.Count() != word.Text.Count())
                    //    continue;

                    //for (var i = 0; i < word.Text.Count(); i++)
                    //{
                    //    var singleChar = word.Text[i];
                    //    var charContour = allContours[i];
                    //    using (var charImg = wordImg.CropImage(charContour.BoundingRectangle))
                    //    {
                    //        SaveCharImgToDisk(charImg, pageImagePath);
                    //    }
                    //}
                    SaveCharImgToDisk(wordImg, pageImagePath);

                }
            }
            //dispose fullimg
            fullImg.Dispose();
        }



        protected void SaveCharImgToDisk(Image charImage, string pageImagePath)
        {
            charImage.Save(pageImagePath + "/" + Guid.NewGuid().ToString() + ".jpg");
        }


    }
}
