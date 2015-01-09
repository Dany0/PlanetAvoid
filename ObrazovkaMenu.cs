using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlanetAvoid
{
    public class ObrazovkaMenu
    {
        private IList<Button> _tlacitka;

        public IList<Button> Tlacitka
        {
            get { return _tlacitka; }
            set { _tlacitka = value; }
        }

        private PictureBox _pbox;

        public void NastavPBox(PictureBox canvas)
        {
            this._pbox = canvas;
        }
    }
}
