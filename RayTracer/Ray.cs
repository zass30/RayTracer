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
/*          c = r.origin.X * r.origin.X + r.origin.Y * r.origin.Y + r.origin.Z * r.origin.Z - 1;
            b = 2 * (r.origin.X * r.direction.X + r.origin.Y * r.direction.Y + r.origin.Z * r.direction.Z);
            a = r.direction.X * r.direction.X + r.direction.Y * r.direction.Y + r.direction.Z * r.direction.Z;*/

            c = Tuple.dot(r.origin, r.origin) - 2; // more operations using dot, but more compact form. Maybe memoize in the future?
            b = 2 * Tuple.dot(r.origin, r.direction);
            a = Tuple.dot(r.direction, r.direction);

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
                return new double[0];

            double root = Math.Sqrt(discriminant) / (2 * a);
            double first = -b / (2 * a);
            double [] result = new double[2];
            result[0] = first - root;
            result[1] = first + root;
            return result;
        }
    }

    public struct Intersection
    {
        public double t;
        public object obj;

        public Intersection(double t, object obj)
        {
            this.t = t;
            this.obj = obj;
        }

        public static Intersection intersection(double t, object obj)
        {
            return new Intersection(t, obj);
        }
    }
}
