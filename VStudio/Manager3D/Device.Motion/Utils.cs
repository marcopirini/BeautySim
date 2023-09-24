using VectorMath;

namespace Device.Motion
{
    public class VectorUtils
    {
        /// <summary>
        /// calculate the relative sensor rotation on transformed reference frame
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Quaternion RelativeRotation(Quaternion from, Quaternion to, Matrix transform)
        {

            Quaternion t = Quaternion.RotationMatrix(transform);

            Quaternion q1 = from * t;
            Quaternion q2 = to * t;
            q1.Invert();
            Quaternion q = q1 * q2;

            return q;

        }

        static Quaternion TransformRotation(Quaternion q, Matrix t)
        {
            Vector3 v = new Vector3(q.X, q.Y, q.Z);

            v = Vector3.TransformCoordinate(v, t);

            return new Quaternion(v.X, v.Y, v.Z, q.W);
        }

        static Quaternion QuaternionToQuaternionRotation(Vector3 from_x, Vector3 from_z, Vector3 to_x, Vector3 to_z)
        {


            Quaternion q_rot1 = D3DXQuaternionAxisToAxis(from_x, to_x);
            Matrix m_rot1 = Matrix.RotationQuaternion(q_rot1);
            from_z = Vector3.TransformNormal(from_z, m_rot1);
            from_z.Normalize();

            Quaternion q_rot2 = D3DXQuaternionAxisToAxis(from_z, to_z);
            Matrix m_rot2 = Matrix.RotationQuaternion(q_rot2);

            return (q_rot1 * q_rot2);
        }

        static Quaternion D3DXQuaternionAxisToAxis(Vector3 fromVector, Vector3 toVector)
        {
            Vector3 vA = Vector3.Normalize(fromVector);
            Vector3 vB = Vector3.Normalize(toVector);
            Vector3 vHalf = Vector3.Add(vA, vB);
            vHalf = Vector3.Normalize(vHalf);
            return D3DXQuaternionUnitAxisToUnitAxis2(vA, vHalf);
        }
        static Quaternion D3DXQuaternionUnitAxisToUnitAxis2(Vector3 fromVector, Vector3 toVector)
        {
            Vector3 axis = Vector3.Cross(fromVector, toVector);    // proportional to sin(theta)
            return new Quaternion(axis.X, axis.Y, axis.Z, Vector3.Dot(fromVector, toVector));
        }
        /// <summary>
        /// Build  roto-translation matrix from pos offset and DCM vectors
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix MatrixFromDCM(Vector3 pos, Vector3 x, Vector3 y, Vector3 z)
        {
            Matrix mat = Matrix.Identity;
            mat.M11 = x.X; mat.M12 = x.Y; mat.M13 = x.Z;
            mat.M21 = y.X; mat.M22 = y.Y; mat.M23 = y.Z;
            mat.M31 = z.X; mat.M32 = z.Y; mat.M33 = z.Z;
            mat.M41 = pos.X; mat.M42 = pos.Y; mat.M43 = pos.Z; mat.M44 = 1;
            return mat;
        }

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
    }
}
