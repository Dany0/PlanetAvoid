using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class Button : AVykreslitelnyObjekt
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public override void VykresliSprite()
        {
            throw new NotImplementedException();
        }
    }
}
