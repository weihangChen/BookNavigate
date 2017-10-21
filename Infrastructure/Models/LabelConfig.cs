using System.Collections.Generic;
using System.Linq;
using System;

namespace Infrastructure.Models
{

    public class LabelData
    {
        public LabelData(string label, byte labelAsInt, string labelAsFolderName)
        {
            Label = label;
            LabelAsInt = labelAsInt;
            LabelAsFolderName = labelAsFolderName;
        }
        public string Label { get; set; }
        public byte LabelAsInt { get; set; }
        public string LabelAsFolderName { get; set; }


        //var tmp = folderName.Split('\\');
        //label = StringResources.FolderToLetterMapping[tmp[tmp.Length - 1]];
    }

    public class LabelConfig
    {
        public LabelConfig()
        {
            LabelDatas = new List<LabelData>();
        }
        public List<LabelData> LabelDatas { get; set; }
    }


    public class TwoLetterConfig : LabelConfig
    {
        public TwoLetterConfig() : base()
        {
            string all_letters_first = "abcdefghijklmnopqrstuvwxyz";
            string all_letters_second = "abcdefghijklmnopqrstuvwxyz";
           
            int index = 0;
            all_letters_first.ToList().ForEach(first_letter =>
            {
                List<string> matched_letters_second = all_letters_second.ToList().Where(x => !first_letter.Equals(x)).Select(x => x.ToString()).ToList();
                matched_letters_second.ForEach(second_letter =>
                {
                    var label = first_letter + second_letter;
                    var ld = new LabelData(label, (byte)index, label);
                    LabelDatas.Add(ld);
                    index++;
                });  
            });
            if (LabelDatas.Count != 650)
            {
                throw new Exception("two letters config should have size of 650");
            }
        }
    }


    //feed it with a bigstring "abcd" or "1234"
    public class SingleLetterConfig : LabelConfig
    {
        public SingleLetterConfig(string bigString) : base()
        {
            LabelDatas = bigString.Select(x =>
            {
                var label = x.ToString();
                int labelAsInt = Convert.ToInt32(StringResources.LetterToByteMapping[label]);
                string labelAsFolderName = StringResources.LetterMapping[label];
                LabelData ld = new LabelData(label, (byte)labelAsInt, labelAsFolderName);
                return ld;
            }).ToList();
        }

    }

}
