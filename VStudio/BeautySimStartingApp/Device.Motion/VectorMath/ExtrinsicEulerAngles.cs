using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorMath
{
    public class ExtrinsicEulerAngles
    {
        public float Azimuth;
        public float Elevation;
        public float Roll;

        public ExtrinsicEulerAngles(float az, float el, float rol)
        {
            Azimuth = az;
            Elevation = el;
            Roll = rol;
        }

    }
}
