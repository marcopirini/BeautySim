using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BeautySim2023
{
    public class ClinicalCase : XMLBase, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<ClinicalCaseStep> Steps;

        private bool selected;

        public ClinicalCase(string folder)
        {
            Folder = folder;
            Steps = new ObservableCollection<ClinicalCaseStep>();
        }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string Folder { get; private set; }

        [XmlAttribute]
        public bool Initialized { get; set; } = false;

        [XmlAttribute]
        public Enum_Modules Module { get; set; }

        [XmlAttribute]
        public int CaseLevel { get; private set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ImageName { get; set; }

        [XmlAttribute]
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

        public void Clear()
        {
            Steps.Clear();
        }

        public bool LoadCaseDescriptor()
        {
            bool toRet = false;
            string descriptorFileName = Folder + "\\CaseDescriptor.txt";

            if (File.Exists(descriptorFileName))
            {
                List<string> linesRead = new List<string>();

                if (BeautySim.Globalization.Language.Culture == null)
                {
                    BeautySim.Globalization.Language.Culture = new CultureInfo("en-US");
                }

                Dictionary<string, string> translationDictionary = BeautySim.Globalization.Language.ResourceManager.GetResourceSet(BeautySim.Globalization.Language.Culture, true, true).Cast<DictionaryEntry>().ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());

                using (StreamReader reader = new StreamReader(descriptorFileName))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        else
                        {
                            linesRead.Add(line);
                        }
                        Console.WriteLine(line); // Use line.
                    }
                }
                if (linesRead.Count > 5)
                {
                    Name = linesRead[0].Replace("Name=", "");
                    Description = linesRead[1].Replace("Description=", "");
                    Module = (Enum_Modules)Enum.Parse(typeof(Enum_Modules), linesRead[2].Replace("Module=", ""));
                    CaseLevel = Convert.ToInt32(linesRead[3].Replace("Level=", ""));
                    ImageName = linesRead[4].Replace("ImageName=", "");
                    for (int i = 5; i < linesRead.Count; i++)
                    {
                        if (linesRead[i].StartsWith("Phase"))
                        {
                            string[] parsed = linesRead[i].Split('\t');
                            if (parsed.Count() == 3)
                            {
                                ClinicalCaseStep toAdd = new ClinicalCaseStep();
                                Enum_ClinicalCaseStepType typeCl = (Enum_ClinicalCaseStepType)Enum.Parse(typeof(Enum_ClinicalCaseStepType), parsed[1]);

                                switch (typeCl)
                                {
                                    case Enum_ClinicalCaseStepType.MESSAGE:
                                        toAdd = new ClinicalCaseStep_Message();
                                        toAdd.ImportInformationFromTxtFile(Folder + "\\" + parsed[2] + ".txt");

                                        break;

                                    case Enum_ClinicalCaseStepType.QUESTIONNAIRE:
                                        toAdd = new ClinicalCaseStep_Questionnaire();
                                        toAdd.ImportInformationFromTxtFile(Folder + "\\" + parsed[2] + ".txt");

                                        break;

                                    case Enum_ClinicalCaseStepType.ANALYSIS_STATIC_FACE:
                                        toAdd = new ClinicalCaseStep_AnalysisStaticFace();
                                        toAdd.ImportInformationFromTxtFile(Folder + "\\" + parsed[2] + ".txt");

                                        break;

                                    case Enum_ClinicalCaseStepType.DIDACTIC_DYNAMIC_FACE:

                                        toAdd = new ClinicalCaseStep_DidacticDynamicFace();
                                        toAdd.ImportInformationFromTxtFile(Folder + "\\" + parsed[2] + ".txt");

                                        break;

                                    case Enum_ClinicalCaseStepType.FACE3D_INTERACTION:
                                        toAdd = new ClinicalCaseStep_Face3DInteraction();
                                        toAdd.ImportInformationFromTxtFile(Folder + "\\" + parsed[2] + ".txt");

                                        break;

                                    default:
                                        break;
                                }
                                Steps.Add(toAdd);
                            }
                        }
                        Initialized = true;
                        toRet = true;
                    }
                    EvaluateStepsToAddOrNot(); 
                }
            }
            else
            {
                toRet = false;
            }
            return toRet;
        }

        public void EvaluateStepsToAddOrNot()
        {
            foreach (ClinicalCaseStep item in Steps)
            {
                if ((item is ClinicalCaseStep_Message) && (Properties.Settings.Default.ViewQuestionnairePhases))
                {
                    item.PresentToUser = true;
                }
                else if ((item is ClinicalCaseStep_Questionnaire) && (Properties.Settings.Default.ViewQuestionnairePhases))
                {
                    item.PresentToUser = true;
                }
                else if ((item is ClinicalCaseStep_AnalysisStaticFace) && (Properties.Settings.Default.ViewImagePhases))
                {
                    item.PresentToUser = true;
                }
                else if ((item is ClinicalCaseStep_DidacticDynamicFace) && (Properties.Settings.Default.ViewImagePhases))
                {
                    item.PresentToUser = true;
                }
                else if ((item is ClinicalCaseStep_Face3DInteraction) && (Properties.Settings.Default.View3DPhases))
                {
                    item.PresentToUser = true;
                }
                else
                {
                    item.PresentToUser = false;
                }

               
            }
        }

        [XmlAttribute]
        public float GlobalScore { get; set; }

        public int CurrentStepIndex { get; internal set; }

        internal void CalculateGlobalScore()
        {
            //OLD ONES
            //float scoreTemp = 0;
            //float numPoints = 0;
            //for (int i = 0; i < Steps.Count; i++)
            //{
            //    if (Steps[i].ImScoreable)
            //    {
            //        scoreTemp = scoreTemp + Steps[i].Score * Steps[i].Points;
            //        numPoints = numPoints + Steps[i].Points;
            //    }
            //}

            //if (numPoints > 0)
            //{
            //    GlobalScore = scoreTemp / (numPoints);
            //}
            //else
            //{
            //    GlobalScore = 0;
            //}

            float scoreTemp = 0;
            float numScorableSteps = 0;
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].ImScoreable)
                {
                    scoreTemp = scoreTemp + Steps[i].Score;
                    numScorableSteps++;
                }
            }

            if (numScorableSteps > 0)
            {
                GlobalScore = scoreTemp / (numScorableSteps);
            }
            else
            {
                GlobalScore = 0;
            }
        }

        protected string TranslateCaseDescriptorString(Dictionary<string, string> translationDictionary, string stringToTranslate)
        {
            string translatedString = stringToTranslate;

            if (translationDictionary != null && translationDictionary.ContainsKey(stringToTranslate))
            {
                translatedString = translationDictionary[stringToTranslate];
            }

            return translatedString;
        }

        internal void ClearAllUserRelatedFields()
        {
            foreach (ClinicalCaseStep item in Steps)
            {
                item.ClearAllUserRelatedFields();
            }
        }
    }
}