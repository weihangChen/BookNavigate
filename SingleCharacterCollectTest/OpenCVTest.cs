//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;
//using System.Drawing;
//using Infrastructure.Extensions;
//using System.Linq;
//using Infrastructure.Services;

//namespace SingleCharacterCollectTest
//{

//    class Test
//    {
//        public Test()
//        {
//            datas = new List<TestData>();
//        }
//        public List<TestData> datas;

//    }
//    class TestData
//    {
//        public int position;
//        public int totalOne;
//        public bool isBad;
//        public Rectangle rec;
//    }
//    [TestClass]
//    public class OpenCVTest

//    {


//        [TestMethod]
//        public void FindContourTest()
//        {
//            try
//            {
//                Bitmap inputImg = new System.Drawing.Bitmap(@"..\..\Files\0f26f3a2-676b-42f4-8a5a-04a1418078a9.jpg");
//                //inputImg = inputImg.RezieWithOneKnowAxis(32, axis: Axis.Y);


//                var processor = new ImageProcessor();
//                var contours = processor.ExtractContours(inputImg);
//                var recs1 = contours.OrderBy(x=>x.BoundingRectangle.X).Select(x => x.BoundingRectangle).ToList();



//                //inputImg.DrawContoursOnImage(recs1.Skip(6).Take(1).ToList());
//                inputImg.DrawContoursOnImage(recs1.ToList());
//                inputImg.Save(@"..\..\Files\12.jpg");
//                //var test = processor.test(inputImg);
//                //test.Save(@"..\..\Files\12.jpg");
//                var features = inputImg.ConvertImageToTwoDimensionArray();
//                //the word is "walked", 6 chars, 7 lines that contains least R=255 will be found
//                //eye check for verification

//                var test = new Test();
//                for (int i = 0; i < features.Length; i++)
//                {
//                    var amoutOfOne = features[i].Count(x => x == 1);
//                    test.datas.Add(new TestData { position = i, totalOne = amoutOfOne });
//                }

//                //var contours = processor.ExtractContours(inputImg);
//                //inputImg.DrawContoursOnImage(contours);
//                //inputImg.Save(@"..\..\Files\12.jpg");
//                //using (var frame = new Image<Bgr, byte>(inputImg))
//                //{
//                //    using (var grayFrame = frame.Convert<Gray, Byte>())
//                //    {
//                //        var c = grayFrame.FindContours();
//                //    }
//                //}
//                var count = 8;


//                var height = features.FirstOrDefault().Length;
//                var tttt = test.datas.Select(x => x.totalOne).OrderBy(x => x).ToList();
//                var ti = test.datas.Select(x => x.totalOne).OrderBy(x => x).Take(count);
//                var datas = test.datas.Where(x => ti.Contains(x.totalOne)).ToList();
//                foreach (var d in datas)
//                {
//                    Console.WriteLine(d.position);
//                }
//                var newpositions = new List<int>();
//                if (datas.Count > count)
//                {
//                    for (int i = 0; i < datas.Count() - 1; i++)
//                    {

//                        var current = datas[i];

//                        var onestepahead = datas[i + 1];
//                        var distance = onestepahead.position - current.position;
//                        //at least we know that for height 18, "l" has a pixel of 5
//                        if (distance < 2)
//                        {
//                            current.isBad = true;
//                        }
//                        else
//                        {
//                            //if (i == 4)
//                            current.rec = new Rectangle(current.position, 0, distance, height);

//                        }
//                    }
//                    datas = datas.Where(x => !x.isBad).ToList();
//                }
//                var recs = datas.Where(x => x.rec != null).Select(x => x.rec).ToList();
//                inputImg.DrawContoursOnImage(recs);
//                inputImg.Save(@"..\..\Files\12.jpg");

//            }
//            catch (Exception e)
//            {
//                Assert.Fail(e.ToString());
//            }

//        }
//    }
//}
