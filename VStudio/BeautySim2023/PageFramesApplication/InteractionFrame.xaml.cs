using BeautySim.Common;
using HelixToolkit.Wpf.SharpDX.Elements2D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

//using NEUROWAVE.Data;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class InteractionFrame : Page
    {
        private bool firstLoad;
        private bool ImTeacher;
        private List<InjectionPointSpecific2D> injectionPoints2D;
        private InjectionPointSpecific2D selectedInjectionPoint;

        public InteractionFrame(bool imTeacher)
        {
            InitializeComponent();

            this.Loaded += ThisPage_Loaded;

            ImTeacher = imTeacher;
            firstLoad = true;
            timer.Interval = TimeSpan.FromSeconds(.5);
            timer.Tick += timerUpdate;

            ConsequencesToShow = new List<AnalysResult>();
            ErrorsDescription = new List<string>();
            //this.DataContext = this; // Or new YourViewModel();
            AlreadyCheckedAllConsequencies = false;
        }

        private void timerUpdate(object sender, EventArgs e)
        {
            var scrollviewer = FindVisualChild<ScrollViewer>(lvInjectionPoints);
            if (scrollviewer != null)
            {
                scrollviewer.ScrollChanged += Scrollviewer_ScrollChanged;
            }
            timer.Stop();
        }

        public ClinicalCaseStep CurrentClinicalStep { get; private set; }

        public ObservableCollection<InjectionPointSpecific2D> InjectionPoints2D { get; set; }

        public InteractionFrame ReferredTeacher { get; set; }
        public List<AnalysResult> ConsequencesToShow { get; internal set; }
        public int NumErrors { get; private set; }
        public double QuestionnaireScore { get; private set; }

        public double OperativeScore { get; private set; }

        public double FinalScore { get; private set; }
        public List<string> ErrorsDescription { get; private set; }
        public bool AlreadyCheckedAllConsequencies { get; internal set; }

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

            string filePoints = AppControl.Instance.CurrentCase.Folder + "\\" + caseStepCurrent.PointDefinitionFileName;
            if (File.Exists(filePoints))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<InjectionPointSpecific2D>));

                // Deserialize from file
                using (FileStream fileStream = new FileStream(filePoints, FileMode.Open))
                {
                    List<InjectionPointSpecific2D> anjectionPoints2D = (List<InjectionPointSpecific2D>)deserializer.Deserialize(fileStream);
                    InjectionPoints2D = new ObservableCollection<InjectionPointSpecific2D>(anjectionPoints2D);

                    Console.WriteLine("Deserialized List:");
                }
            }

            imCaseImage.Source = new BitmapImage(new Uri(AppControl.Instance.CurrentCase.Folder + "\\" + caseStepCurrent.ImageName, UriKind.RelativeOrAbsolute));
        }

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

        public void DrawInjectionPoints(ObservableCollection<InjectionPointSpecific2D> pointsToDraw, InjectionPointSpecific2D selPoint)
        {
            // Clear existing ellipses from the canvas
            injectionPointsCanvas.Children.Clear();

            // Iterate through the injection points and draw ellipses for each point
            foreach (var point in pointsToDraw)
            {
                var injectionPoint = point as InjectionPointSpecific2D;
                if ((injectionPoint == null) || (injectionPoint.Assigned == false))
                    continue;

                // Create an ellipse to represent the injection point
                Ellipse ellipse = new Ellipse
                {
                    Width = sizeEllipse,
                    Height = sizeEllipse,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };
                if (injectionPoint == selPoint)
                {
                    // Increase the size of the ellipse for the selected point
                    ellipse.Width = 14;
                    ellipse.Height = 14;
                }
                // Set the fill color based on the ToTarget field

                if (injectionPoint.ActuallyChosenOrPerformedQuantity == 0)
                {
                    ellipse.Fill = Brushes.Gray;
                }
                else
                {
                    ellipse.Fill = Brushes.Blue;
                }

                // Calculate the position of the ellipse on the image
                double canvasX = injectionPoint.X * imCaseImage.ActualWidth - ellipse.Width / 2;
                double canvasY = injectionPoint.Y * imCaseImage.ActualHeight - ellipse.Height / 2;

                // Set the position of the ellipse on the canvas
                Canvas.SetLeft(ellipse, canvasX);
                Canvas.SetTop(ellipse, canvasY);

                // Add the ellipse to the canvas
                injectionPointsCanvas.Children.Add(ellipse);
            }
        }


        public void DrawInjectionPointsFeedback(ObservableCollection<InjectionPointSpecific2D> pointsToDraw, AnalysResult result)
        {

            Enum_HighlightType highlight = Enum_HighlightType.NONE;

            switch (result.AnalyisCondition)
            {
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_RIGHT:
                    highlight= Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_LEFT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_RIGHT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_LEFT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_LEFT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_RIGHT:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_UP:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_RIGHT:
                    highlight = Enum_HighlightType.ASIMMETRY;
                    break;
                case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_LEFT:
                    highlight = Enum_HighlightType.ASIMMETRY;
                    break;
                case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT:
                    highlight = Enum_HighlightType.TOOMUCH;
                    break;
                case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT:
                        highlight = Enum_HighlightType.TOOMUCH;
                    break;
                case Enum_AnaysisVariablesToCheck.O_FRONTAL_RIGHT:
                        highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.O_FRONTAL_LEFT:
                    highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_LEFT:
                    highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_RIGHT:
                    highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.O_PROCERUS:
                    highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_RIGHT:
                    highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_LEFT:
                    highlight = Enum_HighlightType.OMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_FRONTAL_RIGHT:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_FRONTAL_LEFT:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_LEFT:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_RIGHT:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_PROCERUS:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_RIGHT:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_LEFT:
                    highlight = Enum_HighlightType.COMISSION;
                    break;
                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_RIGHT_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_LEFT_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT_MAX:
                    highlight = Enum_HighlightType.BADERROR;

                    break;
                case Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT_MAX:
                    highlight = Enum_HighlightType.TOOMUCH;
                    break;
                case Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT_MAX:
                    highlight = Enum_HighlightType.TOOMUCH;
                    break;
                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_FRONTAL:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_CORRUGATORS_LATERAL:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.MB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL:
                    highlight = Enum_HighlightType.TOOMUCH;
                    break;
                case Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_FRONTAL_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_CORRUGATORS_LATERAL_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_MAX:
                    highlight = Enum_HighlightType.BADERROR;
                    break;
                case Enum_AnaysisVariablesToCheck.MHB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_MAX:
                    highlight = Enum_HighlightType.TOOMUCH;
                    break;
                case Enum_AnaysisVariablesToCheck.COR_FRONTAL:
                    highlight = Enum_HighlightType.NONE;
                    break;
                case Enum_AnaysisVariablesToCheck.COR_CENTRAL:
                    highlight = Enum_HighlightType.NONE;
                    break;
                case Enum_AnaysisVariablesToCheck.COR_ORBICULAR_LEFT:
                    highlight = Enum_HighlightType.NONE;
                    break;
                case Enum_AnaysisVariablesToCheck.COR_ORBICULAR_RIGHT:
                    highlight = Enum_HighlightType.NONE;
                    break;
                default:
                    break;
            }

            // Clear existing ellipses from the canvas
            injectionPointsCanvas.Children.Clear();

            // Iterate through the injection points and draw ellipses for each point
            foreach (var point in pointsToDraw)
            {
                var injectionPoint = point as InjectionPointSpecific2D;
                if ((injectionPoint == null) || (injectionPoint.Assigned == false))
                    continue;

                // Create an ellipse to represent the injection point
                Ellipse ellipse = new Ellipse
                {
                    Width = sizeEllipse,
                    Height = sizeEllipse,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };

                switch (highlight)
                {
                    case Enum_HighlightType.NONE:
                        if (injectionPoint.ActuallyChosenOrPerformedQuantity == 0)
                        {
                            ellipse.Fill = Brushes.Gray;
                        }
                        else
                        {
                            ellipse.Fill = Brushes.Green;
                        }
                        break;
                    case Enum_HighlightType.BADERROR:
                        if (injectionPoint.ActuallyChosenOrPerformedQuantity == 0)
                        {
                            ellipse.Fill = Brushes.Gray;
                        }
                        else
                        {
                            if (injectionPoint.IsError)
                            {
                                ellipse.Fill = Brushes.Red;
                            }
                            else
                            {
                                ellipse.Fill = Brushes.Gray;
                            }
                           
                        }
                        break;
                    case Enum_HighlightType.TOOMUCH:
                        if (injectionPoint.ActuallyChosenOrPerformedQuantity == 0)
                        {
                            ellipse.Fill = Brushes.Gray;
                        }
                        else
                        {
                            if (injectionPoint.ActuallyChosenOrPerformedQuantity>injectionPoint.PrescribedQuantity)
                            {
                                ellipse.Fill = Brushes.Orange;
                            }
                            else
                            {
                                ellipse.Fill = Brushes.Gray;
                            }
                        }
                        break;
                    case Enum_HighlightType.OMISSION:
                        if (injectionPoint.ActuallyChosenOrPerformedQuantity == 0)
                        {
                            if (injectionPoint.PrescribedQuantity > 0)
                            { 
                                ellipse.Fill = Brushes.Yellow;
                            }
                            else
                            {
                                ellipse.Fill = Brushes.Gray;
                            }
                        
                        }
                        else
                        {
                            
                                ellipse.Fill = Brushes.Gray;
                            
                        }
                        break;
                    case Enum_HighlightType.ASIMMETRY:
                        if (injectionPoint.AreaDef == Enum_AreaDefinition.FRONTAL)
                        {
                            ellipse.Fill = Brushes.LightGreen;
                        }
                        else
                        {
                            ellipse.Fill = Brushes.Gray;
                        }   
                        break;
                    case Enum_HighlightType.COMISSION:
                        if (injectionPoint.ActuallyChosenOrPerformedQuantity == 0)
                        {
                            ellipse.Fill = Brushes.Gray;
                        }
                        else
                        {
                            if (injectionPoint.PrescribedQuantity ==0)
                            {
                                ellipse.Fill = Brushes.Blue;
                            }
                            else
                            {
                                ellipse.Fill = Brushes.Gray;
                            }
                            
                        }
                        break;
                    default:
                        break;
                }

                // Calculate the position of the ellipse on the image
                double canvasX = injectionPoint.X * imCaseImage.ActualWidth - ellipse.Width / 2;
                double canvasY = injectionPoint.Y * imCaseImage.ActualHeight - ellipse.Height / 2;

                // Set the position of the ellipse on the canvas
                Canvas.SetLeft(ellipse, canvasX);
                Canvas.SetTop(ellipse, canvasY);

                // Add the ellipse to the canvas
                injectionPointsCanvas.Children.Add(ellipse);
            }
        }








        private void FaceImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!ImTeacher)
            {
                if (lvInjectionPoints.Visibility == Visibility.Visible)
                {
                    // Get the position of the mouse click relative to the image
                    Point mousePosition = e.GetPosition(imCaseImage);

                    // Calculate the position of the mouse click relative to the image size

                    // Iterate through the injection points and check if the mouse click is within the ellipse
                    foreach (var point in InjectionPoints2D)
                    {
                        var injectionPoint = point as InjectionPointSpecific2D;
                        if ((injectionPoint == null) || (injectionPoint.Assigned == false))
                            continue;

                        // Calculate the position of the ellipse on the image
                        double canvasX = injectionPoint.X * imCaseImage.ActualWidth - sizeEllipse / 2;
                        double canvasY = injectionPoint.Y * imCaseImage.ActualHeight - sizeEllipse / 2;

                        // Check if the mouse click is within the ellipse
                        if ((mousePosition.X >= canvasX) && (mousePosition.X <= canvasX + sizeEllipse) && (mousePosition.Y >= canvasY) && (mousePosition.Y <= canvasY + sizeEllipse))
                        {
                            // Select the injection point
                            lvInjectionPoints.SelectedItem = injectionPoint;
                            break;
                        }
                    }
                }
            }
        }

        private void faceImage_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (firstLoad)
            {
                if (imCaseImage.Source != null)
                {
                    injectionPointsCanvas.Width = imCaseImage.ActualWidth;
                    injectionPointsCanvas.Height = imCaseImage.ActualHeight;
                    if (!ImTeacher)
                    {
                        if (lvInjectionPoints.Visibility == Visibility.Visible)
                        {
                            //DrawInjectionPoints(InjectionPoints2D, null);
                            //ReferredTeacher.DrawInjectionPoints(InjectionPoints2D, null);
                        }
                    }

                    firstLoad = false;
                }
            }
        }

        private void SetSameMultipleChoiceOption(int mcIndex, int index, int state)
        {
            MultipleChoiceControl yourControl = spOperativity.Children[mcIndex] as MultipleChoiceControl;
            yourControl.SetSameMultipleChoiceOption(index, state);
        }

        private void ThisPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!ImTeacher)
            {
                var scrollviewerTeacher = FindVisualChild<ScrollViewer>(ReferredTeacher.lvInjectionPoints);
                if (scrollviewerTeacher != null)
                {
                    scrollviewerTeacher.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        public T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void YourControl_SomethingChangedEvent(MultipleChoiceControl aa, int index, int state)
        {
            if (ReferredTeacher != null)
            {
                int mcIndex = spOperativity.Children.IndexOf(aa);
                ReferredTeacher.SetSameMultipleChoiceOption(mcIndex, index, state);
            }
        }

        private void lvInjectionPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ImTeacher)
            {
                if (lvInjectionPoints.SelectedItem is InjectionPointSpecific2D selectedPoint)
                {
                    // Now, you can use selectedPoint to access properties of the selected InjectionPointSpecific2D
                    // For example:
                    selectedInjectionPoint = selectedPoint;
                    if (lvInjectionPoints.Visibility == Visibility.Visible)
                    {
                        DrawInjectionPoints(InjectionPoints2D, selectedInjectionPoint);
                        ReferredTeacher.DrawInjectionPoints(InjectionPoints2D, selectedInjectionPoint);
                    }
                    lvInjectionPoints.ScrollIntoView(lvInjectionPoints.SelectedItem);

                    // Do something with the selected point...
                }
            }
        }

        private void lvInjectionPoints_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ImTeacher)
            {
                var comboBox = sender as System.Windows.Controls.ComboBox;
                if (comboBox != null)
                {
                    var selectedItem = comboBox.DataContext;
                    lvInjectionPoints.SelectedItem = selectedItem;
                }
                if (lvInjectionPoints.Visibility == Visibility.Visible)
                {
                    DrawInjectionPoints(InjectionPoints2D, selectedInjectionPoint);
                    ReferredTeacher.DrawInjectionPoints(InjectionPoints2D, selectedInjectionPoint);
                }
            }
        }

        private DispatcherTimer timer = new DispatcherTimer();
        private double sizeEllipse = 8;
        private int selectedConsequenceIndex;

        internal void SetScrollSynch()
        {
            timer.Start();
        }

        internal void PrepareFor(Enum_StepDynamicAnalysis phase)
        {
            switch (phase)
            {
                case Enum_StepDynamicAnalysis.ANSWERING:
                    spOperativity.Visibility = Visibility.Visible;
                    grInjectionPoints.Visibility = Visibility.Collapsed;
                    spFeedbackPage.Visibility = Visibility.Collapsed;
                    spFinalFeedback.Visibility = Visibility.Collapsed;
                    break;

                case Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS:
                    spOperativity.Visibility = Visibility.Collapsed;
                    grInjectionPoints.Visibility = Visibility.Visible;
                    spFeedbackPage.Visibility = Visibility.Collapsed;
                    spFinalFeedback.Visibility = Visibility.Collapsed;
                    break;

                case Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS_EFFECTS:
                    spOperativity.Visibility = Visibility.Collapsed;
                    grInjectionPoints.Visibility = Visibility.Collapsed;
                    spFeedbackPage.Visibility = Visibility.Visible;
                    spFeedbackPage.Height = bImageAndPoints.Height;
                    spFinalFeedback.Visibility = Visibility.Collapsed;
                    if (ConsequencesToShow.Count > 1)
                    {
                        btnBack.Visibility = Visibility.Visible;
                        btnNext.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnBack.Visibility = Visibility.Collapsed;
                        btnNext.Visibility = Visibility.Collapsed;
                    }
                    //if (!ImTeacher)
                    //{
                    //    btnBack.Visibility = Visibility.Collapsed;
                    //    btnNext.Visibility = Visibility.Collapsed;
                    //}
                    break;
                case Enum_StepDynamicAnalysis.FINAL_FEEDBACK_SIMPLE:
                    spOperativity.Visibility = Visibility.Collapsed;
                    grInjectionPoints.Visibility = Visibility.Collapsed;
                    spFeedbackPage.Visibility = Visibility.Collapsed;
                    spFinalFeedback.Visibility = Visibility.Visible;

                    tbFinalFeedback.Text = "You made " + NumErrors + " errors. Your score for this step is " + Convert.ToInt32(QuestionnaireScore).ToString() + " / 100.";
                   
                    break;
                case Enum_StepDynamicAnalysis.FINAL_FEEDBACK:
                    spOperativity.Visibility = Visibility.Collapsed;
                    grInjectionPoints.Visibility = Visibility.Collapsed;
                    spFeedbackPage.Visibility = Visibility.Collapsed;
                    spFinalFeedback.Visibility = Visibility.Visible;
                    spQuestionnaireFeedback.Visibility = Visibility.Visible;
                    spOperativityFeedback.Visibility = Visibility.Visible;
                    spOperativityFeedbackItems.Visibility=Visibility.Visible;
                    grIndications.Visibility = Visibility.Hidden;


                    tbQuestionnaireFeedback.Text = "You made " + NumErrors + " errors in the questionnaire. Your questionnaire score is " + Convert.ToInt32(QuestionnaireScore).ToString() + " / 100. Weight: 25%.";

                    tbOperativityFeedbackItems.Text = string.Join("\n", ErrorsDescription);
                    tbOperativityFeedback.Text = "Your score for the operativity phase is " + Convert.ToInt32(OperativeScore).ToString() + " / 100. Weight: 75%.";

                    tbFinalFeedback.Text = "Your score for this step is " + Convert.ToInt32(FinalScore).ToString() + " / 100.";

                    break;

                default:
                    break;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            selectedConsequenceIndex = selectedConsequenceIndex - 1;
            if (selectedConsequenceIndex < 0)
            {
                selectedConsequenceIndex = 0;
            }
            AppControl.Instance.UpdateTheConsequences(selectedConsequenceIndex);
            //UpdateTheConsequence();
            //Updatetheconsequence
        }

        private AnalysResult currentConsequence;

        public void UpdateTheConsequence(int index)
        {
            if (index >= 0 && index < ConsequencesToShow.Count)
            {
                selectedConsequenceIndex = index;
                currentConsequence = ConsequencesToShow[selectedConsequenceIndex];

                string nameFile = AppControl.Instance.CurrentCase.Folder + "\\Consequences\\" + currentConsequence.AnalysisImageName + ".jpg";
                if (File.Exists(nameFile))
                {
                    ImConsequence.Source = new BitmapImage(new Uri(nameFile, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImConsequence.Source = null;
                }
                tbNumber.Text = (selectedConsequenceIndex + 1).ToString() + " / " + ConsequencesToShow.Count.ToString();
                tbWhatYouDid.Text = currentConsequence.WhatYouDidDescription;
                tbConsequence.Text = currentConsequence.WhatWillBeTheConsequence;
            }
            if (index==0)
            {
                btnBack.Visibility = Visibility.Hidden;
            }
            else
            {
                btnBack.Visibility = Visibility.Visible;    
            }
            if (index == ConsequencesToShow.Count-1)
            {
                btnNext.Visibility = Visibility.Hidden;
            }
            else
            {
                btnNext.Visibility = Visibility.Visible;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            selectedConsequenceIndex = selectedConsequenceIndex + 1;
            if (selectedConsequenceIndex >= ConsequencesToShow.Count)
            {
                selectedConsequenceIndex = ConsequencesToShow.Count - 1;
            }
            AppControl.Instance.UpdateTheConsequences(selectedConsequenceIndex);
            //UpdateTheConsequence();
            //updatetheconsequence
        }

        internal void HidePoints()
        {
            injectionPointsCanvas.Children.Clear();
        }

        internal void DrawInjectionPointsSpecial(ObservableCollection<InjectionPointSpecific2D> pointsToDraw)
        {
            injectionPointsCanvas.Children.Clear();

            // Iterate through the injection points and draw ellipses for each point
            foreach (var point in pointsToDraw)
            {
                var ijP = point as InjectionPointSpecific2D;
                if ((ijP == null) || (ijP.Assigned == false))
                    continue;

                // Create an ellipse to represent the injection point
                Ellipse ellipse = new Ellipse
                {
                    Width = sizeEllipse,
                    Height = sizeEllipse,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };

                // Set the fill color based on the ToTarget field

                if (ijP.IsError && ijP.ActuallyChosenOrPerformedQuantity > 0)
                {
                    ellipse.Fill = Brushes.Red;

                }
                else
                {
                    if (ijP.ActuallyChosenOrPerformedQuantity > ijP.PrescribedQuantity)
                    {
                        ellipse.Fill = Brushes.Orange;
                    }
                    else if (ijP.ActuallyChosenOrPerformedQuantity < ijP.PrescribedQuantity)
                    {
                        ellipse.Fill =  Brushes.Yellow;
                    }
                    else
                    {
                        ellipse.Fill = Brushes.Green;
                        
                    }
                }

                // Calculate the position of the ellipse on the image
                double canvasX = ijP.X * imCaseImage.ActualWidth - ellipse.Width / 2;
                double canvasY = ijP.Y * imCaseImage.ActualHeight - ellipse.Height / 2;

                // Set the position of the ellipse on the canvas
                Canvas.SetLeft(ellipse, canvasX);
                Canvas.SetTop(ellipse, canvasY);

                // Add the ellipse to the canvas
                injectionPointsCanvas.Children.Add(ellipse);
            }
            grIndications.Visibility = Visibility.Visible;
        }

        internal void PassScoresQuestionnaire(int numErrors, double questionnaireScore)
        {
           NumErrors = numErrors;
           QuestionnaireScore= questionnaireScore;
        }

        internal void PassInfoOperativity(List<AnalysResult> consequences, List<string> errors, double operativityScore)
        {
            foreach (AnalysResult consequence in consequences)
            {
                ConsequencesToShow.Add(consequence);
            }
            foreach (string error in errors)
            {
                ErrorsDescription.Add(error);
            }

            OperativeScore = operativityScore;


        }

        internal void PassFinalScore(double finalScore)
        {

            FinalScore = finalScore;
        }
    }
}