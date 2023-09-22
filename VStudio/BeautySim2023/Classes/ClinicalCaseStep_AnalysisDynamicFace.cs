using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeautySim2023
{
    public class ClinicalCaseStep_AnalysisDynamicFace : ClinicalCaseStep
    {
        public ClinicalCaseStep_AnalysisDynamicFace() : base(Enum_ClinicalCaseStepType.ANALYSIS_DYNAMIC_FACE)
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

        public string ImageName { get; set; }


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
            // Import any material related to this step from the specified folder
        }


    }
}