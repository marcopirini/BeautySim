using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    [Serializable]
    public class ErrorCase
    {
        [XmlAttribute]
        public int ErrorNumber { get; set; }
        [XmlArray]
        public List<ErrorCondition> ErrorConditions { get; set; }
        [XmlAttribute]
        public string ErrorDescription { get; set; }
        [XmlAttribute]
        public string ErrorImageName { get; set; }
        //[XmlArray]
        //public List<string> InjectionPointsReferenced { get; set; }
   
        public ErrorCase()
        {
            //InjectionPointsReferenced = new List<string>();
            ErrorConditions = new List<ErrorCondition>();
            ErrorDescription = "";
            ErrorImageName = "";
        }
    }
}