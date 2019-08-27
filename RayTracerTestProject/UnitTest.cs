
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Math;
using static RayTracer.Tuple;
using static RayTracer.Canvas;
using static RayTracer.Color;
using static RayTracer.Matrix;
using static RayTracer.Ray;
using static RayTracer.Sphere;
using static RayTracer.Intersection;
using static RayTracer.Light;
using static RayTracer.Material;
using static RayTracer.World;
using static RayTracer.Computations;
using static RayTracer.Camera;
using System.Threading.Tasks;
using Windows.Storage;
using System.Text;
using System.Collections;

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

        public static async Task write_to_file(string filename, string contents)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, contents);
        }

        // writes a position from -1 to 1 on the canvas
        public static void write_to_canvas(RayTracer.Tuple position, RayTracer.Canvas canvas, RayTracer.Color color)
        {
            var canvas_point = point(0, 0, 0);
            canvas_point = canvas_point + position; // make a copy of position
            canvas_point.X *= canvas.width / 2;
            canvas_point.Y *= canvas.height / 2;
            canvas_point = canvas_point + point(canvas.width / 2, canvas.height / 2, 0);

            int x_coord = (int)Math.Round(canvas_point.X);
            int y_coord = canvas.height - (int)Math.Round(canvas_point.Y);

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

            write_pixel(canvas, x_coord, y_coord, color);
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
            await TestHelper.write_to_file("chapter1.txt", s.ToString());

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
            Assert.IsTrue(areEqual(r, c * 2.0));
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
                write_to_canvasCh2(proj.position, c);
                i++;
            }

            await TestHelper.write_to_file("chapter2.ppm", canvas_to_ppm(c));

            void write_to_canvasCh2(RayTracer.Tuple position, RayTracer.Canvas canvas)
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
            Assert.AreEqual(25, minor(A, 1, 0));
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

        [TestMethod]
        public void Invertibility()
        {
            double[,] data1 = { {6, 4, 4, 4},
                                {5, 5, 7, 6},
                                {4, -9, 3, -7},
                                {9, 1, 7, -6},
                             };

            var A = new RayTracer.Matrix(data1);
            Assert.IsTrue(A.isInvertible());

            double[,] data2 = { {-4, 2, -2, -3},
                                {9, 6, 2, 6},
                                {0, -5, 1, -5},
                                {0, 0, 0, 0},
                              };

            A = new RayTracer.Matrix(data2);
            Assert.IsFalse(A.isInvertible());
        }

        [TestMethod]
        public void Inverse()
        {
            double[,] data1 = { {-5, 2, 6, -8},
                                {1, -5, 1, 8},
                                {7, 7, -6, -7},
                                {1, -3, 7, 4},
                              };

            var A = new RayTracer.Matrix(data1);
            var B = inverse(A);
            Assert.AreEqual(532, determinant(A));
            Assert.AreEqual(-160, cofactor(A, 2, 3));
            Assert.AreEqual(-160 / 532.0, B[3, 2]);
            Assert.AreEqual(105, cofactor(A, 3, 2));
            Assert.AreEqual(105 / 532.0, B[2, 3]);

            double[,] data2 = { {0.21805, 0.45113, 0.24060, -0.04511},
                                {-0.80827, -1.45677, -0.44361, 0.52068},
                                {-0.07895, -0.22368, -0.05263, 0.19737},
                                {-0.52256, -0.81391, -0.30075, 0.30639},
                              };

            var R = new RayTracer.Matrix(data2);
            Assert.IsTrue(areEqual(R, B));
        }

        [TestMethod]
        public void InverseMoreTests()
        {
            double[,] data1 = { {8, -5, 9, 2},
                                {7, 5, 6, 1},
                                {-6, 0, 9, 6},
                                {-3, 0, -9, -4},
                              };

            double[,] data2 = { {-0.15385, -0.15385, -0.28205, -0.53846},
                                {-0.07692, 0.12308, 0.02564, 0.03077},
                                {0.35897, 0.35897, 0.43590, 0.92308},
                                {-0.69231, -0.69231, -0.76923, -1.92308},
                              };

            var A = new RayTracer.Matrix(data1);
            var R = new RayTracer.Matrix(data2);
            Assert.IsTrue(areEqual(R, inverse(A)));

            double[,] data3 = { {9, 3, 0, 9},
                                {-5, -2, -6, -3},
                                {-4, 9, 6, 4},
                                {-7, 6, 6, 2},
                              };

            double[,] data4 = { {-0.04074, -0.07778, 0.14444, -0.22222},
                                {-0.07778, 0.03333, 0.36667, -0.33333},
                                {-0.02901, -0.14630, -0.10926, 0.12963},
                                {0.17778, 0.06667, -0.26667, 0.33333},
                              };

            A = new RayTracer.Matrix(data3);
            R = new RayTracer.Matrix(data4);
            Assert.IsTrue(areEqual(R, inverse(A)));
        }

        [TestMethod]
        public void MultiplyProductByInverse()
        {
            double[,] data1 = { {3, -9, 7, 3},
                                {3, -8, 2, -9},
                                {-4, 4, 4, 1},
                                {-6, 5, -1, 1},
                              };

            double[,] data2 = { {8, 2, 2, 2},
                                {4, -1, 7, 0},
                                {7, 0, 5, 4},
                                {6, -2, 0, 5},
                              };

            var A = new RayTracer.Matrix(data1);
            var B = new RayTracer.Matrix(data2);
            var C = A * B;
            Assert.IsTrue(areEqual(A, C * inverse(B)));
        }
    }

    [TestClass]
    public class Chapter4UnitTests
    {
        [TestMethod]
        public void Translation()
        {
            var transform = translation(5, -3, 2);
            var p = point(-3, 4, 5);
            Assert.IsTrue(areEqual(point(2, 1, 7), transform * p));
        }

        [TestMethod]
        public void InverseOfTranslation()
        {
            var transform = translation(5, -3, 2);
            var inv = inverse(transform);
            var p = point(-3, 4, 5);
            Assert.IsTrue(areEqual(point(-8, 7, 3), inv * p));
        }

        [TestMethod]
        public void TranslationDoesNotAffectVectors()
        {
            var transform = translation(5, -3, 2);
            var v = vector(-3, 4, 5);
            Assert.IsTrue(areEqual(v, transform * v));
        }

        [TestMethod]
        public void ScalingPoint()
        {
            var transform = scaling(2, 3, 4);
            var p = point(-4, 6, 8);
            Assert.IsTrue(areEqual(point(-8, 18, 32), transform * p));
        }

        [TestMethod]
        public void ScalingVector()
        {
            var transform = scaling(2, 3, 4);
            var v = vector(-4, 6, 8);
            Assert.IsTrue(areEqual(vector(-8, 18, 32), transform * v));
        }

        [TestMethod]
        public void InverseScaling()
        {
            var transform = scaling(2, 3, 4);
            var inv = inverse(transform);
            var v = vector(-4, 6, 8);
            Assert.IsTrue(areEqual(vector(-2, 2, 2), inv * v));
        }

        [TestMethod]
        public void Reflection()
        {
            var transform = scaling(-1, 1, 1);
            var p = point(2, 3, 4);
            Assert.IsTrue(areEqual(point(-2, 3, 4), transform * p));
        }

        [TestMethod]
        public void Rotation_X()
        {
            var p = point(0, 1, 0);
            var half_quarter = rotation_x(PI / 4);
            var full_quarter = rotation_x(PI / 2);
            Assert.IsTrue(areEqual(point(0, Sqrt(2) / 2, Sqrt(2) / 2), half_quarter * p));
            Assert.IsTrue(areEqual(point(0, 0, 1), full_quarter * p));
        }

        [TestMethod]
        public void Rotation_XInverse()
        {
            var p = point(0, 1, 0);
            var half_quarter = rotation_x(PI / 4);
            var inv = inverse(half_quarter);
            Assert.IsTrue(areEqual(point(0, Sqrt(2) / 2, -Sqrt(2) / 2), inv * p));
        }

        [TestMethod]
        public void Rotation_Y()
        {
            var p = point(0, 0, 1);
            var half_quarter = rotation_y(PI / 4);
            var full_quarter = rotation_y(PI / 2);
            Assert.IsTrue(areEqual(point(Sqrt(2) / 2, 0, Sqrt(2) / 2), half_quarter * p));
            Assert.IsTrue(areEqual(point(1, 0, 0), full_quarter * p));
        }

        [TestMethod]
        public void Rotation_Z()
        {
            var p = point(0, 1, 0);
            var half_quarter = rotation_z(PI / 4);
            var full_quarter = rotation_z(PI / 2);
            Assert.IsTrue(areEqual(point(-Sqrt(2) / 2, Sqrt(2) / 2, 0), half_quarter * p));
            Assert.IsTrue(areEqual(point(-1, 0, 0), full_quarter * p));
        }

        [TestMethod]
        public void Fluent()
        {
            var transform = identity().rotate_x(PI / 2).rotate_y(PI / 2).rotate_z(PI / 2).scale(5, 5, 5).translate(10, 5, 7).shear(1, 2, 3, 4, 5, 6);
            var r = identity();
            r = rotation_x(PI / 2) * r;
            r = rotation_y(PI / 2) * r;
            r = rotation_z(PI / 2) * r;
            r = scaling(5, 5, 5) * r;
            r = translation(10, 5, 7) * r;
            r = shearing(1, 2, 3, 4, 5, 6) * r;
            Assert.IsTrue(areEqual(r, transform));
        }

        [TestMethod]
        public void Shearing()
        {
            var transform = shearing(1, 0, 0, 0, 0, 0);
            var p = point(2, 3, 4);
            Assert.IsTrue(areEqual(point(5, 3, 4), transform * p));

            transform = shearing(0, 1, 0, 0, 0, 0);
            Assert.IsTrue(areEqual(point(6, 3, 4), transform * p));

            transform = shearing(0, 0, 1, 0, 0, 0);
            Assert.IsTrue(areEqual(point(2, 5, 4), transform * p));

            transform = shearing(0, 0, 0, 1, 0, 0);
            Assert.IsTrue(areEqual(point(2, 7, 4), transform * p));

            transform = shearing(0, 0, 0, 0, 1, 0);
            Assert.IsTrue(areEqual(point(2, 3, 6), transform * p));

            transform = shearing(0, 0, 0, 0, 0, 1);
            Assert.IsTrue(areEqual(point(2, 3, 7), transform * p));
        }

        [TestMethod]
        public void TransformationSequenceAndChained()
        {
            var p = point(1, 0, 1);
            var A = rotation_x(PI / 2);
            var B = scaling(5, 5, 5);
            var C = translation(10, 5, 7);

            var p2 = A * p;
            Assert.IsTrue(areEqual(point(1, -1, 0), p2));

            var p3 = B * p2;
            Assert.IsTrue(areEqual(point(5, -5, 0), p3));

            var p4 = C * p3;
            Assert.IsTrue(areEqual(point(15, 0, 7), p4));

            var T = C * B * A;
            Assert.IsTrue(areEqual(point(15, 0, 7), T * p));
        }

        [TestMethod]
        public async Task Chapter4Project()
        {
            RayTracer.Color blue = color(0, 0.5, 1);

            var c = new RayTracer.Canvas(100, 100);

            var p = point(0, 0.8, 0);
            var T = identity();

            for (int i = 0; i < 12; i++)
            {
                TestHelper.write_to_canvas(T * p, c, blue);
                T = T.rotate_z(-PI / 6);
            }

            await TestHelper.write_to_file("chapter4.ppm", canvas_to_ppm(c));
            /*
            void write_to_canvas(RayTracer.Tuple position, RayTracer.Canvas canvas, RayTracer.Color color)
            {
                var canvas_point = point(0, 0, 0);
                canvas_point = canvas_point + position; // make a copy of position
                canvas_point.X *= canvas.width / 2;
                canvas_point.Y *= canvas.height / 2;
                canvas_point = canvas_point + point(canvas.width / 2, canvas.height / 2, 0);

                int x_coord = (int)Math.Round(canvas_point.X);
                int y_coord = canvas.height - (int)Math.Round(canvas_point.Y);

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

                write_pixel(c, x_coord, y_coord, color);
            }*/
        }
    }

    [TestClass]
    public class Chapter5UnitTests
    {
        [TestMethod]
        public void Ray()
        {
            var origin = point(1, 2, 3);
            var direction = vector(4, 5, 6);
            var r = ray(origin, direction);
            Assert.IsTrue(areEqual(origin, r.origin));
            Assert.IsTrue(areEqual(direction, r.direction));
        }

        [TestMethod]
        public void Position()
        {
            var r = ray(point(2, 3, 4), vector(1, 0, 0));
            Assert.IsTrue(areEqual(point(2, 3, 4), position(r, 0)));
            Assert.IsTrue(areEqual(point(3, 3, 4), position(r, 1)));
        }

        [TestMethod]
        public void Sphere()
        {
            var s1 = sphere();
            var s2 = sphere();
            Assert.AreNotEqual(s1.GetHashCode(), s2.GetHashCode());
        }

        [TestMethod]
        public void RayIntersectsSphere()
        {
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var s = sphere();
            var xs = intersect(s, r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(4.0, xs[0].t);
            Assert.AreEqual(6.0, xs[1].t);
            Assert.AreEqual(s.GetHashCode(), xs[0].obj.GetHashCode());
        }

        [TestMethod]
        public void RayAtTangent()
        {
            var r = ray(point(0, 1, -5), vector(0, 0, 1));
            var s = sphere();
            var xs = intersect(s, r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(5.0, xs[0].t);
            Assert.AreEqual(5.0, xs[1].t);
        }

        [TestMethod]
        public void RayInsideSphere()
        {
            var r = ray(point(0, 0, 0), vector(0, 0, 1));
            var s = sphere();
            var xs = intersect(s, r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(-1.0, xs[0].t);
            Assert.AreEqual(1.0, xs[1].t);
        }

        [TestMethod]
        public void SphereBehindRay()
        {
            var r = ray(point(0, 0, 5), vector(0, 0, 1));
            var s = sphere();
            var xs = intersect(s, r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(-6.0, xs[0].t);
            Assert.AreEqual(-4.0, xs[1].t);
        }

        [TestMethod]
        public void RayMissesSphere()
        {
            var r = ray(point(0, 5, 5), vector(0, 0, 1));
            var s = sphere();
            var xs = intersect(s, r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public void IntersectionTest()
        {
            var s = sphere();
            var i = intersection(3.5, s);
            Assert.AreEqual(3.5, i.t);
            Assert.AreEqual(s.GetHashCode(), i.obj.GetHashCode());
        }

        [TestMethod]
        public void IntersectionsTest()
        {
            var s = sphere();
            var i1 = intersection(1, s);
            var i2 = intersection(2, s);
            var xs = intersections(new System.Collections.Generic.List<RayTracer.Intersection> { i1, i2 });
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(1, xs[0].t);
            Assert.AreEqual(2, xs[1].t);
        }

        [TestMethod]
        public void IntersectionsByValue()
        {
            var s = sphere();
            var i1 = intersection(1, s);
            var i2 = intersection(2, s);
            var xs = intersections(new System.Collections.Generic.List<RayTracer.Intersection> { i1, i2 });
            var i = hit(xs);
            Assert.AreEqual(i1, i);

            i1 = intersection(-1, s);
            i2 = intersection(1, s);
            xs = intersections(new System.Collections.Generic.List<RayTracer.Intersection> { i1, i2 });
            i = hit(xs);
            Assert.AreEqual(i2, i);

            i1 = intersection(-2, s);
            i2 = intersection(-1, s);
            xs = intersections(new System.Collections.Generic.List<RayTracer.Intersection> { i1, i2 });
            i = hit(xs);
            Assert.AreEqual(null, i);

            i1 = intersection(5, s);
            i2 = intersection(7, s);
            var i3 = intersection(-3, s);
            var i4 = intersection(2, s);
            xs = intersections(new System.Collections.Generic.List<RayTracer.Intersection> { i1, i2, i3, i4 });
            i = hit(xs);
            Assert.AreEqual(i4, i);
        }

        [TestMethod]
        public void TransformRay()
        {
            var r = ray(point(1, 2, 3), vector(0, 1, 0));
            var m = translation(3, 4, 5);
            var r2 = transform(r, m);
            Assert.IsTrue(areEqual(point(4, 6, 8), r2.origin));
            Assert.IsTrue(areEqual(vector(0, 1, 0), r2.direction));

            r = ray(point(1, 2, 3), vector(0, 1, 0));
            m = scaling(2, 3, 4);
            r2 = transform(r, m);
            Assert.IsTrue(areEqual(point(2, 6, 12), r2.origin));
            Assert.IsTrue(areEqual(vector(0, 3, 0), r2.direction));
        }

        [TestMethod]
        public void TransformSphere()
        {
            var s = sphere();
            Assert.IsTrue(areEqual(identity(), s.transform));

            var t = translation(2, 3, 4);
            set_transform(s, t);
            Assert.IsTrue(areEqual(t, s.transform));
        }

        [TestMethod]
        public void IntersectScaledSphere()
        {
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var s = sphere();
            set_transform(s, scaling(2, 2, 2));
            var xs = intersect(s, r);
            Assert.AreEqual(2, xs.Count);
            Assert.AreEqual(3, xs[0].t);
            Assert.AreEqual(7, xs[1].t);

            set_transform(s, translation(5, 0, 0));
            xs = intersect(s, r);
            Assert.AreEqual(0, xs.Count);
        }

        [TestMethod]
        public async Task Chapter5Project()
        {
            var red = color(1, 0, 0);
            var blue = color(0, 0, 1);
            var c = new RayTracer.Canvas(100, 100);

            var s = sphere();

            Parallel.For(0, c.height * c.width, index =>
            {
                int i = index * c.width / (c.height * c.width);
                int j = index - i * c.width;
                double scaled_x = -1 + 2 * i / (double)(c.width - 1);
                double scaled_y = -1 + 2 * j / (double)(c.height - 1);
                var r = ray(point(scaled_x, scaled_y, -5), vector(0, 0, 1));
                var xs = intersect(s, r);
                var h = hit(xs);
                if (h != null)
                {
                    write_pixel(c, i, c.height - j, blue);
                }
            });

            await TestHelper.write_to_file("chapter5.ppm", canvas_to_ppm(c));
        }
    }

    [TestClass]
    public class Chapter6UnitTests
    {
        [TestMethod]
        public void Normal()
        {
            var s = sphere();
            var n = normal_at(s, point(1, 0, 0));
            Assert.IsTrue(areEqual(vector(1, 0, 0), n));

            n = normal_at(s, point(0, 1, 0));
            Assert.IsTrue(areEqual(vector(0, 1, 0), n));

            n = normal_at(s, point(0, 0, 1));
            Assert.IsTrue(areEqual(vector(0, 0, 1), n));

            n = normal_at(s, point(0, 0, 1));
            Assert.IsTrue(areEqual(vector(0, 0, 1), n));

            n = normal_at(s, point(Sqrt(3) / 3, Sqrt(3) / 3, Sqrt(3) / 3));
            Assert.IsTrue(areEqual(vector(Sqrt(3) / 3, Sqrt(3) / 3, Sqrt(3) / 3), n));
        }

        [TestMethod]
        public void NormalizedNormal()
        {
            var s = sphere();
            var n = normal_at(s, point(Sqrt(3) / 3, Sqrt(3) / 3, Sqrt(3) / 3));
            Assert.IsTrue(areEqual(normalize(n), n));
        }

        [TestMethod]
        public void NormalTranslatedSphere()
        {
            var s = sphere();
            set_transform(s, translation(0, 1, 0));
            var n = normal_at(s, point(0, 1.70711, -0.70711));
            Assert.IsTrue(areEqual(vector(0,0.70711, -0.70711), n));

            var m = scaling(1, 0.5, 1) * rotation_z(PI / 5);
            set_transform(s, m);
            n = normal_at(s, point(0, Sqrt(2)/2, -Sqrt(2)/2));
            Assert.IsTrue(areEqual(vector(0, 0.97014, -0.24254), n));
        }

        [TestMethod]
        public void Reflect()
        {
            var v = vector(1, -1, 0);
            var n = vector(0, 1, 0);
            var r = reflect(v, n);
            Assert.IsTrue(areEqual(vector(1,1,0), r));
        }

        [TestMethod]
        public void PointLight()
        {
            var intensity = color(1, 1, 1);
            var position = point(0, 0, 0);
            var light = point_light(position, intensity);
            Assert.IsTrue(areEqual(position, light.position));
            Assert.IsTrue(areEqual(intensity, light.intensity));
        }

        [TestMethod]
        public void Material()
        {
            var m = material();
            Assert.IsTrue(areEqual(color(1, 1, 1), m.color));
            Assert.AreEqual(0.1, m.ambient);
            Assert.AreEqual(0.9, m.diffuse);
            Assert.AreEqual(0.9, m.specular);
            Assert.AreEqual(200, m.shininess);
        }

        [TestMethod]
        public void SphereMaterial()
        {
            var s = sphere();
            var m = material();
            Assert.IsTrue(areEqual(m, s.material));

            m.ambient = 1;
            s.material = m;
            Assert.IsTrue(areEqual(m, s.material));
        }

        [TestMethod]
        public void Lighting1()
        {
            var m = material();
            var position = point(0, 0, 0);
            var eyev = vector(0, 0, -1);
            var normalv = vector(0, 0, -1);
            var light = point_light(point(0, 0, -10), color(1, 1, 1));
            var result = lighting(m, light, position, eyev, normalv);
            Assert.IsTrue(areEqual(color(1.9, 1.9, 1.9), result));
        }

        [TestMethod]
        public void Lighting2()
        {
            var m = material();
            var position = point(0, 0, 0);
            var eyev = normalize(vector(0, 1, -1));
            var normalv = vector(0, 0, -1);
            var light = point_light(point(0, 0, -10), color(1, 1, 1));
            var result = lighting(m, light, position, eyev, normalv);
            Assert.IsTrue(areEqual(color(1, 1, 1), result));
        }

        [TestMethod]
        public void Lighting3()
        {
            var m = material();
            var position = point(0, 0, 0);
            var eyev = vector(0, 0, -1);
            var normalv = vector(0, 0, -1);
            var light = point_light(point(0, 10, -10), color(1, 1, 1));
            var result = lighting(m, light, position, eyev, normalv);
            Assert.IsTrue(areEqual(color(0.7364, 0.7364, 0.7364), result));
        }

        [TestMethod]
        public void Lighting4()
        {
            var m = material();
            var position = point(0, 0, 0);
            var eyev = normalize(vector(0, -1, -1));
            var normalv = vector(0, 0, -1);
            var light = point_light(point(0, 10, -10), color(1, 1, 1));
            var result = lighting(m, light, position, eyev, normalv);
            Assert.IsTrue(areEqual(color(1.6364, 1.6364, 1.6364), result));
        }

        [TestMethod]
        public void Lighting5()
        {
            var m = material();
            var position = point(0, 0, 0);
            var eyev = vector(0, 0, -1);
            var normalv = vector(0, 0, -1);
            var light = point_light(point(0, 0, 10), color(1, 1, 1));
            var result = lighting(m, light, position, eyev, normalv);
            Assert.IsTrue(areEqual(color(0.1, 0.1, 0.1), result));
        }

        [TestMethod]
        public async Task Chapter6Project()
        {
            var red = color(1, 0, 0);
            var c = new RayTracer.Canvas(100, 100);

            var s = sphere();
            var eye = point(0, 0, -5);
            var light = point_light(point(-0.7, 0.6, -3), color(1, 1, 1));
            s.material.color = color(0.8, 0.1, 0.6);

            double dx = 7.0 / c.width;
            double dy = 7.0 / c.height;
            Parallel.For(0, c.height * c.width, index =>
            {
                int i = index * c.width / (c.height * c.width);
                int j = index - i * c.width;
                var wall_point = point(-3.5 + i * dx, -3.5 + j * dy, 10);
                var r = ray(eye, wall_point - eye);
                var xs = intersect(s, r);
                var h = hit(xs);
                if (h != null)
                {
                    RayTracer.Intersection foo = (RayTracer.Intersection)h;
                    var intersect_position = position(r, foo.t);
                    var n = normal_at(s, intersect_position);
                    var light_result = lighting(s.material, light,
                        intersect_position, normalize(r.direction), n);
                    light_result.clamp();

                    write_pixel(c, i, c.height - j, light_result);
                }
            });
            await TestHelper.write_to_file("chapter6.ppm", canvas_to_ppm(c));
        }
    }

    [TestClass]
    public class Chapter7UnitTests
    {
        [TestMethod]
        public void World()
        {
            var w = world();
            Assert.IsNull(w.light.intensity);
            Assert.IsNull(w.light.position);
            Assert.IsNull(w.objects);
        }

        [TestMethod]
        public void DefaultWorld()
        {
            var light = point_light(point(-10, 10, -10), color(1, 1, 1));
            var s1 = sphere();
            s1.material.color = color(0.8, 1.0, 0.6);
            s1.material.diffuse = 0.7;
            s1.material.specular = 0.2;
            var s2 = sphere();
            s2.transform = scaling(0.5, 0.5, 0.5);

            var w = default_world();
            Assert.IsTrue(areEqual(light, w.light));
        }

        [TestMethod]
        public void IntersectWorld()
        {
            var w = default_world();
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var xs = intersect_world(w, r);
            Assert.AreEqual(4, xs.Count);
            Assert.AreEqual(4, xs[0].t);
            Assert.AreEqual(4.5, xs[1].t);
            Assert.AreEqual(5.5, xs[2].t);
            Assert.AreEqual(6, xs[3].t);
        }

        [TestMethod]
        public void PrecomputeIntersection()
        {
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var shape = sphere();
            var i = intersection(4, shape);
            var comps = prepare_computations(i, r);
            Assert.AreEqual(i.t, comps.t);
            Assert.AreEqual(i.obj, comps.obj);
            Assert.IsTrue(areEqual(point(0, 0, -1), comps.point));
            Assert.IsTrue(areEqual(vector(0, 0, -1), comps.eyev));
            Assert.IsTrue(areEqual(vector(0, 0, -1), comps.normalv));
        }

        [TestMethod]
        public void IntersectionInsideAndOutside()
        {
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var shape = sphere();
            var i = intersection(4, shape);
            var comps = prepare_computations(i, r);
            Assert.IsFalse(comps.inside);

            r = ray(point(0, 0, 0), vector(0, 0, 1));
            shape = sphere();
            i = intersection(1, shape);
            comps = prepare_computations(i, r);
            Assert.IsTrue(areEqual(point(0, 0, 1), comps.point));
            Assert.IsTrue(areEqual(vector(0, 0, -1), comps.eyev));
            Assert.IsTrue(comps.inside);
            Assert.IsTrue(areEqual(vector(0, 0, -1), comps.normalv));
        }

        [TestMethod]
        public void ShadeIntersection()
        {
            var w = default_world();
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var shape = w.objects[0];
            var i = intersection(4, shape);
            var comps = prepare_computations(i, r);
            var c = shade_hit(w, comps);
            Assert.IsTrue(areEqual(color(0.38066, 0.47583, 0.2855), c));

            w = default_world();
            w.light = point_light(point(0, 0.25, 0), color(1, 1, 1));
            r = ray(point(0, 0, 0), vector(0, 0, 1));
            shape = w.objects[1];
            i = intersection(0.5, shape);
            comps = prepare_computations(i, r);
            c = shade_hit(w, comps);
            Assert.IsTrue(areEqual(color(0.90498, 0.90498, 0.90498), c));
        }

        [TestMethod]
        public void ColorAtMiss()
        {
            var w = default_world();
            var r = ray(point(0, 0, -5), vector(0, 1, 0));
            var c = color_at(w, r);
            Assert.IsTrue(areEqual(color(0, 0, 0), c));
        }

        [TestMethod]
        public void ColorAtHit()
        {
            var w = default_world();
            var r = ray(point(0, 0, -5), vector(0, 0, 1));
            var c = color_at(w, r);
            Assert.IsTrue(areEqual(color(0.38066, 0.47583, 0.2855), c));
        }

        [TestMethod]
        public void ColorBehindRay()
        {
            var w = default_world();
            var outer = w.objects[0];
            outer.material.ambient = 1;
            var inner = w.objects[1];
            inner.material.ambient = 1;
            var r = ray(point(0, 0, 0.75), vector(0, 0, -1));
            var c = color_at(w, r);
            Assert.IsTrue(areEqual(inner.material.color, c));
        }

        [TestMethod]
        public void TransformDefault()
        {
            var from = point(0, 0, 0);
            var to = point(0, 0, -1);
            var up = vector(0, 1, 0);
            var t = view_transform(from, to, up);
            Assert.IsTrue(areEqual(identity(), t));
        }

        [TestMethod]
        public void TransformPositiveZ()
        {
            var from = point(0, 0, 0);
            var to = point(0, 0, 1);
            var up = vector(0, 1, 0);
            var t = view_transform(from, to, up);
            Assert.IsTrue(areEqual(scaling(-1, 1, -1), t));
        }

        [TestMethod]
        public void TransformMovesWorld()
        {
            var from = point(0, 0, 8);
            var to = point(0, 0, 0);
            var up = vector(0, 1, 0);
            var t = view_transform(from, to, up);
            Assert.IsTrue(areEqual(translation(0, 0, -8), t));
        }

        [TestMethod]
        public void ArbitraryTransform()
        {
            var from = point(1, 3, 2);
            var to = point(4, -2, 8);
            var up = vector(1, 1, 0);
            var t = view_transform(from, to, up);

            double[,] data = { {-0.50709, 0.50709, 0.67612, -2.36643},
                               {0.76772, 0.60609, 0.12122, -2.82843},
                               {-0.35857, 0.59761, -0.71714, 0.00000},
                               {0.00000, 0.00000, 0.00000, 1.00000}
                             };

            var m = new RayTracer.Matrix(data);
            Assert.IsTrue(areEqual(m, t));
        }

        [TestMethod]
        public void Camera()
        {
            int hsize = 160;
            int vsize = 120;
            double field_of_view = PI / 2;
            var c = camera(hsize, vsize, field_of_view);
            Assert.AreEqual(160, c.hsize);
            Assert.AreEqual(120, c.vsize);
            Assert.AreEqual(PI/2, c.field_of_view);
            Assert.IsTrue(areEqual(identity(), c.transform));
        }

        [TestMethod]
        public void PixelSize()
        {
            var c = camera(200, 125, PI/2);
            Assert.IsTrue(areClose(0.01, c.pixel_size));

            c = camera(125, 200, PI / 2);
            Assert.IsTrue(areClose(0.01, c.pixel_size));

            c = camera(2, 2, PI / 2);
            Assert.IsTrue(areClose(1, c.pixel_size));

            c = camera(20, 2, PI / 2);
            Assert.IsTrue(areClose(0.1, c.pixel_size));

            c = camera(2, 20, PI / 2);
            Assert.IsTrue(areClose(0.1, c.pixel_size));
        }

        [TestMethod]
        public void RayForPixel()
        {
            var c = camera(201, 101, PI / 2);
            var r = ray_for_pixel(c, 100, 50);
            Assert.IsTrue(areEqual(point(0, 0, 0), r.origin));
            Assert.IsTrue(areEqual(vector(0, 0, -1), r.direction));

            r = ray_for_pixel(c, 0, 0);
            Assert.IsTrue(areEqual(point(0, 0, 0), r.origin));
//            Assert.IsTrue(areEqual(vector(0.66519, 0.33259, -0.66851), r.direction));
        }
    }
}
