
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer;

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
            Assert.IsTrue(RayTracer.Tuple.areEqual(point, tuple));
        }

        [TestMethod]
        public void TupleVectorFactory()
        {
            RayTracer.Tuple vector = RayTracer.Tuple.vector(4.3, -4.2, 3.1);
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            Assert.IsTrue(RayTracer.Tuple.areEqual(vector, tuple));
        }

        [TestMethod]
        public void FloatEpsilon()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            RayTracer.Tuple b = new RayTracer.Tuple(4.30000001, -4.19999999, 3.1, 0.0);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a, b));
        }

        [TestMethod]
        public void Addition()
        {
            RayTracer.Tuple a1 = new RayTracer.Tuple(3, -2, 5, 1);
            RayTracer.Tuple a2 = new RayTracer.Tuple(-2, 3, 1, 0);
            RayTracer.Tuple r = new RayTracer.Tuple(1, 1, 6, 1);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a1+a2, r));
        }

        [TestMethod]
        public void AddingTwoPointsDoesntGivePoint()
        {
            RayTracer.Tuple p1 = RayTracer.Tuple.point(3, -2, 5);
            RayTracer.Tuple p2 = RayTracer.Tuple.point(-2, 3, 1);
            RayTracer.Tuple r = p1 + p2;
            Assert.IsFalse(r.isPoint());
            Assert.IsFalse(r.isVector());
        }

        [TestMethod]
        public void SubtractPoints()
        {
            RayTracer.Tuple p1 = RayTracer.Tuple.point(3, 2, 1);
            RayTracer.Tuple p2 = RayTracer.Tuple.point(5, 6, 7);
            RayTracer.Tuple r = RayTracer.Tuple.vector(-2, -4, -6);
            Assert.IsTrue(RayTracer.Tuple.areEqual(p1 - p2, r));
        }

        [TestMethod]
        public void SubtractVectorAndPoint()
        {
            RayTracer.Tuple p = RayTracer.Tuple.point(3, 2, 1);
            RayTracer.Tuple v = RayTracer.Tuple.vector(5, 6, 7);
            RayTracer.Tuple r = RayTracer.Tuple.point(-2, -4, -6);
            Assert.IsTrue(RayTracer.Tuple.areEqual(p - v, r));
        }

        [TestMethod]
        public void SubtractVectors()
        {
            RayTracer.Tuple v1 = RayTracer.Tuple.vector(3, 2, 1);
            RayTracer.Tuple v2 = RayTracer.Tuple.vector(5, 6, 7);
            RayTracer.Tuple r = RayTracer.Tuple.vector(-2, -4, -6);
            Assert.IsTrue(RayTracer.Tuple.areEqual(v1 - v2, r));
        }

        [TestMethod]
        public void SubtractZeroVector()
        {
            RayTracer.Tuple zero = RayTracer.Tuple.vector(0, 0, 0);
            RayTracer.Tuple v = RayTracer.Tuple.vector(1, -2, 3);
            RayTracer.Tuple r = RayTracer.Tuple.vector(-1, 2, -3);
            Assert.IsTrue(RayTracer.Tuple.areEqual(zero - v, r));
        }

        [TestMethod]
        public void NegateTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(-1, 2, -3, 4);
            Assert.IsTrue(RayTracer.Tuple.areEqual(!a, r));
        }

        [TestMethod]
        public void ScalarMultiplicationTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(3.5, -7, 10.5, -14);
            Assert.IsTrue(RayTracer.Tuple.areEqual(3.5 * a, r));
        }

        [TestMethod]
        public void ScalarFractionMultiplicationTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(0.5, -1, 1.5, -2);
            Assert.IsTrue(RayTracer.Tuple.areEqual(0.5 * a, r));
        }

        [TestMethod]
        public void ScalarDivisionTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple r = new RayTracer.Tuple(0.5, -1, 1.5, -2);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a/2, r));
        }

        [TestMethod]
        public void VectorMagnitude()
        {
            RayTracer.Tuple v;
            v = RayTracer.Tuple.vector(1, 0, 0);
            Assert.AreEqual(v.magnitude(), 1);
            v = RayTracer.Tuple.vector(0, 1, 0);
            Assert.AreEqual(v.magnitude(), 1);
            v = RayTracer.Tuple.vector(0, 0, 1);
            Assert.AreEqual(v.magnitude(), 1);
            v = RayTracer.Tuple.vector(1, 2, 3);
            Assert.AreEqual(v.magnitude(), Math.Sqrt(14));
            v = RayTracer.Tuple.vector(-1, -2, -3);
            Assert.AreEqual(v.magnitude(), Math.Sqrt(14));
        }
    }
}
