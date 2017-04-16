using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace SingleCharacterCollectTest
{
    [TestClass]
    public class ByteDateTest
    {
        /// <summary>
        /// create a file, write integers to the file, save it, read it, check the binary data
        /// </summary>
        /// 
        [TestMethod]
        public void Test1()
        {
            File.Delete("test");
            var writer = new BinaryWriter(new FileStream("test", FileMode.CreateNew));

            writer.Write(1);
            writer.Write(9);

            writer.Flush();
            writer.Close();

            //verify total bytes
            var info = new FileInfo("test");
            //one integer is 4 bytes
            Assert.IsTrue(info.Length == 8);
            //convert the bytes back to integer see if it is 1
            var reader = new BinaryReader(new FileStream("test", FileMode.Open));
            var bytes = reader.ReadBytes(4);
            var digit = BitConverter.ToInt32(bytes, 0);
            Assert.IsTrue(digit == 1);
            
            var bytes1 = reader.ReadBytes(4);
            var digit1 = BitConverter.ToInt32(bytes1, 0);
            Assert.IsTrue(digit1 == 9);
        }
    }
}
