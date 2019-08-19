using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace RayTracer
{
    public class Tuple
    {
        private static double epsilon = 0.00001;
        public double X { get { return data[0]; } set { data[0] = value; } }
        public double Y { get { return data[1]; } set { data[1] = value; } }
        public double Z { get { return data[2]; } set { data[2] = value; } }
        public double W { get { return data[3]; } set { data[3] = value; } }
        private double[] data = new double[4];

        public double this[int key]
        {
            get
            {
                return this.data[key];
            }
            set
            {
                this.data[key] = value;
            }
        }

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

        public double magnitude()
        {
            return Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
        }

        // Static operations
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
            if (Math.Abs(a - b) < epsilon)
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

        public static Tuple operator *(Tuple a, double b)
        {
            return new Tuple(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Tuple operator *(double b, Tuple a)
        {
            return a * b;
        }

        public static Tuple operator /(Tuple a, double b)
        {
            return a * (1 / b);
        }

        public static Tuple normalize(Tuple a)
        {
            return a / a.magnitude();
        }

        public static double dot(Tuple a, Tuple b)
        {
            return
                a.X * b.X +
                a.Y * b.Y +
                a.Z * b.Z +
                a.W * b.W;
        }

        public static Tuple cross(Tuple a, Tuple b)
        {
            return vector(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
                );
        }

        public static Tuple reflect(Tuple v, Tuple n)
        {
            return v - n * 2 * dot(v, n);
        }
    }
}
