using BeautySim.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace InjectionPointManager
{
    public partial class MainWindow : Window
    {
        private const double DefaultQuantityCorrect = 1.0;

        private List<InjectionPointBase> injectionPoints;

        private List<InjectionPointSpecific2D> injectionPoints2D;

        private bool firstLoad;

        private string imageFileName;

        public Enum_AreaDefinition area;

        private string dynamicInfoFileNameXml;

        private InjectionPointSpecific2D selectedInjectionPoint;

        private string pathFilePointsBase = "C:\\BeautySim\\BasePointsDefinitions.xml";
        
        public MainWindow()
        {
            InitializeComponent();
            //cmbDepthCorrect.ItemsSource = DepthOptions;
            //cmbQuantityCorrect.ItemsSource = QuantityOptions;
            
            if (!File.Exists(pathFilePointsBase))
            {
                PointsManager.Instance.PopulateInjectionPointsAndSaveThem(pathFilePointsBase);
            }

            injectionPoints = PointsManager.Instance.LoadInjectionPoints(pathFilePointsBase);

            injectionPoints2D = new List<InjectionPointSpecific2D>();

            foreach (Enum_AreaDefinition value in Enum.GetValues(typeof(Enum_AreaDefinition)))
            {
                cbSelectArea.Items.Add(value.ToString());
                cbSelectArea.SelectedIndex = 0;
            }
        }

        private List<Enum_PointDefinition> pointsToWorkOn;
        private bool ableUpdateForQuantityChanged;

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            ableUpdateForQuantityChanged = false;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                firstLoad = true;
                string imagePath = openFileDialog.FileName;
                Console.WriteLine(cbSelectArea.SelectedValue as string);
                Enum_AreaDefinition area = (Enum_AreaDefinition)Enum.Parse(typeof(Enum_AreaDefinition), cbSelectArea.SelectedValue as string);

                pointsToWorkOn = PointsManager.Instance.GiveMePointsBasedOnArea(area);
                LoadImage(imagePath);
                LoadDynamicInfoXML();
                DrawInjectionPoints();
            }
            ableUpdateForQuantityChanged = true;
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
            }
        }

        private void DrawInjectionPoints()
        {
            // Clear existing ellipses from the canvas
            injectionPointsCanvas.Children.Clear();

            // Iterate through the injection points and draw ellipses for each point
            foreach (var point in injectionPoints2D)
            {
                var injectionPoint = point as InjectionPointSpecific2D;
                if ((injectionPoint == null) || (injectionPoint.Assigned == false))
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
                if (injectionPoint.IsError)
                    ellipse.Fill = Brushes.Red;
                else
                {
                    if (injectionPoint.PrescribedQuantity == 0)
                    {
                        ellipse.Fill = Brushes.Yellow;
                    }
                    else
                    {
                        ellipse.Fill = Brushes.Green;
                    }
                }

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

        private void FaceImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked position relative to the image's size
            Point clickPoint = e.GetPosition(faceImage);
            double relativeX = clickPoint.X / faceImage.ActualWidth;
            double relativeY = clickPoint.Y / faceImage.ActualHeight;

            // Create a new injection point with the relative coordinates
            if (selectedInjectionPoint != null)
            {
                selectedInjectionPoint.X = relativeX;
                selectedInjectionPoint.Y = relativeY;
                selectedInjectionPoint.Assigned = true;
            }
            DrawInjectionPoints();
            UpdateChecksAndInfos();
            SaveDynamicInfoXML(dynamicInfoFileNameXml);
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

        private void LoadDynamicInfoXML()
        {
            injectionPoints2D.Clear();
            //errorCases.Clear();
            if (File.Exists(dynamicInfoFileNameXml))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<InjectionPointSpecific2D>));

                // Deserialize from file
                using (FileStream fileStream = new FileStream(dynamicInfoFileNameXml, FileMode.Open))
                {
                    injectionPoints2D = (List<InjectionPointSpecific2D>)deserializer.Deserialize(fileStream);
                    Console.WriteLine("Deserialized List:");
                    // Use or print deserializedList here
                }

                // Clear existing items and load the injection points list

                for (int i = injectionPoints2D.Count - 1; i >= 0; i--)
                {
                    if (!pointsToWorkOn.Contains(injectionPoints2D[i].PointDefinition))
                    {
                        injectionPoints2D.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < pointsToWorkOn.Count; i++)
            {
                if (!injectionPoints2D.Any(item => item.PointDefinition == pointsToWorkOn[i]))
                {
                    InjectionPointBase pp = injectionPoints.FirstOrDefault(item => item.PointDefinition == pointsToWorkOn[i]);

                    injectionPoints2D.Add(new InjectionPointSpecific2D(pp, 0, 0, 0, false));
                }
            }

            foreach (InjectionPointSpecific2D item in injectionPoints2D)
            {
                InjectionPointBase bb = injectionPoints.FirstOrDefault(item2 => item2.PointDefinition == item.PointDefinition);

                item.CopyAllPropertiesFromBaseExcptPrescribedQuantity(bb);
            }

            List<string> pointsTypeList = pointsToWorkOn.Select(e => e.ToString()).ToList();

            lvPointsDefinitions.DataContext = pointsTypeList;
        }

        private void LoadImage(string imagePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imagePath);
            image.EndInit();
            faceImage.Source = image;

            imageFileName = System.IO.Path.GetFileName(imagePath);
            dynamicInfoFileNameXml = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imagePath), System.IO.Path.GetFileNameWithoutExtension(imagePath) + "_injPointsNEW.xml");
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

        private void SaveDynamicInfoXML(string nameFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<InjectionPointSpecific2D>));

            // Serialize to file

            using (FileStream fileStream = new FileStream(nameFile, FileMode.Create))
            {
                serializer.Serialize(fileStream, injectionPoints2D);
                Console.WriteLine($"Serialized to XML file: {nameFile}");
            }
        }

        //private void SaveInjectionPoint_Click(object sender, RoutedEventArgs e)
        //{
        //    if (selectedInjectionPoint != null)
        //    {
        //        SaveDynamicInfoXML(dynamicInfoFileNameXml);
        //    }
        //}

        private void lvPointsDefinitions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvPointsDefinitions.SelectedItem != null)
            {
                selectedInjectionPoint = injectionPoints2D.Where(item => item.PointDefinition.ToString() == lvPointsDefinitions.SelectedItem.ToString()).FirstOrDefault();
                DrawInjectionPoints();
                UpdateChecksAndInfos();
            }
        }

        private void UpdateChecksAndInfos()
        {
            cmbQuantityCorrect.ItemsSource = selectedInjectionPoint.QuantityOptions;
            chkError.IsChecked = selectedInjectionPoint.IsError;
            cmbQuantityCorrect.SelectedItem = selectedInjectionPoint.PrescribedQuantity;
            txtPitchMin.Text = selectedInjectionPoint.PitchMin.ToString();
            txtPitchMax.Text = selectedInjectionPoint.PitchMax.ToString();
            txtYawMin.Text = selectedInjectionPoint.YawMin.ToString();
            txtYawMax.Text = selectedInjectionPoint.YawMax.ToString();
            txtDepthCorrect.Text = selectedInjectionPoint.PrescribedDepth.ToString();
            chkAssigned.IsChecked = selectedInjectionPoint.Assigned;
            txtName.Text = Enum.GetName(typeof(Enum_PointDefinition), selectedInjectionPoint.PointDefinition);
        }

        private void cmbQuantityCorrect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ableUpdateForQuantityChanged)
            {
                if (selectedInjectionPoint != null)
                {
                    selectedInjectionPoint.PrescribedQuantity = double.Parse(cmbQuantityCorrect.SelectedItem.ToString());
                    DrawInjectionPoints();
                    SaveDynamicInfoXML(dynamicInfoFileNameXml);
                }
            }
        }
    }
}