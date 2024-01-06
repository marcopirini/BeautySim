using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace BeautySim2023
{
    public class ResultToSave : XMLBase 
    {
        [XmlAttribute]
        public string CaseName { get; set; }
        [XmlAttribute]
        public string CaseDescription{ get; set; }

        [XmlAttribute]
        public bool IsBotoxCase { get; set; }

        [XmlArray]
        public List<ResultPhaseToSave> ResultPhases { get; set; }

        public ResultToSave()
        {
            ResultPhases = new List<ResultPhaseToSave>();
        }
    }

    public class ResultPhaseToSave
    {
        [XmlAttribute]
        public string PhaseName { get; set; }

        [XmlAttribute]
        public Enum_ClinicalCaseStepType PhaseType { get; set; }

        [XmlAttribute]
        public double QuestionnaireScore { get; set; }

        [XmlAttribute]
        public double TotalScore { get; set; }

        [XmlAttribute]
        public double ActionScore { get; set; }

        [XmlAttribute]
        public int NumErrorsQuestionnaire { get; set; }

        [XmlArray]
        public List<string> ActionsOnError { get; set; }

        public ResultPhaseToSave()
        {
            ActionsOnError = new List<string>();
        }

    }
}