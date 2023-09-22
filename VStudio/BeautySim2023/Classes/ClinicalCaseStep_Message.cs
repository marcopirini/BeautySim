using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BeautySim2023
{
    [Serializable]
    public class ClinicalCaseStep_Message : ClinicalCaseStep
    {
        [XmlAttribute]
        public string Message { get; set; }

        [XmlAttribute]
        public Enum_MessageType MessageType { get; set; }

        public ClinicalCaseStep_Message() : base(Enum_ClinicalCaseStepType.MESSAGE)
        {
            ImScoreable = false;
            IVeDesideredDuration = false;
        }

        public override void ImportInformationFromTxtFile(string fileName)
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            if (lines.Count >= 4)
            {
                MessageType = (Enum_MessageType)Enum.Parse(typeof(Enum_MessageType), lines[0]);
                MessageToStudent = lines[1];
                MessageToTeacher = lines[2];
                Message = lines[3];
            }
        }
    }
}