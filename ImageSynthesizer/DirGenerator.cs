using Infrastructure.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ImageSynthesizer
{
    public class DirGenerator
    {
        public static void CreateDirs(List<string> tobeGeneratedChars, string fontDataDirDest)
        {
            //create folders for each char
            tobeGeneratedChars.ForEach(x =>
            {

                var folderName = StringResources.LetterMapping[x.ToString()];
                var folder = Path.Combine(fontDataDirDest, folderName);
                Directory.CreateDirectory(folder);
            });

        }


        public static void CreateDir(string folderName, string fontDataDirDest)
        {

            var folder = Path.Combine(fontDataDirDest, folderName);
            Directory.CreateDirectory(folder);


        }

    }
}
