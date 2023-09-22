using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySim.Common
{
    public class InjectionPoint3D
    {
        public int PointNumber { get; set; }
        public bool ToTarget { get; set; }
        public string AreaName { get; set; }
        public double DepthMin { get; set; }
        public double DepthMax { get; set; }
        public double QuantityMin { get; set; }
        public double QuantityMax { get; set; }
        public double YawMin { get; set; }
        public double YawMax { get; set; }
        public double PitchMin { get; set; }
        public double PitchMax { get; set; }
        public string Explanation { get; set; }
    }
}
