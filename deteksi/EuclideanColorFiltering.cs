using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;

namespace tugasakhir
{
    public class EuclideanColorFiltering : BaseInPlacePartialFilter
    {
        //private Bitmap imagea = null; //gambar bitmap image
        private short radius = 212;//157&212
        private RGB center = new RGB(255, 255, 255);
        private RGB fill = new RGB(0, 0, 0);
        private bool fillOutside = false;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }


        public short Radius
        {
            get { return radius; }
            set
            {
                radius = System.Math.Max((short)0, System.Math.Min((short)450, value));
            }

        }
        public RGB CenterColor
        {
            get { return center; }
            set { center = value; }

        }

        public RGB FillColor
        {
            get { return fill; }
            set { fill = value; }

        }

        public bool FillOutside
        {
            get { return fillOutside; }
            set { fillOutside = value; }

        }
        public EuclideanColorFiltering()
        {
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            
        }
        public EuclideanColorFiltering(RGB center, short radius)
            : this()
        {
            // InitializeComponent();

            this.center = center;
            this.radius = radius;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage imagea, Rectangle rect)
        {
            // mengambil ukuran gambar
            int pixelSize = (imagea.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            int startX = rect.Left;
            int startY = rect.Top;
            int stopX = startX + rect.Width;
            int stopY = startY + rect.Height;
            int offset = imagea.Stride - rect.Width * pixelSize;
            byte r, g, b;

            //pusat bulatan
            byte cR = center.Red;
            byte cG = center.Green;
            byte cB = center.Blue;
            //mengisi warna
            byte fR = fill.Red;
            byte fG = fill.Green;
            byte fB = fill.Blue;
            //do JOB
            byte* ptr = (byte*)imagea.ImageData.ToPointer();
            //align pointer to the first pixel to process
            ptr += (startY * imagea.Stride + startY * pixelSize);
            //for each row
            for (int y = startY; y < stopY; y++)
            {
                //for each pixel 
                for (int x = startX; x < stopX; x++, ptr += pixelSize)
                {
                    r = ptr[RGB.R];
                    g = ptr[RGB.G];
                    b = ptr[RGB.B];

                    //calculate the distance
                    if ((int)Math.Sqrt(
                        Math.Pow((int)r - (int)cR, 2) +
                        Math.Pow((int)g - (int)cG, 2) +
                        Math.Pow((int)b - (int)cB, 2)) <= radius)
                    {
                        //inside sphere

                        if (!fillOutside)
                        {
                            ptr[RGB.R] = fR;
                            ptr[RGB.G] = fG;
                            ptr[RGB.B] = fB;
                        }
                    }
                    else
                        //outside sphere
                        if (fillOutside)
                        {
                            ptr[RGB.R] = fR;
                            ptr[RGB.G] = fG;
                            ptr[RGB.B] = fB;

                        }
                }
            }
            ptr += offset;
        }

    }
}
