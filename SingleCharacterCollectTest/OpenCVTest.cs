using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHunspell;
using System.Collections.Generic;
using Emgu.CV;
using System.Drawing;
using System.IO;
using Emgu.CV.Structure;

namespace SingleCharacterCollectTest
{
    [TestClass]
    public class OpenCVTest

    {
        [TestMethod]
        public void FindContourTest()
        {
            try
            {
                Bitmap inputImg = new System.Drawing.Bitmap(@"..\..\Files\test.jpg");
                using (var frame = new Image<Bgr, byte>(inputImg))
                {
                    using (var grayFrame = frame.Convert<Gray, Byte>())
                    {
                        var c = grayFrame.FindContours();
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }

        }
    }
}
