using System.Drawing;

namespace Infrastructure.Models
{
    public abstract class FontData
    {
        public int FontSize { get; set; }
        public string FontName { get; set; }
        public abstract Font GetFont();
    }

    public class WindowsFont : FontData
    {
        public WindowsFont(string fontName, int fontSize)
        {
            FontName = fontName;
            FontSize = fontSize;
        }
        public override Font GetFont()
        {
            return new Font(FontName, FontSize);
        }

    }

    public class GoogleFont : FontData
    {
        public GoogleFont(string fontName, int fontSize, string fontPath)
        {
            FontName = fontName;
            FontSize = fontSize;
            FontPath = fontPath;
        }
        public string FontPath { get; set; }

        public override Font GetFont()
        {
            System.Drawing.Text.PrivateFontCollection privateFonts = new System.Drawing.Text.PrivateFontCollection();
            privateFonts.AddFontFile(FontPath);
            Font font = new Font(privateFonts.Families[0], FontSize);
            return font;
        }
    }
}
