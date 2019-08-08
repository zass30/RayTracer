
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RayTracer;

namespace RayTracerTestProject
{
    [TestClass]
    public class TupleUnitTest
    {
        [TestMethod]
        public void TestTuplePoint()
        {
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 1.0);
            Assert.AreEqual(4.3, tuple.X);
            Assert.AreEqual(-4.2, tuple.Y);
            Assert.AreEqual(3.1, tuple.Z);
            Assert.AreEqual(1.0, tuple.W);
            Assert.IsTrue(tuple.isPoint());
            Assert.IsFalse(tuple.isVector());
        }

        [TestMethod]
        public void TestTupleVector()
        {
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            Assert.AreEqual(4.3, tuple.X);
            Assert.AreEqual(-4.2, tuple.Y);
            Assert.AreEqual(3.1, tuple.Z);
            Assert.AreEqual(0.0, tuple.W);
            Assert.IsFalse(tuple.isPoint());
            Assert.IsTrue(tuple.isVector());
        }

        [TestMethod]
        public void TestTuplePointFactory()
        {
            RayTracer.Tuple point = RayTracer.Tuple.point(4.3, -4.2, 3.1);
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 1.0);
            Assert.IsTrue(RayTracer.Tuple.areEqual(point, tuple));
        }

        [TestMethod]
        public void TestTupleVectorFactory()
        {
            RayTracer.Tuple vector = RayTracer.Tuple.vector(4.3, -4.2, 3.1);
            RayTracer.Tuple tuple = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            Assert.IsTrue(RayTracer.Tuple.areEqual(vector, tuple));
        }

        [TestMethod]
        public void TestFloatEpsilon()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(4.3, -4.2, 3.1, 0.0);
            RayTracer.Tuple b = new RayTracer.Tuple(4.30000001, -4.19999999, 3.1, 0.0);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a, b));
        }

        [TestMethod]
        public void TestAddition()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(3, -2, 5, 1);
            RayTracer.Tuple b = new RayTracer.Tuple(-2, 3, 1, 0);
            RayTracer.Tuple c = new RayTracer.Tuple(1, 1, 6, 1);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a+b, c));
        }

        [TestMethod]
        public void AddingTwoPointsDoesntGivePoint()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(3, -2, 5, 1);
            RayTracer.Tuple b = new RayTracer.Tuple(-2, 3, 1, 1);
            RayTracer.Tuple c = a + b;
            Assert.IsFalse(c.isPoint());
            Assert.IsFalse(c.isVector());
        }

        [TestMethod]
        public void SubtractPoints()
        {
            RayTracer.Tuple a = RayTracer.Tuple.point(3, 2, 1);
            RayTracer.Tuple b = RayTracer.Tuple.point(5, 6, 7);
            RayTracer.Tuple c = RayTracer.Tuple.vector(-2, -4, -6);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a - b, c));
        }

        [TestMethod]
        public void SubtractVectorAndPoint()
        {
            RayTracer.Tuple a = RayTracer.Tuple.point(3, 2, 1);
            RayTracer.Tuple b = RayTracer.Tuple.vector(5, 6, 7);
            RayTracer.Tuple c = RayTracer.Tuple.point(-2, -4, -6);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a - b, c));
        }

        [TestMethod]
        public void SubtractVectors()
        {
            RayTracer.Tuple a = RayTracer.Tuple.vector(3, 2, 1);
            RayTracer.Tuple b = RayTracer.Tuple.vector(5, 6, 7);
            RayTracer.Tuple c = RayTracer.Tuple.vector(-2, -4, -6);
            Assert.IsTrue(RayTracer.Tuple.areEqual(a - b, c));
        }

        [TestMethod]
        public void SubtractZeroVector()
        {
            RayTracer.Tuple zero = RayTracer.Tuple.vector(0, 0, 0);
            RayTracer.Tuple v = RayTracer.Tuple.vector(1, -2, 3);
            RayTracer.Tuple c = RayTracer.Tuple.vector(-1, 2, -3);
            Assert.IsTrue(RayTracer.Tuple.areEqual(zero - v, c));
        }

        [TestMethod]
        public void NegateTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple b = new RayTracer.Tuple(-1, 2, -3, 4);
            Assert.IsTrue(RayTracer.Tuple.areEqual(!a, b));
        }

        [TestMethod]
        public void ScalarMultiplicationTuple()
        {
            RayTracer.Tuple a = new RayTracer.Tuple(1, -2, 3, -4);
            RayTracer.Tuple b = new RayTracer.Tuple(3.5, -7, 10.5, -14);
            Assert.IsTrue(RayTracer.Tuple.areEqual(3.5 * a, b));
        }
    }
}
