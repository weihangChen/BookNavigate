using System.Collections.Generic;
using System.IO;
using Infrastructure.Services;
using Infrastructure.Models;
using System.Configuration;
using Infrastructure.SingleImageProcessing;

namespace CharGenerator
{
    /// <summary>
    /// contains the functions saving image to hard drive
    /// </summary>
    public class FontImagePersistentExporter : FontImageExporter
    {
        public static DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["ExportDir"]);

        public static Dictionary<string, DirectoryInfo> folderDic = new Dictionary<string, DirectoryInfo>() {
            { "A",new DirectoryInfo(Path.Combine(dir.ToString(), "AU"))},
            { "B",new DirectoryInfo(Path.Combine(dir.ToString(), "BU"))},
            { "C",new DirectoryInfo(Path.Combine(dir.ToString(), "CU"))},
            { "D",new DirectoryInfo(Path.Combine(dir.ToString(), "DU"))},
            { "E",new DirectoryInfo(Path.Combine(dir.ToString(), "EU"))},
            { "F",new DirectoryInfo(Path.Combine(dir.ToString(), "FU"))},
            { "G",new DirectoryInfo(Path.Combine(dir.ToString(), "GU"))},
            { "H",new DirectoryInfo(Path.Combine(dir.ToString(), "HU"))},
            { "I",new DirectoryInfo(Path.Combine(dir.ToString(), "IU"))},
            { "J",new DirectoryInfo(Path.Combine(dir.ToString(), "JU"))},
            { "K",new DirectoryInfo(Path.Combine(dir.ToString(), "KU"))},
            { "L",new DirectoryInfo(Path.Combine(dir.ToString(), "LU"))},
            { "M",new DirectoryInfo(Path.Combine(dir.ToString(), "MU"))},
            { "N",new DirectoryInfo(Path.Combine(dir.ToString(), "NU"))},
            { "O",new DirectoryInfo(Path.Combine(dir.ToString(), "OU"))},
            { "P",new DirectoryInfo(Path.Combine(dir.ToString(), "PU"))},
            { "Q",new DirectoryInfo(Path.Combine(dir.ToString(), "QU"))},
            { "R",new DirectoryInfo(Path.Combine(dir.ToString(), "RU"))},
            { "S",new DirectoryInfo(Path.Combine(dir.ToString(), "SU"))},
            { "T",new DirectoryInfo(Path.Combine(dir.ToString(), "TU"))},
            { "U",new DirectoryInfo(Path.Combine(dir.ToString(), "UU"))},
            { "V",new DirectoryInfo(Path.Combine(dir.ToString(), "VU"))},
            { "W",new DirectoryInfo(Path.Combine(dir.ToString(), "WU"))},
            { "X",new DirectoryInfo(Path.Combine(dir.ToString(), "XU"))},
            { "Y",new DirectoryInfo(Path.Combine(dir.ToString(), "YU"))},
            { "Z",new DirectoryInfo(Path.Combine(dir.ToString(), "ZU"))},
            { "1",new DirectoryInfo(Path.Combine(dir.ToString(), "1"))},
            { "2",new DirectoryInfo(Path.Combine(dir.ToString(), "2"))},
            { "3",new DirectoryInfo(Path.Combine(dir.ToString(), "3"))},
            { "4",new DirectoryInfo(Path.Combine(dir.ToString(), "4"))},
            { "5",new DirectoryInfo(Path.Combine(dir.ToString(), "5"))},
            { "6",new DirectoryInfo(Path.Combine(dir.ToString(), "6"))},
            { "7",new DirectoryInfo(Path.Combine(dir.ToString(), "7"))},
            { "8",new DirectoryInfo(Path.Combine(dir.ToString(), "8"))},
            { "9",new DirectoryInfo(Path.Combine(dir.ToString(), "9"))},
            { "0",new DirectoryInfo(Path.Combine(dir.ToString(), "0"))},
            { "a",new DirectoryInfo(Path.Combine(dir.ToString(), "a"))},
            { "b",new DirectoryInfo(Path.Combine(dir.ToString(), "b"))},
            { "c",new DirectoryInfo(Path.Combine(dir.ToString(), "c"))},
            { "d",new DirectoryInfo(Path.Combine(dir.ToString(), "d"))},
            { "e",new DirectoryInfo(Path.Combine(dir.ToString(), "e"))},
            { "f",new DirectoryInfo(Path.Combine(dir.ToString(), "f"))},
            { "g",new DirectoryInfo(Path.Combine(dir.ToString(), "g"))},
            { "h",new DirectoryInfo(Path.Combine(dir.ToString(), "h"))},
            { "i",new DirectoryInfo(Path.Combine(dir.ToString(), "i"))},
            { "j",new DirectoryInfo(Path.Combine(dir.ToString(), "j"))},
            { "k",new DirectoryInfo(Path.Combine(dir.ToString(), "k"))},
            { "l",new DirectoryInfo(Path.Combine(dir.ToString(), "l"))},
            { "m",new DirectoryInfo(Path.Combine(dir.ToString(), "m"))},
            { "n",new DirectoryInfo(Path.Combine(dir.ToString(), "n"))},
            { "o",new DirectoryInfo(Path.Combine(dir.ToString(), "o"))},
            { "p",new DirectoryInfo(Path.Combine(dir.ToString(), "p"))},
            { "q",new DirectoryInfo(Path.Combine(dir.ToString(), "q"))},
            { "r",new DirectoryInfo(Path.Combine(dir.ToString(), "r"))},
            { "s",new DirectoryInfo(Path.Combine(dir.ToString(), "s"))},
            { "t",new DirectoryInfo(Path.Combine(dir.ToString(), "t"))},
            { "u",new DirectoryInfo(Path.Combine(dir.ToString(), "u"))},
            { "v",new DirectoryInfo(Path.Combine(dir.ToString(), "v"))},
            { "w",new DirectoryInfo(Path.Combine(dir.ToString(), "w"))},
            { "x",new DirectoryInfo(Path.Combine(dir.ToString(), "x"))},
            { "y",new DirectoryInfo(Path.Combine(dir.ToString(), "y"))},
            { "z",new DirectoryInfo(Path.Combine(dir.ToString(), "z"))}
        };

        public override TrainingDataDTOWindowsFont ExportFontImages(
            List<string> fonts,
            List<IImageDecorator> imageDecorators,
            int fontSize = 50)
        {

            if (dir.Exists)
                dir.Delete(true);

            if (!dir.Exists)
                dir.Create();


            var model = base.ExportFontImages(fonts, imageDecorators,
             fontSize);


            model.ImageDatas.ForEach(x =>
            {
                var folderPerFont = folderDic[x.Text].ToString();
                Directory.CreateDirectory(folderPerFont);
                x.HardDiskPath = new DirectoryInfo(Path.Combine(folderPerFont,
                                                   IDGenerator.GetBase36(5) + ".jpg"));
                x.bitmap.Save(x.HardDiskPath.ToString());
            });

            return model;
        }


    }
}
