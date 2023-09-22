using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Windows.Media.Media3D;

namespace BeautySim2023
{
    public class PointHit
    {
        public Point3D Point { get; set; }
        public string Name { get; set; }

        public double Distance { get; set; }

        //public PointHit(Point3D point, string name, double distance)
        //{
        //    Point = point;
        //    Name = name;
        //    Distance = distance;
        //}


        public PointHit(Vector3 pointHit, string name, double distance)
        {
            Point = new Point3D(pointHit.X, pointHit.Y, pointHit.Z);
            Name = name;
            Distance = distance;
        }

        internal PointHit DeepCopy()
        {
            return new PointHit(Point.ToVector3(), Name, Distance);
        }

    }
}