using BeautySim.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

//using NEUROWAVE.Data;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class Visualization3DFrameStudent : Page, INotifyPropertyChanged
    {
        public Visualization3DFrameStudent()
        {
            InitializeComponent();
        }

        public void UpdateTheConsequence()
        {
            if (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex >= 0 && AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex < AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count)
            {
                AnalysResult currentConsequence = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences[AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex];

                string nameFile = AppControl.Instance.CurrentCase.Folder + "\\Consequences\\" + currentConsequence.AnalysisImageName + ".jpg";
                if (File.Exists(nameFile))
                {
                    ImConsequence.Source = new BitmapImage(new Uri(nameFile, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    nameFile = nameFile.Replace(".jpg", ".png");
                    if (File.Exists(nameFile))
                    {
                        ImConsequence.Source = new BitmapImage(new Uri(nameFile, UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        ImConsequence.Source = null;
                    }

                }
                tbNumber.Text = (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex + 1).ToString() + " / " + AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count.ToString();
                tbWhatYouDid.Text = currentConsequence.WhatYouDidDescription;
                tbConsequence.Text = currentConsequence.WhatWillBeTheConsequence;
            }
            if (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex == 0)
            {
                btnBack.Visibility = Visibility.Hidden;
            }
            else
            {
                btnBack.Visibility = Visibility.Visible;
            }
            if (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex == AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count - 1)
            {
                btnNext.Visibility = Visibility.Hidden;
            }
            else
            {
                btnNext.Visibility = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = AppControl.Instance;
        }

        internal void PrepareFor(Enum_StepFace3DInteraction phase)
        {
            switch (phase)
            {
                case Enum_StepFace3DInteraction.LOADING:

                    grInteractive3D.Visibility = Visibility.Visible;
                    grInformative.Visibility = Visibility.Collapsed;
                    if (!grInteractive3D.Children.Contains(hvView3D))
                    {
                        grInformative.Children.Remove(hvView3D);
                        Grid.SetRow(hvView3D, 0);
                        Grid.SetColumn(hvView3D, 0);
                        Grid.SetRowSpan(hvView3D, 3);
                        Grid.SetColumnSpan(hvView3D, 2);
                        grInteractive3D.Children.Add(hvView3D);
                    }
                    tbMessage.Text = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.MessageToStudent;
                    break;

                case Enum_StepFace3DInteraction.OPERATIVE:
                    grInteractive3D.Visibility = Visibility.Visible;
                    grInformative.Visibility = Visibility.Collapsed;
                    if (!grInteractive3D.Children.Contains(hvView3D))
                    {
                        grInformative.Children.Remove(hvView3D);


                        Grid.SetRow(hvView3D, 0);
                        Grid.SetColumn(hvView3D, 0);
                        Grid.SetRowSpan(hvView3D, 3);
                        Grid.SetColumnSpan(hvView3D, 2);
                        grInteractive3D.Children.Add(hvView3D);
                    }
                    tbMessage.Text = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.MessageToStudent;
                    break;

                case Enum_StepFace3DInteraction.FEEDBACK_ON_INJECTIONS_PERFORMED:

                    grInteractive3D.Visibility = Visibility.Visible;
                    grInformative.Visibility = Visibility.Collapsed;
                    tbMessage.Text = AppControl.Instance.MessageCheckingFeedbacks;

                    break;

                case Enum_StepFace3DInteraction.FEEDBACK_INJECTIONPOINTS_EFFECTS:
                    grInteractive3D.Visibility = Visibility.Collapsed;
                    grInformative.Visibility = Visibility.Visible;
                    if (!grInformative.Children.Contains(hvView3D))
                    {
                        grInteractive3D.Children.Remove(hvView3D);
                        Grid.SetRow(hvView3D, 1);
                        Grid.SetColumn(hvView3D, 0);
                        Grid.SetColumnSpan(hvView3D, 1);
                        Grid.SetRowSpan(hvView3D, 1);
                        grInformative.Children.Add(hvView3D);
                    }

                    spFeedbackPage.Visibility = Visibility.Visible;
                    spFinalFeedback.Visibility = Visibility.Collapsed;
                    if (AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count > 1)
                    {
                        btnBack.Visibility = Visibility.Visible;
                        btnNext.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnBack.Visibility = Visibility.Collapsed;
                        btnNext.Visibility = Visibility.Collapsed;
                    }
                    tbMessage.Text = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.MessageToStudentConsequences;
                    break;

                case Enum_StepFace3DInteraction.FINAL_FEEDBACK:

                    spFeedbackPage.Visibility = Visibility.Collapsed;
                    spFinalFeedback.Visibility = Visibility.Visible;

                    spOperativityFeedback.Visibility = Visibility.Visible;
                    spOperativityFeedbackItems.Visibility = Visibility.Visible;

                    tbOperativityFeedbackItems.Text = string.Join("\n", AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.ErrorsDescription);
                    tbOperativityFeedback.Text = "Your score for the operativity phase is " + Convert.ToInt32(AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.OperativityScore).ToString() + " / 100.";
                    tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                    break;

                default:
                    break;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.IncreaseConsequence();

            AppControl.Instance.UpdateTheConsequences3D();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.DecreaseConsequence();

            AppControl.Instance.UpdateTheConsequences3D();
        }
    }
}