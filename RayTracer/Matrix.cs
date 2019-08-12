using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Matrix
    {
        private static double epsilon = 0.000001;
        public int dimension;
        public double[,] data { get; set; }

        public Matrix(double [,] data)
        {
            dimension = (int)Math.Sqrt(data.Length);
            this.data = data.Clone() as double[,]; ;
        }

        public Matrix(int dim)
        {
            this.dimension = dim;
            this.data = new double[dim, dim];
        }

        public double this[int key1, int key2]
        {
            get
            {
                return this.data[key1, key2];
            }
            set
            {
                this.data[key1, key2] = value;
            }
        }

        public static bool areEqual(Matrix a, Matrix b)
        {
            if (a.dimension != b.dimension)
                return false;

            for (int i = 0; i < a.dimension; i++)
            {
                for (int j = 0; j < a.dimension; j++)
                {
                    if (!areClose(a[i, j], b[i, j]))
                        return false;
                }
            }
            return true;
        }

        private static bool areClose(double a, double b)
        {
            if (Math.Abs(a - b) < epsilon)
                return true;
            return false;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            int dim = a.dimension;
            Matrix result = new Matrix(dim);

            Parallel.For(0, dim, i =>
            {
                for (int j = 0; j < dim; ++j) 
                    for (int k = 0; k < dim; ++k) 
                        result[i,j] += a[i,k] * b[k,j];
            }
            );

            return result;
        }

        public static Tuple operator *(Matrix a, Tuple b)
        {
            double[] bvec = new double[4];
            bvec[0] = b.X;
            bvec[1] = b.Y;
            bvec[2] = b.Z;
            bvec[3] = b.W;
            double[] result = new double[4];

            Parallel.For(0, 4, i =>
            {
                for (int j = 0; j < 4; ++j)
                        result[i] += a[i, j] * bvec[j];
            }
            );
            return new Tuple(result[0], result[1], result[2], result[3]);
        }
    }
}
