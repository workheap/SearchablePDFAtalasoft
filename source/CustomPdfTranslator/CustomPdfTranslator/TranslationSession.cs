using System;
using System.Collections.Generic;
using System.Text;
using Atalasoft.Ocr;
using Atalasoft.PdfDoc.Generating;
using System.IO;

namespace OriginalPdfTranslation
{
    public class TranslationSession
    {
        private OcrPageConstructionEventHandler _pageConstructingDelegate;
        private OcrEngine _engine;
        private OriginalPdfTranslation _owner;
        private PdfGeneratedDocument _pdfDoc;
        private Dictionary<OcrPage, string> _pageImageMap = new Dictionary<OcrPage, string>();

        public TranslationSession(OriginalPdfTranslation owner, PdfGeneratedDocument pdfDoc, Stream outputStream)
        {
            _owner = owner;
            _pdfDoc = pdfDoc;
            OutputStream = outputStream;
        }

        public void ConnectToEngine(OcrEngine engine, OcrDocument doc)
        {
            _pageConstructingDelegate = new OcrPageConstructionEventHandler(engine_PageConstructing);
            engine.PageConstructing += _pageConstructingDelegate;
            _engine = engine;
        }

        public void DisconnectFromEngine(OcrEngine engine, OcrDocument doc)
        {
            if (_engine != engine)
                throw new OcrException("Expected same engine for unwiring.");
            engine.PageConstructing -= _pageConstructingDelegate;
            _pageImageMap.Clear();
            _engine = null;
        }


        private void engine_PageConstructing(object sender, OcrPageConstructionEventArgs e)
        {
        }

        public PdfGeneratedDocument PdfDoc { get { return _pdfDoc; } }

        public Stream OutputStream { get; set; }

        public string GetImageResource(OcrPage page)
        {
            string result = null;
            _pageImageMap.TryGetValue(page, out result);
            return result;
        }
    }
}
