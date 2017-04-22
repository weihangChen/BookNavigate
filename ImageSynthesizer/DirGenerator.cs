using Infrastructure.Models;
using System.IO;
using System.Linq;

namespace ImageSynthesizer
{
    public class DirGenerator
    {
        public static void CreateDirs(string tobeGeneratedChars, string fontDataDirDest)
        {
            //create folders for each char
            tobeGeneratedChars.ToList().ForEach(x =>
            {

                var folderName = StringResources.LetterMapping[x.ToString()];
                var folder = Path.Combine(fontDataDirDest, folderName);
                Directory.CreateDirectory(folder);
            });

        }

    }
}
