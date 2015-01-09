using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class Pozadi : AVykreslitelnyObjekt
    {
        #region singleton

        private static Pozadi _instance;

        public static Pozadi Instance
        {
            get
            {
                return _instance == null ? _instance = new Pozadi() : _instance;
            }
        }
        #endregion
        const int _starCount = 2500;

        private bool poVykresleno = false;

        private Pozadi()
        {
            this.Sprite = new Bitmap(Hra.Instance()._pbox.Width, Hra.Instance()._pbox.Height);
            this.Obdelnik = new Rectangle(Point.Empty, this._sprite.Size);
        }

        public override void VykresliSprite()
        {
            if (!this.poVykresleno)
            {
                using (Graphics g = Graphics.FromImage(this._sprite))
                {
                    g.Clear(Color.Black);
                    for (int l = 0; l <= 2500; l++)
                    {
                        int velikostHvezdy = Hra.randomInstance.Next(1, 4); //ruzna velikost pro pocit hloubky

                        int starX = Hra.randomInstance.Next(1, this._sprite.Width - 1);
                        int starY = Hra.randomInstance.Next(1, this._sprite.Height - 1);

                        g.FillEllipse(new SolidBrush(Color.FromArgb(63, 255, 255, 255)), starX - velikostHvezdy, starY, velikostHvezdy * 3, velikostHvezdy); //ocas
                        g.FillEllipse(Brushes.White, starX, starY, velikostHvezdy, velikostHvezdy);
                    }
                    FastBitmap.FastBoxBlur(this._sprite, 3);
                }
                this.poVykresleno = true;
            }
        }
    }
}
