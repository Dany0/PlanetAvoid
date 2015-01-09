using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PlanetAvoid
{
    public class Vector
    {
        public double X = 0;
        public double Y = 0;

        private static Vector _empty = new Vector(0.0, 0.0);

        public static Vector Empty
        {
            get { return _empty; }
        }

        public static Vector operator +(Vector V1, Vector V2)
        {
            return new Vector(V1.X + V2.X, V1.Y + V2.Y);
        }

        public static Vector operator -(Vector V1, Vector V2)
        {
            return new Vector(V1.X - V2.X, V1.Y - V2.Y);
        }

        public static Vector operator -(double m, Vector V2)
        {
            return new Vector(m - V2.X, m - V2.Y);
        }

        public static Vector operator *(double m, Vector V)
        {
            return new Vector(m * V.X, m * V.Y);
        }

        public static Vector operator *(Vector V, double m)
        {
            return new Vector(m * V.X, m * V.Y);
        }

        public static Vector operator +(Vector V, double m)
        {
            return new Vector(m + V.X, m + V.Y);
        }

        public static Vector operator +(double m ,Vector V)
        {
            return new Vector(m + V.X, m + V.Y);
        }

        public static Vector operator /(double m, Vector V)
        {
            return new Vector(m / V.X, m / V.Y);
        }

        public static Vector operator /(Vector V, double m)
        {
            return new Vector(V.X / m, V.Y / m);
        }

        public static Vector operator *(Vector V1, Vector V2)
        {
            return new Vector(V1.X * V2.X, V1.Y * V2.Y);
        }

        static public double dot(Vector V1, Vector V2)
        {
            return V1.X * V2.X + V1.Y * V2.Y;
        }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector() { }

        public PointF ToPoint()
        {
            return new PointF((float)this.X, (float)this.Y);
        }

        public PointF ToPoint(float dx, float dy)
        {
            return new PointF(dx + (float)this.X, dy + (float)this.Y);
        }
    }
}
