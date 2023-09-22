using BeautySim.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InjectionPointManager
{
    public partial class MainWindow : Window
    {
        public List<double> DepthOptions = new List<double>() { 1, 2, 4, 6, 8 };
        public List<double> QuantityOptions = new List<double>() { 1, 2, 4, 6, 8 };
        private const double DefaultDepthCorrect = 1.0;
        private const double DefaultPitchMax = 45.0;
        private const double DefaultPitchMin = 0.0;
        private const double DefaultQuantityCorrect = 1.0;
        private const double DefaultYawMax = 20.0;
        private const double DefaultYawMin = -20.0;
        private int currentPointNumber;
        private DynamicInfo dynamicInfo;
        private List<ErrorCase> errorCases;
        private List<InjectionPoint> injectionPoints;

        private bool firstLoad;
        private string imageFileName;
        //private string injectionPointsFileName;
        private string dynamicInfoFileNameXml;
        private InjectionPoint selectedInjectionPoint;
        private ErrorCase selectedErrorCase;
        private ErrorCondition selectedErrorCondition;
        private int currentErrorNumber;

        public MainWindow()
        {
            InitializeComponent();

            cmbDepthCorrect.ItemsSource = DepthOptions;
            cmbQuantityCorrect.ItemsSource = QuantityOptions;
            errorCases = new List<ErrorCase>();
            injectionPoints = new List<InjectionPoint>();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                firstLoad = true;
                string imagePath = openFileDialog.FileName;
                LoadImage(imagePath);
                LoadDynamicInfoXML();
            }
        }

        private void Canvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;

                // Get the mouse position relative to the canvas
                Point mousePosition = e.GetPosition(injectionPointsCanvas);

                // Zoom the canvas
                var transform = injectionPointsCanvas.LayoutTransform as ScaleTransform;
                if (transform == null)
                {
                    transform = new ScaleTransform(1, 1);
                    injectionPointsCanvas.LayoutTransform = transform;
                }

                double zoomFactor = e.Delta > 0 ? 1.2 : 0.8;
                transform.ScaleX *= zoomFactor;
                transform.ScaleY *= zoomFactor;

                //// Apply the same zoom to the image
                //var imageTransform = faceImage.LayoutTransform as ScaleTransform;
                //if (imageTransform == null)
                //{
                //    imageTransform = new ScaleTransform(1, 1);
                //    faceImage.LayoutTransform = imageTransform;
                //}

                //imageTransform.ScaleX = transform.ScaleX;
                //imageTransform.ScaleY = transform.ScaleY;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Delete the selected injection point from the list
            
            if (selectedInjectionPoint != null)
            {
                injectionPoints.Remove(selectedInjectionPoint);
                // Clear the property editor controls or update with default values
                txtName.Text = string.Empty;
                // Clear other property controls accordingly
                for (int i = 0; i < injectionPoints.Count; i++)
                {
                    InjectionPoint point = injectionPoints[i] as InjectionPoint;
                    point.PointNumber = i;
                }
                RefreshInjectionPointsListView();
                SaveDynamicInfoXML();
            }
        }

        private void DeleteErrorCaseButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (selectedErrorCase != null)
            {
                errorCases.Remove(selectedErrorCase);

                // Clear other property controls accordingly
                for (int i = 0; i < errorCases.Count; i++)
                {
                    ErrorCase error = errorCases[i] as ErrorCase;
                    error.ErrorNumber = i;
                }

                RefreshErrorCaseListView();
                RefreshErrorConditionListView();
                SaveDynamicInfoXML();
            }
        }

        private void DeleteErrorConditionButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedErrorCase != null && selectedErrorCondition != null)
            {
                selectedErrorCase.ErrorConditions.Remove(selectedErrorCondition);
                RefreshErrorConditionListView();
                SaveDynamicInfoXML();
            }
        }

        private void DrawInjectionPoints()
        {
            // Clear existing ellipses from the canvas
            injectionPointsCanvas.Children.Clear();

            // Iterate through the injection points and draw ellipses for each point
            foreach (var point in injectionPoints)
            {
                var injectionPoint = point as InjectionPoint;
                if (injectionPoint == null)
                    continue;

                // Create an ellipse to represent the injection point
                Ellipse ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    StrokeThickness = 2,
                    Stroke = Brushes.Black
                };
                if (injectionPoint == selectedInjectionPoint)
                {
                    // Increase the size of the ellipse for the selected point
                    ellipse.Width = 15;
                    ellipse.Height = 15;
                }
                // Set the fill color based on the ToTarget field
                if (injectionPoint.ToTarget)
                    ellipse.Fill = Brushes.Green;
                else
                    ellipse.Fill = Brushes.Red;

                // Calculate the position of the ellipse on the image
                double canvasX = injectionPoint.X * faceImage.ActualWidth - ellipse.Width / 2;
                double canvasY = injectionPoint.Y * faceImage.ActualHeight - ellipse.Height / 2;

                // Set the position of the ellipse on the canvas
                Canvas.SetLeft(ellipse, canvasX);
                Canvas.SetTop(ellipse, canvasY);

                // Add the ellipse to the canvas
                injectionPointsCanvas.Children.Add(ellipse);
            }
        }

        private void ErrorCaseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedErrorCase = (ErrorCase)errorCaseListView.SelectedItem;

            if (selectedErrorCase != null)
                errorConditionListView.ItemsSource = selectedErrorCase.ErrorConditions;
            else
                errorConditionListView.ItemsSource = null;

            UpdateUIErrors();
        }

        private void ErrorConditionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedErrorCondition = (ErrorCondition)errorConditionListView.SelectedItem;
            UpdateUIErrors();
        }

        private void FaceImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked position relative to the image's size
            Point clickPoint = e.GetPosition(faceImage);
            double relativeX = clickPoint.X / faceImage.ActualWidth;
            double relativeY = clickPoint.Y / faceImage.ActualHeight;

            //Point clickPoint = e.GetPosition(injectionPointsCanvas);
            //double relativeX = clickPoint.X / injectionPointsCanvas.ActualWidth;
            //double relativeY = clickPoint.Y / injectionPointsCanvas.ActualHeight;

            
            if (false) //(selectedInjectionPoint != null)
            {
                selectedInjectionPoint.X = relativeX;
                selectedInjectionPoint.Y = relativeY;
                selectedInjectionPoint.Coordinates = $"{relativeX.ToString(new CultureInfo("en-US"))},{relativeY.ToString(new CultureInfo("en-US"))}";

                SaveDynamicInfoXML();
                DrawInjectionPoints();

                injectionPointListView.SelectedItem = selectedInjectionPoint;
            }
            else

            {
                // Create a new injection point with the relative coordinates
                InjectionPoint newPoint = new InjectionPoint
                {
                    PointNumber = currentPointNumber,
                    Name = $"Point{injectionPoints.Count + 1}",
                    ToTarget = true,
                    X = relativeX,
                    Y = relativeY,
                    Coordinates = relativeX.ToString(new CultureInfo("en-US")) + "/" + relativeY.ToString(new CultureInfo("en-US")),
                    ChosenDepth = DefaultDepthCorrect,
                    ChosenQuantity = DefaultQuantityCorrect,
                    PitchMin = DefaultPitchMin,
                    PitchMax = DefaultPitchMax,
                    YawMin = DefaultYawMin,
                    YawMax = DefaultYawMax,
                    Explanation = "",

                    DepthOptionsAsString = string.Join("/", DepthOptions),
                    QuantityOptionsAsString = string.Join("/", QuantityOptions)
                };
                currentPointNumber++; // Increment the current point number for the next point
                                      // Add the new injection point to the list
                injectionPoints.Add(newPoint);
                RefreshInjectionPointsListView();
                SaveDynamicInfoXML();

                injectionPointListView.SelectedItem = newPoint;
            }
        }

        private void faceImage_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (firstLoad)
            {
                if (faceImage.Source != null)
                {
                    injectionPointsCanvas.Width = faceImage.ActualWidth;
                    injectionPointsCanvas.Height = faceImage.ActualHeight;
                    DrawInjectionPoints();
                    firstLoad = false;
                }
            }
        }

        private void InjectionPointList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            // Retrieve the selected injection point
            selectedInjectionPoint = injectionPointListView.SelectedItem as InjectionPoint;
            if (selectedInjectionPoint != null)
            {
                // Update the property editor controls with the selected injection point's parameters
                txtName.Text = selectedInjectionPoint.Name;
                chkValid.IsChecked = selectedInjectionPoint.ToTarget;
                cmbDepthCorrect.SelectedItem = selectedInjectionPoint.ChosenDepth;
                cmbQuantityCorrect.SelectedItem = selectedInjectionPoint.ChosenQuantity;
                txtPitchMin.Text = selectedInjectionPoint.ToString();
                txtPitchMax.Text = selectedInjectionPoint.PitchMax.ToString();
                txtYawMin.Text = selectedInjectionPoint.YawMin.ToString();
                txtYawMax.Text = selectedInjectionPoint.YawMax.ToString();
                txtAdditionalDescription.Text = selectedInjectionPoint.Explanation;

                selectedPointIndicator.Text = selectedInjectionPoint.Name;
                bSaveSpecial.IsEnabled = true;
                bDelete.IsEnabled = true;



                // Update other property controls accordingly
            }
            else
            {
                selectedPointIndicator.Text = "NONE SELECTED";
                bSaveSpecial.IsEnabled = false;
                bDelete.IsEnabled = false;
            }
            DrawInjectionPoints();
        }

        private void LoadDynamicInfoXML()
        {
            injectionPoints.Clear();
            errorCases.Clear();
            if (File.Exists(dynamicInfoFileNameXml))
            {
                dynamicInfo = DynamicInfo.Load<DynamicInfo>(dynamicInfoFileNameXml);

                // Clear existing items and load the injection points list

                foreach (InjectionPoint ip in dynamicInfo.InjectionPoints)
                {
                    injectionPoints.Add(ip);
                }
                if (dynamicInfo.InjectionPoints.Count > 0)
                {
                    int highestPointNumber = dynamicInfo.InjectionPoints.Max(point => point.PointNumber);
                    currentPointNumber = highestPointNumber + 1;
                }
                else
                {
                    currentPointNumber = 0;
                }


                foreach (ErrorCase ip in dynamicInfo.ErrorCases)
                {
                    errorCases.Add(ip);
                }
                if (dynamicInfo.ErrorCases.Count > 0)
                {
                    int highestErrorNumber = dynamicInfo.ErrorCases.Max(point => point.ErrorNumber);
                    currentErrorNumber = highestErrorNumber + 1;
                }
                else
                {
                    currentErrorNumber = 0;
                }
            }
            else
            {
                // If the injection points file doesn't exist, clear the list

                currentPointNumber = 0;
                currentErrorNumber = 0;
            }
            RefreshInjectionPointsListView();
            RefreshErrorCaseListView();
            DrawInjectionPoints();
        }

        private void LoadImage(string imagePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imagePath);
            image.EndInit();
            faceImage.Source = image;

            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = image;

            //// Set the ImageBrush as the background of the canvas
            //injectionPointsCanvas.Background = imageBrush;

            imageFileName = System.IO.Path.GetFileName(imagePath);
            //injectionPointsFileName = System.IO.Path.Combine(
            //    System.IO.Path.GetDirectoryName(imagePath),
            //    System.IO.Path.GetFileNameWithoutExtension(imagePath) + "_injPoints.txt");
            dynamicInfoFileNameXml = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imagePath), System.IO.Path.GetFileNameWithoutExtension(imagePath) + "_injPoints.xml");
        }

        //private void LoadInjectionPoints()
        //{
        //    if (File.Exists(injectionPointsFileName))
        //    {
        //        string[] lines = File.ReadAllLines(injectionPointsFileName);
        //        List<InjectionPoint> injectionPoints = new List<InjectionPoint>();

        //        foreach (string line in lines)
        //        {
        //            string[] parts = line.Split('\t');
        //            if (parts[0] == "INJPOINT")
        //            {
        //                if (parts.Length >= 12)
        //                {
        //                    InjectionPoint point = new InjectionPoint
        //                    {
        //                        PointNumber = int.Parse(parts[1]),
        //                        ToTarget = parts[2] == "ToTarget:Y",
        //                        Name = parts[3].Split(':')[1],
        //                        Coordinates = parts[4].Split(':')[1],
        //                        DepthOptions = ParseDoubleList(parts[5].Split(':')[1]),
        //                        ChosenDepth = double.Parse(parts[6].Split(':')[1]),
        //                        QuantityOptions = ParseDoubleList(parts[7].Split(':')[1]),
        //                        ChosenQuantity = double.Parse(parts[8].Split(':')[1]),
        //                        YawMin = double.Parse(parts[9].Split(':')[1].Split('/')[0]),
        //                        YawMax = double.Parse(parts[9].Split(':')[1].Split('/')[1]),
        //                        PitchMin = double.Parse(parts[10].Split(':')[1].Split('/')[0]),
        //                        PitchMax = double.Parse(parts[10].Split(':')[1].Split('/')[1]),
        //                        Explanation = parts[11].Split(':')[1],
        //                        X = double.Parse(parts[4].Split(':')[1].Split('/')[0], CultureInfo.GetCultureInfo("en-US")),
        //                        Y = double.Parse(parts[4].Split(':')[1].Split('/')[1], CultureInfo.GetCultureInfo("en-US"))
        //                    };

        //                    point.DepthOptionsAsString = string.Join("/", point.DepthOptions);
        //                    point.QuantityOptionsAsString = string.Join("/", point.QuantityOptions);

        //                    injectionPoints.Add(point);
        //                }
        //            }
        //            if (parts[0] == "ERRORCASE")
        //            {
        //                if (parts.Length >= 12)
        //                {
        //                    ErrorCase error = new ErrorCase
        //                    {
        //                        ErrorNumber = int.Parse(parts[1]),
        //                        ErrorDescription = parts[4],
        //                        ErrorImageName = parts[5],
        //                        //InjectionPointsReferenced = parts[2].Split('/').ToList()
        //                    };

        //                    List<string> conditions = parts[3].Split('/').ToList();

        //                    foreach (string condition in conditions)
        //                    {
        //                        ErrorCondition c = ErrorCondition.Parse(condition);
        //                        error.ErrorConditions.Add(c);
        //                    }
        //                }
        //            }

        //            // Clear existing items and load the injection points list
        //            injectionPointList.Items.Clear();
        //            foreach (var point in injectionPoints)
        //            {
        //                injectionPointList.Items.Add(point);
        //            }
        //            if (injectionPoints.Count > 0)
        //            {
        //                int highestPointNumber = injectionPoints.Max(point => point.PointNumber);
        //                currentPointNumber = highestPointNumber + 1;
        //            }
        //            else
        //            {
        //                currentPointNumber = 0;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // If the injection points file doesn't exist, clear the list
        //        injectionPointList.Items.Clear();
        //        currentPointNumber = 0;
        //    }
        //    RefreshInjectionPointsList();
        //    DrawInjectionPoints();
        //}
        private void NewErrorCaseButton_Click(object sender, RoutedEventArgs e)
        {
            selectedErrorCase = new ErrorCase();
            selectedErrorCase.ErrorNumber = currentErrorNumber;
            errorCases.Add(selectedErrorCase);
            errorCaseListView.SelectedItem = selectedErrorCase;
            RefreshErrorCaseListView();
            RefreshErrorConditionListView();
            UpdateUIErrors();
        }

        private void NewErrorConditionButton_Click(object sender, RoutedEventArgs e)
        {
            selectedErrorCondition = new ErrorCondition();
            selectedErrorCase?.ErrorConditions.Add(selectedErrorCondition);
            errorConditionListView.SelectedItem = selectedErrorCondition;
            RefreshErrorConditionListView();
            UpdateUIErrors();
        }

        private List<double> ParseDoubleList(string input)
        {
            List<double> result = new List<double>();
            string[] parts = input.Split('/');
            foreach (string part in parts)
            {
                result.Add(double.Parse(part));
            }
            return result;
        }

        private void RefreshErrorCaseListView()
        {
            errorCaseListView.ItemsSource = null;
            errorCaseListView.ItemsSource = errorCases;
        }

        private void RefreshInjectionPointsListView()
        {
            injectionPointListView.ItemsSource = null;
            injectionPointListView.ItemsSource = injectionPoints;
        }

        private void RefreshErrorConditionListView()
        {
            errorConditionListView.ItemsSource = null;
            errorConditionListView.ItemsSource = selectedErrorCase?.ErrorConditions;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save the edited properties of the selected injection point
           
            if (selectedInjectionPoint != null)
            {
                selectedInjectionPoint.Name = txtName.Text;
                selectedInjectionPoint.ToTarget = (bool)chkValid.IsChecked;

                selectedInjectionPoint.ChosenDepth = double.Parse(cmbDepthCorrect.SelectedItem.ToString());

                selectedInjectionPoint.ChosenQuantity = double.Parse(cmbQuantityCorrect.SelectedItem.ToString());

                if (double.TryParse(txtPitchMin.Text, out double pitchMin))
                    selectedInjectionPoint.PitchMin = pitchMin;

                if (double.TryParse(txtPitchMax.Text, out double pitchMax))
                    selectedInjectionPoint.PitchMax = pitchMax;

                if (double.TryParse(txtYawMin.Text, out double yawMin))
                    selectedInjectionPoint.YawMin = yawMin;

                if (double.TryParse(txtYawMax.Text, out double yawMax))
                    selectedInjectionPoint.YawMax = yawMax;

                selectedInjectionPoint.Explanation = txtAdditionalDescription.Text;
                // Update other properties accordingly

                RefreshInjectionPointsListView();
                SaveDynamicInfoXML();
            }





            
        }

        private void SaveErrorCaseButton_Click(object sender, RoutedEventArgs e)
        {

            if (selectedErrorCase != null)
            {
                selectedErrorCase.ErrorImageName = txtErrorImage.Text;
                selectedErrorCase.ErrorDescription = txtErrorDescription.Text;
                RefreshErrorCaseListView();
                RefreshErrorConditionListView();
                SaveDynamicInfoXML();
            }
        }

        private void SaveErrorConditionButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedErrorCondition != null && !string.IsNullOrWhiteSpace(cmbField.Text) && !string.IsNullOrWhiteSpace(cmbInequality.Text) && double.TryParse(txtReference.Text, out double reference))
            {
                selectedErrorCondition.Field = cmbField.Text;
                selectedErrorCondition.Inequality = cmbInequality.Text;
                selectedErrorCondition.Reference = reference;
                RefreshErrorConditionListView();
                SaveDynamicInfoXML();
            }
        }

        //private void SaveInjectionPoints()
        //{
        //    List<InjectionPoint> injectionPoints = new List<InjectionPoint>();
        //    foreach (var item in injectionPointList.Items)
        //    {
        //        injectionPoints.Add((InjectionPoint)item);
        //    }

        //    List<string> lines = new List<string>();

        //    //PIRINI SAVE HERE
        //    foreach (InjectionPoint point in injectionPoints)
        //    {
        //        string depthOptions = string.Join("/", DepthOptions);
        //        string quantityOptions = string.Join("/", QuantityOptions);

        //        string line = $"INJPOINT\t{point.PointNumber}\tToTarget:{(point.ToTarget ? "Y" : "N")}\tNomePunto:{point.Name}\tXY:{point.X.ToString(CultureInfo.GetCultureInfo("en-US"))}/{point.Y.ToString(CultureInfo.GetCultureInfo("en-US"))}\tDepthOptionsmm:{point.DepthOptionsAsString}\tDepthCorrectmm:{point.ChosenDepth}\tQuantityOptionsu:{point.QuantityOptionsAsString}\tQuantityCorrectu:{point.ChosenQuantity}\tYawMinMax:{point.YawMin}/{point.YawMax}\tPitchMinMax:{point.PitchMin}/{point.PitchMax}\tUlteriore spiegazione:{point.Explanation}"; lines.Add(line);
        //    }
        //    File.WriteAllLines(injectionPointsFileName, lines);

        //    RefreshInjectionPointsList();
        //    DrawInjectionPoints();
        //}

        private void SaveDynamicInfoXML()
        {
            DynamicInfo df = new DynamicInfo();
            foreach (InjectionPoint item in injectionPoints)
            {
                df.InjectionPoints.Add(item);
            }
            foreach (ErrorCase item in errorCases)
            {
                df.ErrorCases.Add(item);
            }

            df.Save<DynamicInfo>(dynamicInfoFileNameXml);
        }

        private void SaveInjectionPoint_Click(object sender, RoutedEventArgs e)
        {
            if (selectedInjectionPoint != null)
            {
                selectedInjectionPoint.Name = txtName.Text;
                selectedInjectionPoint.ToTarget = (bool)chkValid.IsChecked;

                selectedInjectionPoint.ChosenDepth = double.Parse(cmbDepthCorrect.SelectedItem.ToString());

                selectedInjectionPoint.ChosenQuantity = double.Parse(cmbQuantityCorrect.SelectedItem.ToString());

                if (double.TryParse(txtPitchMin.Text, out double pitchMin))
                    selectedInjectionPoint.PitchMin = pitchMin;

                if (double.TryParse(txtPitchMax.Text, out double pitchMax))
                    selectedInjectionPoint.PitchMax = pitchMax;

                if (double.TryParse(txtYawMin.Text, out double yawMin))
                    selectedInjectionPoint.YawMin = yawMin;

                if (double.TryParse(txtYawMax.Text, out double yawMax))
                    selectedInjectionPoint.YawMax = yawMax;

                selectedInjectionPoint.Explanation = txtAdditionalDescription.Text;
                // Update other properties accordingly

                RefreshInjectionPointsListView();
                SaveDynamicInfoXML();
            }
        }

        private void UpdateUIErrors()
        {
            if (selectedErrorCase != null)
            {
                txtErrorImage.Text = selectedErrorCase.ErrorImageName.ToString();
                txtErrorDescription.Text = selectedErrorCase.ErrorDescription;
            }
            else
            {
                txtErrorImage.Text = "";
                txtErrorDescription.Text = "";
            }

            if (selectedErrorCondition != null)
            {
                cmbField.Text = selectedErrorCondition.Field;
                cmbInequality.Text = selectedErrorCondition.Inequality;
                txtReference.Text = selectedErrorCondition.Reference.ToString();
            }
            else
            {
                cmbField.Text = "";
                cmbInequality.Text = "";
                txtReference.Text = "";
            }
        }
    }
}