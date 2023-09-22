using System.Xml.Serialization;

namespace BeautySim2023
{
    public class ResultToSave : XMLBase 
    {
        [XmlAttribute]
        public int NumberOfNeedleInsertions { get; set; }

        [XmlAttribute]
        public bool IsMultipleNeedleInjections { get; set; }

        [XmlAttribute]
        public int WrongInjections { get; set; }

        [XmlAttribute]
        public int TotalTargetNerves { get; set; }

        [XmlAttribute]
        public int TotalTargetInjected { get; set; }

        [XmlAttribute]
        public int TotalVascular { get; set; }


        public ResultToSave()
        {
        }
    }
}