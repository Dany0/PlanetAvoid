using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlanetAvoid
{
    public class Scena : IDisposable
    {
        private IList<AVykreslitelnyObjekt> _objektyKVykresleni = new List<AVykreslitelnyObjekt>();

        public IList<AVykreslitelnyObjekt> ObjektyKVykresleni
        {
            get { return _objektyKVykresleni; }
            set { _objektyKVykresleni = value; }
        }

        public Point _MousePosition;


        protected int _canvasWidth;
        protected int _canvasHeight;

        public PictureBox _pbox;
        protected Bitmap __img;
        protected Graphics g = null;

        protected Bitmap _img
        {
            get { return __img; }
            set
            {
                this.__img = value;
                this._pbox.Image = value;
                if (value != null)
                {
                    this._canvasWidth = value.Width;
                    this._canvasHeight = value.Height;
                }
            }
        }
        
        public virtual void Vykresli()
        {
            if (g == null) g = Graphics.FromImage(this._img);
            foreach (AVykreslitelnyObjekt objekt in this._objektyKVykresleni)
            {
                g.DrawImage(objekt.Sprite, objekt.Obdelnik);
            }
            _pbox.Invalidate();
        }

        public void Dispose()
        {
            GC.Collect();
        }

        public Scena(PictureBox canvas)
        {
            this._pbox = canvas; this._img = new Bitmap(canvas.Width, canvas.Height);
        }
    }
}