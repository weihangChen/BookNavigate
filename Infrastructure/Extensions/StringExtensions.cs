using System.Linq;
using Infrastructure.Models;

namespace Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static int GetContourCountForChar(this string c)
        {
            return StringResources.ContoursPerChar[c];
        }

        public static bool ContainsOnlySpecificSetOfChars(this string str, string stringSet)
        {
            bool containOnlyValidStringSet = true;
            foreach (var x in str)
            {
                if (!stringSet.Contains(x))
                {
                    containOnlyValidStringSet = false;
                    break;
                };
            }
            return containOnlyValidStringSet;
        }



    }
}
