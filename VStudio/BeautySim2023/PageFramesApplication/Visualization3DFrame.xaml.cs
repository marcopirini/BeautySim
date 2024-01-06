using BeautySim.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class Visualization3DFrame : Page, INotifyPropertyChanged
    {

        public Visualization3DFrame()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += TimerTick;

            this.Loaded += ThisPage_Loaded;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timer.Stop();
            if (resizeFeature == "PairImage")
            {
                bView3D.Height = bConsequence.ActualHeight;
            }
            if (resizeFeature == "PairGrid")
            {
                bView3D.Height = grInteractive3D.ActualHeight;
                grInteractive3D.Children.Remove(spControls);
                grInteractive3D.Children.Add(spControls);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasBeen3DPointsLoaded { get; private set; }

        public Visibility VisibilityAntenna { private set; get; } = Visibility.Hidden;

        public HitTestResultBehavior ResultCallbackNeedle(System.Windows.Media.HitTestResult result)
        {
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
                if (rayMeshResult != null)
                {
                }
            }
            else
            {
            }

            return HitTestResultBehavior.Continue;
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

        DispatcherTimer timer = new DispatcherTimer();
        private string resizeFeature;

        internal void PrepareFor(Enum_StepFace3DInteraction phase)
        {
            switch (phase)
            {
                case Enum_StepFace3DInteraction.LOADING:

                    grInteractive3D.Visibility = Visibility.Visible;
                    grInformative.Visibility = Visibility.Collapsed;
                    if (!grInteractive3D.Children.Contains(bView3D))
                    {
                        grInformative.Children.Remove(bView3D);
                        Grid.SetRow(bView3D, 0);
                        Grid.SetColumn(bView3D, 0);
                        Grid.SetRowSpan(bView3D, 3);
                        Grid.SetColumnSpan(bView3D, 2);
                        grInteractive3D.Children.Add(bView3D);
  
                    }
                    resizeFeature = "PairGrid";
                    timer.Start();
                    grIndicationsLegend.Visibility = Visibility.Hidden;
                    tbMessage.Text = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.MessageToTeacher;
                    break;

                case Enum_StepFace3DInteraction.OPERATIVE:
                    grInteractive3D.Visibility = Visibility.Visible;
                    grInformative.Visibility = Visibility.Collapsed;
                    if (!grInteractive3D.Children.Contains(bView3D))
                    {
                        grInformative.Children.Remove(bView3D);
                    

                        Grid.SetRow(bView3D, 0);
                        Grid.SetColumn(bView3D, 0);
                        Grid.SetRowSpan(bView3D, 3);
                        Grid.SetColumnSpan(bView3D, 2);
                        grInteractive3D.Children.Add(bView3D);

                    }
                    resizeFeature = "PairGrid";
                    timer.Start();
                    grIndicationsLegend.Visibility = Visibility.Visible;
                    tbMessage.Text = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.MessageToTeacher;
                    break;

                case Enum_StepFace3DInteraction.FEEDBACK_ON_INJECTIONS_PERFORMED:

                    grInteractive3D.Visibility = Visibility.Visible;
                    grInformative.Visibility = Visibility.Collapsed;
                    tbMessage.Text = AppControl.Instance.MessageCheckingFeedbacksTeacher;

                    break;

                case Enum_StepFace3DInteraction.FEEDBACK_INJECTIONPOINTS_EFFECTS:
                    grInteractive3D.Visibility = Visibility.Collapsed;
                    grInformative.Visibility = Visibility.Visible;
                    if (!grInformative.Children.Contains(bView3D))
                    {
                        grInteractive3D.Children.Remove(bView3D);
                        Grid.SetRow(bView3D, 1);
                        Grid.SetColumn(bView3D, 0);
                        Grid.SetColumnSpan(bView3D, 1);
                        Grid.SetRowSpan(bView3D, 1);
                        grInformative.Children.Add(bView3D);
                    }

                    
                    resizeFeature = "PairImage";
                    timer.Start();
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
                    tbMessage.Text = AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.MessageToTeacherConsequences;
                    break;
                //if (!ImTeacher)
                //{
                //    btnBack.Visibility = Visibility.Collapsed;
                //    btnNext.Visibility = Visibility.Collapsed;
                //}

                case Enum_StepFace3DInteraction.FINAL_FEEDBACK:

                    spFeedbackPage.Visibility = Visibility.Collapsed;
                    spFinalFeedback.Visibility = Visibility.Visible;

                    spOperativityFeedback.Visibility = Visibility.Visible;
                    spOperativityFeedbackItems.Visibility = Visibility.Visible;

                    //tbQuestionnaireFeedback.Text = "You made " + NumErrors + " errors in the questionnaire. Your questionnaire score is " + Convert.ToInt32(QuestionnaireScore).ToString() + " / 100. Weight: 25%.";

                    tbOperativityFeedbackItems.Text = string.Join("\n", AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.ErrorsDescription);
                    tbOperativityFeedback.Text = "Your score for the operativity phase is " + Convert.ToInt32(AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.OperativityScore).ToString() + " / 100.";

                    //tbFinalFeedback.Text = "Your score for this step is " + Convert.ToInt32(FinalScore).ToString() + " / 100.";
                    tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                    break;

                default:
                    break;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.DecreaseConsequence();

            AppControl.Instance.UpdateTheConsequences3D();
            //UpdateTheConsequence();
            //Updatetheconsequence
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction.IncreaseConsequence();

            AppControl.Instance.UpdateTheConsequences3D();
            //UpdateTheConsequence();
            //updatetheconsequence
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cbSomething_Changed(object sender, RoutedEventArgs e)
        {
            if (AppControl.Instance.HasBeenLoadedVis3DFrame)
            {
                AppControl.Instance.UpdateVisibilityItems(false, sender);
            }
        }

        private void entranceListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (entranceListView.Items.Count > 0)
            {
                entranceListView.ScrollIntoView(entranceListView.Items[entranceListView.Items.Count - 1]);
            }
        }

        private void InjectionPointListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppControl.Instance.ChangeSelectionPointListView();
        }

        private void ThisPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.InitViewModel();
            DataContext = AppControl.Instance;
            AppControl.Instance.HasBeenLoadedVis3DFrame = true;
            AppControl.Instance.UpdateVisibilityItems(true);
            InjectionPointListView.ItemsSource = AppControl.Instance.InjectionPoints3DThisStep;
        }
        private void ZeroVolume_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.ZeroVolume();
        }
    }
}