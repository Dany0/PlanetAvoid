using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PlanetAvoid
{
    public class Asteroid : GravityObjekt
    {
        private bool spriteVytvoren = false;

        private float _uhelNatoceni;

        private int _vltp;

        private int clockwise = 1;

        private Bitmap _clearSprite;

        private Bitmap _rotateImage;

        public Asteroid(Point poz, int velikostPlanety, double hmotnostFaktor = 1)
        {
            this.Hmotnost = (-1 + Math.Log10(velikostPlanety)) * hmotnostFaktor;
            this.Sprite = new Bitmap(velikostPlanety, velikostPlanety);
            this._rotateImage = new Bitmap(velikostPlanety * 2, velikostPlanety * 2);
            this._vltp = velikostPlanety;
            this.Obdelnik = new Rectangle(poz, this.Sprite.Size);
            this.VykresliSprite();
        }

        public override void VykresliSprite() //neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a SO!
        {
            if (this.spriteVytvoren == false)
            {
                if (Hra.randomInstance.NextDouble() <= 0.75) //75%
                {
                    this.clockwise = -1;
                }

                Bitmap subtract = new Bitmap(this._sprite);
                using (Graphics g = Graphics.FromImage(subtract))
                {
                    g.Clear(Color.White);
                    g.FillEllipse(Brushes.Black, 0, 0, this._vltp, this._vltp);
                    int velikost;
                    for (int i = 0; i <= 16; i++)
                    {
                        velikost = Hra.randomInstance.Next(2, this._vltp / 4);
                        g.FillEllipse(Brushes.White, Hra.randomInstance.Next(0, this._vltp - velikost),
                            Hra.randomInstance.Next(0, this._vltp - velikost),
                            velikost, velikost);
                        velikost = Hra.randomInstance.Next(2, this._vltp / 8);
                        g.FillEllipse(Brushes.Black, Hra.randomInstance.Next(0, this._vltp - velikost),
                            Hra.randomInstance.Next(0, this._sprite.Height - velikost),
                            velikost, velikost);
                    }
                    subtract.MakeTransparent(Color.Black);
                }

                //unsafe //bug ze vysledny obrazek je cerny = problem s pamet?
                //{
                //    int w = this._sprite.Width;
                //    int h = this._sprite.Height;

                //    var lb = this._sprite.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,
                //        PixelFormat.Format32bppArgb);
                //    var scan0 = (byte*)lb.Scan0;

                //    int r;
                //    int g = (int)(127 * this.Hmotnost);
                //    g = 128 <= g ? 128 : g;
                //    int b = (int)(64 * this.Hmotnost);
                //    b = 64 <= b ? 64 : b;
                //    for (int x = 0; x < w * h * 4; x += 4)
                //    {
                //        r = (int)(192 * this.Hmotnost * Hra.randomInstance.Next(1, 3));
                //        r = 192 <= r ? 192 : r;
                //        scan0[x] = (byte)(b); //B
                //        scan0[x + 1] = (byte)(g); //G
                //        scan0[x + 2] = (byte)(r); //R
                //        scan0[x + 3] = (byte)(255); //A
                //    }
                //    this._sprite.UnlockBits(lb);
                //}
                

                using (Graphics g = Graphics.FromImage(this._sprite))
                {
                    g.Clear(Color.BurlyWood);
                    g.DrawImage(subtract, Point.Empty);
                    this._sprite.MakeTransparent(Color.White);
                }

                this._clearSprite = this._sprite;
                this.spriteVytvoren = true;
            }
            else
            {
                this._uhelNatoceni += (float)(clockwise * 10);
                using (Graphics g = Graphics.FromImage(this._rotateImage))
                {
                    int center = this._vltp / 2;
                    g.Clear(Color.Transparent);
                    g.TranslateTransform(center, center);
                    g.RotateTransform(this._uhelNatoceni);
                    g.DrawImage(this._clearSprite, -this._vltp / 2, -this._vltp / 2);
                }

                this._sprite = this._rotateImage;
            }
        }
    }

    public class EliptickaPlaneta : GravityObjekt
    {
        protected bool spriteVytvoren = false;

        private double uhel;

        private PointF stredElipsy;

        private PointF excentricita;

        private double rychlostOtaceniFaktor;

        public override Bitmap Sprite
        {
            get
            {
                this.PosunPoElipse();
                if (this._sprite != null)
                {
                    this.VykresliSprite(); //neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a SO!
                }
                return _sprite;
            }
        }

        public EliptickaPlaneta(int velikostPlanety, double rychlost, PointF sE, PointF ex, double hmotnostFaktor = 1)
        {
            this.rychlostOtaceniFaktor = rychlost;
            this.stredElipsy = sE;
            this.excentricita = ex;
            this.Hmotnost = (-1 + Math.Log10(velikostPlanety)) * hmotnostFaktor;
            this.Sprite = new Bitmap(velikostPlanety, velikostPlanety);
            this.Obdelnik = new Rectangle(new Point(0,0), this.Sprite.Size);
            this.VykresliSprite();
        }

        public override void VykresliSprite() //neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a SO!
        {
            if (this.spriteVytvoren == false)
            {
                Bitmap subtract = new Bitmap(this._sprite);
                using (Graphics g = Graphics.FromImage(subtract))
                {
                    g.Clear(Color.White);
                    g.FillEllipse(Brushes.Black, 0, 0, this._sprite.Width, this._sprite.Height);
                    subtract.MakeTransparent(Color.Black);
                }

                FastBitmap bmp = new FastBitmap(this._sprite);

                bmp.LockImage();
                int rindx = Hra.randomInstance.Next(63, 255);
                int gindx = Hra.randomInstance.Next(63, 255);
                int bindx = Hra.randomInstance.Next(63, 255);
                for (int x = 0; x < this._sprite.Width; x++)
                {
                    for (int y = 0; y < this._sprite.Height; y++)
                    {
                        int num = Hra.randomInstance.Next(1, 4);
                        bmp.SetPixel(x, y,
                            (byte)((Math.Min(rindx, rindx * this.Hmotnost) * num) % 255),
                            (byte)((Math.Min(gindx, gindx * this.Hmotnost) * num) % 255),
                            (byte)((Math.Min(bindx, bindx * this.Hmotnost) * num) % 255)
                            );
                    }
                }
                bmp.UnlockImage();

                FastBitmap.FastVerticalBoxBlur(this._sprite, 6);

                using (Graphics g = Graphics.FromImage(this._sprite))
                {
                    g.DrawImage(subtract, Point.Empty);
                    this._sprite.MakeTransparent(Color.White);
                }

                this.spriteVytvoren = true;
            }
        }

        public void PosunPoElipse()
        {
            this.Posun((float)((this.stredElipsy.X + (this.excentricita.X * Math.Cos(uhel)))) - this._sprite.Width / 2,
                (float)((this.stredElipsy.Y + (this.excentricita.Y * Math.Sin(uhel)))) - this._sprite.Height / 2);
            uhel += Hra.SPEED_FACTOR * this.rychlostOtaceniFaktor;
        }
    }

    public class EliptickaHvezda : EliptickaPlaneta
    {
        public EliptickaHvezda(int velikostPlanety, double rychlost, PointF sE, PointF ex, double hmotnostFaktor = 1) : base(velikostPlanety, rychlost, sE, ex, hmotnostFaktor)
        {

        }
        public override void VykresliSprite() //neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a SO!
        {
            if (this.spriteVytvoren == false)
            {
                Bitmap subtract = new Bitmap(this._sprite);
                using (Graphics g = Graphics.FromImage(subtract))
                {
                    g.Clear(Color.White);
                    g.FillEllipse(Brushes.Black, 0, 0, this._sprite.Width, this._sprite.Height);
                    subtract.MakeTransparent(Color.Black);
                }

                FastBitmap bmp = new FastBitmap(this._sprite);

                bmp.LockImage();
                for (int x = 0; x < this._sprite.Width; x++)
                {
                    for (int y = 0; y < this._sprite.Height; y++)
                    {
                        int num = Hra.randomInstance.Next(1, 255);
                        bmp.SetPixel(x, y,
                            (byte)((num) % 64 + 192),
                            (byte)((num) % 64 + 192),
                            (byte)((num) % 127)
                            );
                    }
                }
                bmp.UnlockImage();
                FastBitmap.FastBoxBlur(this._sprite, 3);
                using (Graphics g = Graphics.FromImage(this._sprite))
                {
                    g.DrawImage(subtract, Point.Empty);
                    this._sprite.MakeTransparent(Color.White);
                }

                this.spriteVytvoren = true;
            }
        }
    }
}