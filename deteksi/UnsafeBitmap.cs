using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace tugasakhir.deteksi
{
    public unsafe class UnsafeBitmapEnumerator : IDisposable
    {
        //---------------------------------------------------
        int x;
        int y;
        UnsafeBitmap fastBitmap;
        BGRA* pCurrentPixel;
        bool locked;
        //---------------------------------------------------

        public UnsafeBitmapEnumerator(UnsafeBitmap fastBitmap)
        {
            fastBitmap.LockBitmap();
            locked=true;
            this.fastBitmap = fastBitmap;
            x = -1;
            y = 0;
            pCurrentPixel = fastBitmap[x,y];
        }
        //---------------------------------------------------
        public void Dispose()
        {
            if(locked)
            {
                fastBitmap.UnlockBitmap();
            }
        }
        //----------------------------------------------------
        public bool MoveNext()
        {
            x++;
            pCurrentPixel++;
            if (x == fastBitmap.Size.X)
            {
                y++;
                if(y == fastBitmap.Size.Y)
                {
                    return false;
                }
                else
                {
                    x = 0;
                    pCurrentPixel = fastBitmap[0,y];
                }
            }
            return true;
        }
        //----------------------------------------------------
        public BGRA* Current
        {
             get
            {
                return pCurrentPixel;
            }
        }
        //----------------------------------------------------
    }



    //-----------------------------------------------------------------------
    public unsafe class UnsafeBitmap
    {
        //-------------------------------------------------------------------
        Bitmap bitmap;

        int width;
        BitmapData bitmapData = null;
        Byte* pBase = null;
        BGRA* pCurrentPixel = null;
        int xLocation;
        int yLocation;
        Point size;
        internal bool locked = false;

        //-------------------------------------------------------------------

         public UnsafeBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;

            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            size = new Point((int)bounds.Width, (int)bounds.Height);
        }

        //-------------------------------------------------------------------
        public UnsafeBitmap(Point ptSize)
        {
            size = ptSize;
        }

        //-------------------------------------------------------------------

        public void Dispose()
        {
            bitmap.Dispose();
        }

        //-------------------------------------------------------------------
         public Point Size
        {
            get { return size;}
        }

        //-------------------------------------------------------------------

        public Point GetSize()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);
            return new Point((int)bounds.Width, (int)bounds.Height);
        }
        //-------------------------------------------------------------------

         public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
            set
            {
                bitmap = value;
            }
        }
        //--------------------------------------------------------------------
         public BGRA* GetNextPixel()
         {
             BGRA* pReturnPixel = pCurrentPixel;
             if (xLocation == size.X)
             {
                 xLocation = 0;
                 yLocation++;
                 if (yLocation == size.Y)
                 {
                     UnlockBitmap();
                     return null;
                 }
                 else
                 {
                     pCurrentPixel = this[0, yLocation];
                 }
             }
             else
             {
                 xLocation++;
                 pCurrentPixel++;
             }
             return pReturnPixel;
         }
        //---------------------------------------------------------------------
         public BGRA* this[int x, int y]
         {
             get
             {
                 return (BGRA*)(pBase + y * width + x * sizeof(BGRA));
             }
         }

         public UnsafeBitmapEnumerator GetEnumerator()
         {
             return new UnsafeBitmapEnumerator(this);
         }
        //---------------------------------------------------------------------
         public void LockBitmap()
         {
             GraphicsUnit unit = GraphicsUnit.Pixel;
             RectangleF boundsF = bitmap.GetBounds(ref unit);
             Rectangle bounds = new Rectangle((int)boundsF.X,
                 (int)boundsF.Y,
                 (int)boundsF.Width,
                 (int)boundsF.Height);
             int t = sizeof(BGRA);
             width = (int)boundsF.Width * sizeof(BGRA);
             if (width % 4 != 0)
             {
                 width = 4 * (width / 4 + 1);
             }
             bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
             pBase = (Byte*)bitmapData.Scan0.ToPointer();
             locked = true;
         }
        //-------------------------------------------------------------------
         public void UnlockBitmap()
         {
             bitmap.UnlockBits(bitmapData);
             bitmapData = null;
             pBase = null;
             locked = false;
         }
        //-------------------------------------------------------------------
    }
}
