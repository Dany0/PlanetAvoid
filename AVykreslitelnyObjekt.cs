using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public abstract class AVykreslitelnyObjekt
    {
        public RectangleF Obdelnik; //pouziva se pro pozici objektu a kolizi objektu

        protected Bitmap _sprite;

        public virtual Bitmap Sprite
        {
            get
            {
                if (this._sprite != null && !this._statickySpriteVykreslen)
                {
                    this.VykresliSprite(); //neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a SO!
                }
                return _sprite;
            }
            set { _sprite = value; }
        }

        protected bool _statickySpriteVykreslen = false;

        /// <summary>
        /// Neodkazovat na this.Sprite z teto metody nebo dojde k zacykleni a StackOverflow!
        /// </summary>
        public abstract void VykresliSprite();

        public void Posun(float x, float y)
        {
            this.Obdelnik = new RectangleF(new PointF(x, y), this.Obdelnik.Size);
        }
    }
}
