using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static RayTracer.Tuple;
using static RayTracer.Matrix;

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

        public static Ray transform(Ray r, Matrix m)
        {
            Ray result;
            result.origin = m * r.origin;
            result.direction = m * r.direction;
            return result;
        }
    }

    public class Sphere
    {
        public Matrix transform { get; set; }
        public Tuple center;
        public Sphere()
        {
            transform = identity();
            center = point(0, 0, 0);
        }

        public static Sphere sphere()
        {
            return new Sphere();
        }
        
        public static void set_transform(Sphere s, Matrix t)
        {
            s.transform = t;
            return;
        }

        public static Intersection[] intersect(Sphere s, Ray ray)
        {
            Ray r = Ray.transform(ray, inverse(s.transform));

            double a, b, c; 
            c = dot(r.origin, r.origin) - 2; // more operations using dot, but more compact form. Maybe memoize in the future?
            b = 2 * dot(r.origin, r.direction);
            a = dot(r.direction, r.direction);

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
                return new Intersection[0];

            double root = Sqrt(discriminant) / (2 * a);
            double first = -b / (2 * a);
            Intersection[] result = new Intersection[2];
            result[0] = Intersection.intersection(first - root, s);
            result[1] = Intersection.intersection(first + root, s);
            return result;
        }

        public static Tuple normal_at(Sphere s, Tuple p)
        {
            var object_point = inverse(s.transform) * p;
            var object_normal = object_point - s.center;
            var world_normal = transpose(inverse(s.transform)) * object_normal;
            world_normal.W = 0;
            return normalize(world_normal);
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

        public static Intersection[] intersections(Intersection[] args)
        {
            return args;
        }

        public static Intersection? hit(Intersection[] args)
        {
            double value = double.MaxValue;
            int index = 0;
            bool wasHitFound = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].t < 0)
                    continue;
                else if (args[i].t < value)
                {
                    wasHitFound = true;
                    value = args[i].t;
                    index = i;
                }
            }

            if (wasHitFound)
                return args[index];
            else
                return null;
        }
    }
}
