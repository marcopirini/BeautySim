using MathNet.Numerics.LinearAlgebra;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Calib3DApp
{
    public class Matrix3DUtils
    {
        public static Vector3D[] ConvertPoint3DArrayToVector3DArray(Point3D[] point3DArray)
        {
            return Array.ConvertAll(point3DArray, point => new Vector3D(point.X, point.Y, point.Z));
        }

        public static System.Windows.Media.Media3D.Quaternion QuaternionFromMatrix(Matrix3D m)
        {
            System.Windows.Media.Media3D.Quaternion q = new System.Windows.Media.Media3D.Quaternion();

            double trace = m.M11 + m.M22 + m.M33;
            if (trace > 0.0)
            {
                double s = Math.Sqrt(trace + 1.0);
                q.W = s * 0.5;
                s = 0.5 / s;
                q.X = (m.M32 - m.M23) * s;
                q.Y = (m.M13 - m.M31) * s;
                q.Z = (m.M21 - m.M12) * s;
            }
            else
            {
                int i = m.M11 < m.M22 ?
                        (m.M22 < m.M33 ? 2 : 1) :
                        (m.M11 < m.M33 ? 2 : 0);
                int j = (i + 1) % 3;
                int k = (i + 2) % 3;

                double ii = i == 0 ? m.M11 : i == 1 ? m.M22 : m.M33;
                double jj = j == 0 ? m.M11 : j == 1 ? m.M22 : m.M33;
                double kk = k == 0 ? m.M11 : k == 1 ? m.M22 : m.M33;

                double s = Math.Sqrt(ii - jj - kk + 1.0);
                double qValue = s * 0.5;
                s = 0.5 / s;

                double kj = k == 0 && j == 1 ? m.M21 : k == 1 && j == 0 ? m.M12 :
                            k == 1 && j == 2 ? m.M32 : k == 2 && j == 1 ? m.M23 :
                            k == 0 && j == 2 ? m.M31 : m.M13;

                double ji = j == 0 && i == 1 ? m.M21 : j == 1 && i == 0 ? m.M12 :
                            j == 1 && i == 2 ? m.M32 : j == 2 && i == 1 ? m.M23 :
                            j == 0 && i == 2 ? m.M31 : m.M13;

                double ki = k == 0 && i == 1 ? m.M21 : k == 1 && i == 0 ? m.M12 :
                            k == 1 && i == 2 ? m.M32 : k == 2 && i == 1 ? m.M23 :
                            k == 0 && i == 2 ? m.M31 : m.M13;

                q.W = (kj - ji) * s;
                if (i == 0) q.X = qValue;
                if (j == 0) q.Y = (ji + ki) * s;
                if (k == 0) q.Z = (ki + kj) * s;
                if (i == 1) q.Y = qValue;
                if (j == 1) q.Z = (ji + ki) * s;
                if (k == 1) q.X = (ki + kj) * s;
                if (i == 2) q.Z = qValue;
            }
            return q;
        }

        public static Matrix3D Transpose(Matrix3D matrix)
        {
            return new Matrix3D(
                matrix.M11, matrix.M21, matrix.M31, matrix.OffsetX,
                matrix.M12, matrix.M22, matrix.M32, matrix.OffsetY,
                matrix.M13, matrix.M23, matrix.M33, matrix.OffsetZ,
                matrix.M14, matrix.M24, matrix.M34, matrix.M44
            );
        }

        public static (Matrix3x3 U, Matrix3x3 S, Matrix3x3 Vt) ComputeSVD(Matrix3x3 matrix)
        {
            // Convert the Matrix3D to a MathNet Numerics matrix
            var mathNetMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
            { matrix.M11, matrix.M12, matrix.M13 },
            { matrix.M21, matrix.M22, matrix.M23 },
            { matrix.M31, matrix.M32, matrix.M33 }
            });

            // Compute the SVD
            var svd = mathNetMatrix.Svd(true);

            // Convert the U, Σ, and Vt matrices from the SVD to Matrix3D objects
            var U = new Matrix3x3(
                (float)svd.U[0, 0], (float)svd.U[0, 1], (float)svd.U[0, 2],
                (float)svd.U[1, 0], (float)svd.U[1, 1], (float)svd.U[1, 2],
                (float)svd.U[2, 0], (float)svd.U[2, 1], (float)svd.U[2, 2]);

            var S = new Matrix3x3(
                (float)svd.S[0], 0, 0,
                0, (float)svd.S[1], 0,
                0, 0, (float)svd.S[2]);

            var Vt = new Matrix3x3(
                (float)svd.VT[0, 0], (float)svd.VT[0, 1], (float)svd.VT[0, 2],
                (float)svd.VT[1, 0], (float)svd.VT[1, 1], (float)svd.VT[1, 2],
                (float)svd.VT[2, 0], (float)svd.VT[2, 1], (float)svd.VT[2, 2]);

            return (U, S, Vt);
        }
    }
}
