using BeautySim.Common;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Manager3D
{
    public partial class Editor3DWindow : Window
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

        public Editor3DWindow()
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
            Editor3DClass.Instance.CloseApp();
        }

        private void BReset_Click(object sender, RoutedEventArgs e)
        {
            //UpdateCamera();
        }

        private void bStartRotation_Click(object sender, RoutedEventArgs e)
        {
            Editor3DClass.Instance.StartRotation();
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
            if (Properties.Settings.Default.WorkingModel==0)
            {
                //STANDARD VIEW
                //hvView3D.Camera.LookDirection = new Vector3D(0, 1, 0);
                //hvView3D.Camera.UpDirection = new Vector3D(1, 0, 0);

                //MODIFIED VIEW WHEN ARTERIES AND VEINS
                //hvView3D.Camera.LookDirection = new Vector3D(1, 0, 0);
                //hvView3D.Camera.UpDirection = new Vector3D(0, 0, 1);

                //FRONTAL!!
                //hvView3D.Camera.UpDirection = new Vector3D(.7, .7, 0);
                //hvView3D.Camera.LookDirection = new Vector3D(-.4, .6, 0);
                //hvView3D.Camera.Position = new Point3D(450, -500, 0);
                //hvView3D.LookAt(new Point3D(360, -300, 0), 3000);

                //CENTRAL!!
                //hvView3D.Camera.UpDirection = new Vector3D(1, 0, 0);
                //hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, 0);
                //hvView3D.Camera.Position = new Point3D(300, -500, 0);
                //hvView3D.LookAt(new Point3D(300, -300, 0), 3000);


                //ORBILULAR RIGHT!
                hvView3D.Camera.UpDirection = new Vector3D(.7, .30, 0);
                hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, -.7);
                hvView3D.Camera.Position = new Point3D(300, -500, 300);
                hvView3D.LookAt(new Point3D(250, -300, 130), 3000);

                //ORBILULAR LEFT!
                //hvView3D.Camera.UpDirection = new Vector3D(.7, .30, 0);
                //hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, .7);
                //hvView3D.Camera.Position = new Point3D(300, -500, -300);
                //hvView3D.LookAt(new Point3D(250, -300, -130), 3000);
            }
            else
            {
                hvView3D.Camera.UpDirection = new Vector3D(.7, .7, 0);
                hvView3D.Camera.LookDirection = new Vector3D(-.4, .6, 0);
                hvView3D.Camera.Position = new Point3D(450, -500, 0);
                hvView3D.LookAt(new Point3D(360, -300, 0), 3000);
            }


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
            Editor3DClass.Instance.Init(this);
        }

        private void Windows3D_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

    }
}