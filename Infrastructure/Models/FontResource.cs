using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Infrastructure.Models
{
    public class FontResource
    {
        public static int HoneyFontSize = 50;
        public static int HoneyWidth = 60;
        public static int HoneyHeight = 90;




        //following fonts are too weired, exclude them if fonts_all is used

        public static List<string> Fonts_Exclude = new List<string> { "Agency FB", "Baskerville Old Face", "Blackadder ITC", "Bodoni MT Black", "Bookshelf Symbol 7", "Broadway", "Castellar", "Colonna MT", "Consolas", "Curlz MT", "French Script MT", "Gill Sans Ultra Bold Condensed", "Gill Sans Ultra Bold", "Haettenschweiler", "Imprint MT Shadow", "Jokerman", "Jokerman", "Kunstler Script", "Marlett", "Matura MT Script Capitals", "MS Outlook", "MS Reference Specialty", "MT Extra", "Niagara Engraved", "Niagara Solid", "OCR A Extended", "Old English Text MT", "Palace Script MT", "Parchment", "Playbill", "Ravie", "Segoe MDL2 Assets", "Showcard Gothic", "Snap ITC", "Stencil", "Webdings", "Wingdings 2", "Wingdings 3", "Wingdings", "Bauhaus 93", "Chiller", "Edwardian Script ITC", "Freestyle Script", "Gadugi", "Gill Sans MT Condensed", "Gill Sans MT Ext Condensed Bold", "Gill Sans MT", "High Tower Text", "Lucida Sans Unicode", "Lucida Sans", "Magneto", "MV Boli", "Papyrus", "Tw Cen MT Condensed Extra Bold", "Tw Cen MT Condensed", "Wide Latin", "Tw Cen MT", "Algerian", "Brush Script MT", "Copperplate Gothic Bold", "Copperplate Gothic Light", "Engravers MT", "Felix Titling", "Forte", "Gigi", "Goudy Stout", "Lucida Handwriting", "Mistral", "Rage Italic", "Script MT Bold", "Symbol", "Viner Hand ITC", "Vladimir Script", "Juice ITC", "Informal Roman", "Pristina", "Tempus Sans ITC", "Vivaldi", "Segoe Script", "Berlin Sans FB Demi", "Bernard MT Condensed", "Bodoni MT Poster Compressed", "Cooper Black", "Elephant", "Impact", "Onyx", "Segoe UI Black", "Segoe WP Black", "Perpetua Titling MT", "Bradley Hand ITC", "Harlow Solid Italic", "Harrington", "Javanese Text", "Lucida Calligraphy", "Segoe Print", "Rockwell Extra Bold", "Myanmar Text", "Microsoft Himalaya", "Kristen ITC", "Gabriola", "Constantia", "Corbel", "Rockwell Condensed", "Monotype Corsiva", "TeamViewer10" };



        public static List<string> Fonts_Small = new List<string> { "Arial", "Calibri", "Calibri Light", "Cambria", "Times New Roman", "Verdana", "Calisto MT", "Abadi MT Condensed Light", "Aldhabi", "Calisto MT", "Cambria Math", "Georgia Pro", "Georgia", "News Gothic MT" };


        public static List<string> Fonts_All
        {
            get
            {
                var fontAll = FontFamily.Families.Select(x => x.Name).ToList();
                var myfonts = new List<string>();
                foreach (var font in fontAll)
                {
                    if (!Fonts_Exclude.Contains(font))
                        myfonts.Add(font);
                }
                return myfonts;
            }
        }

    }

}
