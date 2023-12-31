using BeautySim.Common;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Media3D;

namespace BeautySim2023
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
            this.Loaded += new RoutedEventHandler(Window3D_Loaded);

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


        internal void ShowControls(bool v)
        {
            //spSystems.Visibility = v ? Visibility.Visible : Visibility.Hidden;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Calib3DClass.Instance.CloseApp();
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


        private void hvView3D_Loaded_1(object sender, RoutedEventArgs e)
        {
            hvView3D.Camera.UpDirection = new Vector3D(.7, .7, 0);
            hvView3D.Camera.LookDirection = new Vector3D(-.4, .6, 0);
            hvView3D.Camera.Position = new Point3D(450, -500, 0);
            hvView3D.LookAt(new Point3D(360, -300, 0), 3000);

            hvView3D.ShowCoordinateSystem = true;
            hvView3D.ShowViewCube = true;
        }



        private void Window3D_Loaded(object sender, RoutedEventArgs e)
        {
            Calib3DClass.Instance.InitCalibrationEnvironment(this);
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