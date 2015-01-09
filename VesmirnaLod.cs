using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class VesmirnaLod : GravityObjekt
    {
        private int w;
        private int h;

        private Bitmap _cleanImg;
        public VesmirnaLod(Point poz, int velikost, double hmotnost)
        {
            this.Hmotnost = hmotnost;
            this.w = (int)(velikost * ((1+Math.Sqrt(5))/2)); 
            this.h = velikost;
            this.Sprite = new Bitmap(this.w, this.h);
            this._cleanImg = new Bitmap((int)(this.w * 1.75), (int)(this.h * 1.75));
            this.Obdelnik = new Rectangle(poz, this.Sprite.Size);
            this.VykresliSprite();
            #region vykresli clean img
            using (Graphics g = Graphics.FromImage(this._cleanImg))
            {
                g.Clear(Color.Transparent);
                var points = new[]
                    {
                        Point.Empty,
                        new Point(this.w / 2, 0),
                        new Point(this.w, this.h / 2),
                        new Point(this.w / 2, this.h),
                        new Point(0, this.h)
                    };

                g.FillPolygon(new HatchBrush(HatchStyle.Trellis, Color.DarkGray, Color.Cyan), points);

                LinearGradientBrush linGrBrush = new LinearGradientBrush(
                   new Point(0, 0),
                   new Point(0, 10),
                   Color.White,
                   Color.Black);



                // Create color and points arrays
                Color[] clrArray =
                    {
                    Color.Black, Color.Black, Color.Transparent, Color.Black, Color.Black
                    };
                float[] posArray =
                    {
                    0.0f, 0.05f, 0.5f, 0.95f, 1.0f,
                    };

                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = clrArray;
                colorBlend.Positions = posArray;

                linGrBrush.InterpolationColors = colorBlend;

                linGrBrush.ScaleTransform(3.0f, 1);

                linGrBrush.TranslateTransform(23.5f, 0);

                g.FillPolygon(linGrBrush, points);

                g.DrawLine(new Pen(Brushes.Red, 16), new Point(0, this.h / 2), new Point(this.w / 3, this.h / 2));

                FastBitmap.FastBoxBlur(this._cleanImg, 3);
            }
            #endregion
        }

        public override void VykresliSprite() //neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a SO!
        {
            if (this.Smer.X != 0 && this.Smer.Y != 0)
            {
                using (Graphics gfx = Graphics.FromImage(this._sprite))
                {
                    gfx.Clear(Color.Transparent);
                    gfx.TranslateTransform(this.w / 2, this.h / 2);
                    float uhel = (float)(Math.Atan2(this.Smer.X, this.Smer.Y) * (180.0 / Math.PI) - 90);
                    gfx.RotateTransform(uhel);
                    gfx.DrawImage(this._cleanImg, -this.w / 2, -this.h / 2);
                }
            }
        }
    }
}
