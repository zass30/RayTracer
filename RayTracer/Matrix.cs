using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Matrix
    {
        private static double epsilon = 0.00001;
        public int dimension;
        public double[,] data { get; set; }

        public Matrix(double[,] data)
        {
            dimension = (int)Math.Sqrt(data.Length);
            this.data = data.Clone() as double[,]; ;
        }

        public Matrix(int dim)
        {
            this.dimension = dim;
            this.data = new double[dim, dim];
        }

        public static Matrix identity()
        {
            double[,] data = { {1, 0, 0, 0},
                               {0, 1, 0, 0},
                               {0, 0, 1, 0},
                               {0, 0, 0, 1}
                             };
            return new Matrix(data);
        }

        public double this[int row, int column]
        {
            get
            {
                return this.data[row, column];
            }
            set
            {
                this.data[row, column] = value;
            }
        }

        public bool isInvertible()
        {
            return determinant(this) != 0;
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
            for (int i = 0; i < dim; ++i)
            {
                for (int j = 0; j < dim; ++j)
                    for (int k = 0; k < dim; ++k)
                        result[i, j] += a[i, k] * b[k, j];
            }
            return result;
        }

        public static Tuple operator *(Matrix a, Tuple b)
        {
            Tuple result = new Tuple(0, 0, 0, 0);
            for (int i = 0; i < 4; ++i)
            { 
                for (int j = 0; j < 4; ++j)
                    result[i] += a[i, j] * b[j];
            }
            return result;
        }

        public static Matrix transpose(Matrix a)
        {
            double[,] data = {{a[0,0], a[1,0], a[2,0], a[3,0]},
                              {a[0,1], a[1,1], a[2,1], a[3,1]},
                              {a[0,2], a[1,2], a[2,2], a[3,2]},
                              {a[0,3], a[1,3], a[2,3], a[3,3]},
                             };
            return new Matrix(data);
        }

        public static double determinant(Matrix a)
        {
            if (a.dimension == 2)
                return a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
            else
            {
                double result = 0;
                for (int i = 0; i < a.dimension; i++)
                {
                    result += a[0, i] * cofactor(a, 0, i);
                }
                return result;
            }
        }

        public static Matrix submatrix(Matrix a, int r, int c)
        {
            int dim = a.dimension;
            double[,] data = new double[dim - 1, dim - 1];
            int skip_i = 0;
            for (int i = 0; i < dim - 1; i++)
            {
                if (i >= r)
                    skip_i = 1;
                int skip_j = 0;
                for (int j = 0; j < dim - 1; j++)
                {
                    if (j >= c)
                        skip_j = 1;
                    data[i, j] = a[i + skip_i, j + skip_j];
                }
            }
            return new Matrix(data);
        }

        public static double minor(Matrix a, int r, int c)
        {
            return determinant(submatrix(a, r, c));
        }

        public static double cofactor(Matrix a, int r, int c)
        {
            return (((r+c) % 2 == 0) ? 1 : -1) * determinant(submatrix(a, r, c));
        }

        public static Matrix inverse(Matrix a)
        {
            double det = determinant(a);
            int dim = a.dimension;
            double c;
            Matrix r = new Matrix(dim);

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    c = cofactor(a, i, j);
                    r[j, i] = c / det;
                }
            }
            return r;
        }

        public Matrix translate(double x, double y, double z)
        {
            return translation(x, y, z);
        }

        public static Matrix translation(double x, double y, double z)
        {
            Matrix r = identity();
            r[0, 3] = x;
            r[1, 3] = y;
            r[2, 3] = z;
            return r;
        }

        public Matrix scale(double x, double y, double z)
        {
            return scaling(x, y, z);
        }

        public static Matrix scaling(double x, double y, double z)
        {
            Matrix r = identity();
            r[0, 0] = x;
            r[1, 1] = y;
            r[2, 2] = z;
            return r;
        }

        public Matrix rotate_x(double rad)
        {
            return rotation_x(rad);
        }

        public static Matrix rotation_x(double rad)
        {
            Matrix r = identity();
            r[1, 1] = Math.Cos(rad);
            r[1, 2] = -Math.Sin(rad);
            r[2, 2] = Math.Cos(rad);
            r[2, 1] = Math.Sin(rad);
            return r;
        }

        public Matrix rotate_y(double rad)
        {
            return rotation_y(rad);
        }

        public static Matrix rotation_y(double rad)
        {
            Matrix r = identity();
            r[0, 0] = Math.Cos(rad);
            r[0, 2] = Math.Sin(rad);
            r[2, 0] = -Math.Sin(rad);
            r[2, 2] = Math.Cos(rad);
            return r;
        }

        public Matrix rotate_z(double rad)
        {
            return rotation_z(rad);
        }

        public static Matrix rotation_z(double rad)
        {
            Matrix r = identity();
            r[0, 0] = Math.Cos(rad);
            r[0, 1] = -Math.Sin(rad);
            r[1, 0] = Math.Sin(rad);
            r[1, 1] = Math.Cos(rad);
            return r;
        }
    }
}
