using BeautySim.Common;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Calib3DApp
{
    public partial class Calib3DWindow : Window
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;

        private List<string> arteriesStrings = new List<string>() { "Arteries" };

        private bool firstEvent = false;

        private List<string> headStrings = new List<string>() { "Head" };

        private List<int> indexesMannikin = new List<int>();

        private List<int> indexesProbe = new List<int>() { 0, 1 };

        private bool loadedModel = false;

        private bool loadedViewer = false;

        private Model3DGroup modelArteries;

        private Model3DGroup modelGroupGeneral;

        private Model3DGroup modelNerves;

        private Model3DGroup modelSkin;

        //private HelixToolkit.Wpf.SharpDX.Material materialSkin = MaterialHelper.CreateMaterial(Colors.Pink, .5);
        //private HelixToolkit.Wpf.SharpDX.Material materialUnderSkin = MaterialHelper.CreateMaterial(Colors.HotPink, .5);
        //private HelixToolkit.Wpf.SharpDX.Material materialVeins = MaterialHelper.CreateMaterial(Colors.Blue, .5);
        //private HelixToolkit.Wpf.SharpDX.Material materialArteries = MaterialHelper.CreateMaterial(Colors.Red, .5);
        //private HelixToolkit.Wpf.SharpDX.Material materialNerves = MaterialHelper.CreateMaterial(Colors.Yellow, .5);
        private Model3DGroup modelUnderSkin;

        private Model3DGroup modelVeins;

        private List<string> nervesStrings = new List<string>() { "Nerves" };

        private List<string> veinsStrings = new List<string>() { "Veins" };

        public Calib3DWindow()
        {
            InitializeComponent();

            this.ContentRendered += Window3D_ContentRendered;
            this.Loaded += new RoutedEventHandler(Window3D_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(Windows3D_Closing);
        }

        public delegate void LoadingModel_Delegate(bool loading);

        public delegate void ManikinLoaded_Delegate();

        private delegate void MethodInvoker();

        public event LoadingModel_Delegate LoadingModel_Event;

        public event ManikinLoaded_Delegate ManikinLoadedEvent;

        public bool ContentRenderedReally { get; set; } = false;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        internal void CenterView()
        {
        }

        internal void ShowControls(bool v)
        {
            //spSystems.Visibility = v ? Visibility.Visible : Visibility.Hidden;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Calib3DClass.Instance.CloseApp();
        }

        private void BReset_Click(object sender, RoutedEventArgs e)
        {
            //UpdateCamera();
        }

        private void bStartRotation_Click(object sender, RoutedEventArgs e)
        {
            Calib3DClass.Instance.StartRotation();
        }

        private void BUpdate_Click(object sender, RoutedEventArgs e)
        {
            //UpdateVisualizationModel();
        }

        private void cbsModified_Checked(object sender, RoutedEventArgs e)
        {
            //UpdateVisualizationModel();
        }

        private void entranceListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (entranceListView.Items.Count > 0)
            {
                entranceListView.ScrollIntoView(entranceListView.Items[entranceListView.Items.Count - 1]);
            }
        }

        private void HvView3D_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void hvView3D_Loaded_1(object sender, RoutedEventArgs e)
        {
            hvView3D.Camera.UpDirection = new Vector3D(.7, .7, 0);
            hvView3D.Camera.LookDirection = new Vector3D(-.4, .6, 0);
            hvView3D.Camera.Position = new Point3D(450, -500, 0);
            hvView3D.LookAt(new Point3D(360, -300, 0), 3000);

            hvView3D.ShowCoordinateSystem = true;
            hvView3D.ShowViewCube = true;
            //hvView3D.ZoomExtents();
        }

        private void manikinControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
        }

        private void MeshGeometryModel3D_Mouse3DDown(object sender, MouseDown3DEventArgs e)
        {
        }

        private void PageContainer_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void SetManikinType(string folder)
        {
        }

        private void UpdateVisualizationModel()
        {
            if (false)
            {
                if (loadedModel && loadedViewer)
                {
                    //Model3DGroup modelToView = new Model3DGroup();

                    //if ((bool)cbProbe.IsChecked)
                    //{
                    //    for (int i = 0; i < indexesProbe.Count; i++)
                    //    {
                    //        modelToView.Children.Add((GeometryModel3D)modelProbe.Children[indexesProbe[i]]);
                    //    }
                    //}

                    //Model3DGroup modelToView = new Model3DGroup();

                    //if ((bool)cbUterus.IsChecked)
                    //{
                    //    for (int i = 0; i < indexesUterus.Count; i++)
                    //    {
                    //        modelToView.Children.Add((GeometryModel3D)modelFemale.Children[indexesUterus[i]]);
                    //    }
                    //}

                    //if ((bool)cbFemale.IsChecked)
                    //{
                    //    bool trasparency = false;

                    //    Material mat3 = MaterialHelper.CreateMaterial(Colors.Pink, trasparency ? 0.15 : 0.9);
                    //    Material mat4 = MaterialHelper.CreateMaterial(Colors.RosyBrown, trasparency ? 0.1 : 0.9);

                    //    for (int i = 0; i < indexesFemale.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            GeometryModel3D geometryModel = (GeometryModel3D)modelFemale.Children[indexesFemale[i]];

                    //            string c = geometryModel.GetName();

                    //            if (c.Contains("Hair"))
                    //            {
                    //                geometryModel.Material = mat4;
                    //                geometryModel.BackMaterial = mat4;
                    //            }
                    //            if (c.Contains("Lady"))
                    //            {
                    //                geometryModel.Material = mat3;
                    //                geometryModel.BackMaterial = mat3;
                    //            }
                    //            modelToView.Children.Add((GeometryModel3D)modelFemale.Children[indexesFemale[i]]);
                    //        }
                    //        catch (Exception)
                    //        {
                    //        }
                    //    }
                    //}

                    //var transformGroup = new Transform3DGroup();

                    //transformGroup.Children.Add(new ScaleTransform3D(1000, 1000, 1000));

                    //modelToView.Transform = transformGroup;

                    //modelHumanBlock.Content = modelToView;
                    //modelProbeBlock.Content = modelHead;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window3D_ContentRendered(object sender, EventArgs e)
        {
        }

        private void Window3D_Loaded(object sender, RoutedEventArgs e)
        {
            Calib3DClass.Instance.Init(this);
        }

        private void Windows3D_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void bCalibrate_Click(object sender, RoutedEventArgs e)
        {
            Calib3DClass.Instance.Calibrate();
        }

        private void bCalibrateSinglePoint_Click(object sender, RoutedEventArgs e)
        {
            if (pointsListView3D.SelectedIndex > -1)
            {
                InjectionPoint3DCalib point = Calib3DClass.Instance.InjectionPoints3DCalib[pointsListView3D.SelectedIndex];
                point.AssignedCalibration = true;
                point.XAssigned = Calib3DClass.Instance.TipNeedle.X;
                point.YAssigned = Calib3DClass.Instance.TipNeedle.Y;
                point.ZAssigned = Calib3DClass.Instance.TipNeedle.Z;
                int nextSelectedIndex=pointsListView3D.SelectedIndex+1;
                if (nextSelectedIndex>= Calib3DClass.Instance.InjectionPoints3DCalib.Count)
                {
                    nextSelectedIndex = 0;
                }
                pointsListView3D.SelectedIndex = nextSelectedIndex;
            }
            
        }
    }
}