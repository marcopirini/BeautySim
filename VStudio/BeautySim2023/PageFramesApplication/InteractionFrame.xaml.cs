using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

//using NEUROWAVE.Data;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class InteractionFrame
        : Page
    {
        public InteractionFrame ReferredTeacher { get; set; }

        public InteractionFrame(bool imTeacher)
        {
            InitializeComponent();

            this.Loaded += ThisPage_Loaded;

            
            
            ImTeacher = imTeacher;

        }

        private void ThisPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        
       
        private bool ImTeacher;


        public List<MultipleChoiceControl> GetChoiceControls()
        {
            List<MultipleChoiceControl> ret = new List<MultipleChoiceControl>();
            foreach (var item in spOperativity.Children)
            {
                if (item is MultipleChoiceControl)
                {
                    ret.Add(item as MultipleChoiceControl);
                }
            }
            return ret;
        }


        internal void SetContent(ClinicalCaseStep_Questionnaire caseStepCurrent)
        {
            CurrentClinicalStep = caseStepCurrent;
            grSimplmeQuestions.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);

            if (ImTeacher)
            {
                grSimplmeQuestions.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Pixel);

                FontSize = 20;
                tbMessage.Text = caseStepCurrent.MessageToTeacher;
            }
            else
            {
                FontSize = 35;
                tbMessage.Text = caseStepCurrent.MessageToStudent;
            }
            tbMessage.FontSize = FontSize;

            for (int i=0; i<caseStepCurrent.Questions.Count; i++)
            {
                MultipleChoiceControl yourControl = new MultipleChoiceControl();
                yourControl.Configure("Vertical");
                yourControl.Answers = caseStepCurrent.Questions[i].Options;
                yourControl.CorrectAnswers = caseStepCurrent.Questions[i].CorrectAnswers;
                yourControl.Question = caseStepCurrent.Questions[i].QuestionText;
                yourControl.FillControls(ImTeacher);
                
                spOperativity.Children.Add(yourControl);

                if (ImTeacher)
                {
                    yourControl.IsEnabled = false;
                    yourControl.FontSizeAnswers = 20;
                    yourControl.FontSizeQuestion = 20;
                }
                else
                {
                    yourControl.FontSizeAnswers = 35;
                    yourControl.FontSizeQuestion = 35;
                    yourControl.SomethingChangedEvent += YourControl_SomethingChangedEvent;
                }
                    
            }
        }


        internal void SetContent(ClinicalCaseStep_DidacticDynamicFace caseStepCurrent)
        {
            CurrentClinicalStep = caseStepCurrent;
            grSimplmeQuestions.ColumnDefinitions[0].Width = new GridLength(.8, GridUnitType.Star);

            if (ImTeacher)
            {
                grSimplmeQuestions.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Pixel);

                FontSize = 20;
                tbMessage.Text = caseStepCurrent.MessageToTeacher;
            }
            else
            {
                FontSize = 35;
                tbMessage.Text = caseStepCurrent.MessageToStudent;
            }
            tbMessage.FontSize = FontSize;

            for (int i = 0; i < caseStepCurrent.Questions.Count; i++)
            {
                MultipleChoiceControl yourControl = new MultipleChoiceControl();
                yourControl.Configure("Vertical");
                yourControl.Answers = caseStepCurrent.Questions[i].Options;
                yourControl.CorrectAnswers = caseStepCurrent.Questions[i].CorrectAnswers;
                yourControl.Question = caseStepCurrent.Questions[i].QuestionText;
                yourControl.FillControls(ImTeacher);

                spOperativity.Children.Add(yourControl);

                if (ImTeacher)
                {
                    yourControl.IsEnabled = false;
                    yourControl.FontSizeAnswers = 20;
                    yourControl.FontSizeQuestion = 20;
                }
                else
                {
                    yourControl.FontSizeAnswers = 35;
                    yourControl.FontSizeQuestion = 35;
                    yourControl.SomethingChangedEvent += YourControl_SomethingChangedEvent;
                }

            }

            imCaseImage.Source = new BitmapImage(new Uri(AppControl.Instance.CurrentCase.Folder + "\\" + caseStepCurrent.ImageName, UriKind.RelativeOrAbsolute));
        }

        public ClinicalCaseStep CurrentClinicalStep { get; private set; }


        internal void SetContent(ClinicalCaseStep_AnalysisStaticFace caseStepCurrent)
        {
            CurrentClinicalStep = caseStepCurrent;
            grSimplmeQuestions.ColumnDefinitions[0].Width = new GridLength(.8, GridUnitType.Star);

            if (ImTeacher)
            {
                grSimplmeQuestions.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Pixel);

                FontSize = 20;
                tbMessage.Text = caseStepCurrent.MessageToTeacher;
            }
            else
            {
                FontSize = 35;
                tbMessage.Text = caseStepCurrent.MessageToStudent;
            }
            tbMessage.FontSize = FontSize;
            
            for (int i = 0; i < caseStepCurrent.Questions.Count; i++)
            {
                MultipleChoiceControl yourControl = new MultipleChoiceControl();
                yourControl.Configure("Horizontal");
                yourControl.Answers = caseStepCurrent.Questions[i].Options;
                yourControl.CorrectAnswers = caseStepCurrent.Questions[i].CorrectAnswers;
                yourControl.Question = caseStepCurrent.Questions[i].QuestionText;
                yourControl.FillControls(ImTeacher);

                spOperativity.Children.Add(yourControl);

                if (ImTeacher)
                {
                    yourControl.IsEnabled = false;
                    yourControl.FontSizeAnswers = 20;
                    yourControl.FontSizeQuestion = 20;
                }
                else
                {
                    yourControl.FontSizeAnswers = 25;
                    yourControl.FontSizeQuestion = 25;
                    yourControl.SomethingChangedEvent += YourControl_SomethingChangedEvent;
                }

            }

            imCaseImage.Source = new BitmapImage(new Uri(AppControl.Instance.CurrentCase.Folder + "\\" + caseStepCurrent.ImageName, UriKind.RelativeOrAbsolute));
        }

        private void YourControl_SomethingChangedEvent(MultipleChoiceControl aa, int index, int state)
        {
            if (ReferredTeacher != null)
            {
                int mcIndex = spOperativity.Children.IndexOf(aa);
                ReferredTeacher.SetSameMultipleChoiceOption(mcIndex, index, state);
            }
        }

        private void SetSameMultipleChoiceOption(int mcIndex, int index, int state)
        {
            MultipleChoiceControl yourControl = spOperativity.Children[mcIndex] as MultipleChoiceControl;
            yourControl.SetSameMultipleChoiceOption(index, state);
        }
    }
}