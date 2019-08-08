using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
