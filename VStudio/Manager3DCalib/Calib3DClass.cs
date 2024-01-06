using BeautySim.Common;
using Device.Polhemus;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml.Serialization;
using HitTestResult = HelixToolkit.Wpf.SharpDX.HitTestResult;

namespace Calib3DApp
{
    public class Calib3DClass : DemoCore.BaseViewModel, INotifyPropertyChanged
    {
        public const string CoordFile = "C:\\BeautySim\\CalibrationFile.txt";
        public System.Windows.Media.Brush ActiveEllipse = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 150, 136));
        private static Calib3DClass instance;
        private bool alreadyConnectedPolhemus;
        private List<long> averageCollisionTime = new List<long>();

        //private float axisPositionOffsetNeedle = 5.5f;
        //private float axisPositionOffsetNeedle = 0f;

        private Point3D baseNeedle;
        private Point3D baseNeedleOrigin;
        private Dictionary<string, List<Guid>> collisionItemsGuid = new Dictionary<string, List<Guid>>();
        private Device.Motion.FRAME f1 = null;
        private string hitStructure;
        private DateTime lastArrivedInput;
        private DateTime lastMouseMove;
        private int lengthCable = 50;
        private float lengthNeedle;
        private int lengthSensor = 14;
        private List<Object3D> modelsArteries;
        private List<Object3D> modelsVeins;
        private bool mouseClickOrMove = true;
        private string pathFilePointsBase = "C:\\BeautySim\\BasePointsDefinitions.xml";
        private string pathFilePointsBase3D = "C:\\BeautySim\\BasePointsDefinitions3D.xml";
        private string pathFilePointsBase3DCalib = "C:\\BeautySim\\BasePointsDefinitions3DCalib.xml";
        private string pathFilePointsBaseCalib = "C:\\BeautySim\\BasePointsDefinitionsCalib.xml";
        private PDIClass pDIClass;
        private List<MeshGeometryModel3D> pointsToBeShown = new List<MeshGeometryModel3D>();
        private System.Windows.Media.Media3D.Quaternion rotation_Manikin;
        //private float rotationAngleOffsetNeedle = 0;
        private System.Windows.Media.Media3D.Quaternion rotationSensor_WRS;
        private int rotationTimer;
        private Enum_StateCollision sc = Enum_StateCollision.NONE;
        private int selectedIndexInjectionPoint;
        private InjectionPoint3DCalib selectedInjectionPoint;
        private Stopwatch stopwatch = new Stopwatch();
        private int thetaDivNeedle = 18;
        private DispatcherTimer timerCheckPolhemus;
        private DispatcherTimer timerRotation;
        private Point3D tipNeedle;
        private Point3D tipNeedleOrigin;
        private Vector3D upDirection;
        private float xPosSensor02 = 50;
        private float yPosSensor02;
        private float zPosSensor02;
        private List<PointHit> actualPointsHit;

        private Calib3DClass()
        {
            timerRotation = new DispatcherTimer();
            timerRotation.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerRotation.Tick += TimerRotation_Tick1;
            InjectionPoints3DCalib = new ObservableCollection<InjectionPoint3DCalib>();
        }

        private delegate void SetIndicatorDelegate(System.Windows.Shapes.Ellipse bb, bool aactive);

        public event PropertyChangedEventHandler PropertyChanged;

        public static Calib3DClass Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Calib3DClass();
                }
                return instance;
            }
        }

        //public float AxisPositionOffsetNeedle
        //{
        //    get { return axisPositionOffsetNeedle; }
        //    set
        //    {
        //        axisPositionOffsetNeedle = value;
        //        OnPropertyChanged(nameof(AxisPositionOffsetNeedle));
        //    }
        //}

        public Point3D BaseNeedle
        {
            get { return baseNeedle; }
            set
            {
                baseNeedle = value;
                OnPropertyChanged(nameof(BaseNeedle));
            }
        }

        public LineGeometry3D Coordinate { private set; get; }
        public BillboardText3D CoordinateText { private set; get; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryAntenna { private set; get; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryCable { get; private set; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryConnector { get; private set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryHeadSkin { private set; get; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryNeedle { get; private set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryProcerus { private set; get; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometrySensor { get; private set; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometrySensor2 { get; private set; }
        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere2 { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere3 { private set; get; }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometrySphereOrigin { get; private set; }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryTConnector { get; private set; }

        public ObservableElement3DCollection GroupModelSourceCorrugators { private set; get; } = new ObservableElement3DCollection();

        public ObservableElement3DCollection GroupModelSourceNerves { private set; get; } = new ObservableElement3DCollection();

        public ObservableElement3DCollection GroupModelSourceOrbicularisSuperiorLateral { private set; get; } = new ObservableElement3DCollection();

        public ObservableElement3DCollection GroupModelSourceUnderSkin { private set; get; } = new ObservableElement3DCollection();

        public ObservableElement3DCollection GroupModelSourceVeins { private set; get; } = new ObservableElement3DCollection();

        public string HitStructure
        {
            get => hitStructure;
            set
            {
                if (hitStructure != value)
                {
                    hitStructure = value;
                    OnPropertyChanged(nameof(HitStructure));
                }
            }
        }

        public bool HitThrough { set; get; }

        public ObservableCollection<InjectionPoint3DCalib> InjectionPoints3DCalib { get; set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialAntenna { private set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialCable { get; private set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialConnector { get; private set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialHeadSkin { private set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialNeedle { get; private set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialProcerus { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSensor { get; private set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSensor2 { get; private set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere2 { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere3 { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSphereOrigin { get; private set; }
        public double PitchNeedle { get; private set; }
        public HelixToolkit.Wpf.SharpDX.Geometry3D RectGeometry { private set; get; }

        public System.Windows.Media.Media3D.Quaternion Rotation_Manikin
        {
            get { return rotation_Manikin; }
            set
            {
                rotation_Manikin = value;
                OnPropertyChanged(nameof(Rotation_Manikin));
            }
        }

        //public float RotationAngleOffsetNeedle
        //{
        //    get { return rotationAngleOffsetNeedle; }
        //    set
        //    {
        //        rotationAngleOffsetNeedle = value;
        //        OnPropertyChanged(nameof(RotationAngleOffsetNeedle));
        //    }
        //}

        public System.Windows.Media.Media3D.Quaternion RotationSensor_WRS_real
        {
            get { return rotationSensor_WRS; }
            set
            {
                rotationSensor_WRS = value;
                OnPropertyChanged(nameof(RotationSensor_WRS_real));
            }
        }

        public Point3D TipNeedle
        {
            get { return tipNeedle; }
            set
            {
                tipNeedle = value;
                OnPropertyChanged(nameof(TipNeedle));
            }
        }

        public TranslateTransform3D TransformSphere { private set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);

        public TranslateTransform3D TransformSphere2 { private set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);

        public TranslateTransform3D TransformSphere3 { private set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);

        public TranslateTransform3D TrasformAntenna { private set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);

        public Vector3D UpDirection
        {
            get { return upDirection; }
            set
            {
                upDirection = value;
                OnPropertyChanged(nameof(UpDirection));
            }
        }

        public HelixToolkit.Wpf.SharpDX.Material ViewCubeMaterial2 { private set; get; }

        public System.Windows.Media.Media3D.Transform3D ViewCubeTransform3 { private set; get; }

        public Visibility VisibilityAntenna { private set; get; } = Visibility.Hidden;

        public Calib3DWindow WindowMain { get; internal set; }

        public float XPosSensor02
        {
            get { return xPosSensor02; }
            set
            {
                xPosSensor02 = value;
                OnPropertyChanged(nameof(XPosSensor02));
            }
        }

        public double YawNeedle { get; private set; }

        public float YPosSensor02
        {
            get { return yPosSensor02; }
            set
            {
                yPosSensor02 = value;
                OnPropertyChanged(nameof(YPosSensor02));
            }
        }

        public float ZPosSensor02
        {
            get { return zPosSensor02; }
            set
            {
                zPosSensor02 = value;
                OnPropertyChanged(nameof(ZPosSensor02));
            }
        }

        public static System.Windows.Media.Media3D.Quaternion CreateQuaternionFromAxisAngle(float angleInDegrees, float axisX, float axisY, float axisZ)
        {
            float angleInRadians = (float)(Math.PI * angleInDegrees / 180.0);
            float sinHalfAngle = (float)Math.Sin(angleInRadians / 2.0);
            float cosHalfAngle = (float)Math.Cos(angleInRadians / 2.0);

            return new System.Windows.Media.Media3D.Quaternion(sinHalfAngle * axisX, sinHalfAngle * axisY, sinHalfAngle * axisZ, cosHalfAngle);
        }

        public static System.Windows.Media.Media3D.Quaternion CreateRotationQuaternionAlongAxis(float angleInDegrees, int axis)
        {
            if (axis == 0)
            {
                return CreateQuaternionFromAxisAngle(angleInDegrees, 1, 0, 0);
            }
            if (axis == 1)
            {
                return CreateQuaternionFromAxisAngle(angleInDegrees, 0, 1, 0);
            }
            if (axis == 2)
            {
                return CreateQuaternionFromAxisAngle(angleInDegrees, 0, 0, 1);
            }
            return CreateQuaternionFromAxisAngle(angleInDegrees, 0, 0, 1);
        }

        public static Transform3DGroup FindAlignment(Point3D[] modelPoints, Point3D[] worldPoints)
        {
            if (modelPoints.Length != worldPoints.Length || modelPoints.Length < 3)
                throw new ArgumentException("Invalid input, arrays must have the same length and at least three points.");

            Vector3D[] modelPointsVec = Matrix3DUtils.ConvertPoint3DArrayToVector3DArray(modelPoints);
            Vector3D[] worldPointsVec = Matrix3DUtils.ConvertPoint3DArrayToVector3DArray(worldPoints);

            // Compute the centroids of the point sets
            Vector3D modelCentroid = modelPointsVec.Aggregate(new Vector3D(0, 0, 0), (acc, p) => acc + p) / modelPoints.Length;
            Vector3D worldCentroid = worldPointsVec.Aggregate(new Vector3D(0, 0, 0), (acc, p) => acc + p) / worldPoints.Length;

            // Build the covariance matrix
            Matrix3x3 H = Matrix3x3.Identity;
            for (int i = 0; i < modelPoints.Length; i++)
            {
                Vector3D modelPoint = modelPointsVec[i] - modelCentroid;
                Vector3D worldPoint = worldPointsVec[i] - worldCentroid;
                H.M11 += (float)modelPoint.X * (float)worldPoint.X;
                H.M12 += (float)modelPoint.X * (float)worldPoint.Y;
                H.M13 += (float)modelPoint.X * (float)worldPoint.Z;
                H.M21 += (float)modelPoint.Y * (float)worldPoint.X;
                H.M22 += (float)modelPoint.Y * (float)worldPoint.Y;
                H.M23 += (float)modelPoint.Y * (float)worldPoint.Z;
                H.M31 += (float)modelPoint.Z * (float)worldPoint.X;
                H.M32 += (float)modelPoint.Z * (float)worldPoint.Y;
                H.M33 += (float)modelPoint.Z * (float)worldPoint.Z;
            }

            // Compute the Singular Value Decomposition of H
            // Assume a function ComputeSVD exists that computes the SVD of a 3x3 matrix
            (Matrix3x3 U, Matrix3x3 S, Matrix3x3 V) = Matrix3DUtils.ComputeSVD(H);

            // The rotation matrix is U * V^T
            V.Transpose();
            Matrix3x3 rotationMatrix = U * V;

            // The translation vector is the difference in centroids, adjusted by the rotation
            Vector3D translationVector = new Vector3D(0, 0, 0);// worldCentroid - rotationMatrix.Transform(modelCentroid);

            // Create the Transform3DGroup
            Transform3DGroup transformGroup = new Transform3DGroup();

            //System.Windows.Media.Media3D.Quaternion quaternion = Matrix3DUtils.QuaternionFromMatrix(rotationMatrix);  // Convert matrix to quaternion
            //RotateTransform3D rotateTransform = new RotateTransform3D(new QuaternionRotation3D(quaternion));

            //transformGroup.Children.Add(rotateTransform);
            //transformGroup.Children.Add(new TranslateTransform3D(translationVector));

            return transformGroup;
        }

        public Point3D GetRotoTransatedPoint(Transform3DGroup tsg, Point3D origin)
        {
            Point3D ptd = tsg.Transform(origin);
            return ptd;
        }

        public void OnMouseMoveHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!mouseClickOrMove)
            {
                if ((DateTime.Now - lastMouseMove).TotalMilliseconds > 100)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        DateTime c = DateTime.Now;
                        var viewport = sender as Viewport3DX;
                        var cameraPosition = viewport.Camera.Position;
                        List<PointHit> pointsHit = new List<PointHit>();
                        if (viewport == null) { return; }
                        var point = e.GetPosition(viewport);

                        //DENIS
                        //creo il RAY dal point....tu devi crearlo dal POLHEMUS
                        Ray ray = new Ray();
                        viewport.RenderHost.RenderContext.UnProject(point.ToVector2(), out ray);

                        Vector2 pos = new Vector2(0, 0);
                        var hitContext = new HitTestContext(viewport.RenderHost.RenderContext, ref ray, ref pos);
                        List<HitTestResult> hits = new List<HitTestResult>();
                        var rend = viewport.Renderables.ToList();

                        stopwatch.Restart();

                        foreach (var element in rend)
                        {
                            var t = element.GetType().ToString();
                            CheckSceneNode(element, hitContext, ray.Direction, ray.Position, ref pointsHit);
                        }
                        stopwatch.Stop();

                        averageCollisionTime.Add(stopwatch.ElapsedMilliseconds);
                        while (averageCollisionTime.Count > 100)
                        {
                            averageCollisionTime.RemoveAt(0);
                        }

                        double average = averageCollisionTime.Average() / 1000;

                        if (pointsHit.Count > 0)
                        {
                            HitStructure = $"{pointsHit[0]} AVG TIME: {average}";
                        }
                        else
                        {
                            HitStructure = $"NONE AVG TIME: {average}";
                        }

                        double minDistance = double.MaxValue;
                        int indexMin = -1;
                        if (pointsHit.Count > 0)
                        {
                            for (int i = 0; i < pointsHit.Count; i++)
                            {
                                var distance = CalcDistance(pointsHit[i].Point, cameraPosition);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    indexMin = i;
                                }
                            }

                            TransformSphere.OffsetX = pointsHit[indexMin].Point.X;
                            TransformSphere.OffsetY = pointsHit[indexMin].Point.Y;
                            TransformSphere.OffsetZ = pointsHit[indexMin].Point.Z;

                            OnPropertyChanged(nameof(TransformSphere));
                            HitStructure = pointsHit[indexMin].Name;
                            //Debug.WriteLine(HitStructure);
                        }
                        else
                        {
                            HitStructure = "None";
                        }

                        lastMouseMove = DateTime.Now;
                        //Debug.WriteLine((DateTime.Now - c).TotalMilliseconds.ToString());
                    }
                }
            }
        }

        public HitTestResultBehavior ResultCallbackNeedle(System.Windows.Media.HitTestResult result)
        {
            // Did we hit 3D?
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
                // Did we hit a MeshGeometry3D?
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

        public void SetIndicator(System.Windows.Shapes.Ellipse bb, bool active)
        {
            if (!WindowMain.Dispatcher.CheckAccess())
            {
                SetIndicatorDelegate d = new SetIndicatorDelegate(SetIndicator);
                WindowMain.Dispatcher.Invoke(d, new object[] { bb, active });
            }
            else
            {
                bb.Fill = active ? ActiveEllipse : System.Windows.Media.Brushes.Gray;
            }
        }

        internal void Calibrate()
        {

                var averageXAssigned = InjectionPoints3DCalib.Average(point => point.XAssigned);
                var averageYAssigned = InjectionPoints3DCalib.Average(point => point.YAssigned);
                var averageZAssigned = InjectionPoints3DCalib.Average(point => point.ZAssigned);
                var averageX = InjectionPoints3DCalib.Average(point => point.X);
                var averageY = InjectionPoints3DCalib.Average(point => point.Y);

                var averageZ = InjectionPoints3DCalib.Average(point => point.Z);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog.Title = "Save Averages";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // File path chosen by the user
                    string filePath = saveFileDialog.FileName;

                    // Write the averages to the file
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine((averageXAssigned-averageX).ToString("00.00",new CultureInfo("en-US")));
                        writer.WriteLine((averageYAssigned-averageY).ToString("00.00", new CultureInfo("en-US")));
                        writer.WriteLine((averageZAssigned-averageZ).ToString("00.00", new CultureInfo("en-US")));
                    }
                }
            
        }

        internal void CloseApp()
        {
            DisconnectPDI();
            WindowMain.Close();
        }

        internal void ConnectPDI()
        {
            if (pDIClass != null)
            {
                pDIClass.OnConnectionStatusChanged += PDIClass_OnConnectionStatusChanged;
                pDIClass.OnNewFrameAvailable += PDIClass_OnNewFrameAvailable;
                pDIClass.Connect(new WindowInteropHelper(WindowMain).Handle);
                timerCheckPolhemus = new DispatcherTimer();
                timerCheckPolhemus.Interval = new TimeSpan(0, 0, 1);
                timerCheckPolhemus.IsEnabled = true;
                timerCheckPolhemus.Tick += new EventHandler(TimerCheckPolhemusTick_Listener);
                timerCheckPolhemus.Start();
            }
        }

        internal void DisconnectPDI()
        {
            if (pDIClass != null)
            {
                pDIClass.OnConnectionStatusChanged -= PDIClass_OnConnectionStatusChanged;
                pDIClass.OnNewFrameAvailable -= PDIClass_OnNewFrameAvailable;
                pDIClass.Disconnect();
            }
        }

        internal void Init(Calib3DWindow window3D)
        {
            using (StreamReader sr = new StreamReader(CoordFile))
            {
                string xLine, yLine, zLine, lengthNeedleLine;
                double x = 0;
                double y = 0;
                double z = 0;
                double lengthNeedleR = 0;
                while ((xLine = sr.ReadLine()) != null &&
                       (yLine = sr.ReadLine()) != null &&
                       (zLine = sr.ReadLine()) != null &&
                       (lengthNeedleLine = sr.ReadLine()) != null)
                {
                    if (double.TryParse(xLine, out x) &&
                        double.TryParse(yLine, out y) &&
                        double.TryParse(zLine, out z) &&
                        double.TryParse(lengthNeedleLine, out lengthNeedleR))
                    {
                        Console.WriteLine($"Read coordinates: x={x}, y={y}, z={z}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse coordinates");
                    }
                }
                lengthNeedle = (float)lengthNeedleR;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;

            WindowMain = window3D;

            var hwnd2 = new WindowInteropHelper(window3D).Handle;
            Calib3DWindow.SetWindowLong(hwnd2, Calib3DWindow.GWL_STYLE, Calib3DWindow.GetWindowLong(hwnd2, Calib3DWindow.GWL_STYLE) & ~Calib3DWindow.WS_SYSMENU);

            pDIClass = new PDIClass();
            ConnectPDI();
            WindowMain.entranceListView.Visibility = Visibility.Collapsed;
            WindowMain.pointsListView3D.Visibility = Visibility.Visible;
            WindowMain.gmModels.Transform = new TranslateTransform3D(0, 0, 0);
            //load the points from file

            ObservableCollection<InjectionPoint3D> temporaryList = new ObservableCollection<InjectionPoint3D>();
            if (File.Exists(pathFilePointsBase3DCalib))
            {
                temporaryList = PointsManager.Instance.LoadInjectionPoints3D(pathFilePointsBase3DCalib);
            }

            foreach (InjectionPoint3D item in temporaryList)
            {
                InjectionPoints3DCalib.Add(new InjectionPoint3DCalib(item));
            }

            WindowMain.pointsListView3D.SelectionChanged += PointsListView3D_SelectionChanged;
            WindowMain.pointsListView3D.SelectedIndex = 0;

            for (int i = 0; i < InjectionPoints3DCalib.Count; i++)
            {
                MeshGeometryModel3D aa = new MeshGeometryModel3D();
                aa.CullMode = SharpDX.Direct3D11.CullMode.Back;

                var sphereA = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                sphereA.AddSphere(new Vector3(0, 0, 0), 2);
                HelixToolkit.Wpf.SharpDX.Geometry3D geometrySphere = sphereA.ToMeshGeometry3D();
                HelixToolkit.Wpf.SharpDX.Material MaterialSphere = DiffuseMaterials.Blue;
                aa.Geometry = geometrySphere;
                aa.Material = MaterialSphere;

                if (InjectionPoints3DCalib[i].Assigned)
                {
                    aa.Transform = new TranslateTransform3D(InjectionPoints3DCalib[i].X, InjectionPoints3DCalib[i].Y, InjectionPoints3DCalib[i].Z);
                }

                pointsToBeShown.Add(aa);

                WindowMain.hvView3D.Items.Add(aa);
            }

            EffectsManager = new DefaultEffectsManager();

            InitializeModels("C:\\BeautySim\\Models3D");
            InitializeViewCubes();
            InitializeCoordinates();

            WindowMain.DataContext = Calib3DClass.Instance;
        }

        internal void StartRotation()
        {
            UpDirection = new Vector3D(0, 0, -1);
            //timerRotation.Start();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double CalcDistance(Point3D point1, Point3D point2)
        {
            double deltaX = point1.X - point2.X;
            double deltaY = point1.Y - point2.Y;
            double deltaZ = point1.Z - point2.Z;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        private void CheckSceneNode(SceneNode node, HitTestContext context, Vector3 rayDir, Vector3 tip, ref List<PointHit> pointsHit)
        {
            try
            {
                if (pointsHit==null)
                {
                    pointsHit = new List<PointHit>();
                }
                if (node is GroupNode gn)
                {
                    List<HitTestResult> hits = new List<HitTestResult>();
                    gn.HitTest(context, ref hits);
                    if (hits != null)
                    {
                        //List<string> nodeNames = new List<string>();
                        foreach (var hit in hits)
                        {
                            foreach (var item in collisionItemsGuid)
                            {
                                if (item.Value.Contains(hit.Geometry.GUID))
                                {
                                    Vector3 diff = hit.PointHit - tip;

                                    double distance = Vector3.Dot(diff, rayDir);

                                    //nodeNames.Add(item.Key);
                                    pointsHit.Add(new PointHit(hit.PointHit, item.Key, distance));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return;
        }

        private PhongMaterial CreatePhongMaterial(Color4 color)
        {
            return new PhongMaterial
            {
                DiffuseColor = color,
                SpecularColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f),
                SpecularShininess = 32f
            };
        }

        private HelixToolkit.Wpf.SharpDX.MeshGeometry3D CreateTruncatedConeGeometry(float baseRadius, float topRadius, float height, int thetaDiv, Vector3 normal, Vector3 origin, bool baseCap)
        {
            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, true);
            builder.AddCone(origin, normal, baseRadius, topRadius, height, baseCap, false, thetaDiv);
            var geometry = builder.ToMeshGeometry3D();
            return geometry;
        }

        private void InitializeCoordinates()
        {
            var builder = new LineBuilder();
            builder.AddLine(Vector3.Zero, Vector3.UnitX * 5);
            builder.AddLine(Vector3.Zero, Vector3.UnitY * 5);
            builder.AddLine(Vector3.Zero, Vector3.UnitZ * 5);
            Coordinate = builder.ToLineGeometry3D();
            Coordinate.Colors = new Color4Collection(Enumerable.Repeat<Color4>(SharpDX.Color.White, 6));
            Coordinate.Colors[0] = Coordinate.Colors[1] = SharpDX.Color.Red;
            Coordinate.Colors[2] = Coordinate.Colors[3] = SharpDX.Color.Green;
            Coordinate.Colors[4] = Coordinate.Colors[5] = SharpDX.Color.Blue;

            CoordinateText = new BillboardText3D();
            CoordinateText.TextInfo.Add(new HelixToolkit.Wpf.SharpDX.TextInfo("X", Vector3.UnitX * 6));
            CoordinateText.TextInfo.Add(new HelixToolkit.Wpf.SharpDX.TextInfo("Y", Vector3.UnitY * 6));
            CoordinateText.TextInfo.Add(new HelixToolkit.Wpf.SharpDX.TextInfo("Z", Vector3.UnitZ * 6));
        }

        private void InitializeModels(string folder)
        {
            bool showSkinComplete = false;
            bool showSkin = true;
            bool showNoSkin = false;
            bool showArteries = false; //Properties.Settings.Default.WorkingModeVisualizerOrPointManager;

            bool showVeins = false; //Properties.Settings.Default.WorkingModeVisualizerOrPointManager;
            bool showNerves = false;
            bool showProcerus = false; // Properties.Settings.Default.WorkingModeVisualizerOrPointManager;
            bool showOrbicularisSuperiorLateral = false; //Properties.Settings.Default.WorkingModeVisualizerOrPointManager;
            bool showCorrugator = false; //Properties.Settings.Default.WorkingModeVisualizerOrPointManager;

            var sphere = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere.AddSphere(new Vector3(0, 0, 0), 2);
            GeometrySphere = sphere.ToMeshGeometry3D();
            MaterialSphere = DiffuseMaterials.Green;

            //var sphereOrigin = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            //sphereOrigin.AddSphere(new Vector3(0, 0, 0), 40);
            //GeometrySphereOrigin = sphereOrigin.ToMeshGeometry3D();
            //MaterialSphereOrigin = DiffuseMaterials.Red;

            var sphere2 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere2.AddSphere(new Vector3(0, 0, 0), 2);
            GeometrySphere2 = sphere2.ToMeshGeometry3D();
            MaterialSphere2 = DiffuseMaterials.Yellow;

            var sphere3 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere3.AddSphere(new Vector3(0, 0, 0), 2);
            GeometrySphere3 = sphere3.ToMeshGeometry3D();
            MaterialSphere3 = DiffuseMaterials.Red;

            var antenna = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            antenna.AddBox(Vector3.Zero, 54, 54, 54);
            GeometryAntenna = antenna.ToMeshGeometry3D();
            MaterialAntenna = DiffuseMaterials.Gray;


            GeometrySensor = CreateTruncatedConeGeometry(1, 1, lengthSensor, thetaDivNeedle, new Vector3(1, 0, 0), new Vector3(-lengthSensor / 2.0f, 0, 0), true);
            MaterialSensor = DiffuseMaterials.Black;

            GeometryConnector = CreateTruncatedConeGeometry(3, 3, 21.72f, thetaDivNeedle, new Vector3(1, 0, 0), new Vector3(0, 0, 0), true);
            MaterialConnector = DiffuseMaterials.Gray;

            GeometryTConnector = CreateTruncatedConeGeometry(2, 2, 10, thetaDivNeedle, new Vector3(0, 1, 0), new Vector3(0, 0, 0), true);

            GeometryNeedle = CreateTruncatedConeGeometry(0.4f, 0.1f, lengthNeedle, thetaDivNeedle, new Vector3(1, 0, 0), new Vector3(lengthSensor / 2.0f, 0, 0), true);
            tipNeedleOrigin = new Point3D(lengthNeedle + lengthSensor / 2.0, 0, 0);
            baseNeedleOrigin = new Point3D(lengthSensor / 2.0, 0, 0);
            MaterialNeedle = DiffuseMaterials.Blue;

            GeometryCable = CreateTruncatedConeGeometry(0.5f, 0.5f, -lengthCable, thetaDivNeedle, new Vector3(1, 0, 0), new Vector3(-lengthSensor, 0, 0), true);
            MaterialCable = DiffuseMaterials.Yellow;

            GeometryModel3DOctreeManager octreeManager = new GeometryModel3DOctreeManager();
            octreeManager.Cubify = true;

            if (showSkin)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                //var models = reader.Read(folder + "\\Testa pelle mirror3.obj");
                var models = reader.Read(folder + "\\TestaNew3.obj");

                collisionItemsGuid.Add("SKIN", new List<Guid>());

                for (int i = 0; i < models.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = models[i].Geometry;
                    model.Material = DiffuseMaterials.Gray;
                    //model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    WindowMain.groupSkin.Children.Add(model);
                    collisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
                }
            }

            if (showSkinComplete)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var models = reader.Read(folder + "\\Pelle combinata5.obj");
                collisionItemsGuid.Add("SKIN", new List<Guid>());
                for (int i = 0; i < models.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = models[i].Geometry;
                    model.Material = DiffuseMaterials.Gray;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    WindowMain.groupSkin.Children.Add(model);
                    collisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
                }
            }

            //if (showProcerus)
            //{
            //    var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            //    builder.AddBox(Vector3.Zero, 1, 1, 1);
            //    var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
            //    var modelProcerus = reader.Read(folder + "\\ProcerusSmoothed.obj");
            //    collisionItemsGuid.Add("PROCERUS", new List<Guid>());
            //    for (int i = 0; i < modelProcerus.Count(); i++)
            //    {
            //        var model = new MeshGeometryModel3D();
            //        model.Geometry = modelProcerus[i].Geometry;
            //        model.Material = DiffuseMaterials.Orange;
            //        model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
            //        WindowMain.groupProcerus.Children.Add(model);
            //        collisionItemsGuid["PROCERUS"].Add(model.Geometry.GUID);
            //    }

            //    //GeometryProcerus = modelProcerus[0].Geometry;
            //    //MaterialProcerus = DiffuseMaterials.Orange;
            //    //modelProcerus.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
            //    //builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            //    //builder.AddBox(new Vector3(0, 0, -4), 2, 2, 6);
            //    //RectGeometry = builder.ToMeshGeometry3D();
            //}

            //if (showOrbicularisSuperiorLateral)
            //{
            //    var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            //    builder.AddBox(Vector3.Zero, 1, 1, 1);
            //    var readerOrbicularisSuperiorLateral = new HelixToolkit.Wpf.SharpDX.ObjReader();
            //    var modelOrbicularisSuperiorLateral = readerOrbicularisSuperiorLateral.Read(folder + "\\OrbicularisSuperiorLateralSmoothed.obj");
            //    collisionItemsGuid.Add("ORBICULARIS", new List<Guid>());
            //    for (int i = 0; i < modelOrbicularisSuperiorLateral.Count(); i++)
            //    {
            //        var model = new MeshGeometryModel3D();
            //        model.Geometry = modelOrbicularisSuperiorLateral[i].Geometry;
            //        model.Material = DiffuseMaterials.Orange;
            //        model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
            //        WindowMain.groupOrbicularisSuperiorLateral.Children.Add(model);
            //        collisionItemsGuid["ORBICULARIS"].Add(model.Geometry.GUID);
            //    }
            //}
            //if (showCorrugator)
            //{
            //    var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            //    builder.AddBox(Vector3.Zero, 1, 1, 1);
            //    var readerCorrugator = new HelixToolkit.Wpf.SharpDX.ObjReader();
            //    var modelCorrugators = readerCorrugator.Read(folder + "\\CorrugatorSmoothed.obj");
            //    collisionItemsGuid.Add("CORRUGATOR", new List<Guid>());
            //    for (int i = 0; i < modelCorrugators.Count(); i++)
            //    {
            //        var model = new MeshGeometryModel3D();
            //        model.Geometry = modelCorrugators[i].Geometry;
            //        model.Material = DiffuseMaterials.Orange;
            //        model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
            //        WindowMain.groupCorrugators.Children.Add(model);
            //        collisionItemsGuid["CORRUGATOR"].Add(model.Geometry.GUID);
            //    }
            //}

            //if (showArteries)
            //{
            //    var readerArteries = new HelixToolkit.Wpf.SharpDX.ObjReader();
            //    modelsArteries = readerArteries.Read(folder + "\\Arteries_Cleaned.obj");
            //    collisionItemsGuid.Add("ARTERIES", new List<Guid>());
            //    for (int i = 0; i < modelsArteries.Count(); i++)
            //    {
            //        var model = new MeshGeometryModel3D();
            //        model.Geometry = modelsArteries[i].Geometry;
            //        model.Material = DiffuseMaterials.Red;
            //        model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;

            //        WindowMain.groupArteries.Children.Add(model);
            //        collisionItemsGuid["ARTERIES"].Add(model.Geometry.GUID);
            //    }
            //}

            //if (showVeins)
            //{
            //    GroupModelSourceVeins = new ObservableElement3DCollection();
            //    var readerVeins = new HelixToolkit.Wpf.SharpDX.ObjReader();
            //    modelsVeins = readerVeins.Read(folder + "\\Veins_Cleaned.obj");
            //    collisionItemsGuid.Add("VEINS", new List<Guid>());
            //    for (int i = 0; i < modelsVeins.Count(); i++)
            //    {
            //        var model = new MeshGeometryModel3D();
            //        model.Geometry = modelsVeins[i].Geometry;
            //        model.Material = DiffuseMaterials.Blue;
            //        model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
            //        WindowMain.groupVeins.Children.Add(model);
            //        collisionItemsGuid["VEINS"].Add(model.Geometry.GUID);
            //    }
            //}

            if (showNoSkin)
            {
                GroupModelSourceUnderSkin = new ObservableElement3DCollection();
                var readerUnderSkin = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelsUnderSkin = readerUnderSkin.Read(folder + "\\NoSkin_Complete_CleanFinal.obj");
                for (int i = 0; i < modelsUnderSkin.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsUnderSkin[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;

                    GroupModelSourceUnderSkin.Add(model);
                }
            }
        }
        private void InitializeViewCubes()
        {
            //var builder = new MeshBuilder();
            //builder.AddPyramid(Vector3.Zero, 10, 10, true);
            //ViewCubeGeometry1 = builder.ToMesh();
            //ViewCubeMaterial1 = DiffuseMaterials.Orange;

            //builder = new MeshBuilder();
            //builder.AddDodecahedron(Vector3.Zero, Vector3.UnitX, Vector3.UnitY, 5);
            //ViewCubeGeometry2 = builder.ToMesh();
            //ViewCubeMaterial2 = DiffuseMaterials.Blue;

            //ViewCubeMaterial3 = DiffuseMaterials.Gray;
            //ViewCubeMaterial4 = DiffuseMaterials.Pearl;
            //Center the model first and do scaling
            //var transform = Matrix.Translation(0, -2, 0) * Matrix.Scaling(3.5f);
            //ViewCubeTransform3 = new System.Windows.Media.Media3D.MatrixTransform3D(transform.ToMatrix3D());
        }

        private void PDIClass_OnConnectionStatusChanged(Device.Motion.CONNECTIONSTATUS status)
        {
            SetIndicator(WindowMain.elSensors, status == Device.Motion.CONNECTIONSTATUS.ACQUIRING);
        }

        private void PDIClass_OnNewFrameAvailable(List<Device.Motion.FRAME> frames)
        {
            try
            {
                WindowMain.Dispatcher.Invoke((Action)(() =>
                {
                    if (!alreadyConnectedPolhemus)
                    {
                        alreadyConnectedPolhemus = true;
                        SetIndicator(WindowMain.elSensors, alreadyConnectedPolhemus);
                    }
                    if (true)// (WindowMain.ContentRenderedReally)
                    {
                        if (frames.Count > 0)
                        {
                            f1 = frames[0];
                            XPosSensor02 = f1.Pos.X * 10;
                            YPosSensor02 = f1.Pos.Y * 10;
                            ZPosSensor02 = f1.Pos.Z * 10;
                            RotationSensor_WRS_real = new System.Windows.Media.Media3D.Quaternion(f1.Ori.X, f1.Ori.Y, f1.Ori.Z, f1.Ori.W);

                            //AxisAngleRotation3D T1pre = new AxisAngleRotation3D(new Vector3D(1, 0, 0), RotationAngleOffsetNeedle);
                            //RotateTransform3D T1 = new RotateTransform3D(T1pre);
                            //TranslateTransform3D T2 = new TranslateTransform3D(0, AxisPositionOffsetNeedle, 0);
                            QuaternionRotation3D rotation = new QuaternionRotation3D(RotationSensor_WRS_real);
                            RotateTransform3D T3 = new RotateTransform3D(rotation);
                            TranslateTransform3D T4 = new TranslateTransform3D(XPosSensor02, YPosSensor02, ZPosSensor02);

                            Transform3DGroup transformGroup = new Transform3DGroup();
                            //transformGroup.Children.Add(T2);
                            //transformGroup.Children.Add(T1);
                            transformGroup.Children.Add(T3);
                            transformGroup.Children.Add(T4);

                            TipNeedle = transformGroup.Transform(tipNeedleOrigin);
                            BaseNeedle = transformGroup.Transform(baseNeedleOrigin);

                            TransformSphere2.OffsetX = TipNeedle.X;
                            TransformSphere2.OffsetY = TipNeedle.Y;
                            TransformSphere2.OffsetZ = TipNeedle.Z;

                            TransformSphere3.OffsetX = BaseNeedle.X;
                            TransformSphere3.OffsetY = BaseNeedle.Y;
                            TransformSphere3.OffsetZ = BaseNeedle.Z;

                            OnPropertyChanged(nameof(TransformSphere2));
                            OnPropertyChanged(nameof(TransformSphere3));

                            Vector3D directionNeedle = TipNeedle - BaseNeedle;
                            directionNeedle.Normalize();

                            PitchNeedle = -Math.Atan2(directionNeedle.X, Math.Sqrt(Math.Pow(directionNeedle.Z, 2) + Math.Pow(directionNeedle.Y, 2))) * 180.0 / Math.PI; ;
                            YawNeedle = Math.Atan2(-directionNeedle.Y, -directionNeedle.X) * 180.0 / Math.PI;
                            var viewport = WindowMain.hvView3D;

                            //SharpDX.Ray ray = new Ray(new Vector3((float)TipNeedle.X, (float)TipNeedle.Y, (float)TipNeedle.Z), new SharpDX.Vector3((float)directionNeedle.X, (float)directionNeedle.Y, (float)directionNeedle.Z));
                            SharpDX.Ray ray = new Ray(new Vector3((float)BaseNeedle.X, (float)BaseNeedle.Y, (float)BaseNeedle.Z), new SharpDX.Vector3((float)directionNeedle.X, (float)directionNeedle.Y, (float)directionNeedle.Z));

                            var hitContext = new HitTestContext(viewport.RenderHost.RenderContext, ref ray);
                            List<HitTestResult> hits = new List<HitTestResult>();

                            var rend = viewport.Renderables.ToList();

                            stopwatch.Restart();

                            foreach (var element in rend)
                            {
                                var t = element.GetType().ToString();
                                CheckSceneNode(element, hitContext, ray.Direction, new Vector3((float)TipNeedle.X, (float)TipNeedle.Y, (float)TipNeedle.Z), ref actualPointsHit);//, ref hitNames);
                            }
                            stopwatch.Stop();

                            averageCollisionTime.Add(stopwatch.ElapsedMilliseconds);
                            while (averageCollisionTime.Count > 100)
                            {
                                averageCollisionTime.RemoveAt(0);
                            }

                            double average = averageCollisionTime.Average() / 1000;

                            double minDistance = double.MaxValue;
                            int indexMin = -1;
                            if (actualPointsHit.Count > 0)
                            {
                                for (int i = 0; i < actualPointsHit.Count; i++)
                                {
                                    var distance = CalcDistance(actualPointsHit[i].Point, TipNeedle);
                                    if ((distance >= 0) && (distance < minDistance))
                                    {
                                        minDistance = distance;
                                        indexMin = i;
                                    }
                                }

                                TransformSphere.OffsetX = actualPointsHit[indexMin].Point.X;
                                TransformSphere.OffsetY = actualPointsHit[indexMin].Point.Y;
                                TransformSphere.OffsetZ = actualPointsHit[indexMin].Point.Z;

                                OnPropertyChanged(nameof(TransformSphere));
                                HitStructure = "next: " + actualPointsHit[indexMin].Name + " " + actualPointsHit.Count + " X: " + tipNeedle.X.ToString("00.00") + "; Y: " + tipNeedle.Y.ToString("00.00") + "; Z: " + tipNeedle.Z.ToString("00.00");
                                //Debug.WriteLine(HitStructure);
                            }
                            else
                            {
                                HitStructure = "next: none; X: " + tipNeedle.X.ToString("00.00") + "; Y: " + tipNeedle.Y.ToString("00.00") + "; Z: " + tipNeedle.Z.ToString("00.00");
                            }

                        }
                    }
                }));
            }
            catch (Exception)
            {
            }
        }

        private void PointsListView3D_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (WindowMain.pointsListView3D.SelectedItem != null)
            {
                selectedInjectionPoint = InjectionPoints3DCalib.Where(item => item.PointDefinition.ToString() == ((InjectionPoint3DCalib)(WindowMain.pointsListView3D.SelectedItem)).PointDefinition.ToString()).FirstOrDefault();
                selectedIndexInjectionPoint = WindowMain.pointsListView3D.SelectedIndex;
                UpdateVisualizationPoints();
            }
            else
            {
                selectedIndexInjectionPoint = -1;
            }
        }

        private void SaveDynamicInfo3DXML(string nameFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<InjectionPoint3D>));
            List<InjectionPoint3DCalib> toSave = new List<InjectionPoint3DCalib>();
            foreach (var item in InjectionPoints3DCalib)
            {
                toSave.Add(item);
            }

            // Serialize to file

            using (FileStream fileStream = new FileStream(nameFile, FileMode.Create))
            {
                serializer.Serialize(fileStream, toSave);
                Console.WriteLine($"Serialized to XML file: {nameFile}");
            }
        }

        private void TimerCheckPolhemusTick_Listener(object sender, EventArgs e)
        {
            //if ((DateTime.Now - lastArrivedInput).TotalSeconds > 0.5)
            //{
            //    alreadyConnectedPolhemus = false;
            //    SetIndicator(WindowMain.elSensors, alreadyConnectedPolhemus);
            //}
        }

        private void TimerRotation_Tick1(object sender, EventArgs e)
        {
            // Update the rotation angle offset (e.g., increment it by 1 degree)
            rotationTimer += 10;
            //XPosSensor02 += 1;
            //RotationSensor_WRS_real = CreateRotationQuaternionAlongXAxis(rotationTimer);
        }

        private void UpdateVisualizationPoints()
        {
            for (int i = 0; i < InjectionPoints3DCalib.Count; i++)
            {
                if (InjectionPoints3DCalib[i].Assigned)
                {
                    if (InjectionPoints3DCalib[i].IsError)
                    {
                        pointsToBeShown[i].Material = DiffuseMaterials.Red;
                    }
                    else
                    {
                        pointsToBeShown[i].Material = DiffuseMaterials.Green;
                    }
                }
                if (i == selectedIndexInjectionPoint)
                {
                    var meshBuilder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                    meshBuilder.AddSphere(new Vector3(0, 0, 0), 4);
                    var mesh = meshBuilder.ToMeshGeometry3D();
                    pointsToBeShown[i].Geometry = mesh;
                }
                else
                {
                    var meshBuilder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                    meshBuilder.AddSphere(new Vector3(0, 0, 0), 2);
                    var mesh = meshBuilder.ToMeshGeometry3D();
                    pointsToBeShown[i].Geometry = mesh;
                }
            }
        }
    }
}