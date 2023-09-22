using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace BeautySim2023
{
    internal static class ClassExtension
    {


        public static float DistanceTo(this Vector3 a, Vector3 b)
        {
            return (a - b).Length();
        }

        public static Point3D ToPoint3D(this Vector3 p)
        {
            return new Point3D(p.X, p.Y, p.Z);
        }


        public static Vector3D ToVector3D(this Point3D p)
        {
            return new Vector3D(p.X, p.Y, p.Z);
        }

        public static Vector3 CloneVector3(this Vector3 p)
        {
            return new Vector3(p.X, p.Y, p.Z);
        }
    }
}