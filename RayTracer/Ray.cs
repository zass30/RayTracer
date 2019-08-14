using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public struct Ray
    {
        public Tuple origin;
        public Tuple direction;

        public Ray(Tuple origin, Tuple direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public static Ray ray(Tuple origin, Tuple direction)
        {
            return new Ray(origin, direction);
        }

        public static Tuple position(Ray r, double t)
        {
            return r.origin + t * r.direction;
        }
    }

    public class Sphere
    {
        public Sphere()
        {
        }

        public static Sphere sphere()
        {
            return new Sphere();
        }

        public static double[] intersect(Sphere s, Ray r)
        {
            double a, b, c; 
            c = r.origin.X * r.origin.X + r.origin.Y * r.origin.Y + r.origin.Z * r.origin.Z - 1;
            b = 2 * (r.origin.X * r.direction.X + r.origin.Y * r.direction.Y + r.origin.Z * r.direction.Z);
            a = r.direction.X * r.direction.X + r.direction.Y * r.direction.Y + r.direction.Z * r.direction.Z;
            double root = Math.Sqrt(b * b - 4 * a * c) / (2 * a);
            double first = -b / (2 * a);
            double [] result = new double[2];
            result[0] = first - root;
            result[1] = first + root;
            return result;
        }
    }
}
