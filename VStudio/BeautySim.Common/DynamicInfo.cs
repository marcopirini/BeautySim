using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    [Serializable]
    public class DynamicInfo : XMLBase
    {
        [XmlArray]
        public List<InjectionPoint> InjectionPoints { get; set; }

        [XmlArray]
        public List<ErrorCase> ErrorCases { get; set; }

        public DynamicInfo()
        {
            InjectionPoints = new List<InjectionPoint>();
            ErrorCases = new List<ErrorCase>();
        }

    }
}
