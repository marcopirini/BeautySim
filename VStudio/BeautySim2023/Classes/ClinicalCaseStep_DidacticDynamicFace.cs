using BeautySim.Common;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BeautySim2023
{
    public class ClinicalCaseStep_DidacticDynamicFace : ClinicalCaseStep
    {
        public Enum_StepDynamicAnalysis Step = Enum_StepDynamicAnalysis.INITIAL;
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
            ErrorCases = new List<ErrorCase>();
            Questions = new List<Question>();
        }

        public string ImageName { get; set; }
        public int NumInjectionsPoints { get; set; }
        public List<InjectionPointSpecific2D> InjectionPoints { get; set; }
        public int NumAdditionalIndications { get; set; }
        public List<AdditionalIndication> AdditionalIndications { get; set; }
        public string NumErrorCases { get; private set; }
        public List<ErrorCase> ErrorCases { get; private set; }

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
                else if (line.StartsWith("NumInjectionsPoints="))
                {
                    string numInjectionsPointsValue = line.Replace("NumInjectionsPoints=", "");
                    NumInjectionsPoints = int.Parse(numInjectionsPointsValue);
                    InjectionPoints = new List<InjectionPointSpecific2D>();
                }
                else if (line.StartsWith("INJPOINT"))
                {
                    string[] injectionPointParts = line.Split('\t');

                    if (injectionPointParts.Length >= 12)
                    {
                        InjectionPointSpecific2D injectionPoint = new InjectionPointSpecific2D
                        {
                            PointNumber = int.Parse(injectionPointParts[1]),
                            ToTarget = injectionPointParts[2].Replace("ToTarget:", "") == "Y",
                            Name = injectionPointParts[3],
                            Coordinates = injectionPointParts[4].Replace("XY:", ""),
                            DepthOptions = injectionPointParts[5].Replace("DepthOptionsmm:", "").Split('/').Select(double.Parse).ToList(),
                            PrescribedDepth = double.Parse(injectionPointParts[6].Replace("DepthCorrectmm:", "")),
                            QuantityOptions = injectionPointParts[7].Replace("QuantityOptionsu:", "").Split('/').Select(double.Parse).ToList(),
                            PrescribedQuantity = double.Parse(injectionPointParts[8].Replace("QuantityCorrectu:", "")),
                            YawMin = double.Parse(injectionPointParts[9].Replace("YawMinMax:", "").Split('/')[0]),
                            YawMax = double.Parse(injectionPointParts[9].Replace("YawMinMax:", "").Split('/')[1]),
                            PitchMin = double.Parse(injectionPointParts[10].Replace("PitchMinMax:", "").Split('/')[0]),
                            PitchMax = double.Parse(injectionPointParts[10].Replace("PitchMaxMax:", "").Split('/')[1]),
                            Explanation = injectionPointParts[11]
                        };

                        string[] coordinateParts = injectionPoint.Coordinates.Split('/');
                        if (coordinateParts.Length >= 2)
                        {
                            double x, y;
                            if (double.TryParse(coordinateParts[0], NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out x))
                            {
                                injectionPoint.X = x;
                            }
                            if (double.TryParse(coordinateParts[1], NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out y))
                            {
                                injectionPoint.Y = y;
                            }
                        }

                        InjectionPoints.Add(injectionPoint);
                    }
                }
                else if (line.StartsWith("NumErrorCases="))
                {
                    string numErrorCasesValue = line.Replace("NumErrorCases=", "");
                    NumErrorCases = numErrorCasesValue;
                    ErrorCases = new List<ErrorCase>();
                }
                //else if (line.StartsWith("ERRORCASE"))
                //{
                //    string[] errorCaseParts = line.Split('\t');

                //    if (errorCaseParts.Length >= 6)
                //    {
                //        ErrorCase errorCase = new ErrorCase
                //        {
                //            ErrorNumber = int.Parse(errorCaseParts[1]),
                //            //InjectionPointsReferenced = errorCaseParts[2].Split('/').Select(int.Parse).ToList(),
                //            ErrorDescription = errorCaseParts[4],
                //            ErrorImageName = errorCaseParts[5],
                //            ErrorConditions = new List<ErrorCondition>()
                //        };

                //        ErrorCases.Add(errorCase);

                //        string conditionPart = errorCaseParts[3];

                //        string[] conditionParts = conditionPart.Split('/');
                //        foreach (string condition in conditionParts)
                //        {
                //            string[] conditionSubparts = condition.Split(new[] { '>', '<', '=' }, 2);

                //            if (conditionSubparts.Length == 2)
                //            {
                //                string field = conditionSubparts[0];
                //                string inequality = conditionPart.Substring(conditionPart.IndexOf(conditionSubparts[0]) + conditionSubparts[0].Length, 1);
                //                double reference = double.Parse(conditionSubparts[1]);

                //                ErrorCondition errorCondition = new ErrorCondition
                //                {
                //                    Field = field,
                //                    Inequality = inequality,
                //                    Reference = reference
                //                };

                //                errorCase.ErrorConditions.Add(errorCondition);
                //            }
                //        }
                //    }
                //}
                else if (line.StartsWith("NumAdditionalIndications="))
                {
                    string numAdditionalIndicationsValue = line.Replace("NumAdditionalIndications=", "");
                    NumAdditionalIndications = int.Parse(numAdditionalIndicationsValue);
                    AdditionalIndications = new List<AdditionalIndication>();
                }
                else if (line.StartsWith("ADDINFO"))
                {
                    string[] additionalIndicationParts = line.Split('\t');

                    if (additionalIndicationParts.Length >= 4)
                    {
                        AdditionalIndication additionalIndication = new AdditionalIndication
                        {
                            Type = additionalIndicationParts[1],
                            Description = additionalIndicationParts[2],
                            ImageName = additionalIndicationParts[3]
                        };

                        AdditionalIndications.Add(additionalIndication);
                    }
                }
            }
        }

        public int NumQuestions { get; set; }
        public List<Question> Questions { get; set; }

        public override void ImportMaterial(string folderMaterial)
        {
            // Import any material related to this step from the specified folder
        }
    }
}