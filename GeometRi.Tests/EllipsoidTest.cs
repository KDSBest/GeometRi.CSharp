﻿using System;
using static System.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeometRi;

namespace GeometRi_Tests
{
    [TestClass]
    public class EllipsoidTest
    {
        [TestMethod()]
        public void EllipsoidIntersectionWithLineTest()
        {
            Point3d p = new Point3d(0, 0, 0);
            Vector3d v1 = new Vector3d(4, 0, 0);
            Vector3d v2 = new Vector3d(0, 6, 0);
            Vector3d v3 = new Vector3d(0, 0, 9);
            Ellipsoid e = new Ellipsoid(p, v1, v2, v3);

            p = new Point3d(0, 2, 1);
            v1 = new Vector3d(-1, 1, 3);
            Line3d l = new Line3d(p, v1);

            Segment3d s = (Segment3d)e.IntersectionWith(l);

            Assert.IsTrue(s.P1.BelongsTo(e));
            Assert.IsTrue(s.P2.BelongsTo(e));
        }

        [TestMethod()]
        public void EllipsoidIntersectionWithPlaneTest()
        {
            Point3d p = new Point3d(0, 0, 0);
            Vector3d v1 = new Vector3d(5, 0, 0);
            Vector3d v2 = new Vector3d(0, 4, 0);
            Vector3d v3 = new Vector3d(0, 0, 3);
            Ellipsoid e = new Ellipsoid(p, v1, v2, v3);

            Plane3d s = new Plane3d(1, 2, 3, 4);

            Ellipse res = (Ellipse)e.IntersectionWith(s);

            Assert.IsTrue(res.Center.IsInside(e));
            Assert.IsTrue(res.Center.Translate(res.MajorSemiaxis).BelongsTo(e));
            Assert.IsTrue(res.Center.Translate(res.MinorSemiaxis).BelongsTo(e));
            Assert.IsTrue(res.Center.Translate(-res.MajorSemiaxis).BelongsTo(e));
            Assert.IsTrue(res.Center.Translate(-res.MinorSemiaxis).BelongsTo(e));
            Assert.IsTrue(res.ParametricForm(0.01).BelongsTo(e));
            Assert.IsTrue(res.ParametricForm(0.11).BelongsTo(e));
            Assert.IsTrue(res.ParametricForm(0.55).BelongsTo(e));
            Assert.IsTrue(res.ParametricForm(0.876).BelongsTo(e));
        }

        [TestMethod()]
        public void EllipsoidProjectionToLineTest_1()
        {
            Point3d p = new Point3d(0, 0, 0);
            Vector3d v1 = new Vector3d(4, 0, 0);
            Vector3d v2 = new Vector3d(0, 6, 0);
            Vector3d v3 = new Vector3d(0, 0, 9);
            Ellipsoid e = new Ellipsoid(p, v1, v2, v3);

            p = new Point3d(1, 1, 1);
            v1 = new Vector3d(0, 1, 0);
            Line3d l = new Line3d(p, v1);
            Segment3d s = e.ProjectionTo(l);

            Segment3d res = new Segment3d(new Point3d(1, 6, 1), new Point3d(1, -6, 1));

            Assert.AreEqual(s,res);
        }

        [TestMethod()]
        public void EllipsoidProjectionToLineTest_2()
        {
            Point3d p = new Point3d(0, 0, 0);
            Vector3d v1 = new Vector3d(4, 0, 0);
            Vector3d v2 = new Vector3d(0, 6, 0);
            Vector3d v3 = new Vector3d(0, 0, 9);
            Ellipsoid e = new Ellipsoid(p, v1, v2, v3);

            p = new Point3d(1, 1, 1);
            v1 = new Vector3d(1, 1, 3);
            Line3d l = new Line3d(p, v1);
            Segment3d s = e.ProjectionTo(l);

            // Construct plane orthogonal to line and passing through segment end point
            // And check if it is touching ellipsoid
            Plane3d pl1 = new Plane3d(s.P1, v1);
            object obj = e.IntersectionWith(pl1);

            if (obj.GetType() == typeof(Point3d))
            {
                Point3d pres = (Point3d)obj;
                if (pres.BelongsTo(e)) {
                    Assert.IsTrue(true);
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }

        }
    }
}