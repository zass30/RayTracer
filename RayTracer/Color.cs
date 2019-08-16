using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace RayTracer
{
    public class Color : Tuple
    {
        public Color(double r, double g, double b) : base(r, g, b, 0)
        {
        }

        public double r
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }

        public double g
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        public double b
        {
            get
            {
                return Z;
            }
            set
            {
                Z = value;
            }
        }

        public double rI
        {
            get
            {
                return toi(r);
            }
        }

        public double gI
        {
            get
            {
                return toi(g);
            }
        }

        public double bI
        {
            get
            {
                return toi(b);
            }
        }

        private int toi(double x)
        {
            return Max(0,Min((int)Math.Round(x * 255), 255));
        }

        // factory class for consistency
        public static Color color(double r, double g, double b)
        {
            return new Color(r, g, b);
        }

        public static Color operator *(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b);
        }

        public static Color operator *(Color a, double b)
        {
            return new Color(a.r * b, a.g * b, a.b * b);
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b);
        }

        public static Color operator *(double b, Color a)
        {
            return a * b;
        }
    }
}
