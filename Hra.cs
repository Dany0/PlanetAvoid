using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PlanetAvoid
{
    public class Hra : Scena
    {
        public const float FPS = 30;

        public const float TICK_INTERVAL = (1 / FPS) * 1000;

        public const int SPEED_FACTOR = (int)FPS / 10;

        public System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();

        private IList<GravityObjekt> _gravityObjekty = new List<GravityObjekt>();

        public VesmirnaLod _hrac;

        private int _gameTime;

        public static Random randomInstance = new Random(DateTime.Now.Millisecond);

        private TlacitkoZavrit _closeSign;

        private int _level = 1;

        #region singleton

        private static Hra _instance;
        
        protected Hra(PictureBox canvas, int lvl = 0) : base(canvas)
        {
 
            using (Graphics g = Graphics.FromImage(canvas.Image))
            {
                if (lvl > 0)
                {
                    this._level = lvl;
                    g.DrawString("LEVEL " + this._level.ToString(), new Font("Verdana", 64), new HatchBrush(HatchStyle.Percent70, Color.Green, Color.Black), new Point(canvas.Image.Width / 2 - 170, canvas.Image.Height / 8 - 64));
                }
                else
                {
                    g.DrawString("WELCOME!", new Font("Verdana", 64), new HatchBrush(HatchStyle.Percent70, Color.Green, Color.Black), new Point(canvas.Image.Width / 2 - 240, canvas.Image.Height / 8 - 64));
                }
                
                g.DrawString("LOADING...", new Font("Verdana", 64), new HatchBrush(HatchStyle.Trellis, Color.White, Color.Black), new Point(canvas.Image.Width / 2 - 240, canvas.Image.Height / 2 - 64));
            }


            canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxHra_MouseMove);

            this.gameTimer.Interval = (int)Math.Floor(TICK_INTERVAL);
            this.gameTimer.Tick += gameTimer_Tick;

            this.Inicializuj();

            this.gameTimer.Start();

            //using (StreamWriter myWriter = new StreamWriter(Environment.CurrentDirectory + "config.xml", false))
            //{
            //    XmlSerializer mySerializer = new XmlSerializer(typeof(HraSettings));
            //    mySerializer.Serialize(myWriter, HraSettings);
            //    XmlSerializer mySerializer = new XmlSerializer(typeof(object));
            //    mySerializer.Serialize(myWriter, new
            //    {
            //        Debug = HraSettings.Debug,
            //        Disco = HraSettings.Disco,
            //        MotionBlur = HraSettings.EyeCandySettings.MotionBlur
            //    });
            //}
        }

        public static Hra Instance(PictureBox canvas = null, int lvl = 0)
        {
            return _instance ?? (_instance = new Hra(canvas, lvl));
        }

        #endregion singleton

        #region settings

        [Serializable]
        public static class HraSettings
        {
            public enum DebugSettings
            {
                On,
                Off
            };

            public static DebugSettings Debug = DebugSettings.Off;

            public enum DiscoSettings
            {
                OnPermamently,
                On,
                Off
            };

            public static DiscoSettings Disco = DiscoSettings.Off;

            [Serializable]
            public static class EyeCandySettings
            {
                public enum MotionBlurSettings
                {
                    On,
                    Off
                };
                public static MotionBlurSettings MotionBlur = MotionBlurSettings.Off;
            }
        }
        #endregion

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            this._gameTime++;
            Hra.Instance().HraLoop();
        }

        private bool _lodPustena = false;

        public void HraLoop()
        {
            //simulace gravitace
            for (int i = 0; i <= FPS; i++)
            {
                this.SimulaceGravitace();
            }
            float newX;
            float newY;
            foreach (GravityObjekt gObjekt in this._gravityObjekty.Where(obj => obj.GetType() != typeof(EliptickaPlaneta)))
            {
                if (gObjekt.GetType() == typeof(VesmirnaLod))
                {
                    if (_lodPustena)
                    {
                        newX = (float)(gObjekt.Obdelnik.X + (float)gObjekt.Smer.X * gravSimFaktor);
                        newY = (float)(gObjekt.Obdelnik.Y + (float)gObjekt.Smer.Y * gravSimFaktor);
                        if (newX >= this._canvasWidth - 1)
                        {
                            this.NextLevel();
                        }
                        else if (newX < 0)
                        {
                            this.Reset();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    newX = (float)((gObjekt.Obdelnik.X + gObjekt.Smer.X * gravSimFaktor) + this._canvasWidth) % this._canvasWidth;
                    newY = (float)((gObjekt.Obdelnik.Y + gObjekt.Smer.Y * gravSimFaktor) + this._canvasHeight) % this._canvasHeight;
                }
                gObjekt.Posun(newX, newY);
            }

            this.ObjektyKVykresleni.Clear();

            //pridani objektu k vykresleni
            foreach (GravityObjekt obj in this._gravityObjekty)
            {
                this.ObjektyKVykresleni.Add(obj);
            }
            this.ObjektyKVykresleni.Add(this._closeSign);

            this.Vykresli();
        }

        private void Inicializuj() //tady se inicializuje level
        {
            this._closeSign = new TlacitkoZavrit(this._canvasWidth / 16,
                new Point(
                    this._canvasWidth - this._canvasWidth / 16 - this._canvasWidth / 16 / 4,
                    this._canvasWidth / 16 / 4));
            var pocetSlunecnichSoustav = this._level + this._level / 2 + 1;
            double prumernaHmotnost = 0;
            for (int i = 1; i <= pocetSlunecnichSoustav; i++)
            {
                var slunecniSoustava = new List<EliptickaPlaneta>();
                var slunecniSoustavaVelikost = Math.Min(this._canvasWidth / 2, randomInstance.Next((this._canvasHeight + this._canvasWidth) / 6 / pocetSlunecnichSoustav, (this._canvasHeight + this._canvasWidth) / pocetSlunecnichSoustav));
                var slunecniSoustavaPocetHvezd = randomInstance.Next(1, 2 + 1);
                var slunecniSoustavaPocetPlanet = randomInstance.Next(1, 6 + 1);
                var slunecniSoustavaPozice = new Point(randomInstance.Next(slunecniSoustavaVelikost / 2, this._canvasWidth - slunecniSoustavaVelikost / 2),
                    randomInstance.Next(slunecniSoustavaVelikost / 2, this._canvasHeight - slunecniSoustavaVelikost / 2));

                #region vytvoreni slunce
                double slunceRychlost;
                PointF slunceExcentricita;
                if (slunecniSoustavaPocetHvezd == 1)
                {
                    slunceRychlost = randomInstance.NextDouble() / slunecniSoustavaVelikost;
                    slunceExcentricita = new PointF(slunecniSoustavaVelikost / 16,
                    slunecniSoustavaVelikost / 16);
                    slunecniSoustava.Add(new EliptickaHvezda(slunecniSoustavaVelikost / 4 + 1,
                        slunceRychlost, slunecniSoustavaPozice, slunceExcentricita,
                        randomInstance.Next(slunecniSoustavaPocetHvezd, slunecniSoustavaVelikost / (slunecniSoustavaVelikost / 4)) * slunecniSoustavaPocetPlanet));
                }
                else
                {
                    slunceRychlost = randomInstance.NextDouble() / slunecniSoustavaVelikost * 4;
                    slunceExcentricita = new PointF(slunecniSoustavaVelikost / 8,
                    slunecniSoustavaVelikost / 8);
                    slunecniSoustava.Add(new EliptickaHvezda(slunecniSoustavaVelikost / 4 + 1,
                        slunceRychlost, slunecniSoustavaPozice, slunceExcentricita,
                        randomInstance.Next(slunecniSoustavaPocetHvezd, slunecniSoustavaVelikost / (slunecniSoustavaVelikost / 4)) * slunecniSoustavaPocetPlanet));
                    slunecniSoustava.Add(new EliptickaHvezda(slunecniSoustavaVelikost / 4,
                        slunceRychlost, slunecniSoustavaPozice, new PointF(slunceExcentricita.X * -1, slunceExcentricita.Y * -1),
                        randomInstance.Next(slunecniSoustavaPocetHvezd, slunecniSoustavaVelikost / (slunecniSoustavaVelikost / 4)) * slunecniSoustavaPocetPlanet));
                }

                if (prumernaHmotnost == 0) { prumernaHmotnost = slunecniSoustava.ElementAt(0).Hmotnost; }
                #endregion

                for (int j = 1; j <= slunecniSoustavaPocetPlanet; j++)
                {
                    slunecniSoustava.Add(new EliptickaPlaneta(randomInstance.Next(slunecniSoustavaVelikost / 8,
                        slunecniSoustavaVelikost / 4 * slunecniSoustavaPocetHvezd) / slunecniSoustavaPocetPlanet + 4,
                        randomInstance.NextDouble() / slunecniSoustavaVelikost * 8,
                        slunecniSoustavaPozice, new PointF((slunecniSoustavaVelikost - slunceExcentricita.X) / i + slunceExcentricita.X,
                            (slunecniSoustavaVelikost - slunceExcentricita.Y) / i + slunceExcentricita.Y),
                        randomInstance.Next(slunecniSoustavaPocetHvezd, slunecniSoustavaVelikost / (slunecniSoustavaVelikost / 4))));
                }

                foreach (var gObjekt in slunecniSoustava)
                {
                    if (gObjekt.GetType() != typeof(EliptickaHvezda))
                    {
                        for (int x = 0; x <= FPS * randomInstance.Next(2, 32); x++)
                        {
                            gObjekt.PosunPoElipse();
                        }
                    }
                    prumernaHmotnost = (prumernaHmotnost + gObjekt.Hmotnost) / 2;
                    this._gravityObjekty.Add(gObjekt);
                }
            }

            for (int i = 1; i <= pocetSlunecnichSoustav * 1.5; i++)
            {
                var newAstr = new Asteroid(
                        new Point(
                            this._canvasWidth - this._canvasWidth / 5 + (randomInstance.Next(0, 1000 + 1) - 500),
                            this._canvasHeight),
                        randomInstance.Next(16,
                        64),
                        (int)prumernaHmotnost / randomInstance.Next(2, 8 + 1));
                newAstr.Smer.Y = -1500;
                this._gravityObjekty.Add(newAstr);
            }

            this._hrac = new VesmirnaLod(new Point(0, randomInstance.Next(128, this._canvasHeight - 128)), 32, prumernaHmotnost * this._gravityObjekty.Count * 2);
            this._gravityObjekty.Add(this._hrac);
        }

        private const double SQRT2 = 0.707106781186545; //1 / 1.4142135623731
        private const double CF = 0.926776695296637;
        private const double gravSimFaktor = 0.001;

        private void SimulaceGravitace()
        {
            double oSx;
            double oSy;
            double oSpx;
            double oSpy;

            Vector delta;
            double dxAbs;
            double dyAbs;
            double kosoCtverec;
            double ctverec;
            double koeficient = 0;
            foreach (GravityObjekt objektStred in this._gravityObjekty)
            {
                oSx = objektStred.Obdelnik.X;
                oSy = objektStred.Obdelnik.Y;
                oSpx = oSx + objektStred.Obdelnik.Width / 2;
                oSpy = oSy + objektStred.Obdelnik.Height / 2;
                foreach (GravityObjekt objekt in this._gravityObjekty)
                {
                    if (objekt != objektStred)
                    {
                        delta = new Vector(
                            (objekt.Obdelnik.X + objekt.Obdelnik.Width / 2) - oSpx,
                            (objekt.Obdelnik.Y + objekt.Obdelnik.Height / 2) - oSpy
                            );
                        dxAbs = Math.Abs(delta.X);
                        dyAbs = Math.Abs(delta.Y);
                        kosoCtverec = SQRT2 * (dxAbs + dyAbs);
                        ctverec = dxAbs >= dyAbs ? dxAbs : dyAbs;
                        koeficient = CF * (kosoCtverec <= ctverec ? kosoCtverec : ctverec); //super rychle pocitani vzdalenosti - osmistran
                        if (koeficient >= objekt.Obdelnik.Width / 2)
                        {
                            if (koeficient <= (this._canvasWidth + this._canvasHeight) / 8)
                            {
                                objektStred.Smer += delta * (objekt.Hmotnost / koeficient); //nejpomalejsi cast
                            }
                            //objekt.Smer -= delta * (objektStred.Hmotnost / koeficient);
                        }
                        else
                        {
                            //Vector norm = new Vector(-1 * objekt.Smer.Y, objekt.Smer.X);
                            //objekt.Smer = (objekt.Smer - 2 * Vector.dot(objekt.Smer, norm) * norm)/ objektStred.Hmotnost;
                            //Vector norm2 = new Vector(-1 * objektStred.Smer.Y, objektStred.Smer.X);
                            //objektStred.Smer = (objektStred.Smer - 2 * Vector.dot(objektStred.Smer, norm2) * norm2) / objekt.Hmotnost;
                            //var odrazovyObjekt = objekt.Hmotnost > objektStred.Hmotnost ? objektStred : objekt;
                            //odrazovyObjekt.Smer *= -1;
                            if (objekt.Hmotnost > objektStred.Hmotnost)
                            {
                                objekt.Smer *= 1 - gravSimFaktor;
                                objektStred.Smer *= -1;
                            }
                            else
                            {
                                objekt.Smer *= -1;
                                objektStred.Smer *= 1 - gravSimFaktor;
                            }
                        }
                    }
                }
            }
            //Debug.Print(koeficient.ToString());
        }
        
        public void OnKlavesaNahoru()
        {
            this.NextLevel();
        }

        public void OnKlavesaDolu()
        {
            this._lodPustena = true;
        }

        public void OnKlavesaMezernik()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.gameTimer.Stop();
            _instance = null;
            Hra.Instance(this._pbox);
            GC.Collect();
        }

        public void NextLevel()
        {
            this.gameTimer.Stop();
            _instance = null;
            Hra.Instance(this._pbox, this._level + 1);
            if ((this._level + 1) % 10 == 0 & Hra.HraSettings.Disco != HraSettings.DiscoSettings.OnPermamently)
            {
                Hra.HraSettings.Disco = HraSettings.DiscoSettings.On;
            }
            else if (Hra.HraSettings.Disco != HraSettings.DiscoSettings.OnPermamently)
            {
                Hra.HraSettings.Disco = HraSettings.DiscoSettings.Off;
            }
            GC.Collect();
        }

        public void OnClick(MouseEventArgs e)
        {
            //Vector delta = new Vector(_MousePosition.X - this._hrac.Obdelnik.X + this._hrac.Obdelnik.Width,
            //    _MousePosition.Y - this._hrac.Obdelnik.Y + this._hrac.Obdelnik.Height / 2);
            //var a = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y); ;
            if (!_lodPustena)
            {
                this._hrac.Smer = new Vector((_MousePosition.X - this._hrac.Obdelnik.Width) * (50 + (this._canvasWidth / 20) / 10),
                    (_MousePosition.Y - this._hrac.Obdelnik.Height / 2) * this._canvasWidth / 75);
                this._lodPustena = true;
            }
        }

        private void pictureBoxHra_MouseMove(object sender, MouseEventArgs e)
        {
            Hra.Instance()._MousePosition = new Point(e.X, e.Y);
        }


        private Stopwatch _FPSCounter = new Stopwatch();

        private Bitmap debugTextImage = new Bitmap(128, 32);

        private int _avgFps;
        private int _lastFps;

        private Font hudFont = new Font("System", 12, FontStyle.Bold);

        public override unsafe void Vykresli()
        {
            this._FPSCounter.Start();

            if (g == null) g = Graphics.FromImage(this._img);

            if (Hra.HraSettings.EyeCandySettings.MotionBlur == Hra.HraSettings.EyeCandySettings.MotionBlurSettings.Off)
            {
                if (Hra.HraSettings.Disco != Hra.HraSettings.DiscoSettings.Off) //disco
                {
                    if (this._FPSCounter.ElapsedMilliseconds % 5 == 0)
                    {
                        g.Clear(Color.FromArgb((int)(randomInstance.NextDouble() * 127 + 64),
                            (int)(randomInstance.NextDouble() * 127 + 64),
                            (int)(randomInstance.NextDouble() * 127 + 64)));
                    }
                }
                else
                {
                    g.Clear(Color.Black);
                    g.DrawImage(Pozadi.Instance.Sprite, Point.Empty);
                }
            }
            else
            {
                g.DrawImage(Pozadi.Instance.Sprite, Point.Empty);
            }

            if (!this._lodPustena)
            {
                g.DrawLine(new Pen(Brushes.Red, 5), new Point((int)(this._hrac.Obdelnik.X + this._hrac.Obdelnik.Width),
                    (int)(Hra.Instance()._hrac.Obdelnik.Y + Hra.Instance()._hrac.Obdelnik.Height / 2)), new Point(_MousePosition.X, _MousePosition.Y));
                g.FillEllipse(Brushes.White, _MousePosition.X - 8, _MousePosition.Y - 8, 16, 16);
            }
            if (Hra.HraSettings.Debug == Hra.HraSettings.DebugSettings.On)
            {
                g.DrawLine(new Pen(Brushes.Red, 10), new Point(_MousePosition.X, 0), new Point(_MousePosition.X, this._canvasHeight));
                g.DrawLine(new Pen(Brushes.Red, 10), new Point(0, _MousePosition.Y), new Point(this._canvasWidth, _MousePosition.Y));
                g.FillRectangle(Brushes.YellowGreen, _MousePosition.X - 5, _MousePosition.Y - 5, 10, 10);
                using (Graphics gfx = Graphics.FromImage(this.debugTextImage))
                {
                    gfx.Clear(Color.Transparent);
                    gfx.DrawString(String.Format("X: {0} Y {1}", _MousePosition.X, _MousePosition.Y), hudFont, Brushes.Red, new PointF(0, 15));
                    gfx.DrawString("FPS: " + this._avgFps.ToString(), hudFont, Brushes.Red, Point.Empty);
                    FastBitmap.FastBoxBlur(this.debugTextImage, 9);
                    gfx.DrawString(String.Format("X: {0} Y {1}", _MousePosition.X, _MousePosition.Y), hudFont, Brushes.Red, new PointF(0, 15));
                    gfx.DrawString("FPS: " + this._avgFps.ToString(), hudFont, Brushes.Red, Point.Empty);
                }
                g.DrawImage(this.debugTextImage, _MousePosition.X + 10, _MousePosition.Y + 10, 128, 32);
            }

            g.DrawString("Level: " + this._level.ToString(), hudFont, Brushes.White, new Point(10, 10));
            if (Hra.HraSettings.EyeCandySettings.MotionBlur == Hra.HraSettings.EyeCandySettings.MotionBlurSettings.On) //motion blur
            {
                var lb = this._img.LockBits(new Rectangle(0, 0, this._canvasWidth, this._canvasHeight), ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);
                var scan0 = (byte*)lb.Scan0;

                for (int x = 3; x < this._canvasWidth * this._canvasHeight * 4; x += 4)
                {
                    if (scan0[x] != 0)
                    {
                        scan0[x] = (byte)(scan0[x] * 0.75);
                    }
                }
                this.__img.UnlockBits(lb);
            }

            base.Vykresli();


            this._lastFps = (int)(1000 / this._FPSCounter.ElapsedMilliseconds);
            this._avgFps = (this._avgFps + this._lastFps) / 2;
            this._FPSCounter.Reset();
        }
    }
}