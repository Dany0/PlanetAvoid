using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class GravityObjekt : AVykreslitelnyObjekt
    {
        public Vector Smer = new Vector();

        public double Hmotnost;

        public override void VykresliSprite()
        {
            throw new InvalidOperationException();
        }
    }
}
