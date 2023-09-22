using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    [Serializable]
    public class ErrorCondition
    {
        [XmlAttribute]
        public string Field { get; set; }
        [XmlAttribute]
        public string Inequality { get; set; }
        [XmlAttribute]
        public double Reference { get; set; }

        public static ErrorCondition Parse(string condition)
        {
            ErrorCondition errorCondition = new ErrorCondition();
            string[] parts = condition.Split('<','>');
            errorCondition.Field = parts[0];
            errorCondition.Inequality = parts[1];
            errorCondition.Reference = double.Parse(parts[2]);
            return errorCondition;
        }

       
    }
}
