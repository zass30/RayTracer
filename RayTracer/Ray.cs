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
}
