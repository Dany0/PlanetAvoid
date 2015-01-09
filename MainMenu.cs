using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetAvoid
{
    public class MainMenu : ObrazovkaMenu
    {
        #region singleton

        private static MainMenu _instance;
        private MainMenu() { }

        public static MainMenu Instance
        {
           get 
           {
               if (_instance == null)
              {
                  _instance = new MainMenu();
              }
               return _instance;
           }
        }
        #endregion
    }
}
