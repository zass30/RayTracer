using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Matrix
    {
        public int dimension;
        public double[,] data { get; set; }

        public Matrix(double [,] data)
        {
            dimension = data.Length;
            this.data = data.Clone() as double[,]; ;
        }

        public Matrix(int dim)
        {
            this.dimension = dim;
            this.data = new double[dim, dim];
        }
    }
}
