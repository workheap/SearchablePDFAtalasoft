using System;
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using Atalasoft.Ocr;

namespace OriginalPdfTranslation
{
    public class InternalStaticFontMapper: BasicFontMapper
    {
        private string _fontName;

        public InternalStaticFontMapper(string fontName):base()
		{
            _fontName = fontName;
		}

        public override string MapToFontName(string name)
		{
            return base.MapToFontName(_fontName);
		}

        public override FontFamily MapToFontFamily(string name)
		{
            return base.MapToFontFamily(_fontName);
		}
    }
}
