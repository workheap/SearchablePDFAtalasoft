// This source code is property of Atalasoft, Inc. (www.atalasoft.com)
// Permission for usage and modification of this code is only permitted 
// with the purchase of a DotImage source code license.

// Change History:

using System;
using System.Collections.Generic;
using System.Text;
using Atalasoft.Imaging;
using Atalasoft.PdfDoc.Geometry;
using System.Drawing;

namespace OriginalPdfTranslation
{
    /// <summary>
    /// AtalaImageCoordinateConverter is used to convert coordinates in AtalaImage space to
    /// coordinates in PDF space and vice versa.
    /// </summary>
    public class CoordinateConverter
    {
        private PdfTransform _toPdf, _toPix;
        private Size _imageSize;
        private Dpi _resolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtalaImageCoordinateConverter"/> class.  This image must have a
        /// valid Resolution property.  Image resolution is used for the transformation and compound resolution is supported.
        /// </summary>
        /// <param name="image">The image used for building transformations.</param>
        public CoordinateConverter(AtalaImage image)
            : this(image.Width, image.Height, image.Resolution)
        {
        }

        public CoordinateConverter(int width, int height, Dpi resolution)
        {
            _imageSize = new Size(width, height);
            double dpix = ToDpi(resolution.Units, resolution.X);
            double dpiy = ToDpi(resolution.Units, resolution.Y);
            if (dpix == 0 || dpiy == 0)
                throw new ArgumentOutOfRangeException("image resolution can't be 0");
            _toPdf = new PdfTransform(72.0 / dpix, 0, 0, -72 / dpiy, 0, height * 72 / dpiy);
            _toPix = _toPdf.GetInverse();
            _resolution = resolution;
        }

        /// <summary>
        /// Converts the image size to the equivalent size in PDF space.
        /// </summary>
        /// <returns>a PdfBounds object set to (0, 0, width, height)</returns>
        public PdfBounds ToPdfImageSize()
        {
            PdfPoint point = new PdfPoint(_imageSize.Width, 0);
            point = _toPdf.Transform(point);
            return new PdfBounds(0, 0, point.X, point.Y);
        }

        /// <summary>
        /// Converts the image size to the equivalent size in PDF space.
        /// This is equivalent to <code>new AtalaImageCoordinateConverter(image).ToPdfImageSize()</code>
        /// </summary>
        /// <param name="image">The image to use for coverting the size</param>
        /// <returns></returns>
        public static PdfBounds ToPdfImageSize(AtalaImage image)
        {
            return new CoordinateConverter(image).ToPdfImageSize();
        }

        /// <summary>
        /// Converts the input Point in image coordinates to PDF coordinates
        /// </summary>
        /// <param name="p">The point to convert</param>
        /// <returns>a PdfPoint in PDF coordinates</returns>
        public PdfPoint ToPdf(Point p)
        {
            return _toPdf.Transform(new PdfPoint(p.X, p.Y));
        }

        /// <summary>
        /// Converts the input Rectangle in image coordinates to PDF coordinates
        /// </summary>
        /// <param name="r">The rectangle to convert.</param>
        /// <returns>a PdfBounds in PDF coordinates</returns>
        public PdfBounds ToPdf(Rectangle r)
        {
            PdfPoint ll = new PdfPoint(r.Left, r.Bottom);
            PdfPoint ur = new PdfPoint(r.Right, r.Top);
            ll = _toPdf.Transform(ll);
            ur = _toPdf.Transform(ur);
            return new PdfBounds(Math.Min(ll.X, ur.X), Math.Min(ll.Y, ur.Y), Math.Abs(ll.X - ur.X), Math.Abs(ll.Y - ur.Y));
        }

        /// <summary>
        /// Converts the input PdfPoint in PDF coordinates to image coordinates.
        /// </summary>
        /// <param name="pt">The PdfPoint to convert</param>
        /// <returns>a PointF in image coordinates</returns>
        public PointF ToImage(PdfPoint pt)
        {
            return _toPix.Transform(new PointF((float)pt.X, (float)pt.Y));
        }

        /// <summary>
        /// Converts the input PdfPoint in PDF coordinates to integer-based image coordinates, rounding the values
        /// </summary>
        /// <param name="pt">The PdfPoint to convert</param>
        /// <returns>a Point in image coordinates</returns>
        public Point ToImageI(PdfPoint pt)
        {
            PointF pf = _toPix.Transform(new PointF((float)pt.X, (float)pt.Y));
            return new Point((int)Math.Round(pf.X), (int)Math.Round(pf.Y));
        }

        /// <summary>
        /// Converts the input PdfPoint in PDF coordinates to integer-based image coordinates, truncating the values
        /// </summary>
        /// <param name="pt">The PdfPoint to convert</param>
        /// <returns>a Point in image coordinates</returns>
        public Point ToImageITruncated(PdfPoint pt)
        {
            PointF pf = _toPix.Transform(new PointF((float)pt.X, (float)pt.Y));
            return new Point((int)pf.X, (int)pf.Y);
        }

        /// <summary>
        /// Converts the input PdfBounds in PDF coordinates to image coordinates
        /// </summary>
        /// <param name="r">The rectangle to convert</param>
        /// <returns>a RectangleF in image coordinates</returns>
        public RectangleF ToImage(PdfBounds r)
        {
            PointF ll = new PointF((float)r.Left, (float)r.Bottom);
            PointF ur = new PointF((float)r.Right, (float)r.Top);
            ll = _toPix.Transform(ll);
            ur = _toPix.Transform(ur);
            return new RectangleF(Math.Min(ll.X, ur.X), Math.Min(ll.Y, ur.Y), Math.Abs(ll.X - ur.X), Math.Abs(ll.Y - ur.Y));
        }

        /// <summary>
        /// Converts the input PdfBounds in PDF coordinates to integer-based image coordinates, rounding the values
        /// </summary>
        /// <param name="r">The rectangle to convert</param>
        /// <returns>a Rectangle in image coordinates</returns>
        public Rectangle ToImageI(PdfBounds r)
        {
            RectangleF rf = ToImage(r);
            return new Rectangle((int)Math.Round(rf.Left), (int)Math.Round(rf.Top),
                (int)Math.Round(rf.Width), (int)Math.Round(rf.Height));
        }

        /// <summary>
        /// Converts the input PdfBounds in PDF coordinates to integer-based image coordinates, truncating the values
        /// </summary>
        /// <param name="r">The rectangle to convert</param>
        /// <returns>a Rectangle in image coordinates</returns>
        public Rectangle ToImageITruncated(PdfBounds r)
        {
            RectangleF rf = ToImage(r);
            return new Rectangle((int)rf.Left, (int)rf.Top,
                (int)rf.Width, (int)rf.Height);
        }

        private double ToDpi(ResolutionUnit units, double x)
        {
            switch (units)
            {
                default:
                case ResolutionUnit.DotsPerInch:
                    return x;
                case ResolutionUnit.DotsPerCentimeters:
                    return x * 2.54;
            }
        }

        public Dpi Resolution { get { return _resolution; } }
    }
}
