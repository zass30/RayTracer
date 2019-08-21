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
using static RayTracer.Ray;
using static RayTracer.World;
using static RayTracer.Intersection;
using static RayTracer.Computations;
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

        public static Tuple position(Ray ray, double t)
        {
            return ray.origin + t * ray.direction;
        }

        public static Ray transform(Ray ray, Matrix transform)
        {
            Ray result;
            result.origin = transform * ray.origin;
            result.direction = transform * ray.direction;
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

        public static void set_transform(Sphere sphere, Matrix transform)
        {
            sphere.transform = transform;
            return;
        }

        public static List<Intersection> intersect(Sphere sphere, Ray ray)
        {
            Ray r = Ray.transform(ray, inverse(sphere.transform));

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
            result.Add(Intersection.intersection(first - root, sphere));
            result.Add(Intersection.intersection(first + root, sphere));
            return result;
        }

        public static Tuple normal_at(Sphere sphere, Tuple point)
        {
            var object_point = inverse(sphere.transform) * point;
            var object_normal = object_point - sphere.center;
            var world_normal = transpose(inverse(sphere.transform)) * object_normal;
            world_normal.W = 0;
            return normalize(world_normal);
        }
    }

    public struct Intersection
    {
        public double t;
        public Sphere obj;

        public Intersection(double t, Sphere obj)
        {
            this.t = t;
            this.obj = obj;
        }

        public static Intersection intersection(double t, Sphere obj)
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
            return Tuple.areEqual(a.intensity, b.intensity) && RayTracer.Tuple.areEqual(a.position, b.position);
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
            World w = new World();
            return w;
        }

        public static World default_world()
        {
            World w = new World();
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
        public static List<Intersection> intersect_world(World world, Ray ray)
        {
            List<Intersection> results = new List<Intersection>();
            foreach (Sphere s in world.objects)
            {
                var xs = intersect(s, ray);
                results.AddRange(xs);
            }
            results.Sort((x, y) => x.t.CompareTo(y.t));
            return results;
        }

        public static Color shade_hit(World world, Computations computations)
        {
            return lighting(computations.obj.material, world.light, computations.point, computations.eyev, computations.normalv);
        }

        public static Color color_at(World world, Ray ray)
        {
            var xs = intersect_world(world, ray);
            var h = hit(xs);
            if (h == null)
                return color(0, 0, 0);
            var comps = prepare_computations((Intersection)h, ray);

            var c = shade_hit(world, comps);
            return c;
        }
    }

    public struct Computations
    {
        public double t;
        public Sphere obj;
        public Tuple point;
        public Tuple eyev;
        public Tuple normalv;
        public bool inside;

        public static Computations prepare_computations(Intersection intersection, Ray ray)
        {
            Computations comps = new Computations();
            comps.t = intersection.t;
            comps.obj = intersection.obj;
            comps.point = position(ray, comps.t);
            comps.eyev = -1 * ray.direction;
            comps.normalv = normal_at(comps.obj, comps.point);

            if (dot(comps.eyev, comps.normalv) < 0)
            {
                comps.inside = true;
                comps.normalv *= -1;
            }
            else
                comps.inside = false;
            return comps;
        }
    }

    public struct Camera
    {
        public int hsize;
        public int vsize;
        public double field_of_view;
        public Matrix transform;

        public static Camera camera(int hsize, int vsize, double field_of_view)
        {
            Camera c = new Camera();
            c.hsize = hsize;
            c.vsize = vsize;
            c.field_of_view = field_of_view;
            c.transform = identity();
            return c;
        }
    }
}
