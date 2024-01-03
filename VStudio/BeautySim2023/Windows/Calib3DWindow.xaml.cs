using BeautySim.Common;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Media3D;

namespace BeautySim2023
{
    public partial class Calib3DWindow : Window
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;

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

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.Calibrating = false;
            this.Close();

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

            Quaternion c = AppControl.Instance.Rotation_Manikin;
            AppControl.Instance.InitCalibrationEnvironment(this);
            AppControl.Instance.Calibrating = true;
        }

        private void bCalibrate_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.Calibrate();
        }

        private void bCalibrateSinglePoint_Click(object sender, RoutedEventArgs e)
        {
            if (pointsListView3D.SelectedIndex > -1)
            {
                InjectionPoint3DCalib point = AppControl.Instance.InjectionPoints3DCalib[pointsListView3D.SelectedIndex];
                point.AssignedCalibration = true;
                point.XAssigned = AppControl.Instance.TipNeedle.X;
                point.YAssigned = AppControl.Instance.TipNeedle.Y;
                point.ZAssigned = AppControl.Instance.TipNeedle.Z;
                int nextSelectedIndex = pointsListView3D.SelectedIndex + 1;
                if (nextSelectedIndex >= AppControl.Instance.InjectionPoints3DCalib.Count)
                {
                    nextSelectedIndex = 0;
                }
                pointsListView3D.SelectedIndex = nextSelectedIndex;
            }
        }
    }
}