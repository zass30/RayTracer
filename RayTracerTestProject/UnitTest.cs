
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Math;
using static RayTracer.Tuple;

namespace RayTracerTestProject
{
    [TestClass]
    public class TupleUnitTest
    {
        [TestMethod]
        public void TuplePoint()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(4.3, -4.2, 3.1, 1.0);
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
            RayTracer.Tuple a = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
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
            RayTracer.Tuple point = RayTracer.Tuple.point(4.3, -4.2, 3.1);
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 1.0);
            Assert.IsTrue(areEqual(point, tuple));
        }

        [TestMethod]
        public void TupleVectorFactory()
        {
            RayTracer.Tuple vector = RayTracer.Tuple.vector(4.3, -4.2, 3.1);
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            Assert.IsTrue(areEqual(vector, tuple));
        }

        [TestMethod]
        public void FloatEpsilon()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            RayTracer.Tuple r = new RayTracer.Tuple(4.30000001, -4.19999999, 3.1, 0.0);
            Assert.IsTrue(areEqual(r, a));
        }

        [TestMethod]
        public void Addition()
        {
            RayTracer.Tuple a1 = new RayTracer.Tuple(3, -2, 5, 1);
            RayTracer.Tuple a2 = new RayTracer.Tuple(-2, 3, 1, 0);
            RayTracer.Tuple r = new RayTracer.Tuple(1, 1, 6, 1);
            Assert.IsTrue(areEqual(r, a1 + a2));
        }

        [TestMethod]
        public void AddingTwoPointsDoesntGivePoint()
        {
            RayTracer.Tuple p1 = point(3, -2, 5);
            RayTracer.Tuple p2 = point(-2, 3, 1);
            RayTracer.Tuple r = p1 + p2;
            Assert.IsFalse(r.isPoint());
            Assert.IsFalse(r.isVector());
        }

        [TestMethod]
        public void SubtractPoints()
        {
            RayTracer.Tuple p1 = point(3, 2, 1);
            RayTracer.Tuple p2 = point(5, 6, 7);
            RayTracer.Tuple r = vector(-2, -4, -6);
            Assert.IsTrue(areEqual(r, p1 - p2));
        }

        [TestMethod]
        public void SubtractVectorAndPoint()
        {
            RayTracer.Tuple p = point(3, 2, 1);
            RayTracer.Tuple v = vector(5, 6, 7);
            RayTracer.Tuple r = point(-2, -4, -6);
            Assert.IsTrue(areEqual(r, p - v));
        }

        [TestMethod]
        public void SubtractVectors()
        {
            RayTracer.Tuple v1 = vector(3, 2, 1);
            RayTracer.Tuple v2 = vector(5, 6, 7);
            RayTracer.Tuple r = vector(-2, -4, -6);
            Assert.IsTrue(areEqual(r, v1 - v2));
        }

        [TestMethod]
        public void SubtractZeroVector()
        {
            RayTracer.Tuple zero = vector(0, 0, 0);
            RayTracer.Tuple v = vector(1, -2, 3);
            RayTracer.Tuple r = vector(-1, 2, -3);
            Assert.IsTrue(areEqual(r, zero - v));
        }

        [TestMethod]
        public void NegateTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(-1, 2, -3, 4);
            Assert.IsTrue(areEqual(r, !a));
        }

        [TestMethod]
        public void ScalarMultiplicationTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(3.5, -7, 10.5, -14);
            Assert.IsTrue(areEqual(r, 3.5 * a));
        }

        [TestMethod]
        public void ScalarFractionMultiplicationTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(0.5, -1, 1.5, -2);
            Assert.IsTrue(areEqual(r, 0.5 * a));
        }

        [TestMethod]
        public void ScalarDivisionTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(0.5, -1, 1.5, -2);
            Assert.IsTrue(areEqual(r, a/2));
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
            r = vector(1/Sqrt(14), 2/Sqrt(14), 3/ Sqrt(14));
            Assert.IsTrue(areEqual(r, n));
        }

        [TestMethod]
        public void VectorDot()
        {
            RayTracer.Tuple a = vector(1, 2, 3);
            RayTracer.Tuple b = vector(2, 3, 4);
            Assert.AreEqual(20, dot(a,b));
        }

        [TestMethod]
        public void VectorCross()
        {
            RayTracer.Tuple a = vector(1, 2, 3);
            RayTracer.Tuple b = vector(2, 3, 4);
            RayTracer.Tuple r1 = vector(-1, 2, -1);
            RayTracer.Tuple r2 = vector(1, -2, 1);

            Assert.IsTrue(areEqual(r1, cross(a, b)));
            Assert.IsTrue(areEqual(r2, cross(b, a)));
        }
    }
}
