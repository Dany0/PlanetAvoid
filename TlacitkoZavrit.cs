using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class TlacitkoZavrit : AVykreslitelnyObjekt
    {
        public static Color TLACITKO_MAGICKA_BARVA = Color.FromArgb(255, 0, 0, 254);

        private int _velikostTlacitka;

        public TlacitkoZavrit(int velikost, Point pozice)
        {
            this._velikostTlacitka = velikost;
            this.Sprite = new Bitmap(this._velikostTlacitka, this._velikostTlacitka);
            this.Obdelnik = new Rectangle(pozice, this.Sprite.Size);
            this.VykresliSprite();
            FastBitmap.FastBoxBlur(this._sprite, 6);
            this.VykresliSprite();
        }

        public override void VykresliSprite()
        {
            using (Graphics g = Graphics.FromImage(this._sprite))
            {
                //stin
                Pen stinPen = new Pen(new SolidBrush(Color.FromArgb(127, 127, 127, 127)), _velikostTlacitka / 4);
                g.DrawLine(stinPen, _velikostTlacitka / 16, 0, _velikostTlacitka + _velikostTlacitka / 16, _velikostTlacitka);
                g.DrawLine(stinPen, _velikostTlacitka + _velikostTlacitka / 16, 0, _velikostTlacitka / 16, _velikostTlacitka);

                //krizek
                Pen krizekPen = new Pen(new SolidBrush(TLACITKO_MAGICKA_BARVA), _velikostTlacitka / 4);
                g.DrawLine(krizekPen, 0, 0, _velikostTlacitka, _velikostTlacitka);
                g.DrawLine(krizekPen, _velikostTlacitka, 0, 0, _velikostTlacitka);
            }
            this._statickySpriteVykreslen = true;
        }
    }
}
