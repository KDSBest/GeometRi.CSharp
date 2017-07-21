﻿using System;
using static System.Math;

namespace GeometRi
{
    public class Plane3d
    {

        private Point3d _point;
        private Vector3d _normal;
        private Coord3d _coord;

        #region "Constructors"
        /// <summary>
        /// Create default XY plane
        /// </summary>
        public Plane3d()
        {
            _point = new Point3d(0, 0, 0);
            _normal = new Vector3d(0, 0, 1);
        }

        /// <summary>
        /// Create plane using general equation in 3D space: A*x+B*y+C*z+D=0.
        /// </summary>
        /// <param name="a">Parameter "A" in general plane equation.</param>
        /// <param name="b">Parameter "B" in general plane equation.</param>
        /// <param name="c">Parameter "C" in general plane equation.</param>
        /// <param name="d">Parameter "D" in general plane equation.</param>
        /// <param name="coord">Coordinate system in which plane equation is defined (default: Coord3d.GlobalCS).</param>
        public Plane3d(double a, double b, double c, double d, Coord3d coord = null)
        {
            if (coord == null)
            {
                coord = Coord3d.GlobalCS;
            }
            if (Abs(a) > Abs(b) && Abs(a) > Abs(c))
            {
                _point = new Point3d(-d / a, 0, 0, coord);
            }
            else if (Abs(b) > Abs(a) && Abs(b) > Abs(c))
            {
                _point = new Point3d(0, -d / b, 0, coord);
            }
            else
            {
                _point = new Point3d(0, 0, -d / c, coord);
            }
            _normal = new Vector3d(a, b, c, coord);
        }

        /// <summary>
        /// Create plane by three points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <param name="p3">Third point.</param>
        public Plane3d(Point3d p1, Point3d p2, Point3d p3)
        {
            Vector3d v1 = new Vector3d(p1, p2);
            Vector3d v2 = new Vector3d(p1, p3);
            _normal = v1.Cross(v2);
            _point = p1.Copy();
        }

        /// <summary>
        /// Create plane by point and two vectors lying in the plane.
        /// </summary>
        public Plane3d(Point3d p1, Vector3d v1, Vector3d v2)
        {
            _normal = v1.Cross(v2);
            _point = p1.Copy();
        }

        /// <summary>
        /// Create plane by point and normal vector.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="v1"></param>
        public Plane3d(Point3d p1, Vector3d v1)
        {
            _normal = v1.Copy();
            _point = p1.Copy();
        }
        #endregion

        /// <summary>
        /// Creates copy of the object
        /// </summary>
        public Plane3d Copy()
        {
            return new Plane3d(_point,_normal);
        }

        /// <summary>
        /// Point on the plane
        /// </summary>
        /// <returns></returns>
        public Point3d Point
        {
            get { return _point.Copy(); }
            set { _point = value.Copy(); }
        }

        /// <summary>
        /// Normal vector of the plane
        /// </summary>
        /// <returns></returns>
        public Vector3d Normal
        {
            get { return _normal.Copy(); }
            set { _normal = value.Copy(); }
        }

        /// <summary>
        /// Set reference coordinate system for general plane equation
        /// </summary>
        public void SetCoord(Coord3d coord)
        {
            _coord = coord;
        }

        /// <summary>
        /// Coefficient A in the general plane equation
        /// </summary>
        public double A
        {
            get { return _normal.ConvertTo(_coord).X; }
        }

        /// <summary>
        /// Coefficient B in the general plane equation
        /// </summary>
        public double B
        {
            get { return _normal.ConvertTo(_coord).Y; }
        }

        /// <summary>
        /// Coefficient C in the general plane equation
        /// </summary>
        public double C
        {
            get { return _normal.ConvertTo(_coord).Z; }
        }

        /// <summary>
        /// Coefficient D in the general plane equation
        /// </summary>
        public double D
        {
            get
            {
                Point3d p = _point.ConvertTo(_coord);
                Vector3d v = _normal.ConvertTo(_coord);
                return -v.X * p.X - v.Y * p.Y - v.Z * p.Z;
            }
        }



        /// <summary>
        /// Get intersection of line with plane.
        /// Returns object of type 'Nothing', 'Point3d' or 'Line3d'.
        /// </summary>
        public object IntersectionWith(Line3d l)
        {
            Vector3d r1 = new Vector3d(l.Point);
            Vector3d s1 = l.Direction;
            Vector3d n2 = this.Normal;
            if (Abs(s1 * n2) < GeometRi3D.Tolerance)
            {
                // Line and plane are parallel
                if (l.Point.BelongsTo(this))
                {
                    // Line lies in the plane
                    return l;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                r1 = r1 - ((r1 * n2) + this.D) / (s1 * n2) * s1;
                return r1.ToPoint;
            }
        }

        /// <summary>
        /// Finds the common intersection of three planes.
        /// Return object of type 'Nothing', 'Point3d', 'Line3d' or 'Plane3d'
        /// </summary>
        public object IntersectionWith(Plane3d s2, Plane3d s3)
        {
            // Set all planes to global CS
            this.SetCoord(Coord3d.GlobalCS);
            s2.SetCoord(Coord3d.GlobalCS);
            s3.SetCoord(Coord3d.GlobalCS);
            double det = new Matrix3d(new[] { A, B, C }, new[] { s2.A, s2.B, s2.C }, new [] { s3.A, s3.B, s3.C }).Det;
            if (Abs(det) < GeometRi3D.Tolerance)
            {
                if (this.Normal.IsParallelTo(s2.Normal) && this.Normal.IsParallelTo(s3.Normal))
                {
                    // Planes are coplanar
                    if (this.Point.BelongsTo(s2) && this.Point.BelongsTo(s3))
                    {
                        return this;
                    }
                    else
                    {
                        return null;
                    }
                }
                if (this.Normal.IsNotParallelTo(s2.Normal) && this.Normal.IsNotParallelTo(s3.Normal))
                {
                    // Planes are not parallel
                    // Find the intersection (Me,s2) and (Me,s3) and check if it is the same line
                    Line3d l1 = (Line3d)this.IntersectionWith(s2);
                    Line3d l2 = (Line3d)this.IntersectionWith(s3);
                    if (l1 == l2)
                    {
                        return l1;
                    }
                    else
                    {
                        return null;
                    }
                }

                // Two planes are parallel, third plane is not
                return null;

            }
            else
            {
                double x = -new Matrix3d(new[] { D, B, C }, new[] { s2.D, s2.B, s2.C }, new[] { s3.D, s3.B, s3.C }).Det / det;
                double y = -new Matrix3d(new[] { A, D, C }, new[] { s2.A, s2.D, s2.C }, new[] { s3.A, s3.D, s3.C }).Det / det;
                double z = -new Matrix3d(new[] { A, B, D }, new[] { s2.A, s2.B, s2.D }, new[] { s3.A, s3.B, s3.D }).Det / det;
                return new Point3d(x, y, z);
            }
        }

        /// <summary>
        /// Get intersection of two planes.
        /// Returns object of type 'Nothing', 'Line3d' or 'Plane3d'.
        /// </summary>
        public object IntersectionWith(Plane3d s2)
        {
            Vector3d v = this.Normal.Cross(s2.Normal).ConvertToGlobal();
            if (v.Norm < GeometRi3D.Tolerance)
            {
                // Planes are coplanar
                if (this.Point.BelongsTo(s2))
                {
                    return this;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                // Find the common point for two planes by intersecting with third plane
                // (using the 'most orthogonal' plane)
                // This part needs to be rewritten
                if (Abs(v.X) > Abs(v.Y) && Abs(v.X) > Abs(v.Z))
                {
                    Point3d p = (Point3d)this.IntersectionWith(s2, Coord3d.GlobalCS.YZ_plane);
                    return new Line3d(p, v);
                }
                else if (Abs(v.Y) > Abs(v.X) && Abs(v.Y) > Abs(v.Z))
                {
                    Point3d p = (Point3d)this.IntersectionWith(s2, Coord3d.GlobalCS.XZ_plane);
                    return new Line3d(p, v);
                }
                else
                {
                    Point3d p = (Point3d)this.IntersectionWith(s2, Coord3d.GlobalCS.XY_plane);
                    return new Line3d(p, v);
                }
            }
        }

        /// <summary>
        /// Get intersection of plane with sphere.
        /// Returns object of type 'Nothing', 'Point3d' or 'Circle3d'.
        /// </summary>
        public object IntersectionWith(Sphere s)
        {
            return s.IntersectionWith(this);
        }

        /// <summary>
        /// Intersection of circle with plane.
        /// Returns object of type 'Nothing', 'Circle3d', 'Point3d' or 'Segment3d'.
        /// </summary>
        public object IntersectionWith(Circle3d c)
        {
            return c.IntersectionWith(this);
        }


        #region "AngleTo"
        /// <summary>
        /// Angle between vector and plane in radians (0 &lt; angle &lt; Pi/2)
        /// </summary>
        public double AngleTo(Vector3d v)
        {
            return Abs(PI / 2 - this.Normal.AngleTo(v));
        }
        /// <summary>
        /// Angle between vector and plane in degrees (0 &lt; angle &lt; 90)
        /// </summary>
        public double AngleToDeg(Vector3d v)
        {
            return Abs(90.0 - this.Normal.AngleToDeg(v));
        }

        /// <summary>
        /// Angle between line and plane in radians (0 &lt; angle &lt; Pi/2)
        /// </summary>
        public double AngleTo(Line3d l)
        {
            return Abs(PI / 2 - this.Normal.AngleTo(l.Direction));
        }
        /// <summary>
        /// Angle between line and plane in degrees (0 &lt; angle &lt; 90)
        /// </summary>
        public double AngleToDeg(Line3d l)
        {
            return Abs(90.0 - this.Normal.AngleToDeg(l.Direction));
        }

        /// <summary>
        /// Angle between two planes in radians (0 &lt; angle &lt; Pi/2)
        /// </summary>
        public double AngleTo(Plane3d s)
        {
            double ang = this.Normal.AngleTo(s.Normal);
            if (ang <= PI / 2)
            {
                return ang;
            }
            else
            {
                return PI - ang;
            }
        }
        /// <summary>
        /// Angle between two planes in degrees (0 &lt; angle &lt; 90)
        /// </summary>
        public double AngleToDeg(Plane3d s)
        {
            return AngleTo(s) * 180 / PI;
        }
        #endregion


        #region "TranslateRotateReflect"
        /// <summary>
        /// Translate plane by a vector
        /// </summary>
        public Plane3d Translate(Vector3d v)
        {
            return new Plane3d(this.Point.Translate(v), this.Normal);
        }

        /// <summary>
        /// Rotate plane by a given rotation matrix
        /// </summary>
        public Plane3d Rotate(Matrix3d m)
        {
            return new Plane3d(this.Point.Rotate(m), this.Normal.Rotate(m));
        }

        /// <summary>
        /// Rotate plane by a given rotation matrix around point 'p' as a rotation center
        /// </summary>
        public Plane3d Rotate(Matrix3d m, Point3d p)
        {
            return new Plane3d(this.Point.Rotate(m, p), this.Normal.Rotate(m));
        }

        /// <summary>
        /// Reflect plane in given point
        /// </summary>
        public Plane3d ReflectIn(Point3d p)
        {
            return new Plane3d(this.Point.ReflectIn(p), this.Normal.ReflectIn(p));
        }

        /// <summary>
        /// Reflect plane in given line
        /// </summary>
        public Plane3d ReflectIn(Line3d l)
        {
            return new Plane3d(this.Point.ReflectIn(l), this.Normal.ReflectIn(l));
        }

        /// <summary>
        /// Reflect plane in given plane
        /// </summary>
        public Plane3d ReflectIn(Plane3d s)
        {
            return new Plane3d(this.Point.ReflectIn(s), this.Normal.ReflectIn(s));
        }
        #endregion

        /// <summary>
        /// Determines whether two objects are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (!object.ReferenceEquals(this.GetType(), obj.GetType())))
            {
                return false;
            }
            Plane3d s = (Plane3d)obj;

            return s.Point.BelongsTo(this) && s.Normal.IsParallelTo(this.Normal);
        }

        /// <summary>
        /// Returns the hascode for the object.
        /// </summary>
        public override int GetHashCode()
        {
            return GeometRi3D.HashFunction(_point.GetHashCode(), _normal.GetHashCode());
        }

        /// <summary>
        /// String representation of an object in global coordinate system.
        /// </summary>
        public override String ToString()
        {
            return ToString(Coord3d.GlobalCS);
        }

        /// <summary>
        /// String representation of an object in reference coordinate system.
        /// </summary>
        public String ToString(Coord3d coord)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            string nl = System.Environment.NewLine;

            if (coord == null) { coord = Coord3d.GlobalCS; }
            Point3d P = _point.ConvertTo(coord);
            Vector3d normal = _normal.ConvertTo(coord);

            str.Append("Plane3d:" + nl);
            str.Append(string.Format("Point  -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", P.X, P.Y, P.Z) + nl);
            str.Append(string.Format("Normal -> ({0,10:g5}, {1,10:g5}, {2,10:g5})", normal.X, normal.Y, normal.Z));
            return str.ToString();
        }

        // Operators overloads
        //-----------------------------------------------------------------

        public static bool operator ==(Plane3d s1, Plane3d s2)
        {
            return s1.Equals(s2);
        }
        public static bool operator !=(Plane3d s1, Plane3d s2)
        {
            return !s1.Equals(s2);
        }

    }
}


