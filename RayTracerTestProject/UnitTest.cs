
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Math;
using static RayTracer.Tuple;
using static RayTracer.Canvas;
using static RayTracer.Color;
using static RayTracer.Matrix;
using System.Threading.Tasks;
using Windows.Storage;
using System.Text;

namespace RayTracerTestProject
{
    public class TestHelper
    {
        public struct projectile
        {
            public RayTracer.Tuple position;
            public RayTracer.Tuple velocity;
        }

        public struct environment
        {
            public RayTracer.Tuple gravity;
            public RayTracer.Tuple wind;
        }

        public static projectile tick(projectile pr, environment en)
        {
            projectile r;
            r.position = pr.position + pr.velocity;
            r.velocity = pr.velocity + en.gravity + en.wind;
            return r;
        }
    }

    [TestClass]
    public class Chapter1UnitTests
    {
        [TestMethod]
        public void TuplePoint()
        {
            var a = new RayTracer.Tuple(4.3, -4.2, 3.1, 1.0);
            Assert.AreEqual(4.3, a.X);
            Assert.AreEqual(-4.2, a.Y);
            Assert.AreEqual(3.1, a.Z);
            Assert.AreEqual(1.0, a.W);
            Assert.IsTrue(a.isPoint());
            Assert.IsFalse(a.isVector());
        }

        [TestMethod]
        public void TupleVector()
        {
            var a = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            Assert.AreEqual(4.3, a.X);
            Assert.AreEqual(-4.2, a.Y);
            Assert.AreEqual(3.1, a.Z);
            Assert.AreEqual(0.0, a.W);
            Assert.IsFalse(a.isPoint());
            Assert.IsTrue(a.isVector());
        }

        [TestMethod]
        public void TuplePointFactory()
        {
            var p = point(4.3, -4.2, 3.1);
            var t = new RayTracer.Tuple(4.3, -4.2, 3.1, 1.0);
            Assert.IsTrue(areEqual(p, t));
        }

        [TestMethod]
        public void TupleVectorFactory()
        {
            var v = vector(4.3, -4.2, 3.1);
            var t = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            Assert.IsTrue(areEqual(v, t));
        }

        [TestMethod]
        public void FloatEpsilon()
        {
            var a = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            var r = new RayTracer.Tuple(4.30000001, -4.19999999, 3.1, 0.0);
            Assert.IsTrue(areEqual(r, a));
        }

        [TestMethod]
        public void Addition()
        {
            var a1 = new RayTracer.Tuple(3, -2, 5, 1);
            var a2 = new RayTracer.Tuple(-2, 3, 1, 0);
            var r = new RayTracer.Tuple(1, 1, 6, 1);
            Assert.IsTrue(areEqual(r, a1 + a2));
        }

        [TestMethod]
        public void AddingTwoPointsDoesntGivePoint()
        {
            var p1 = point(3, -2, 5);
            var p2 = point(-2, 3, 1);
            var r = p1 + p2;
            Assert.IsFalse(r.isPoint());
            Assert.IsFalse(r.isVector());
        }

        [TestMethod]
        public void SubtractPoints()
        {
            var p1 = point(3, 2, 1);
            var p2 = point(5, 6, 7);
            var r = vector(-2, -4, -6);
            Assert.IsTrue(areEqual(r, p1 - p2));
        }

        [TestMethod]
        public void SubtractVectorAndPoint()
        {
            var p = point(3, 2, 1);
            var v = vector(5, 6, 7);
            var r = point(-2, -4, -6);
            Assert.IsTrue(areEqual(r, p - v));
        }

        [TestMethod]
        public void SubtractVectors()
        {
            var v1 = vector(3, 2, 1);
            var v2 = vector(5, 6, 7);
            var r = vector(-2, -4, -6);
            Assert.IsTrue(areEqual(r, v1 - v2));
        }

        [TestMethod]
        public void SubtractZeroVector()
        {
            var zero = vector(0, 0, 0);
            var v = vector(1, -2, 3);
            var r = vector(-1, 2, -3);
            Assert.IsTrue(areEqual(r, zero - v));
        }

        [TestMethod]
        public void NegateTuple()
        {
            var a = new RayTracer.Tuple(1, -2, 3, -4);
            var r = new RayTracer.Tuple(-1, 2, -3, 4);
            Assert.IsTrue(areEqual(r, !a));
        }

        [TestMethod]
        public void ScalarMultiplicationTuple()
        {
            var a = new RayTracer.Tuple(1, -2, 3, -4);
            var r = new RayTracer.Tuple(3.5, -7, 10.5, -14);
            Assert.IsTrue(areEqual(r, 3.5 * a));
        }

        [TestMethod]
        public void ScalarFractionMultiplicationTuple()
        {
            var a = new RayTracer.Tuple(1, -2, 3, -4);
            var r = new RayTracer.Tuple(0.5, -1, 1.5, -2);
            Assert.IsTrue(areEqual(r, 0.5 * a));
        }

        [TestMethod]
        public void ScalarDivisionTuple()
        {
            var a = new RayTracer.Tuple(1, -2, 3, -4);
            var r = new RayTracer.Tuple(0.5, -1, 1.5, -2);
            Assert.IsTrue(areEqual(r, a / 2));
        }

        [TestMethod]
        public void VectorMagnitude()
        {
            RayTracer.Tuple v;
            v = vector(1, 0, 0);
            Assert.AreEqual(1, v.magnitude());
            v = vector(0, 1, 0);
            Assert.AreEqual(1, v.magnitude());
            v = vector(0, 0, 1);
            Assert.AreEqual(1, v.magnitude());
            v = vector(1, 2, 3);
            Assert.AreEqual(Sqrt(14), v.magnitude());
            v = vector(-1, -2, -3);
            Assert.AreEqual(Sqrt(14), v.magnitude());
        }

        [TestMethod]
        public void VectorNormalize()
        {
            RayTracer.Tuple v, n, r;
            v = vector(4, 0, 0);
            n = normalize(v);
            r = vector(1, 0, 0);
            Assert.IsTrue(areEqual(r, n));

            v = vector(1, 2, 3);
            n = normalize(v);
            r = vector(1 / Sqrt(14), 2 / Sqrt(14), 3 / Sqrt(14));
            Assert.IsTrue(areEqual(r, n));
        }

        [TestMethod]
        public void VectorDot()
        {
            var a = vector(1, 2, 3);
            var b = vector(2, 3, 4);
            Assert.AreEqual(20, dot(a, b));
        }

        [TestMethod]
        public void VectorCross()
        {
            var a = vector(1, 2, 3);
            var b = vector(2, 3, 4);
            var r1 = vector(-1, 2, -1);
            var r2 = vector(1, -2, 1);

            Assert.IsTrue(areEqual(r1, cross(a, b)));
            Assert.IsTrue(areEqual(r2, cross(b, a)));
        }

        [TestMethod]
        public async Task Chapter1Project()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync("chapter1.txt", CreationCollisionOption.ReplaceExisting);
            var s = new StringBuilder();

            TestHelper.projectile proj;
            proj.position = point(0, 1, 0);
            proj.velocity = normalize(vector(1, 1, 0));

            TestHelper.environment env;
            env.gravity = vector(0, -0.1, 0);
            env.wind = vector(-0.01, 0, 0);

            int i = 1;
            while (proj.position.Y > 0)
            {
                proj = TestHelper.tick(proj, env);
                s.AppendLine("x: " + proj.position.X + "\ty: " + proj.position.Y + "\t t: " + i);
                i++;
            }
            await FileIO.WriteTextAsync(sampleFile, s.ToString());
            Assert.AreEqual(18, i);

        }

        [TestMethod]
        public void TupleData()
        {
            var a = new RayTracer.Tuple(1, 2, 3, 4);
            double[] data = new double[4];
            data[0] = 1;
            data[1] = 2;
            data[2] = 3;
            data[3] = 4;

            Assert.AreEqual(data[0], a[0]);
            Assert.AreEqual(data[1], a[1]);
            Assert.AreEqual(data[2], a[2]);
            Assert.AreEqual(data[3], a[3]);
        }
    }

    [TestClass]
    public class Chapter2UnitTests
    {
        [TestMethod]
        public void Colors()
        {
            var c = color(-0.5, 0.4, 1.7);
            Assert.AreEqual(-0.5, c.r);
            Assert.AreEqual(0.4, c.g);
            Assert.AreEqual(1.7, c.b);
        }

        [TestMethod]
        public void AddColors()
        {
            var c1 = color(0.9, 0.6, 0.75);
            var c2 = color(0.7, 0.1, 0.25);
            var r = color(1.6, 0.7, 1);
            Assert.IsTrue(areEqual(r, c1 + c2));
        }

        [TestMethod]
        public void SubtractColors()
        {
            var c1 = color(0.9, 0.6, 0.75);
            var c2 = color(0.7, 0.1, 0.25);
            var r = color(0.2, 0.5, 0.5);
            Assert.IsTrue(areEqual(r, c1 - c2));
        }

        [TestMethod]
        public void ScalarMultiplyColors()
        {
            var c = color(0.2, 0.3, 0.4);
            var r = color(0.4, 0.6, 0.8);
            Assert.IsTrue(areEqual(r, c * 2));
        }

        [TestMethod]
        public void MultiplyColors()
        {
            var c1 = color(1, 0.2, 0.4);
            var c2 = color(0.9, 1, 0.1);
            var r = color(0.9, 0.2, 0.04);
            Assert.IsTrue(areEqual(r, c1 * c2));
        }

        [TestMethod]
        public void Canvas()
        {
            var c = new RayTracer.Canvas(10, 20);
            var black = color(0, 0, 0);
            Assert.AreEqual(10, c.width);
            Assert.AreEqual(20, c.height);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Assert.IsTrue(areEqual(black, pixel_at(c, i, j)));
                }
            }
        }

        [TestMethod]
        public void WritePixel()
        {
            var c = new RayTracer.Canvas(10, 20);
            var red = color(1, 0, 0);
            write_pixel(c, 2, 3, red);
            Assert.IsTrue(areEqual(red, pixel_at(c, 2, 3)));
        }

        [TestMethod]
        public void PPMHeader()
        {

            var c = new RayTracer.Canvas(5, 3);
            string s = canvas_to_ppm(c);
            var reader = new System.IO.StringReader(s);
            string r1 = "P3";
            string r2 = "5 3";
            string r3 = "255";
            Assert.AreEqual(r1, reader.ReadLine());
            Assert.AreEqual(r2, reader.ReadLine());
            Assert.AreEqual(r3, reader.ReadLine());
        }

        [TestMethod]
        public void ColorInt()
        {
            var c = color(1, 0.5, 1.5);
            Assert.AreEqual(255, c.rI);
            Assert.AreEqual(128, c.gI);
            Assert.AreEqual(255, c.bI);

            c = color(-1, -2.5, 1.5);
            Assert.AreEqual(0, c.rI);
            Assert.AreEqual(0, c.gI);
            Assert.AreEqual(255, c.bI);
        }


        [TestMethod]
        public void PPMData()
        {
            var c = new RayTracer.Canvas(5, 3);
            var c1 = color(1.5, 0, 0);
            var c2 = color(0, 0.5, 0);
            var c3 = color(-0.5, 0, 1);

            write_pixel(c, 0, 0, c1);
            write_pixel(c, 2, 1, c2);
            write_pixel(c, 4, 2, c3);

            string s = canvas_to_ppm(c);
            var reader = new System.IO.StringReader(s);

            //skim header
            reader.ReadLine(); reader.ReadLine(); reader.ReadLine();

            string r1 = "255 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            string r2 = "0 0 0 0 0 0 0 128 0 0 0 0 0 0 0";
            string r3 = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 255";
            Assert.AreEqual(r1, reader.ReadLine());
            Assert.AreEqual(r2, reader.ReadLine());
            Assert.AreEqual(r3, reader.ReadLine());
        }

        [TestMethod]
        public void PaintCanvas()
        {
            var blue = color(0, 0, 1);
            var c = new RayTracer.Canvas(2, 2, blue);
            Assert.IsTrue(areEqual(blue, pixel_at(c, 1, 1)));
            Assert.IsTrue(areEqual(blue, pixel_at(c, 1, 0)));
            Assert.IsTrue(areEqual(blue, pixel_at(c, 0, 1)));
            Assert.IsTrue(areEqual(blue, pixel_at(c, 0, 0)));
        }

        [TestMethod]
        public void PPMLongLines()
        {
            var c = new RayTracer.Canvas(10, 2, color(1, 0.8, 0.6));

            string s = canvas_to_ppm(c);
            var reader = new System.IO.StringReader(s);

            //skim header
            reader.ReadLine(); reader.ReadLine(); reader.ReadLine();

            string r1 = "255 204 153 255 204 153 255 204 153 255 204 153 255 204 153 255 204";
            string r2 = "153 255 204 153 255 204 153 255 204 153 255 204 153";
            Assert.AreEqual(r1, reader.ReadLine());
            Assert.AreEqual(r2, reader.ReadLine());
            Assert.AreEqual(r1, reader.ReadLine());
            Assert.AreEqual(r2, reader.ReadLine());
        }

        [TestMethod]
        public void PPMEndNewLine()
        {
            var c = new RayTracer.Canvas(5, 3);
            string s = canvas_to_ppm(c);
            Assert.IsTrue(s.EndsWith("\n"));
        }

        [TestMethod]
        public async Task Chapter2Project()
        {
            var c = new RayTracer.Canvas(900, 550);

            TestHelper.projectile proj;
            proj.position = point(0, 1, 0);
            proj.velocity = normalize(vector(1, 1.8, 0)) * 11.25;

            TestHelper.environment env;
            env.gravity = vector(0, -0.1, 0);
            env.wind = vector(-0.01, 0, 0);

            int i = 1;
            while (proj.position.Y > 0)
            {
                proj = TestHelper.tick(proj, env);
                write_to_canvas(proj.position, c);
                //s.AppendLine("x: " + proj.position.X + "\ty: " + proj.position.Y + "\t t: " + i);
                i++;
            }

            write_pixel(c, 0, 0, color(1, 0.4, 0.7));
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync("chapter2.ppm", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, canvas_to_ppm(c));


            void write_to_canvas(RayTracer.Tuple position, RayTracer.Canvas canvas)
            {
                int x_coord = (int)Math.Round(position.X);
                int y_coord = canvas.height - (int)Math.Round(position.Y);
                if (x_coord >= canvas.width)
                {
                    x_coord = canvas.width - 1;
                }
                if (x_coord < 0)
                {
                    x_coord = 0;
                }
                if (y_coord >= canvas.height)
                {
                    y_coord = canvas.height - 1;
                }
                if (y_coord < 0)
                {
                    y_coord = 0;
                }

                RayTracer.Color red = color(1, 0.4, 0.7);
                write_pixel(c, x_coord, y_coord, red);
            }
        }



    }

    [TestClass]
    public class Chapter3UnitTests
    {
        [TestMethod]
        public void Matrix4x4()
        {
            double[,] data = { {1, 2, 3, 4},
                               {5.5, 6.5, 7.5, 8.5},
                               {9, 10, 11, 12},
                               {13.5, 14.5, 15.5, 16.5}
                             };

            var m = new RayTracer.Matrix(data);
            Assert.AreEqual(1, m[0, 0]);
            Assert.AreEqual(4, m[0, 3]);
            Assert.AreEqual(5.5, m[1, 0]);
            Assert.AreEqual(7.5, m[1, 2]);
            Assert.AreEqual(11, m[2, 2]);
            Assert.AreEqual(13.5, m[3, 0]);
            data[3, 2] = 6; // check changing original data doesn't affect matrix
            Assert.AreEqual(15.5, m[3, 2]);
            Assert.AreEqual(4, m.dimension);
        }

        [TestMethod]
        public void MatrixOtherSizes()
        {
            var m = new RayTracer.Matrix(new double[,] { { -3, 5 }, { 1, -2 } });
            Assert.AreEqual(-3, m[0, 0]);
            Assert.AreEqual(5, m[0, 1]);
            Assert.AreEqual(1, m[1, 0]);
            Assert.AreEqual(-2, m[1, 1]);

            double[,] data = { {-3, 5, 0},
                               {1, -2, 7},
                               {0, 1, 1},
                             };

            m = new RayTracer.Matrix(data);
            Assert.AreEqual(-3, m[0, 0]);
            Assert.AreEqual(-2, m[1, 1]);
            Assert.AreEqual(1, m[2, 2]);
            Assert.AreEqual(3, m.dimension);
        }

        [TestMethod]
        public void MatrixEquality()
        {
            double[,] data1 = { {1, 2, 3, 4},
                                {5, 6, 7, 8},
                                {9, 8, 7, 6},
                                {5, 4, 3, 2}
                             };
            var A = new RayTracer.Matrix(data1);
            var B = new RayTracer.Matrix(data1);
            Assert.IsTrue(areEqual(A, B));


            double[,] data2 = { {2, 3, 4, 5},
                                {6, 7, 8, 9},
                                {8, 7, 6, 5},
                                {4, 3, 2, 1}
                             };

            B = new RayTracer.Matrix(data2);
            Assert.IsFalse(areEqual(A, B));
        }

        [TestMethod]
        public void MatrixMultiplication()
        {
            double[,] data1 = { {1, 2, 3, 4},
                                {5, 6, 7, 8},
                                {9, 8, 7, 6},
                                {5, 4, 3, 2}
                             };


            double[,] data2 = { {-2, 1, 2, 3},
                                {3, 2, 1, -1},
                                {4, 3, 6, 5},
                                {1, 2, 7, 8}
                             };

            var A = new RayTracer.Matrix(data1);
            var B = new RayTracer.Matrix(data2);


            double[,] result = { {20, 22, 50, 48},
                                 {44, 54, 114, 108},
                                 {40, 58, 110, 102},
                                 {16, 26, 46, 42}
                             };

            var R = new RayTracer.Matrix(result);
            Assert.IsTrue(areEqual(R, A * B));
        }

        [TestMethod]
        public void MatrixMultByTuple()
        {
            double[,] data = { {1, 2, 3, 4},
                               {2, 4, 4, 2},
                               {8, 6, 4, 1},
                               {0, 0, 0, 1}
                             };
            var A = new RayTracer.Matrix(data);
            var b = new RayTracer.Tuple(1, 2, 3, 1);
            var r = new RayTracer.Tuple(18, 24, 33, 1);
            Assert.IsTrue(areEqual(r, A * b));
        }

        [TestMethod]
        public void IdentityMatrix()
        {
            double[,] data = { {0, 1, 2, 4},
                               {1, 2, 4, 8},
                               {2, 4, 8, 16},
                               {4, 8, 16, 32}
                             };
            var A = new RayTracer.Matrix(data);
            var I = identity();
            Assert.IsTrue(areEqual(A, A * I));

            var a = new RayTracer.Tuple(1, 2, 3, 4);
            Assert.IsTrue(areEqual(a, I * a));
        }

        [TestMethod]
        public void MatrixTranspose()
        {
            double[,] data = { {0, 9, 3, 0},
                               {9, 8, 0, 8},
                               {1, 8, 5, 3},
                               {0, 0, 5, 8}
                             };
            var A = new RayTracer.Matrix(data);

            double[,] result = { {0, 9, 1, 0},
                                 {9, 8, 8, 0},
                                 {3, 0, 5, 5},
                                 {0, 8, 3, 8}
                               };
            var R = new RayTracer.Matrix(result);
            Assert.IsTrue(areEqual(R, transpose(A)));
        }

        [TestMethod]
        public void IdentityTranspose()
        {
            var I = identity();
            Assert.IsTrue(areEqual(I, transpose(I)));
        }

        [TestMethod]
        public void Determinant2x2()
        {
            var A = new RayTracer.Matrix(new double[,] { { 1, 5 }, { -3, 2 } });
            Assert.AreEqual(17, determinant(A));
        }

        [TestMethod]
        public void SubMatrix()
        {
            double[,] data1 = { {1, 5, 0},
                                {-3, 2, 7},
                                {0, 6, -3},
                              };
            var A = new RayTracer.Matrix(data1);
            var R = new RayTracer.Matrix(new double[,] { { -3, 2 }, { 0, 6 } });
            Assert.IsTrue(areEqual(R, submatrix(A, 0, 2)));

            double[,] data2 = {  {-6, 1, 1, 6},
                                 {-8, 5, 8, 6},
                                 {-1, 0, 8, 2},
                                 {-7, 1, -1, 1}
                              };

            double[,] result = { {-6, 1, 6},
                                 {-8, 8, 6},
                                 {-7, -1, 1},
                               };

            A = new RayTracer.Matrix(data2);
            R = new RayTracer.Matrix(result);
            Assert.IsTrue(areEqual(R, submatrix(A, 2, 1)));
        }

        [TestMethod]
        public void Minor()
        {
            double[,] data = { {3, 5, 0},
                               {2, -1, -7},
                               {6, -1, 5},
                             };
            var A = new RayTracer.Matrix(data);
            var B = submatrix(A, 1, 0);
            Assert.AreEqual(25, determinant(B));
            Assert.AreEqual(25, minor(A,1,0));
        }

        [TestMethod]
        public void Cofactor()
        {
            double[,] data = { {3, 5, 0},
                               {2, -1, -7},
                               {6, -1, 5},
                             };
            var A = new RayTracer.Matrix(data);
            Assert.AreEqual(-12, minor(A, 0, 0));
            Assert.AreEqual(-12, cofactor(A, 0, 0));
            Assert.AreEqual(25, minor(A, 1, 0));
            Assert.AreEqual(-25, cofactor(A, 1, 0));
        }

        [TestMethod]
        public void DeterminantLarger()
        {
            double[,] data1 = { {1, 2, 6},
                               {-5, 8, -4},
                               {2, 6, 4},
                             };
            var A = new RayTracer.Matrix(data1);
            Assert.AreEqual(56, cofactor(A, 0, 0));
            Assert.AreEqual(12, cofactor(A, 0, 1));
            Assert.AreEqual(-46, cofactor(A, 0, 2));
            Assert.AreEqual(-196, determinant(A));

            double[,] data2 = { {-2, -8, 3, 5},
                               {-3, 1, 7, 3},
                               {1, 2, -9, 6},
                               {-6, 7, 7, -9},
                             };
            A = new RayTracer.Matrix(data2);
            Assert.AreEqual(690, cofactor(A, 0, 0));
            Assert.AreEqual(447, cofactor(A, 0, 1));
            Assert.AreEqual(210, cofactor(A, 0, 2));
            Assert.AreEqual(51, cofactor(A, 0, 3));
            Assert.AreEqual(-4071, determinant(A));
        }
    }
}
