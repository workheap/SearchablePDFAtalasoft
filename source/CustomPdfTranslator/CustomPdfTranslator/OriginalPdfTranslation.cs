using System;
using System.Collections.Generic;
using System.Text;
using Atalasoft.Ocr;
using Atalasoft.dotImage.PdfDoc.Bridge;
using System.IO;
using Atalasoft.PdfDoc.Generating;
using Atalasoft.Imaging;
using Atalasoft.PdfDoc.Geometry;
using System.Drawing;
using Atalasoft.PdfDoc.Generating.Shapes;
using System.Runtime.InteropServices;
using Atalasoft.PdfDoc.Generating.ResourceHandling.Fonts;

namespace OriginalPdfTranslation
{
    public class OriginalPdfTranslation : IForeignTranslator, IPerPageTranslator
    {
        private FileStream _openedStm;
        private FileStream _existingDocStm;

        public string sourcePdf { get; set; }

        public void Finish(OcrEngine engine, OcrDocument doc, bool successful, object translationObject)
        {
            TranslationSession session = translationObject as TranslationSession;
            if (session != null)
            {
                session.DisconnectFromEngine(engine, doc);
            }
            if (successful && session.PdfDoc.Pages.Count > 0)
            {
                session.PdfDoc.Save(session.OutputStream);
            }
            if (_openedStm != null)
                _openedStm.Close();

            if (_existingDocStm != null)
            {
                _existingDocStm.Close();
                _existingDocStm.Dispose();
            }
        }

        public object Prepare(OcrEngine engine, OcrDocument doc)
        {
            throw new NotImplementedException();
        }

        public void Translate(OcrEngine engine, OcrDocument doc, string mimeType, System.IO.Stream outStream, object translationObject)
        {
            throw new NotImplementedException();
        }

        public void Translate(OcrEngine engine, OcrDocument doc, string mimeType, string outFileName, object translationObject)
        {
            throw new NotImplementedException();
        }

        public bool CanStream()
        {
            return true;
        }

        private static string[] _supportedMimes = new string[] { "application/pdf" };

        public string[] Supported()
        {
            return _supportedMimes;
        }

        public bool Supports(string mimeType)
        {
            return Array.IndexOf(_supportedMimes, mimeType) >= 0;
        }

        public object Prepare(OcrEngine engine, OcrDocument doc, string mimeType, string outFileName)
        {
            _openedStm = new FileStream(outFileName, FileMode.Create);
            _existingDocStm = new FileStream(sourcePdf, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Prepare(engine, doc, mimeType, _openedStm);
        }

        public object Prepare(OcrEngine engine, OcrDocument doc, string mimeType, Stream outStream)
        {
            PdfGeneratedDocument pdfDoc = new PdfGeneratedDocument(null, null, _existingDocStm, true, null, new Atalasoft.PdfDoc.Repair.RepairOptions() );
            pdfDoc.EmbedGeneratedContent = false;
            TranslationSession session = new TranslationSession(this, pdfDoc, outStream);
            session.ConnectToEngine(engine, doc);
            return session;
        }

        public bool UseIForeignTranslatorInterface
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public void TranslatePage(OcrEngine engine, OcrDocument doc, OcrPage page, object translationObject, bool totalPagesKnown, int pageIndex, int totalPages)
        {
            TranslationSession session = translationObject as TranslationSession;

            CoordinateConverter converter = new CoordinateConverter(page.Width, page.Height, page.Resolution);

            PdfBounds bounds = converter.ToPdf(new Rectangle(0, 0, page.Width, page.Height));
            
            PdfGeneratedPage pdfPage = (PdfGeneratedPage)session.PdfDoc.Pages[pageIndex];

            string imageResource = session.GetImageResource(page);
            TranslateRegions(engine, converter, page.Regions, session.PdfDoc, pdfPage);
        }

        private void TranslateRegions(OcrEngine engine, CoordinateConverter converter, OcrRegionCollection regions, PdfGeneratedDocument pdfDoc, PdfGeneratedPage pdfPage)
        {
            foreach (OcrRegion region in regions)
            {
                if (region is OcrTextRegion)
                {
                    TranslateTextRegion(engine, converter, (OcrTextRegion)region, pdfDoc, pdfPage);
                }
                else if (region is OcrTableRegion)
                {
                    OcrTableRegion table = (OcrTableRegion)region;
                    TranslateRegions(engine, converter, table.Cells, pdfDoc, pdfPage);
                }
                else if (region is OcrImageRegion)
                {
                    OcrImageRegion image = (OcrImageRegion)region;
                    PdfBounds bounds = converter.ToPdf(image.Bounds);
                    string imageResName = pdfDoc.Resources.Images.AddImage(image.Image);
                    PdfImageShape imageShape = new PdfImageShape(imageResName, bounds);
                    pdfPage.DrawingList.Add(imageShape);
                }
                else if (region is OcrBarcodeRegion)
                {
                    throw new NotImplementedException();
                }
                else if (region is OcrFormElementRegion)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void TranslateTextRegion(OcrEngine engine, CoordinateConverter converter, OcrTextRegion tr, PdfGeneratedDocument pdfDoc, PdfGeneratedPage pdfPage)
        {
            foreach (OcrLine line in tr.Lines)
            {
                if (line.StyleIsUniform(engine.FontMapper, engine.FontBuilder))
                {
                    PlaceText(engine, converter, pdfDoc, pdfPage, line.GetFontNameAt(engine.FontMapper, 0),
                        line.GetFontSizeAt(converter.Resolution, 0), new Rectangle(line.Bounds.Left, line.Baseline, line.Bounds.Width, line.Bounds.Height),  line.Text);
                }
                else
                {
                    foreach (OcrWord word in line.Words)
                    {
                        PlaceText(engine, converter, pdfDoc, pdfPage, word.GetFontNameAt(engine.FontMapper, 0),
                            word.GetFontSizeAt(converter.Resolution, 0), new Rectangle(word.Bounds.Left, word.Baseline, word.Bounds.Width, word.Bounds.Height), word.Text);
                    }
                }
            }
        }

        private void PlaceText(OcrEngine engine, CoordinateConverter converter, PdfGeneratedDocument pdfDoc, PdfGeneratedPage pdfPage,
            string fontName, double fontSize, Rectangle textBounds, string text)
        {
            PdfBounds pdfTextBounds = converter.ToPdf(textBounds);
            
            string fontResourceName = GetFontResource(pdfDoc, fontName);
            PdfFontResource fontResource = pdfDoc.Resources.Fonts.Get(fontResourceName);
            
            PdfPoint actualSize = fontResource.Metrics.MeasureText(fontSize, text);

            double horizScale = 100 * (pdfTextBounds.Width / actualSize.X);

            PdfTextLine textLine = new PdfTextLine(fontResourceName, fontSize, text, new PdfPoint(pdfTextBounds.Left, pdfTextBounds.Top));
            textLine.RenderMode = PdfTextRenderMode.Invisible;
            textLine.HorizontalScaling = horizScale;

            pdfPage.DrawingList.Add(textLine);
        }


        private static string GetFontResource(PdfGeneratedDocument pdfDoc, string windowsFontName)
        {
            string prefixedName = "xx" + windowsFontName;
            if (pdfDoc.Resources.Fonts.Contains(prefixedName))
                return prefixedName;

            PdfFontResource resource = pdfDoc.Resources.Fonts.FromFontName(windowsFontName);
            pdfDoc.Resources.Fonts.Add(prefixedName, resource);
            return prefixedName;
        }

    }
}
