using BeautySim.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BeautySim2023
{
    public class ClinicalCaseStep_DidacticDynamicFace : ClinicalCaseStep
    {
        public Enum_StepDynamicAnalysis Step = Enum_StepDynamicAnalysis.INITIAL;

        public Enum_AreaDefinition AreaDefinition { get; set; }

        public double QuestionnaireScore = 0;

        public double OperativityScore = 0;

        public ClinicalCaseStep_DidacticDynamicFace() : base(Enum_ClinicalCaseStepType.DIDACTIC_DYNAMIC_FACE)
        {
            if (ImScoreable)
            {
                Score = -1;
            }
            else
            {
                Score = -2;
            }
            InjectionPoints = new List<InjectionPointSpecific2D>();
            AdditionalIndications = new List<AdditionalIndication>();
            ErrorCases = new List<AnalysResult>();
            Questions = new List<Question>();
        }

        public string ImageName { get; set; }
        public int NumInjectionsPoints { get; set; }
        public List<InjectionPointSpecific2D> InjectionPoints { get; set; }
        public int NumAdditionalIndications { get; set; }
        public List<AdditionalIndication> AdditionalIndications { get; set; }
        public string NumErrorCases { get; private set; }
        public List<AnalysResult> ErrorCases { get; private set; }

        public override void ClearAllUserRelatedFields()
        {
            // Clear any user-related fields in this step
        }

        public override void ImportInformationFromTxtFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File not found.", fileName);
            }

            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                if (line.StartsWith("NameStep="))
                {
                    NameStep = line.Replace("NameStep=", "");
                }
                else if (line.StartsWith("MessageToStudent="))
                {
                    MessageToStudent = line.Replace("MessageToStudent=", "");
                }
                else if (line.StartsWith("MessageToTeacher="))
                {
                    MessageToTeacher = line.Replace("MessageToTeacher=", "");
                }
                else if (line.StartsWith("MessageToStudentAction="))
                {
                    MessageToStudentAction = line.Replace("MessageToStudentAction=", "");
                }
                else if (line.StartsWith("MessageToTeacherAction="))
                {
                    MessageToTeacherAction = line.Replace("MessageToTeacherAction=", "");
                }
                else if (line.StartsWith("MessageToStudentConsequences="))
                {
                    MessageToStudentConsequences = line.Replace("MessageToStudentConsequences=", "");
                }
                else if (line.StartsWith("MessageToTeacherConsequences="))
                {
                    MessageToTeacherConsequences = line.Replace("MessageToTeacherConsequences=", "");
                }
                else if (line.StartsWith("ImReallyPresent="))
                {
                    string imReallyPresentValue = line.Replace("ImReallyPresent=", "");
                    ImReallyPresent = bool.Parse(imReallyPresentValue);
                }




                else if (line.StartsWith("ImageName="))
                {
                    ImageName = line.Replace("ImageName=", "");
                }
                else if (line.StartsWith("Area="))
                {
                    AreaDefinition = (Enum_AreaDefinition)Enum.Parse(typeof(Enum_AreaDefinition), line.Replace("Area=", ""));
                }
                else if (line.StartsWith("NumQuestions="))
                {
                    string numQuestionsValue = line.Replace("NumQuestions=", "");
                    NumQuestions = int.Parse(numQuestionsValue);
                }
                else if (line.StartsWith("QUESTION"))
                {
                    string[] questionParts = line.Split('\t');

                    if (questionParts.Length >= 7)
                    {
                        Question question = new Question
                        {
                            QuestionText = questionParts[2],
                            Options = questionParts[3].Split('/').ToList(),
                            CorrectAnswers = questionParts[4].Split('/').ToList(),
                            Explanation = questionParts[5],
                            Weight = float.Parse(questionParts[6])
                        };
                        //question.Options.Shuffle();
                        Questions.Add(question);
                    }
                }
                else if (line.StartsWith("PointDefinitionFile"))
                {
                    PointDefinitionFileName = line.Replace("PointDefinitionFile=", "");
                }
            }
        }

        public int NumQuestions { get; set; }
        public List<Question> Questions { get; set; }
        public string PointDefinitionFileName { get; private set; }
        public string MessageToStudentAction { get; private set; }
        public string MessageToTeacherAction { get; private set; }
        public string MessageToStudentConsequences { get; private set; }
        public string MessageToTeacherConsequences { get; private set; }
        public List<AnalysResult> Consequences { get; internal set; }

        public override void ImportMaterial(string folderMaterial)
        {
            // Import any material related to this step from the specified folder
        }
    }
}