//Downloaded from
//Visual C# Kicks - http://vckicks.110mb.com/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace PlanetAvoid
{
    unsafe public class FastBitmap
    {
        private struct PixelData
        {
            public byte blue;
            public byte green;
            public byte red;
            public byte alpha;

            public override string ToString()
            {
                return "(" + alpha.ToString() + ", " + red.ToString() + ", " + green.ToString() + ", " + blue.ToString() + ")";
            }
        }

        private Bitmap workingBitmap = null;
        private int width = 0;
        private BitmapData bitmapData = null;
        private Byte* pBase = null;

        public FastBitmap(Bitmap inputBitmap)
        {
            workingBitmap = inputBitmap;
        }

        public void LockImage()
        {
            Rectangle bounds = new Rectangle(Point.Empty, workingBitmap.Size);

            width = (int)(bounds.Width * sizeof(PixelData));
            if (width % 4 != 0) width = 4 * (width / 4 + 1);

            //Lock Image
            bitmapData = workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            pBase = (Byte*)bitmapData.Scan0.ToPointer();
        }

        private PixelData* pixelData = null;

        public Color GetPixelColor(int x, int y)
        {
            pixelData = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            return Color.FromArgb(pixelData->alpha, pixelData->red, pixelData->green, pixelData->blue);
        }

        private byte[] thing = new byte[4];
        public byte[] GetPixel(int x, int y)
        {
            PixelData* pixelData = ((PixelData*)(pBase + y * width + x * sizeof(PixelData)));
            thing[0] = pixelData->alpha;
            thing[1] = pixelData->red;
            thing[2] = pixelData->green;
            thing[3] = pixelData->blue;
            return thing;
        }

        public Color GetPixelNext()
        {
            pixelData++;
            return Color.FromArgb(pixelData->alpha, pixelData->red, pixelData->green, pixelData->blue);
        }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData* data = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            data->alpha = color.A;
            data->red = color.R;
            data->green = color.G;
            data->blue = color.B;
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            PixelData* data = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            data->alpha = 255;
            data->red = r;
            data->green = g;
            data->blue = b;
        }

        public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
        {
            PixelData* data = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            data->alpha = a;
            data->red = r;
            data->green = g;
            data->blue = b;
        }

        public void UnlockImage()
        {
            workingBitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }

        private static void Convolve(Bitmap input, float[,] filter)
        {
            //Find center of filter
            int xMiddle = (int)(filter.GetLength(0) / 2.0);
            int yMiddle = (int)(filter.GetLength(1) / 2.0);

            byte[] clr = null;

            int imgWidth = input.Width;
            int imgHeight = input.Height;


            FastBitmap reader = new FastBitmap(input);
            reader.LockImage();

            int fa = filter.GetLength(0);
            int fb = filter.GetLength(1);

            float r = 0;
            float g = 0;
            float b = 0;

            for (int x = 0; x < imgWidth; x++)
            {
                for (int y = 0; y < imgHeight; y++)
                {
                    r = 0;
                    g = 0;
                    b = 0;
                    //Apply filter
                    for (int xFilter = 0; xFilter < fa; xFilter++)
                    {
                        for (int yFilter = 0; yFilter < fb; yFilter++)
                        {
                            int x0 = x - xMiddle + xFilter;
                            int y0 = y - yMiddle + yFilter;

                            //Only if in bounds
                            if (x0 >= 0 && x0 < imgWidth &&
                                y0 >= 0 && y0 < imgHeight)
                            {
                                clr = reader.GetPixel(x0, y0);

                                r += clr[1] * filter[xFilter, yFilter];
                                g += clr[2] * filter[xFilter, yFilter];
                                b += clr[3] * filter[xFilter, yFilter];
                            }
                        }
                    }

                    r = 255 <= r ? 255 : r;
                    r = 0 >= r ? 0 : r;
                    g = 255 <= g ? 255 : g;
                    g = 0 >= g ? 0 : g;
                    b = 255 <= b ? 255 : b;
                    b = 0 >= b ? 0 : b;
                    //Set the pixel
                    reader.SetPixel(x, y, (byte)((0.2126 * r + 0.7152 * g + 0.0722 * b) *2), (byte)r, (byte)g, (byte)b);
                }
            }

            reader.UnlockImage();
        }

        /// <summary>
        /// Returns a box filter 1D kernel that is in the format {1,..,n}
        /// </summary>
        private static float[,] GetHorizontalFilter(int size)
        {
            float[,] smallFilter = new float[size, 1];
            float constant = size;

            for (int i = 0; i < size; i++)
            {
                smallFilter[i, 0] = 1.0f / constant;
            }

            return smallFilter;
        }

        /// <summary>
        /// Returns a box filter 1D kernel that is in the format {1},...,{n}
        /// </summary>
        private static float[,] GetVerticalFilter(int size)
        {
            float[,] smallFilter = new float[1, size];
            float constant = size;

            for (int i = 0; i < size; i++)
            {
                smallFilter[0, i] = 1.0f / constant;
            }

            return smallFilter;
        }

        /// <summary>
        /// Returns a box filter 2D kernel in the format {1,...,n},...,{1,...,n}
        /// </summary>
        private static float[,] GetBoxFilter(int size)
        {
            float[,] filter = new float[size, size];
            float constant = size * size;

            for (int i = 0; i < filter.GetLength(0); i++)
            {
                for (int j = 0; j < filter.GetLength(1); j++)
                {
                    filter[i, j] = 1.0f / constant;
                }
            }

            return filter;
        }

        public static void FastVerticalBoxBlur(Image img, int size)
        {
            var x = GetHorizontalFilter(size);
            Convolve((Bitmap)img, x);
        }

        public static void FastHorizontalBoxBlur(Image img, int size)
        {
            //Apply a box filter by convolving the image with two separate 1D kernels (faster)
            var x = GetVerticalFilter(size);
            Convolve((Bitmap)img, x);
        }

        public static void FastBoxBlur(Image img, int size)
        {
            Convolve((Bitmap)img, GetHorizontalFilter(size));
            Convolve((Bitmap)img, GetVerticalFilter(size));
        }
    }
}