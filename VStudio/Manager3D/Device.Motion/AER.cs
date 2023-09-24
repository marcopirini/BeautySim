/******************************************
 * Class name:
 * Author:
 * Creation:
 * Last modify:
 * Version:
 * 
 * DESCRIPTION
 * 
 * 
 * *****************************************/

using System;
using VectorMath;

namespace Device.Motion
{
    public class AER
    {


        public double Elevation, Azimuth, Rotation;

        public AER(double azimuth, double elevation, double rotation)
        {
            Elevation = elevation;
            Azimuth = azimuth;
            Rotation = rotation;

        }
        public AER()
        { 
        }
        public AER(Quaternion q)
        {
            //http://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            //Conversion between quaternions and Euler angles
            double q0 = q.W;
            double q1 = q.X;
            double q2 = q.Y;
            double q3 = q.Z;

            Azimuth = Math.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (Math.Pow(q2, 2) + Math.Pow(q3, 2)));
            Elevation = Math.Asin(2 * (q0 * q2 - q3 * q1));
            Rotation = Math.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (Math.Pow(q1, 2) + Math.Pow(q2, 2)));
        }

        /// <summary>
        /// convert to matrix
        /// http://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        /// </summary>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            Matrix m = Matrix.Identity;

            m.M11 = c(Elevation) * c(Rotation);
            m.M12 = -c(Azimuth) * s(Rotation) + s(Azimuth) * s(Elevation) * c(Rotation);
            m.M13 = s(Azimuth) * s(Rotation) + c(Azimuth) * s(Elevation) * c(Rotation);
            m.M21 = c(Elevation) * s(Rotation);
            m.M22 = c(Azimuth) * c(Rotation) + s(Azimuth) * s(Elevation) * s(Rotation);
            m.M23 = -s(Azimuth) * c(Rotation) + c(Azimuth) * s(Elevation) * s(Rotation);
            m.M31 = -s(Elevation);
            m.M32 = s(Azimuth) * c(Elevation);
            m.M33 = c(Azimuth) * c(Elevation);
            return m;
        }
        /// <summary>
        /// Convert To quaterinon
        /// </summary>
        /// <returns></returns>
        public Quaternion ToQuaternion()
        {
            Quaternion q = Quaternion.RotationMatrix(ToMatrix());
            return q;
        }


        float c(double value)
        {
            return (float)Math.Cos(value);
        }
        float s(double value)
        {
            return (float)Math.Sin(value);
        }
    }
}
