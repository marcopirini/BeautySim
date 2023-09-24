using BeautySim.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeautySim2023
{
    public class ClinicalCaseStep_Face3DInteraction : ClinicalCaseStep
    {

        public string ImageName { get; set; }
        public int NumInjectionsPoints { get; set; }
        public List<InjectionPoint3D> InjectionPoints3D { get; set; }

        public List<ErrorCase> ErrorCases { get; private set; }
        public Enum_StepFace3DInteraction Step { get; internal set; }

        public ClinicalCaseStep_Face3DInteraction() : base(Enum_ClinicalCaseStepType.FACE3D_INTERACTION)
        {
            if (ImScoreable)
            {
                Score = -1;
            }
            else
            {
                Score = -2;
            }

            InjectionPoints3D = new List<InjectionPoint3D>();
            ErrorCases = new List<ErrorCase>();
        }

        public override void ClearAllUserRelatedFields()
        {
        }

        public override void ImportInformationFromTxtFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
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
                    else if (line.StartsWith("NumInjectionsPoints="))
                    {
                        string numInjectionsPointsValue = line.Replace("NumInjectionsPoints=", "");
                        NumInjectionsPoints = int.Parse(numInjectionsPointsValue);
                        InjectionPoints3D = new List<InjectionPoint3D>();
                    }
                    else if(line.StartsWith("INJPOINT3D"))
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length >= 9)
                        {
                            int pointNumber = int.Parse(parts[1]);
                            bool toTarget = parts[2].Split(':')[1] == "Y";
                            string areaName = parts[3];
                            string[] depthMinMax = parts[4].Split(':')[1].Split('/');
                            double depthMin = double.Parse(depthMinMax[0]);
                            double depthMax = double.Parse(depthMinMax[1]);
                            string[] quantityMinMax = parts[5].Split(':')[1].Split('/');
                            double quantityMin = double.Parse(quantityMinMax[0]);
                            double quantityMax = double.Parse(quantityMinMax[1]);
                            string[] yawMinMax = parts[6].Split(':')[1].Split('/');
                            double yawMin = double.Parse(yawMinMax[0]);
                            double yawMax = double.Parse(yawMinMax[1]);
                            string[] pitchMinMax = parts[7].Split(':')[1].Split('/');
                            double pitchMin = double.Parse(pitchMinMax[0]);
                            double pitchMax = double.Parse(pitchMinMax[1]);
                            string explanation = parts[8];

                            // Create and populate the InjectionPoint3D object
                            InjectionPoint3D injectionPoint3D = new InjectionPoint3D
                            {
                                PointNumber = pointNumber,
                                ToTarget = toTarget,
                                AreaName = areaName,
                                DepthMin = depthMin,
                                DepthMax = depthMax,
                                QuantityMin = quantityMin,
                                QuantityMax = quantityMax,
                                YawMin = yawMin,
                                YawMax = yawMax,
                                PitchMin = pitchMin,
                                PitchMax = pitchMax,
                                Explanation = explanation
                            };

                            // Add the InjectionPoint3D to the collection
                            InjectionPoints3D.Add(injectionPoint3D);
                        }
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
                }
            }
        }
    }

 
}