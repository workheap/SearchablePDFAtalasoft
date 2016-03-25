using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using Atalasoft.Imaging.Codec.Pdf;
using Atalasoft.Ocr;
using Atalasoft.Ocr.GlyphReader;
using Atalasoft.Imaging;
using System.Globalization;
using Atalasoft.PdfDoc.Generating;

namespace OriginalPdfTranslation
{
    class Program
    {
        static GlyphReaderLoader _loader;
        static Program()
        {
            _loader = new GlyphReaderLoader();
        }

        static void Main(string[] args)
        {
            PdfDecoder decoder = new PdfDecoder();
            decoder.Resolution = 200;
            Atalasoft.Imaging.Codec.RegisteredDecoders.Decoders.Add(decoder);

            OriginalPdfTranslation updfxlator = new OriginalPdfTranslation();

            using (GlyphReaderEngine engine = new GlyphReaderEngine())
            {
                engine.Translators.Add(updfxlator);
                engine.FontMapper = new InternalStaticFontMapper("Arial");

                string sourceDocument = @"TestDoc.pdf";

                try
                {
                    engine.Initialize();
                    using (FileSystemImageSource source = new FileSystemImageSource(sourceDocument, true))
                    {
                        updfxlator.sourcePdf = sourceDocument;
                        engine.Translate(source, "application/pdf", "output.pdf");
                    }
                }
                finally
                {
                    engine.ShutDown();
                }
                
            }
        }
    }
}
