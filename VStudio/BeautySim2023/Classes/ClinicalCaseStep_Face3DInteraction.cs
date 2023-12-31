using BeautySim.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace BeautySim2023
{
    public class ClinicalCaseStep_Face3DInteraction : ClinicalCaseStep
    {
        public double OperativityScore = 0;
        public string ImageName { get; set; }
        public int NumInjectionsPoints { get; set; }
        public List<InjectionPoint3D> InjectionPoints3D { get; set; }

        public Enum_StepFace3DInteraction Step { get; internal set; }
        public string PointDefinitionFileName { get; private set; }
        public Enum_AreaDefinition AreaDefinition { get; private set; }
        public string MessageToStudentConsequences { get; private set; }
        public string MessageToTeacherConsequences { get; private set; }
        public string ImageNameReference { get; private set; }

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
                    else if (line.StartsWith("Area="))
                    {
                        AreaDefinition = (Enum_AreaDefinition)Enum.Parse(typeof(Enum_AreaDefinition), line.Replace("Area=", ""));
                    }
                    else if (line.StartsWith("MessageToStudent="))
                    {
                        MessageToStudent = line.Replace("MessageToStudent=", "");
                    }
                    else if (line.StartsWith("MessageToTeacher="))
                    {
                        MessageToTeacher = line.Replace("MessageToTeacher=", "");
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
                    else if (line.StartsWith("PointDefinitionFile"))
                    {
                        PointDefinitionFileName = line.Replace("PointDefinitionFile=", "");
                    }
                    else if (line.StartsWith("ImageName="))
                    {
                        ImageName = line.Replace("ImageName=", "");
                    }
                    else if (line.StartsWith("ImageNameReference"))
                    {
                        ImageNameReference = line.Replace("ImageNameReference=", "");
                    }
                }
            }
        }
    }
}