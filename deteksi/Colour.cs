using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace tugasakhir.deteksi
{
    public unsafe class Colour
    {
        public enum Types { RGB };
        public static PixelData GetPixelData(BGRA* pixel, Types model)
        {
            Color clr = Color.FromArgb(pixel->red, pixel->green, pixel->blue);
            PixelData pd = new PixelData(0, 0, 0, clr.Name);
            switch (model)
            {
                case Types.RGB:
                    pd.Ch1 = pixel->red;
                    pd.Ch2 = pixel->green;
                    pd.Ch3 = pixel->blue;
                    break;
                default:
                    throw new Exception("Konversi tidak terimplementasi");
            }
            pd.Name = clr.Name;
            return pd;

        }
        public static PixelData GetPixelData(int R, int G, int B, Types model)
        {
            Color clr = Color.FromArgb(R, G, B);
            PixelData pd = new PixelData(0, 0, 0, clr.Name);
            switch (model)
            {
                case Types.RGB:
                    pd.Ch1 = R;
                    pd.Ch2 = G;
                    pd.Ch3 = B;
                    break;
                default:
                    throw new Exception("Konversi tidak terimplementasi");
            }
            pd.Name = clr.Name;
            return pd;
        }
        private static float Min(float r, float g, float b)
        {
            float m = Math.Min(r, g);
            m = Math.Min(m, b);
            return m;
        }

        private static float Max(float r, float g, float b)
        {
            float m = Math.Max(r, g);
            m = Math.Max(m, b);
            return m;
        }

        private static float Min(BGRA* pixel)
        {
            return Min((float)pixel->red, (float)pixel->green, (float)pixel->blue);
        }

        private static float Max(BGRA* pixel)
        {
            return Max((float)pixel->red, (float)pixel->green, (float)pixel->blue);
        }
    }
}
