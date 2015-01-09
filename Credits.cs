using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class Credits : ObrazovkaMenu
    {
        #region singleton

        private static Credits _instance;
        private Credits() { }

        public static Credits Instance
        {
           get 
           {
               if (_instance == null)
              {
                  _instance = new Credits();
              }
               return _instance;
           }
        }
        #endregion
    }
}
