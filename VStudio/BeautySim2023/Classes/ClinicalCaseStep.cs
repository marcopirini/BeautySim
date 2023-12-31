namespace BeautySim2023
{
    public class ClinicalCaseStep : NotifyClass
    {
        public string FolderImages = "";
        private bool currentlySelected;
        private bool excludable = false;
        private float score;
        private bool selected;
        public int NumErrors { get; internal set; }
        public double QuestionnaireScore = 0;
        public ClinicalCaseStep()
        {
        }

        public ClinicalCaseStep(Enum_ClinicalCaseStepType type)
        {
            Type = type;
        }

        public bool CurrentlySelected
        {
            get
            {
                return currentlySelected;
            }
            set
            {
                if (value != currentlySelected)
                {
                    SendPropertyChanging();
                    currentlySelected = value;
                    SendPropertyChanged("CurrentlySelected");
                }
            }
        }

        public string Description { get; private set; }

        public bool Excludable
        {
            get
            {
                return excludable;
            }
            set
            {
                if (value != excludable)
                {
                    SendPropertyChanging();
                    excludable = value;
                    SendPropertyChanged("Excludable");
                }
            }
        }

        public bool ImReallyPresent { get; set; }
        public bool ImScoreable { get; set; }
        public bool ImTakeableMultipleTimes { get; set; }
        public bool IVeDesideredDuration { get; set; }
        public string MessageToStudent { get; set; }
        public string MessageToTeacher { get; set; }
        public string NameStep { get; set; }
        public int Number { get; private set; }

        public float Score
        {
            get
            {
                return score;
            }
            set
            {
                if (value != score)
                {
                    SendPropertyChanging();
                    score = value;
                    SendPropertyChanged("Score");
                }
            }
        }

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value != selected)
                {
                    SendPropertyChanging();
                    selected = value;
                    SendPropertyChanged("Selected");
                }
            }
        }

        public bool ToBeExcluded { get; set; }

        public Enum_ClinicalCaseStepType Type { get; set; }
        public bool PresentToUser { get; internal set; }

        public virtual void ClearAllUserRelatedFields()
        {
        }

        public virtual void ImportInformationFromTxtFile(string fileName)
        {
        }

        public virtual void ImportMaterial(string folderMaterial)
        {
        }
    }
}