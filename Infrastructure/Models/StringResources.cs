﻿
using System.Collections.Generic;

namespace Infrastructure.Models
{

    public class StringResources
    {
        //public static string NumberString = "0123456789";
        //public static string UpperString = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ";
        //public static string LowerString = "abcdefghijklmnopqrstuvwxyzåäö";
        public const string SixtyTwoChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYX";
        public const string Digits = "0123456789";

        public static Dictionary<string, string> LetterMapping = new Dictionary<string, string>()
        {
            { "A","AU"},
            { "B","BU"},
            { "C","CU"},
            { "D","DU"},
            { "E","EU"},
            { "F","FU"},
            { "G","GU"},
            { "H","HU"},
            { "I","IU"},
            { "J","JU"},
            { "K","KU"},
            { "L","LU"},
            { "M","MU"},
            { "N","NU"},
            { "O","OU"},
            { "P","PU"},
            { "Q","QU"},
            { "R","RU"},
            { "S","SU"},
            { "T","TU"},
            { "U","UU"},
            { "V","VU"},
            { "W","WU"},
            { "X","XU"},
            { "Y","YU"},
            { "Z","ZU"},
            { "1","1"},
            { "2","2"},
            { "3","3"},
            { "4","4"},
            { "5","5"},
            { "6","6"},
            { "7","7"},
            { "8","8"},
            { "9","9"},
            { "0","0"},
            { "a","a"},
            { "b","b"},
            { "c","c"},
            { "d","d"},
            { "e","e"},
            { "f","f"},
            { "g","g"},
            { "h","h"},
            { "i","i"},
            { "j","j"},
            { "k","k"},
            { "l","l"},
            { "m","m"},
            { "n","n"},
            { "o","o"},
            { "p","p"},
            { "q","q"},
            { "r","r"},
            { "s","s"},
            { "t","t"},
            { "u","u"},
            { "v","v"},
            { "w","w"},
            { "x","x"},
            { "y","y"},
            { "z","z"}
        };


        public static Dictionary<string, string> FolderToLetterMapping = new Dictionary<string, string>()
        {
            { "AU","A"},
            { "BU","B"},
            { "CU","C"},
            { "DU","D"},
            { "EU","E"},
            { "FU","F"},
            { "GU","G"},
            { "HU","H"},
            { "IU","I"},
            { "JU","J"},
            { "KU","K"},
            { "LU","L"},
            { "MU","M"},
            { "NU","N"},
            { "OU","O"},
            { "PU","P"},
            { "QU","Q"},
            { "RU","R"},
            { "SU","S"},
            { "TU","T"},
            { "UU","U"},
            { "VU","V"},
            { "WU","W"},
            { "XU","X"},
            { "YU","Y"},
            { "ZU","Z"},
            { "1","1"},
            { "2","2"},
            { "3","3"},
            { "4","4"},
            { "5","5"},
            { "6","6"},
            { "7","7"},
            { "8","8"},
            { "9","9"},
            { "0","0"},
            { "a","a"},
            { "b","b"},
            { "c","c"},
            { "d","d"},
            { "e","e"},
            { "f","f"},
            { "g","g"},
            { "h","h"},
            { "i","i"},
            { "j","j"},
            { "k","k"},
            { "l","l"},
            { "m","m"},
            { "n","n"},
            { "o","o"},
            { "p","p"},
            { "q","q"},
            { "r","r"},
            { "s","s"},
            { "t","t"},
            { "u","u"},
            { "v","v"},
            { "w","w"},
            { "x","x"},
            { "y","y"},
            { "z","z"}
        };


        /// <summary>
        /// map only lowercase, because my own ML pipeline only deals with lowercase characters
        /// </summary>
        public static Dictionary<string, string> LetterToByteMapping = new Dictionary<string, string>()
        {
            //{ "0","0"},
            //{ "1","1"},
            //{ "2","2"},
            //{ "3","3"},
            //lowercase
            { "a","0"},
            { "b","1"},
            { "c","2"},
            { "d","3"},
            //{ "e","4"},
            //{ "f","5"},
            //{ "g","6"},
            //{ "h","7"},
            //{ "i","8"},
            //{ "j","9"},
            //{ "k","10"},
            //{ "l","11"},
            //{ "m","12"},
            //{ "n","13"},
            //{ "o","14"},
            //{ "p","15"},
            //{ "q","16"},
            //{ "r","17"},
            //{ "s","18"},
            //{ "t","19"},
            //{ "u","20"},
            //{ "v","21"},
            //{ "w","22"},
            //{ "x","23"},
            //{ "y","24"},
            //{ "z","25"},
            //uppercase
            //{ "A","26"},
            //{ "B","27"},
            //{ "C","28"},
            //{ "D","29"},
            //{ "E","30"},
            //{ "F","31"},
            //{ "G","32"},
            //{ "H","33"},
            //{ "I","34"},
            //{ "J","35"},
            //{ "K","36"},
            //{ "L","37"},
            //{ "M","38"},
            //{ "N","39"},
            //{ "O","40"},
            //{ "P","41"},
            //{ "Q","42"},
            //{ "R","43"},
            //{ "S","44"},
            //{ "T","45"},
            //{ "U","46"},
            //{ "V","47"},
            //{ "W","48"},
            //{ "X","49"},
            //{ "Y","50"},
            //{ "Z","51"},
            //digit

            //{ "1","52"},
            //{ "2","53"},
            //{ "3","54"},
            //{ "4","55"},
            //{ "5","56"},
            //{ "6","57"},
            //{ "7","58"},
            //{ "8","59"},
            //{ "9","60"},
            //{ "0","61"},


        };
    }
}
