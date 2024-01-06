using BeautySim.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace BeautySim2023
{
    public class ClinicalCaseStep_Face3DInteraction : ClinicalCaseStep
    {

        internal void DecreaseConsequence()
        {
            AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex - 1;
            if (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex < 0)
            {
                AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex = 0;
            }
        }

        internal void IncreaseConsequence()
        {
            AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex + 1;
            if (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex >= AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count)
            {
                AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count - 1;
            }
        }

        public double OperativityScore = 0;
        public string ImageName { get; set; }
        public int NumInjectionsPoints { get; set; }
        public List<InjectionPoint3D> InjectionPoints3D { get; set; }

        public List<AnalysResult> Consequences { get; internal set; }
        public Enum_StepFace3DInteraction Step { get; internal set; }
        public string PointDefinitionFileName { get; private set; }
        public Enum_AreaDefinition AreaDefinition { get; private set; }
        public string MessageToStudentConsequences { get; private set; }
        public string MessageToTeacherConsequences { get; private set; }
        public string ImageNameReference { get; private set; }
        public bool AlreadyCheckedAllConsequencies { get; internal set; }
        public List<string> ErrorsDescription { get; internal set; }

        public int SelectedConsequenceIndex = 0;

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
            ErrorsActive = new List<string>();
        }

        public List<string> ErrorsActive { get; internal set; }

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