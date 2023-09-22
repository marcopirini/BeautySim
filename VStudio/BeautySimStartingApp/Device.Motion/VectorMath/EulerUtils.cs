using Accurate.Device.VectorMath.VectorMath;
using System;

namespace VectorMath
{
    public class EulerUtils
    {
        /// <summary>
        /// Get X Y Z (DCM cosine vectors) from matrix m
        /// </summary>
        /// <param name="m"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void DCMFromMatrix(Matrix m, ref Vector3 x, ref Vector3 y, ref Vector3 z)
        {
            x.X = m.M11; x.Y = m.M12; x.Z = m.M13;
            y.X = m.M21; y.Y = m.M22; y.Z = m.M23;
            z.X = m.M31; z.Y = m.M32; z.Z = m.M33;
        }

        public static Vector3 DCMtoEA(Matrix m, EULER_CALC_MODE mode)
        {
            Vector3 res = new Vector3();
            float e = 1 - 1e-6f;
            float pi2 = (float)Math.PI / 2;
            switch (mode)
            {
                case EULER_CALC_MODE.XZY:

                    if (Math.Abs(m.M12) > e)
                    {
                        res.X = (float)Math.Atan2(m.M31, m.M21);
                        res.Z = -pi2 * Math.Sign(m.M12);
                        res.Y = 0;
                    }
                    else
                    {
                        res.Y = (float)Math.Atan2(m.M13, m.M11);
                        res.X = (float)Math.Atan2(m.M32, m.M22);
                        res.Z = (float)Math.Asin(-m.M12);
                    }

                    break;

                case EULER_CALC_MODE.XYZ:

                    if (Math.Abs(m.M13) > e)
                    {
                        res.X = (float)Math.Atan2(m.M21, -m.M31);
                        res.Z = 0;
                        res.Y = pi2 * Math.Sign(m.M13);
                    }
                    else
                    {
                        res.Z = (float)Math.Atan2(-m.M12, m.M11);
                        res.Y = (float)Math.Asin(m.M13);
                        res.X = (float)Math.Atan2(-m.M23, m.M33);
                    }

                    break;

                case EULER_CALC_MODE.YXZ:

                    if (Math.Abs(m.M23) > e)
                    {
                        res.X = pi2 * Math.Sign(m.M23);
                        res.Z = 0;
                        res.Y = (float)Math.Atan2(m.M12, m.M32);
                    }
                    else
                    {
                        res.Z = (float)Math.Atan2(m.M21, m.M22);
                        res.Y = (float)Math.Atan2(m.M13, m.M33);
                        res.X = (float)Math.Asin(-m.M23);
                    }

                    break;

                case EULER_CALC_MODE.YZX:

                    if (Math.Abs(m.M21) > e)
                    {
                        res.X = 0;
                        res.Z = pi2 * Math.Sign(m.M21);
                        res.Y = (float)Math.Atan2(m.M13, -m.M12);
                    }
                    else
                    {
                        res.Z = (float)Math.Asin(m.M21);
                        res.Y = (float)Math.Atan2(-m.M31, m.M11);
                        res.X = (float)Math.Atan2(-m.M23, m.M22);
                    }

                    break;

                case EULER_CALC_MODE.ZYX:

                    if (Math.Abs(m.M31) > e)
                    {
                        res.X = 0;
                        res.Z = (float)Math.Atan2(m.M23, m.M13);
                        res.Y = pi2 * Math.Sign(m.M31);
                    }
                    else
                    {
                        res.Z = (float)Math.Atan2(m.M21, m.M11);
                        res.Y = (float)Math.Asin(-m.M31);
                        res.X = (float)Math.Atan2(m.M32, m.M33);
                    }

                    break;

                case EULER_CALC_MODE.ZXY:

                    if (Math.Abs(m.M32) > e)
                    {
                        res.Y = 0;
                        res.Z = (float)Math.Atan2(m.M13, -m.M23);
                        res.X = pi2 * Math.Sign(m.M32);
                    }
                    else
                    {
                        res.Z = (float)Math.Atan2(-m.M12, m.M22);
                        res.Y = (float)Math.Atan2(-m.M31, m.M33);
                        res.X = (float)Math.Asin(m.M32);
                    }

                    break;

                default:
                    break;
            }

            return res;
        }

        public static Vector3 DCMtoEA(Vector3 x, Vector3 y, Vector3 z, EULER_CALC_MODE mode)
        {
            double yaw = 0, pitch = 0, roll = 0;

            switch (mode)
            {
                case EULER_CALC_MODE.XZY:

                    #region MyRegion

                    yaw = Math.Atan2(z.X, x.X);
                    roll = Math.Asin(-y.X);
                    pitch = Math.Atan2(y.Z, y.Y);

                    #endregion MyRegion

                    break;

                case EULER_CALC_MODE.XYZ:

                    #region MyRegion

                    if (Math.Abs(z.Z) < 0.0000001) z.Z = 0;
                    pitch = -Math.Atan2(z.Y, z.Z);  //pitch
                    if (Math.Abs(z.X) > 1)
                    {
                        z.X = Math.Sign(z.X);
                    }
                    yaw = -Math.Asin(-z.X);        //yaw

                    if (Math.Abs(x.X) < 0.0000001) x.X = 0;
                    roll = -Math.Atan2(y.X, x.X);  //roll

                    #endregion MyRegion

                    break;

                case EULER_CALC_MODE.YXZ:

                    #region MyRegion

                    if (Math.Abs(z.Y) > 1)
                    {
                        z.Y = Math.Sign(z.Y);
                    }

                    yaw = -Math.Atan2(-z.X, z.Z);
                    pitch = -Math.Asin(z.Y);
                    roll = Math.Atan2(x.Y, y.Y);

                    #endregion MyRegion

                    break;

                case EULER_CALC_MODE.YZX:

                    #region MyRegion

                    yaw = -Math.Atan2(x.Z, x.X);
                    roll = Math.Asin(x.Y);
                    pitch = -Math.Atan2(z.Y, y.Y);

                    #endregion MyRegion

                    break;

                case EULER_CALC_MODE.ZYX:
                    roll = Math.Atan2(x.Y, x.X);
                    yaw = -Math.Asin(x.Z);
                    pitch = Math.Atan2(y.Z, z.Z);
                    break;

                case EULER_CALC_MODE.ZXY:
                    roll = -Math.Atan2(y.X, y.Y);
                    pitch = Math.Asin(y.Z);
                    yaw = -Math.Atan2(x.Z, z.Z);
                    break;

                default:
                    break;
            }

            return new Vector3((float)pitch, (float)yaw, (float)roll);
        }

        /// <summary>
        /// Transofrm a quaternion into an Extrinsic Euler Angles representation
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static ExtrinsicEulerAngles FromQuaternioToEuler(Quaternion q)
        {
            //q.Normalize();

            double q0 = q.W;
            double q1 = q.X;
            double q2 = q.Y;
            double q3 = q.Z;

            float rol = (float)Math.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (Math.Pow(q1, 2) + Math.Pow(q2, 2)));
            float el = (float)Math.Asin(2 * (q0 * q2 - q3 * q1));
            float az = (float)Math.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (Math.Pow(q2, 2) + Math.Pow(q3, 2)));

            return new ExtrinsicEulerAngles(MathHelper.RadiansToDegrees(az), MathHelper.RadiansToDegrees(el), MathHelper.RadiansToDegrees(rol));

            //return new EulerAngles(q.W, q.X, q.Y);
        }



    }
}