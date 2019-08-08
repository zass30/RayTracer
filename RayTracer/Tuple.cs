using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Tuple
    {
        private static double epsilon = 0.000001;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public Tuple(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public bool isPoint()
        {
            if (W == 1)
                return true;
            return false;
        }

        public bool isVector()
        {
            if (W == 0)
                return true;
            return false;
        }

        public static Tuple point(double x, double y, double z)
        {
            return new Tuple(x, y, z, 1.0);
        }

        public static Tuple vector(double x, double y, double z)
        {
            return new Tuple(x, y, z, 0.0);
        }

        public static bool areEqual(Tuple a, Tuple b)
        {
            if (areClose(a.X, b.X) &&
                areClose(a.Y, b.Y) &&
                areClose(a.Z, b.Z) &&
                areClose(a.W, b.W))
                return true;
            return false;
        }

        private static bool areClose(double a, double b)
        {
            if (Math.Abs(a-b) < epsilon)
                return true;
            return false;
        }

        public static Tuple operator +(Tuple a, Tuple b)
        {
            return new Tuple(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Tuple operator -(Tuple a, Tuple b)
        {
            return a + !b;
        }

        public static Tuple operator !(Tuple a)
        {
            return a * -1;
        }

        public static Tuple operator*(Tuple a, double b)
        {
            return new Tuple(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Tuple operator*(double b, Tuple a)
        {
            return a * b;
        }

    }

}
