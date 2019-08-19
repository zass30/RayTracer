using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using static RayTracer.Tuple;
using static RayTracer.Matrix;
using static RayTracer.Color;
using static RayTracer.Sphere;
using static RayTracer.Light;
using System.Collections;

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
        public Material material;

        public Sphere()
        {
            transform = identity();
            center = point(0, 0, 0);
            material = Material.material();
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

        public static List<Intersection> intersect(Sphere s, Ray ray)
        {
            Ray r = Ray.transform(ray, inverse(s.transform));

            double a, b, c;
            c = dot(r.origin, r.origin) - 2; // more operations using dot, but more compact form. Maybe memoize in the future?
            b = 2 * dot(r.origin, r.direction);
            a = dot(r.direction, r.direction);

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
                return new List<Intersection>();

            double root = Sqrt(discriminant) / (2 * a);
            double first = -b / (2 * a);
            List<Intersection> result = new List<Intersection>(2);
            result.Add(Intersection.intersection(first - root, s));
            result.Add(Intersection.intersection(first + root, s));
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

        public static List<Intersection> intersections(List<Intersection> args)
        {
            return args;
        }

        public static Intersection? hit(List<Intersection> args)
        {
            double value = double.MaxValue;
            int index = 0;
            bool wasHitFound = false;
            for (int i = 0; i < args.Count; i++)
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

    public struct Light
    {
        public Color intensity;
        public Tuple position;

        public static Light point_light(Tuple position, Color intensity)
        {
            Light light;
            light.intensity = intensity;
            light.position = position;
            return light;
        }

        public static bool areEqual(Light a, Light b)
        {
            return RayTracer.Color.areEqual(a.intensity, b.intensity) && RayTracer.Tuple.areEqual(a.position, b.position);
        }

        public static Color lighting(Material material, Light light, Tuple point, Tuple eyev, Tuple normalv)
        {
            var black = color(0, 0, 0);
            var effective_color = material.color * light.intensity;
            var lightv = normalize(light.position - point);
            Color ambient = effective_color * material.ambient;
            Color diffuse, specular;

            var light_dot_normal = dot(lightv, normalv);
            if (light_dot_normal < 0)
            {
                diffuse = black;
                specular = black;
            }
            else
            {
                diffuse = effective_color * material.diffuse * light_dot_normal;
                var reflectv = reflect(-1 * lightv, normalv);
                var reflect_dot_eye = dot(reflectv, eyev);
                if (reflect_dot_eye <= 0)
                    specular = black;
                else
                {
                    var factor = Math.Pow(reflect_dot_eye, material.shininess);
                    specular = light.intensity * material.specular * factor;
                }
            }
            return ambient + diffuse + specular;
        }
    }

    public struct Material
    {
        public Color color;
        public double ambient;
        public double diffuse;
        public double specular;
        public double shininess;

        public static Material material()
        {
            Material m;
            m.color = new Color(1, 1, 1);
            m.ambient = 0.1;
            m.diffuse = 0.9;
            m.specular = 0.9;
            m.shininess = 200;
            return m;
        }

        public static bool areEqual(Material a, Material b)
        {
            return Tuple.areEqual(a.color, b.color) &&
                a.ambient == b.ambient &&
                a.diffuse == b.diffuse &&
                a.specular == b.specular &&
                a.shininess == b.shininess;
        }
    }

    public struct World
    {
        public Sphere[] objects;
        public Light light;

        public static World world()
        {
            World w;
            w.objects = new Sphere[0];
            w.light = new Light();
            return w;
        }

        public static World default_world()
        {
            World w;
            var s1 = sphere();
            s1.material.color = color(0.8, 1.0, 0.6);
            s1.material.diffuse = 0.7;
            s1.material.specular = 0.2;
            var s2 = sphere();
            s2.transform = scaling(0.5, 0.5, 0.5);

            w.objects = new Sphere[2];
            w.objects[0] = s1;
            w.objects[1] = s2;

            w.light = point_light(point(-10, 10, -10), color(1, 1, 1));
            return w;
        }
        public static List<Intersection> intersect_world(World w, Ray r)
        {
            List<Intersection> results = new List<Intersection>();
            foreach (Sphere s in w.objects)
            {
                var xs = intersect(s, r);
                results.AddRange(xs);
            }
            results.Sort((x, y) => x.t.CompareTo(y.t));
            return results;
        }
    }
}
