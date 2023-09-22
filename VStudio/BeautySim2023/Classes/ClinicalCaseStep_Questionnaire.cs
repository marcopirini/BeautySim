using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BeautySim2023
{
    public class ClinicalCaseStep_Questionnaire : ClinicalCaseStep
    {
        public string ImageName { get; set; }
        public int NumQuestions { get; set; }
        public List<Question> Questions { get; set; }

        public bool MultipleSelectionAllowed { get; internal set; }
        public string Question { get; internal set; }
        public Enum_StepQuestionnaire Step = Enum_StepQuestionnaire.INITIAL;
        public ClinicalCaseStep_Questionnaire(): base(Enum_ClinicalCaseStepType.QUESTIONNAIRE)
        {
            if (ImScoreable)
            {
                Score = -1;
            }
            else
            {
                Score = -2;
            }
            Questions = new List<Question>();
        }

        public override void ClearAllUserRelatedFields()
        {
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
                if (line.StartsWith("MessageToStudent="))
                {
                    MessageToStudent = line.Replace("MessageToStudent=", "");
                }
                else if (line.StartsWith("MessageToTeacher="))
                {
                    MessageToTeacher = line.Replace("MessageToTeacher=", "");
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
                        question.Options.Shuffle();
                        Questions.Add(question);
                    }
                }
            }
        }

        public override void ImportMaterial(string folderMaterial)
        {
        }
    }
}