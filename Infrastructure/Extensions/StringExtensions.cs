using System.Linq;
using Infrastructure.Models;

namespace Infrastructure.Extensions
{
    public static class StringExtensions
    {
        

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
