using System;
using System.Windows.Media.Media3D;

namespace BeautySim2023
{
    /// <summary>
    /// Singleton class exposing methods to perform calculations
    /// </summary>
    public class Calculator
    {
        private static Calculator instance;

        private Calculator()
        {
        }

        public static Calculator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Calculator();
                }
                return instance;
            }
        }

        public VectorMath.Quaternion CopyQuat(VectorMath.Quaternion q)
        {
            return new VectorMath.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public VectorMath.Vector3 FindProjection(VectorMath.Vector3 point, double A, double B, double C, VectorMath.Vector3 planePoint)
        {
            // Find projection k of point p onto plane with normal n passing by q
            // t = A(q.x-p-x)+B(q.y-p-y)+C(q.z-p-z) / (A*A+B*B+C*C)
            // k = [p.x+t*A, p.y+t*B, p.z+t*C]

            double t = A * (planePoint.X - point.X) + B * (planePoint.Y - point.Y) + C * (planePoint.Z - point.Z) / (A * A + B * B + C * C);
            return new VectorMath.Vector3((float)(point.X + t * A), (float)(point.Y + t * B), (float)(point.Z + t * C));
        }

        public SharpDX.Vector3 GetMedianPoint(SharpDX.Vector3 start, SharpDX.Vector3 end)
        {
            return new SharpDX.Vector3((end.X - start.X) / 2 + start.X, (end.Y - start.Y) / 2 + start.Y, (end.Z - start.Z) / 2 + start.Z);
        }

        public VectorMath.Quaternion GetQuatFromMedia3D(System.Windows.Media.Media3D.Quaternion q)
        {
            return new VectorMath.Quaternion((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
        }

        public System.Windows.Media.Media3D.Quaternion GetQuatFromVectorMat(VectorMath.Quaternion q)
        {
            return new System.Windows.Media.Media3D.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public double[,] GetRotationMatrix(VectorMath.Matrix ma)
        {
            double[,] m = new double[3, 3];
            m[0, 0] = ma.M11;
            m[0, 1] = ma.M12;
            m[0, 2] = ma.M13;
            m[1, 0] = ma.M21;
            m[1, 1] = ma.M22;
            m[1, 2] = ma.M23;
            m[2, 0] = ma.M31;
            m[2, 1] = ma.M32;
            m[2, 2] = ma.M33;
            return m;
        }

        /// <summary>
        /// Implements PFinal=M*PInitial + delta
        /// </summary>
        /// <param name="p"></param>
        /// <param name="matr"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public VectorMath.Vector3 MultiplyMatrixByPoint(VectorMath.Vector3 p, double[,] matr)
        {
            double x = matr[0, 0] * p.X + matr[0, 1] * p.Y + matr[0, 2] * p.Z;
            double y = matr[1, 0] * p.X + matr[1, 1] * p.Y + matr[1, 2] * p.Z;
            double z = matr[2, 0] * p.X + matr[2, 1] * p.Y + matr[2, 2] * p.Z;

            return new VectorMath.Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Implements PFinal=M*PInitial + delta
        /// </summary>
        /// <param name="p"></param>
        /// <param name="matr"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public VectorMath.Vector3 RototraslatePoint(VectorMath.Vector3 p, double[,] matr, VectorMath.Vector3 delta)
        {
            double x = matr[0, 0] * p.X + matr[0, 1] * p.Y + matr[0, 2] * p.Z + delta.X;
            double y = matr[1, 0] * p.X + matr[1, 1] * p.Y + matr[1, 2] * p.Z + delta.Y;
            double z = matr[2, 0] * p.X + matr[2, 1] * p.Y + matr[2, 2] * p.Z + delta.Z;

            return new VectorMath.Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Traspose a Matrix as a double[3,3]
        /// </summary>
        /// <param name="secondProduct"></param>
        /// <returns></returns>
        public double[,] TrasposeMatrix(double[,] matrixToTraspose)
        {
            double[,] toret = new double[3, 3];
            toret[0, 0] = matrixToTraspose[0, 0];
            toret[0, 1] = matrixToTraspose[1, 0];
            toret[0, 2] = matrixToTraspose[2, 0];
            toret[1, 0] = matrixToTraspose[0, 1];
            toret[1, 1] = matrixToTraspose[1, 1];
            toret[1, 2] = matrixToTraspose[2, 1];
            toret[2, 0] = matrixToTraspose[0, 2];
            toret[2, 1] = matrixToTraspose[1, 2];
            toret[2, 2] = matrixToTraspose[2, 2];

            return toret;
        }

        public VectorMath.Vector3 ChangeCoordinatesOfAPoint(VectorMath.Vector3 pointOnSystem1, VectorMath.Vector3 CenterOfSystem2, double[,] rotationMatrixFromM2toM1)
        {
            try
            {
                VectorMath.Vector3 aa = pointOnSystem1 - CenterOfSystem2;

                //double[,] matrixToUse = rotationMatrixFromM2toM1;
                double[,] matrixToUse = Calculator.Instance.TrasposeMatrix(rotationMatrixFromM2toM1);

                return MultiplyMatrixByPoint(aa, matrixToUse);
            }
            catch (Exception)
            {
                return new VectorMath.Vector3();
            }
        }

        public VectorMath.Vector3 CopyVector3(VectorMath.Vector3 v)
        {
            return new VectorMath.Vector3(v.X, v.Y, v.Z);
        }

        public SharpDX.Vector3 GetPointAtRatio(SharpDX.Vector3 vectorStart, SharpDX.Vector3 vectorEnd, float ratioPointDeformation)
        {
            SharpDX.Vector3 diff = (vectorEnd - vectorStart); //normalize.Normalize();

            SharpDX.Vector3 scaled = new SharpDX.Vector3(diff.X * ratioPointDeformation, diff.Y * ratioPointDeformation, diff.Z * ratioPointDeformation);

            return (scaled + vectorStart);
        }
        public Point3D GiveMePoint3D(VectorMath.Vector3 v)
        {
            return new Point3D(v.X, v.Y, v.Z);
        }

        public System.Windows.Media.Media3D.Point3D GiveMeVector3(VectorMath.Vector3 projectionNeedleOrigin)
        {
            return new System.Windows.Media.Media3D.Point3D(projectionNeedleOrigin.X, projectionNeedleOrigin.Y, projectionNeedleOrigin.Z);
        }
        public Vector3D GiveMeVector3D(VectorMath.Vector3 v)
        {
            return new Vector3D(v.X, v.Y, v.Z);
        }
    }
}