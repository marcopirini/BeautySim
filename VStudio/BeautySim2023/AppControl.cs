using BeautySim.Common;
using BeautySim2023.DataModel;
using BeautySim2023.Windows;
using Device.BeautySim;
using Device.Motion;
using Device.Polhemus;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml.Serialization;
using HelixToolkitException = HelixToolkit.Wpf.SharpDX.HelixToolkitException;
using HitTestResult = HelixToolkit.Wpf.SharpDX.HitTestResult;

namespace BeautySim2023
{
    public class AppControl : IDisposable, INotifyPropertyChanged
    {
        public const string ADMIN_USERNAME = "Accurate.Admin";
        public const string BeautySim_USERNAME = "Accurate.BeautySim";
        public const string CadsFolder = "C:\\BeautySim\\Models3D";
        public const string CasesFolder = "C:\\BeautySim\\Cases";
        public const string CoordFile = "C:\\BeautySim\\CalibrationFile.txt";
        public const string DBPath = "C:\\BeautySim\\Database\\BeautySimDB.db";
        public const string Orthographic = "Orthographic Camera";
        public const string PathFilePointsBase3D = "C:\\BeautySim\\BasePointsDefinitions3D.xml";
        public const string Perspective = "Perspective Camera";
        public const string ResultsTempFolder = "C:\\BeautySim\\ResultsTemp";
        public const string SaveReportsFolder = "C:\\BeautySim_SavedReports";
        public const string SaveResultsFolder = "C:\\BeautySim\\DataBase\\Results\\";
        public Brush ActiveEllipse = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 150, 136));
        public List<PointHit> ActualPointsHit = new List<PointHit>();
        public List<ClinicalCase> AvailableCases;
        public Point3D BaseNeedleOrigin;

        public ObservableCollection<ClinicalCase> Cases;
        public bool CheckCollisions = false;
        public Dictionary<string, List<Guid>> CollisionItemsGuid = new Dictionary<string, List<Guid>>();
        public ClinicalCase CurrentCase = null;
        public CurrentCaseStateChangedDelegate CurrentCaseStateChangedEvent;
        public Events CurrentEvent = null;
        public string CurrentModule = null;
        public Users CurrentStudent = null;
        public Users CurrentTeacher = null;

        public bool DemoMode = true;
        public FRAME F1;
        public bool HasBeenLoadedVis3DFrame = false;
        public float Height = 100;
        public int intervalMsTimer = 30;
        public DateTime lastMouseMove;
        public string LastSerial;
        public int LengthCable = 50;
        public int LengthSensor = 14;
        public double LimitAngleDeg = 10;
        public ObservableCollection<string> Modules;
        public ObservableCollection<string> ModulesNames;
        public bool mouseClickOrMove = false;
        public SharpDX.Vector3 NedleAreaEnd = new SharpDX.Vector3(0, 0, 0);
        public List<VectorMath.Vector3> needlePoints_NRS;
        public List<VectorMath.Vector3> needlePoints_WRS;
        public int numPointsNeedle;
        public int NumPointsX = 100;
        public int NumPointsY = 100;
        public Users OldTeacher = null;
        public ObservableCollection<MeshGeometryModel3D> PointsToBeShown = new ObservableCollection<MeshGeometryModel3D>();
        public int SelectedIndexInjectionPointCalibration;
        public InjectionPoint3DCalib SelectedInjectionPointCalibration;
        public bool StopLoadCommand = false;
        public string StringCurrentCase = BeautySim.Globalization.Language.str_curr_case;
        public string StringLoadCase = BeautySim.Globalization.Language.str_sel_load_case;
        public string StringNeedleInjCorrectTarget = BeautySim.Globalization.Language.str_great;
        public string StringNeedleInjTakeAim = BeautySim.Globalization.Language.str_go_ahead;
        public string StringNeedleInjWrongTarget = BeautySim.Globalization.Language.str_oh_no;
        public List<string> StringSkinAreas = new List<string>();
        public int ThetaDivNeedle = 18;
        public Point3D TipNeedleOrigin;
        public Point3D TraslationFromSensor02ToNeedleStart = new Point3D(10, 0, 0);
        public float Width = 100;
        private static AppControl instance = null;
        private float additionalCorrectionLengthNeedle;
        private bool alreadyConnectedPolhemus;
        private bool arteriesVisible;
        //private float axisPositionOffsetNeedle = 0f;

        private Point3D baseNeedle;
        private HelixToolkit.Wpf.SharpDX.Camera camera;
        private string cameraModel;
        private ClinicalCaseStep caseStepCurrent;
        private System.Windows.Forms.Timer caseTimer;
        private Point3D collidePointNeedle;
        private bool corrugatorVsible;
        private decimal CurrentCaseScore = 0;
        private Enum_CaseState currentCaseState;
        private HelixToolkit.Wpf.SharpDX.OrthographicCamera defaultOrthographicCamera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 1, FarPlaneDistance = 100 };
        private HelixToolkit.Wpf.SharpDX.PerspectiveCamera defaultPerspectiveCamera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 0.5, FarPlaneDistance = 150 };
        private double depthImage;
        private double depthInjection;
        private bool disposedValue = false;
        private IEffectsManager effectsManager;
        private int fsamp = 20;
        private string hitStructure;
        private Point3D impactPointToShow;
        private int insertionNeedle;
        private string insertionNeedleText;
        private DateTime lastArrivedInput;
        private float lengthNeedle;
        private bool loadedCase;
        private CaseStudentFrame MainStudentFrame;
        private CaseTeacherFrame MainTeacherFrame;
        private HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelMask;
        private HelixToolkit.Wpf.SharpDX.DiffuseMaterial modelMaterialMask;
        private bool needleAlignedCorrectly;
        private bool needleAngleOk;
        private System.Windows.Media.Color needleColor;
        private System.Windows.Media.Color needleIndicationColor;

        private System.Windows.Media.Color needleTipIndicationColor;
        private bool orbicularisVisible;

        private Point3D originPlaneImage;
        public string pathFilePointsBase3DCalib = "C:\\BeautySim\\BasePointsDefinitions3DCalib.xml";
        private PDIClass pDIClass;
        private double pitchNeedle;
        private double pitchSensor02;
        private Point3D pointEntrance;
        private List<PointHit> previousPointsHit = new List<PointHit>();
        private bool procerusVisible = true;
        private double rollSensor02;
        private System.Windows.Media.Media3D.Quaternion rotation_Manikin;

        //private float rotationAngleOffsetNeedle = 0;
        private System.Windows.Media.Media3D.Quaternion rotationNeedle_WRS_m;

        private System.Windows.Media.Media3D.Quaternion rotationOperableEnsamble;
        private System.Windows.Media.Media3D.Quaternion rotationSensor_WRS;
        private bool showIndicationNeedle;
        private bool showTargetPosition;
        private bool skinVisible;
        private Page StudentFrame;
        private string subTitle;
        private Color targetSpecialNerveColor;
        private Page TeacherFrame;
        private string textNeedlePuntures;
        private string textNeedlePunturesExp;
        private string textNerveTarget;
        private string textNerveTargetExp;
        private string textNerveWrong;
        private string textNerveWrongExp;
        private string textNerveWrongIntro;
        private string textScore;
        private string textTime;
        private string textTimeExp;
        private string textToShowYawPitch;
        private string textVascularWrong;
        private string textVascularWrongExp;
        private string textVascularWrongIntro;
        private string textWrongInjections;
        private string textWrongInjectionsExp;
        private DispatcherTimer timerCheckPolhemus;
        private int timeStepSimulation = 50;
        private Point3D tipNeedle;
        private string title;
        private Vector3D upDirection;
        private bool veinsVisible;
        private Visibility visibilityBorderArrowDown;
        private Visibility visibilityBorderArrowLeft;
        private Visibility visibilityBorderArrowRight;
        private Visibility visibilityBorderArrowRotation;
        private Visibility visibilityBorderArrowRotation2;
        private Visibility visibilityBorderArrowUp;
        private Visibility visibilityLogoOnStudentPage;
        private bool visibilityNeedleCollisionCheck = false;
        private Visibility visibilityResults;
        private Visibility visibleLine;
        private double widthImage;
        private float xPosOperableEnsamble;
        private float xPosSensor02;
        private float xTipNeedle = 0;
        private double yawNeedle;
        private double yawSensor02;
        private float yPosOperableEnsamble;
        private float yPosSensor02;
        private float yTipNeedle = 0;
        private float zPosOperableEnsamble;
        private float zPosSensor02;
        private float zTipNeedle = 0;

        private AppControl()
        {
            Cases = new ObservableCollection<ClinicalCase>();

            // camera models
            CameraModelCollection = new List<string>()
            {
                Orthographic,
                Perspective,
            };

            // on camera changed callback
            CameraModelChanged += (s, e) =>
            {
                if (cameraModel == Orthographic)
                {
                    if (!(Camera is HelixToolkit.Wpf.SharpDX.OrthographicCamera))
                    {
                        Camera = defaultOrthographicCamera;
                    }
                }
                else if (cameraModel == Perspective)
                {
                    if (!(Camera is HelixToolkit.Wpf.SharpDX.PerspectiveCamera))
                    {
                        Camera = defaultPerspectiveCamera;
                    }
                }
                else
                {
                    throw new HelixToolkitException(BeautySim.Globalization.Language.str_camera_model_err);
                }
            };

            // default camera model
            CameraModel = Perspective;

            Title = BeautySim.Globalization.Language.str_demo_helix;
            SubTitle = BeautySim.Globalization.Language.str_def_base_view_mod;

            LastIndexSelectedCase = 0;
            LastIndexSelectedModule = 0;

            ModulesActivation = new bool[Enum.GetNames(typeof(Enum_Modules)).Length];

            for (int i = 0; i < ModulesActivation.Length; i++)
            {
                ModulesActivation[i] = false;
            }

            LastSerial = string.Empty;

            AvailableCases = new List<ClinicalCase>();
            //DummyCase = false;
            caseTimer = new System.Windows.Forms.Timer();
            timeStepSimulation = (int)(Math.Ceiling((1000.0 / fsamp)));
            caseTimer.Interval = timeStepSimulation;
            caseTimer.Tick += new EventHandler(CaseTimeTickListener);
            SetTimer(false);

            InjectionPoints3DCalib = new ObservableCollection<InjectionPoint3DCalib>();
            CollisionItemsGuid = new Dictionary<string, List<Guid>>();
        }

        ~AppControl()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public delegate void ChangedTimerStateDelegate(bool active);

        public delegate void CurrentCaseStateChangedDelegate();

        public delegate void ReportCompletedDelegate();

        public delegate void ReportProgressDelegate(int percentage);

        private delegate void MouseMoveDelegate();

        private delegate void SetEnabledDelegate(UIElement lbl, bool enabled);

        private delegate void SetImageDelegate(float err);

        private delegate void SetIndicatorDelegate(System.Windows.Shapes.Ellipse bb, bool aactive);

        private delegate void SetTextDelegate(TextBlock lbl, string text);

        public event EventHandler CameraModelChanged;

        public event ChangedTimerStateDelegate ChangeTimerStateEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        public static AppControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppControl();
                }
                return instance;
            }
        }

        public float AdditionalCorrectionLengthNeedle
        {
            get
            {
                return additionalCorrectionLengthNeedle;
            }
            set
            {
                additionalCorrectionLengthNeedle = value;
                RaisePropertyChanged("AdditionalCorrectionLengthNeedle");
            }
        }

        public bool AreImagesVisualizedOn3D { get; internal set; }

        public bool ArteriesVisible
        {
            get { return arteriesVisible; }
            set
            {
                arteriesVisible = value;
                OnPropertyChanged(nameof(ArteriesVisible));
            }
        }

        private List<MeshGeometryModel3D> pointsToBeShown = new List<MeshGeometryModel3D>();
        private float fluxLiquid;
        private float volumeLiquid;

        //public float AxisPositionOffsetNeedle
        //{
        //    get { return axisPositionOffsetNeedle; }
        //    set
        //    {
        //        axisPositionOffsetNeedle = value;
        //        OnPropertyChanged(nameof(AxisPositionOffsetNeedle));
        //    }
        //}

        internal void InitCalibrationEnvironment(Calib3DWindow window3D)
        {
            //Read current calibration tralsation parameters
            using (StreamReader sr = new StreamReader(AppControl.CoordFile))
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
                AppControl.Instance.LengthNeedle = (float)lengthNeedleR;
            }

            CalibrationWindow = window3D;
            CalibrationWindow.DataContext = AppControl.Instance;

            var hwnd2 = new WindowInteropHelper(window3D).Handle;
            Calib3DWindow.SetWindowLong(hwnd2, Calib3DWindow.GWL_STYLE, Calib3DWindow.GetWindowLong(hwnd2, Calib3DWindow.GWL_STYLE) & ~Calib3DWindow.WS_SYSMENU);

            CalibrationWindow.pointsListView3D.Visibility = Visibility.Visible;
            //CalibrationWindow.gmModels.Transform = new TranslateTransform3D(0, 0, 0);

            ObservableCollection<InjectionPoint3D> temporaryList = new ObservableCollection<InjectionPoint3D>();
            if (File.Exists(AppControl.Instance.pathFilePointsBase3DCalib))
            {
                temporaryList = PointsManager.Instance.LoadInjectionPoints3D(AppControl.Instance.pathFilePointsBase3DCalib);
            }
            AppControl.Instance.InjectionPoints3DCalib.Clear();
            foreach (InjectionPoint3D item in temporaryList)
            {
                AppControl.Instance.InjectionPoints3DCalib.Add(new InjectionPoint3DCalib(item));
            }

            CalibrationWindow.pointsListView3D.SelectionChanged += PointsListView3DCalibration_SelectionChanged;
            CalibrationWindow.pointsListView3D.SelectedIndex = 0;

            for (int i = 0; i < AppControl.Instance.InjectionPoints3DCalib.Count; i++)
            {
                MeshGeometryModel3D aa = new MeshGeometryModel3D();
                aa.CullMode = SharpDX.Direct3D11.CullMode.Back;

                var sphereA = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                sphereA.AddSphere(new SharpDX.Vector3(0, 0, 0), 2);
                HelixToolkit.Wpf.SharpDX.Geometry3D geometrySphere = sphereA.ToMeshGeometry3D();
                HelixToolkit.Wpf.SharpDX.Material MaterialSphere = DiffuseMaterials.Blue;
                aa.Geometry = geometrySphere;
                aa.Material = MaterialSphere;

                Transform3DGroup transformGroupRotation = new Transform3DGroup();
                transformGroupRotation.Children.Add(new RotateTransform3D(RotationManikin3D));
                var translateTransform = new TranslateTransform3D(0, 0, 0);

                Point3D rotated = AppControl.Instance.InjectionPoints3DCalib[i].GetRotoTransatedPoint(transformGroupRotation);
                AppControl.Instance.InjectionPoints3DCalib[i].XRotated = rotated.X;
                AppControl.Instance.InjectionPoints3DCalib[i].YRotated = rotated.Y;
                AppControl.Instance.InjectionPoints3DCalib[i].ZRotated = rotated.Z;

                if (AppControl.Instance.InjectionPoints3DCalib[i].Assigned)
                {
                    translateTransform = new TranslateTransform3D(AppControl.Instance.InjectionPoints3DCalib[i].XRotated, AppControl.Instance.InjectionPoints3DCalib[i].YRotated, AppControl.Instance.InjectionPoints3DCalib[i].ZRotated);
                }
                //transformGroup.Children.Add(translateTransform);
                aa.Transform = translateTransform;
                pointsToBeShown.Add(aa);

                CalibrationWindow.hvView3D.Items.Add(aa);
            }

            EffectsManager = new DefaultEffectsManager();

            AppControl.Instance.CollisionItemsGuid = new Dictionary<string, List<Guid>>();

            var sphere = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere.AddSphere(new SharpDX.Vector3(0, 0, 0), 2);
            AppControl.Instance.GeometrySphere = sphere.ToMeshGeometry3D();
            AppControl.Instance.MaterialSphere = DiffuseMaterials.Green;

            var sphere2 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere2.AddSphere(new SharpDX.Vector3(0, 0, 0), 2);
            AppControl.Instance.GeometrySphere2 = sphere2.ToMeshGeometry3D();
            AppControl.Instance.MaterialSphere2 = DiffuseMaterials.Yellow;

            var antenna = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            antenna.AddBox(SharpDX.Vector3.Zero, 54, 54, 54);
            AppControl.Instance.MaterialAntenna = DiffuseMaterials.Gray;

            AppControl.Instance.GeometrySensor = AppControl.Instance.CreateTruncatedConeGeometry(1, 1, AppControl.Instance.LengthSensor, AppControl.Instance.ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(-AppControl.Instance.LengthSensor / 2.0f, 0, 0), true);
            AppControl.Instance.MaterialSensor = DiffuseMaterials.Black;

            AppControl.Instance.GeometryConnector = AppControl.Instance.CreateTruncatedConeGeometry(3, 3, 21.72f, AppControl.Instance.ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(0, 0, 0), true);
            AppControl.Instance.MaterialConnector = DiffuseMaterials.Gray;

            AppControl.Instance.GeometryTConnector = AppControl.Instance.CreateTruncatedConeGeometry(2, 2, 10, AppControl.Instance.ThetaDivNeedle, new SharpDX.Vector3(0, 1, 0), new SharpDX.Vector3(0, 0, 0), true);

            AppControl.Instance.GeometryNeedle = AppControl.Instance.CreateTruncatedConeGeometry(0.4f, 0.1f, AppControl.Instance.LengthNeedle, AppControl.Instance.ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(AppControl.Instance.LengthSensor / 2.0f, 0, 0), true);
            AppControl.Instance.TipNeedleOrigin = new Point3D(AppControl.Instance.LengthNeedle + AppControl.Instance.LengthSensor / 2.0, 0, 0);
            AppControl.Instance.BaseNeedleOrigin = new Point3D(AppControl.Instance.LengthSensor / 2.0, 0, 0);
            AppControl.Instance.MaterialNeedle = DiffuseMaterials.Blue;

            AppControl.Instance.GeometryCable = AppControl.Instance.CreateTruncatedConeGeometry(0.5f, 0.5f, -AppControl.Instance.LengthCable, AppControl.Instance.ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(-AppControl.Instance.LengthSensor, 0, 0), true);
            AppControl.Instance.MaterialCable = DiffuseMaterials.Yellow;

            GeometryModel3DOctreeManager octreeManager = new GeometryModel3DOctreeManager();
            octreeManager.Cubify = true;

            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            builder.AddBox(SharpDX.Vector3.Zero, 1, 1, 1);
            var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
            var models = reader.Read("C:\\BeautySim\\Models3D" + "\\TestaNew3.obj");

            AppControl.Instance.CollisionItemsGuid.Add("SKIN", new List<Guid>());

            for (int i = 0; i < models.Count(); i++)
            {
                var model = new MeshGeometryModel3D();
                model.Geometry = models[i].Geometry;
                model.Material = DiffuseMaterials.Gray;
                CalibrationWindow.groupSkin.Children.Add(model);
                AppControl.Instance.CollisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
            }

            InitializeCoordinates();
        }

        private void PointsListView3DCalibration_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CalibrationWindow.pointsListView3D.SelectedItem != null)
            {
                AppControl.Instance.SelectedInjectionPointCalibration = AppControl.Instance.InjectionPoints3DCalib.Where(item => item.PointDefinition.ToString() == ((InjectionPoint3DCalib)(CalibrationWindow.pointsListView3D.SelectedItem)).PointDefinition.ToString()).FirstOrDefault();
                AppControl.Instance.SelectedIndexInjectionPointCalibration = CalibrationWindow.pointsListView3D.SelectedIndex;
                UpdateVisualizationPointsCalibration();
            }
            else
            {
                AppControl.Instance.SelectedIndexInjectionPointCalibration = -1;
            }
        }

        private void SaveDynamicInfo3DXML(string nameFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<InjectionPoint3D>));
            List<InjectionPoint3DCalib> toSave = new List<InjectionPoint3DCalib>();
            foreach (var item in AppControl.Instance.InjectionPoints3DCalib)
            {
                toSave.Add(item);
            }

            using (FileStream fileStream = new FileStream(nameFile, FileMode.Create))
            {
                serializer.Serialize(fileStream, toSave);
                Console.WriteLine($"Serialized to XML file: {nameFile}");
            }
        }

        private void UpdateVisualizationPointsCalibration()
        {
            for (int i = 0; i < AppControl.Instance.InjectionPoints3DCalib.Count; i++)
            {
                if (AppControl.Instance.InjectionPoints3DCalib[i].Assigned)
                {
                    if (AppControl.Instance.InjectionPoints3DCalib[i].IsError)
                    {
                        pointsToBeShown[i].Material = DiffuseMaterials.Red;
                    }
                    else
                    {
                        pointsToBeShown[i].Material = DiffuseMaterials.Green;
                    }
                }
                if (i == AppControl.Instance.SelectedIndexInjectionPointCalibration)
                {
                    var meshBuilder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                    meshBuilder.AddSphere(new SharpDX.Vector3(0, 0, 0), 4);
                    var mesh = meshBuilder.ToMeshGeometry3D();
                    pointsToBeShown[i].Geometry = mesh;
                }
                else
                {
                    var meshBuilder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                    meshBuilder.AddSphere(new SharpDX.Vector3(0, 0, 0), 2);
                    var mesh = meshBuilder.ToMeshGeometry3D();
                    pointsToBeShown[i].Geometry = mesh;
                }
            }
        }

        internal void Calibrate()
        {
            if (Properties.Settings.Default.CalibrationMode == "Mean")
            {
                var averageXAssigned = AppControl.Instance.InjectionPoints3DCalib.Average(point => point.XAssigned);
                var averageYAssigned = AppControl.Instance.InjectionPoints3DCalib.Average(point => point.YAssigned);
                var averageZAssigned = AppControl.Instance.InjectionPoints3DCalib.Average(point => point.ZAssigned);
                var averageX = AppControl.Instance.InjectionPoints3DCalib.Average(point => point.XRotated);
                var averageY = AppControl.Instance.InjectionPoints3DCalib.Average(point => point.YRotated);
                var averageZ = AppControl.Instance.InjectionPoints3DCalib.Average(point => point.ZRotated);

                // File path chosen by the user
                string filePath = AppControl.CoordFile;

                // Write the averages to the file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine((averageXAssigned - averageX).ToString("00.00", new CultureInfo("en-US")));
                    writer.WriteLine((averageYAssigned - averageY).ToString("00.00", new CultureInfo("en-US")));
                    writer.WriteLine((averageZAssigned - averageZ).ToString("00.00", new CultureInfo("en-US")));
                    writer.WriteLine(AppControl.Instance.LengthNeedle.ToString("00.00", new CultureInfo("en-US")));
                }
            }
            else
            {
            }
        }

        public Calib3DWindow CalibrationWindow { get; internal set; }

        public Point3D BaseNeedle
        {
            get { return baseNeedle; }
            set
            {
                baseNeedle = value;
                OnPropertyChanged(nameof(BaseNeedle));
            }
        }

        public bool BTYSimulatorConnected { get; private set; }

        public HelixToolkit.Wpf.SharpDX.Camera Camera
        {
            get
            {
                return camera;
            }

            protected set
            {
                SetValue(ref camera, value, "Camera");
                CameraModel = value is HelixToolkit.Wpf.SharpDX.PerspectiveCamera
                                       ? Perspective
                                       : value is HelixToolkit.Wpf.SharpDX.OrthographicCamera ? Orthographic : null;
            }
        }

        public string CameraModel
        {
            get
            {
                return cameraModel;
            }
            set
            {
                if (SetValue(ref cameraModel, value, "CameraModel"))
                {
                    OnCameraModelChanged();
                }
            }
        }

        public List<string> CameraModelCollection { get; private set; }

        public Point3D CollidePointNeedle
        {
            get
            {
                return collidePointNeedle;
            }
            set
            {
                collidePointNeedle = value;
                RaisePropertyChanged("CollidePointNeedle");
            }
        }

        public LineGeometry3D Coordinate { set; get; }
        public BillboardText3D CoordinateText { set; get; }

        public bool CorrugatorVisible
        {
            get { return corrugatorVsible; }
            set
            {
                corrugatorVsible = value;
                OnPropertyChanged(nameof(CorrugatorVisible));
            }
        }

        public Enum_AreaDefinition CurrentArea { get; private set; }

        public Enum_CaseState CurrentCaseState
        {
            get
            {
                return currentCaseState;
            }
            set
            {
                currentCaseState = value;
                if (CurrentCaseStateChangedEvent != null)
                {
                    CurrentCaseStateChangedEvent();
                }
            }
        }

        public bool CurrentLocateMouseSOrT { get; private set; }

        public string CurrentModel { get; internal set; }

        public double DepthImage_mm
        {
            get { return depthImage; }
            set
            {
                depthImage = value;
                RaisePropertyChanged("DepthImage");
            }
        }

        public double DepthInjection
        {
            get
            {
                return depthInjection;
            }
            set
            {
                depthInjection = value;
                OnPropertyChanged(nameof(DepthInjection));
            }
        }

        public double Diameter3DPoints { get; internal set; } = 2;

        public TimeSpan DurationNeedlePhase { get; private set; }

        public IEffectsManager EffectsManager
        {
            get { return effectsManager; }
            protected set
            {
                SetValue(ref effectsManager, value);
            }
        }

        public ObservableCollection<EntranceData> EntranceDataList { get; set; }

        public string EvaluateTheCorrectAnswers { get; private set; } = "Evaluate the correct answers";

        public string EvaluateTheCorrectAnswersTeacher { get; private set; } = "The student is evaluation feedbacks on the answers provided";

        public bool Feedback3DOn { get; set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryAntenna { set; get; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryCable { get; set; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryConnector { get; set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryHeadSkin { set; get; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryNeedle { get; set; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometrySensor { get; set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere { get; set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere2 { set; get; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryTConnector { get; set; }

        public bool HasBeen3DPointsLoaded { get; private set; }

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

        public Point3D ImpactPointToShow
        {
            get { return impactPointToShow; }
            set
            {
                impactPointToShow = value;
                OnPropertyChanged(nameof(ImpactPointToShow));
            }
        }

        public int IndexRealFinishNeedle { get; internal set; }

        public ObservableCollection<InjectionPointSpecific2D> InjectionPoints2D { get; set; }

        public ObservableCollection<InjectionPoint3D> InjectionPoints3D { get; set; }

        public ObservableCollection<InjectionPoint3DCalib> InjectionPoints3DCalib { get; set; }

        public ObservableCollection<InjectionPoint3D> InjectionPoints3DThisStep { get; set; }

        public int InsertionNeedle
        {
            get
            {
                return insertionNeedle;
            }
            set
            {
                if (insertionNeedle != value)
                {
                    insertionNeedle = value;
                    InsertionNeedleText = insertionNeedle.ToString();
                    RaisePropertyChanged("InsertionNeedle");
                }
            }
        }

        public string InsertionNeedleText
        {
            get
            {
                return insertionNeedleText;
            }
            set
            {
                if (insertionNeedleText != value)
                {
                    insertionNeedleText = value;
                    RaisePropertyChanged("InsertionNeedleText");
                }
            }
        }

        public VectorMath.Vector3 InsertionPointNeedle_WRS { get; private set; }

        public DateTime LastCollisionNeedleDetected { get; private set; }

        public int LastIndexSelectedCase { get; internal set; }

        public int LastIndexSelectedModule { get; internal set; }

        public DateTime LastOrientationBeautySimArrived { get; private set; }

        public DateTime LastTimeInjection { get; private set; }

        public float LengthNeedle
        {
            get
            {
                return lengthNeedle;
            }
            set
            {
                lengthNeedle = value;
                RaisePropertyChanged("LengthNeedle");
            }
        }

        public bool LoadedCase
        {
            get
            {
                return loadedCase;
            }
            set
            {
                loadedCase = value;
                RaisePropertyChanged("LoadedCase");
            }
        }

        public bool LoadingCase { get; internal set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialAntenna { set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialCable { get; set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialConnector { get; set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialHeadSkin { set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialNeedle { get; set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialProcerus { set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialSensor { get; set; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere { set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere2 { set; get; }

        public string MessageCheckingFeedbacks { get; private set; } = "Check the feedback of your choices";

        public string MessageCheckingFeedbacksTeacher { get; private set; } = "The student is checking the feedback of its choices";

        public string MessageFinalScoreStep { get; private set; } = "Final score for this step";

        public double MinutesNeedlePhase { get; private set; }

        public bool Model3DInitialized { get; set; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D ModelMask
        {
            get
            {
                return modelMask;
            }
            set
            {
                SetValue(ref modelMask, value, "ModelMask");
            }
        }

        public HelixToolkit.Wpf.SharpDX.DiffuseMaterial ModelMaterialMask
        {
            get
            {
                return modelMaterialMask;
            }
            set
            {
                SetValue(ref modelMaterialMask, value, "ModelMaterialMask");
            }
        }

        public bool[] ModulesActivation { get; private set; }

        public bool ModulesChecked { get; set; }

        public bool NeedleAlignedCorrectly
        {
            get { return needleAlignedCorrectly; }
            set
            {
                needleAlignedCorrectly = value;
                RaisePropertyChanged("NeedleAlignedCorrectly");
            }
        }

        public bool NeedleAngleOk
        {
            get { return needleAngleOk; }
            set
            {
                needleAngleOk = value;
                RaisePropertyChanged("NeedleAngleOk");
            }
        }

        public System.Windows.Media.Color NeedleColor
        {
            get
            {
                return needleColor;
            }
            set
            {
                if (value != needleColor)
                {
                    needleColor = value;
                    RaisePropertyChanged("NeedleColor");
                }
            }
        }

        public bool NeedleHits { get; private set; }

        public System.Windows.Media.Color NeedleIndicationColor
        {
            get
            {
                return needleIndicationColor;
            }
            set
            {
                if (value != needleIndicationColor)
                {
                    needleIndicationColor = value;
                    RaisePropertyChanged("NeedleIndicationColor");
                }
            }
        }

        public System.Windows.Media.Color NeedleTipIndicationColor
        {
            get
            {
                return needleTipIndicationColor;
            }
            set
            {
                if (value != needleTipIndicationColor)
                {
                    needleTipIndicationColor = value;
                    RaisePropertyChanged("NeedleTipIndicationColor");
                }
            }
        }

        public bool OrbicularisVisible
        {
            get { return orbicularisVisible; }
            set
            {
                orbicularisVisible = value;
                OnPropertyChanged(nameof(OrbicularisVisible));
            }
        }

        public string OrientationBeautySim { get; private set; }

        public Point3D OriginPlaneImage
        {
            get { return originPlaneImage; }
            set
            {
                originPlaneImage = value;
                RaisePropertyChanged("OriginPlaneImage");
            }
        }

        public double PitchNeedle
        {
            get { return pitchNeedle; }
            set
            {
                pitchNeedle = value;
                OnPropertyChanged(nameof(PitchNeedle));
            }
        }

        public double PitchSensor02
        {
            get { return pitchSensor02; }
            set
            {
                pitchSensor02 = value;
                RaisePropertyChanged("PitchSensor02");
            }
        }

        public Point3D PointEntrance
        {
            get { return pointEntrance; }
            set
            {
                pointEntrance = value;
                OnPropertyChanged(nameof(PointEntrance));
            }
        }

        public bool ProcerusVisible
        {
            get { return procerusVisible; }
            set
            {
                procerusVisible = value;
                OnPropertyChanged(nameof(ProcerusVisible));
            }
        }

        public float RatioVisualizationMMperPixel { get; private set; } = 10;

        public double RollSensor02
        {
            get { return rollSensor02; }
            set
            {
                rollSensor02 = value;
                RaisePropertyChanged("RollSensor02");
            }
        }

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

        public QuaternionRotation3D RotationManikin3D { get; private set; }

        public System.Windows.Media.Media3D.Quaternion RotationNeedle_WRS_real
        {
            get { return rotationNeedle_WRS_m; }
            set
            {
                rotationNeedle_WRS_m = value;
                RaisePropertyChanged("RotationNeedle_WRS_real");
            }
        }

        public System.Windows.Media.Media3D.Quaternion RotationNeedle_WRS_WhenInsertion { get; private set; }

        public System.Windows.Media.Media3D.Quaternion RotationOperableEnsamble
        {
            get { return rotationOperableEnsamble; }
            set
            {
                rotationOperableEnsamble = value;
                RaisePropertyChanged("RotationOperableEnsamble");
            }
        }

        public System.Windows.Media.Media3D.Quaternion RotationSensor_WRS_real
        {
            get { return rotationSensor_WRS; }
            set
            {
                rotationSensor_WRS = value;
                OnPropertyChanged(nameof(RotationSensor_WRS_real));
            }
        }

        public int SelectedPoint3DIndex { get; private set; }

        public ShowImagesWindow ShowImagesOn3D { get; internal set; }

        public bool ShowIndicationNeedle
        {
            get { return showIndicationNeedle; }
            set
            {
                showIndicationNeedle = value;
                RaisePropertyChanged("ShowIndicationNeedle");
            }
        }

        public bool ShowInfoToStudent { get; internal set; } = false;

        public bool ShowTargetPosition
        {
            get { return showTargetPosition; }
            set
            {
                showTargetPosition = value;
                RaisePropertyChanged("ShowTargetPosition");
            }
        }

        public bool SkinVisible
        {
            get { return skinVisible; }
            set
            {
                skinVisible = value;
                OnPropertyChanged(nameof(SkinVisible));
            }
        }

        public DateTime StartNeedlePhaseTime { get; private set; }

        public string SubTitle
        {
            get
            {
                return subTitle;
            }
            set
            {
                SetValue(ref subTitle, value, "SubTitle");
            }
        }

        public int TargetMissedNerve_Injections { get; private set; }

        public System.Windows.Media.Color TargetSpecialNerveColor
        {
            get
            {
                return targetSpecialNerveColor;
            }
            set
            {
                if (value != targetSpecialNerveColor)
                {
                    targetSpecialNerveColor = value;
                    RaisePropertyChanged("TargetSpecialNerveColor");
                }
            }
        }

        public string TextNeedlePuntures
        {
            get { return textNeedlePuntures; }
            set
            {
                textNeedlePuntures = value;
                RaisePropertyChanged("TextNeedlePuntures");
            }
        }

        public string TextNeedlePunturesExp
        {
            get { return textNeedlePunturesExp; }
            set
            {
                textNeedlePunturesExp = value;
                RaisePropertyChanged("TextNeedlePunturesExp");
            }
        }

        public string TextNerveTarget
        {
            get { return textNerveTarget; }
            set
            {
                textNerveTarget = value;
                RaisePropertyChanged("TextNerveTarget");
            }
        }

        public string TextNerveTargetExp
        {
            get { return textNerveTargetExp; }
            set
            {
                textNerveTargetExp = value;
                RaisePropertyChanged("TextNerveTargetExp");
            }
        }

        public string TextNerveWrong
        {
            get { return textNerveWrong; }
            set
            {
                textNerveWrong = value;
                RaisePropertyChanged("TextNerveWrong");
            }
        }

        public string TextNerveWrongExp
        {
            get { return textNerveWrongExp; }
            set
            {
                textNerveWrongExp = value;
                RaisePropertyChanged("TextNerveWrongExp");
            }
        }

        public string TextNerveWrongIntro
        {
            get { return textNerveWrongIntro; }
            set
            {
                textNerveWrongIntro = value;
                RaisePropertyChanged("TextNerveWrongIntro");
            }
        }

        public string TextScore
        {
            get { return textScore; }
            set
            {
                textScore = value;
                RaisePropertyChanged("TextScore");
            }
        }

        public string TextTime
        {
            get { return textTime; }
            set
            {
                textTime = value;
                RaisePropertyChanged("TextTime");
            }
        }

        public string TextTimeExp
        {
            get { return textTimeExp; }
            set
            {
                textTimeExp = value;
                RaisePropertyChanged("TextTimeExp");
            }
        }

        public string TextToShowYawPitch
        {
            get { return textToShowYawPitch; }
            set
            {
                textToShowYawPitch = value;
                OnPropertyChanged(nameof(TextToShowYawPitch));
            }
        }

        public string TextVascularWrong
        {
            get { return textVascularWrong; }
            set
            {
                textVascularWrong = value;
                RaisePropertyChanged("TextVascularWrong");
            }
        }

        public string TextVascularWrongExp
        {
            get { return textVascularWrongExp; }
            set
            {
                textVascularWrongExp = value;
                RaisePropertyChanged("TextVascularWrongExp");
            }
        }

        public string TextVascularWrongIntro
        {
            get { return textVascularWrongIntro; }
            set
            {
                textVascularWrongIntro = value;
                RaisePropertyChanged("TextVascularWrongIntro");
            }
        }

        public string TextWrongInjections
        {
            get { return textWrongInjections; }
            set
            {
                textWrongInjections = value;
                RaisePropertyChanged("TextWrongInjections");
            }
        }

        public string TextWrongInjectionsExp
        {
            get { return textWrongInjectionsExp; }
            set
            {
                textWrongInjectionsExp = value;
                RaisePropertyChanged("TextWrongInjectionsExp");
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

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetValue(ref title, value, "Title");
            }
        }

        public bool ToElaborateData { get; set; } = false;

        public Transform3DGroup TransformGroupMankin { get; private set; }

        public TranslateTransform3D TransformSphere { set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(5, 0, 0);

        public TranslateTransform3D TransformSphere2 { set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(5, 0, 0);

        public TranslateTransform3D TranslationManikin3D { get; private set; }

        public Point3D TranslationPointModel { get; set; }

        public TranslateTransform3D TrasformAntenna { set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);

        public Vector3D UpDirection
        {
            get { return upDirection; }
            set
            {
                upDirection = value;
                OnPropertyChanged(nameof(UpDirection));
            }
        }

        public bool VeinsVisible
        {
            get { return veinsVisible; }
            set
            {
                veinsVisible = value;
                OnPropertyChanged(nameof(VeinsVisible));
            }
        }

        public Visualization3DFrame Vis3DFrame { get; private set; }

        public Visualization3DFrameStudent Vis3DFrameStudent { get; private set; }

        public Visibility VisibilityBorderArrowDown
        {
            get
            {
                return visibilityBorderArrowDown;
            }
            set
            {
                if (value != visibilityBorderArrowDown)
                {
                    visibilityBorderArrowDown = value;
                    RaisePropertyChanged("VisibilityBorderArrowDown");
                }
            }
        }

        public Visibility VisibilityBorderArrowLeft
        {
            get
            {
                return visibilityBorderArrowLeft;
            }
            set
            {
                if (value != visibilityBorderArrowLeft)
                {
                    visibilityBorderArrowLeft = value;
                    RaisePropertyChanged("VisibilityBorderArrowLeft");
                }
            }
        }

        public Visibility VisibilityBorderArrowRight
        {
            get
            {
                return visibilityBorderArrowRight;
            }
            set
            {
                if (value != visibilityBorderArrowRight)
                {
                    visibilityBorderArrowRight = value;
                    RaisePropertyChanged("VisibilityBorderArrowRight");
                }
            }
        }

        public Visibility VisibilityBorderArrowRotation
        {
            get
            {
                return visibilityBorderArrowRotation;
            }
            set
            {
                if (value != visibilityBorderArrowRotation)
                {
                    visibilityBorderArrowRotation = value;
                    RaisePropertyChanged("VisibilityBorderArrowRotation");
                }
            }
        }

        public Visibility VisibilityBorderArrowRotation2
        {
            get
            {
                return visibilityBorderArrowRotation2;
            }
            set
            {
                if (value != visibilityBorderArrowRotation2)
                {
                    visibilityBorderArrowRotation2 = value;
                    RaisePropertyChanged("VisibilityBorderArrowRotation2");
                }
            }
        }

        public Visibility VisibilityBorderArrowUp
        {
            get
            {
                return visibilityBorderArrowUp;
            }
            set
            {
                if (value != visibilityBorderArrowUp)
                {
                    visibilityBorderArrowUp = value;
                    RaisePropertyChanged("VisibilityBorderArrowUp");
                }
            }
        }

        public Visibility VisibilityLogoOnStudentPage
        {
            get
            {
                return visibilityLogoOnStudentPage;
            }
            set
            {
                if (value != visibilityLogoOnStudentPage)
                {
                    visibilityLogoOnStudentPage = value;
                    RaisePropertyChanged("VisibilityLogoOnStudentPage");
                }
            }
        }

        public bool VisibilityNeedleCollisionCheck
        {
            get
            {
                return visibilityNeedleCollisionCheck;
            }
            set
            {
                visibilityNeedleCollisionCheck = value;
                RaisePropertyChanged("VisibilityNeedleCollisionCheck");
            }
        }

        public Visibility VisibilityResults
        {
            get
            {
                return visibilityResults;
            }
            set
            {
                if (value != visibilityResults)
                {
                    visibilityResults = value;
                    RaisePropertyChanged("VisibilityResults");
                }
            }
        }

        public Visibility VisibleLine
        {
            get
            {
                return visibleLine;
            }
            set
            {
                if (value != visibleLine)
                {
                    visibleLine = value;
                    RaisePropertyChanged("VisibleLine");
                }
            }
        }

        public double WidthImage_mm
        {
            get { return widthImage; }
            set
            {
                widthImage = value;
                RaisePropertyChanged("WidthImage");
            }
        }

        public WindowStudent WindowStudent { get; internal set; }

        public WindowTeacher WindowTeacher { get; internal set; }

        public float XPosOperableEnsamble
        {
            get
            {
                return xPosOperableEnsamble;
            }
            set
            {
                xPosOperableEnsamble = value;
                RaisePropertyChanged("XPosOperableEnsamble");
            }
        }

        public float XPosSensor02
        {
            get
            {
                return xPosSensor02;
            }
            set
            {
                xPosSensor02 = value;
                RaisePropertyChanged("XPosSensor02");
            }
        }

        public float XTipNeedle
        {
            get
            {
                return xTipNeedle;
            }
            set
            {
                xTipNeedle = value;
                RaisePropertyChanged("XTipNeedle");
            }
        }

        public double YawNeedle
        {
            get { return yawNeedle; }
            set
            {
                yawNeedle = value;
                OnPropertyChanged(nameof(YawNeedle));
            }
        }

        public double YawSensor02
        {
            get { return yawSensor02; }
            set
            {
                yawSensor02 = value;
                RaisePropertyChanged("YawSensor02");
            }
        }

        public float YPosOperableEnsamble
        {
            get
            {
                return yPosOperableEnsamble;
            }
            set
            {
                yPosOperableEnsamble = value;
                RaisePropertyChanged("YPosOperableEnsamble");
            }
        }

        public float YPosSensor02
        {
            get
            {
                return yPosSensor02;
            }
            set
            {
                yPosSensor02 = value;
                RaisePropertyChanged("YPosSensor02");
            }
        }

        public float YTipNeedle
        {
            get
            {
                return yTipNeedle;
            }
            set
            {
                yTipNeedle = value;
                RaisePropertyChanged("YTipNeedle");
            }
        }

        public float ZPosOperableEnsamble
        {
            get
            {
                return zPosOperableEnsamble;
            }
            set
            {
                zPosOperableEnsamble = value;
                RaisePropertyChanged("ZPosOperableEnsamble");
            }
        }

        public float ZPosSensor02
        {
            get
            {
                return zPosSensor02;
            }
            set
            {
                zPosSensor02 = value;
                RaisePropertyChanged("ZPosSensor02");
            }
        }

        public float FluxLiquid
        {
            get
            {
                return fluxLiquid;
            }
            set
            {
                fluxLiquid = value;
                RaisePropertyChanged("FluxLiquid");
            }
        }

        public float VolumeLiquid
        {
            get
            {
                return volumeLiquid;
            }
            set
            {
                volumeLiquid = value;
                RaisePropertyChanged("VolumeLiquid");
            }
        }

        public float ZTipNeedle
        {
            get
            {
                return zTipNeedle;
            }
            set
            {
                zTipNeedle = value;
                RaisePropertyChanged("ZTipNeedle");
            }
        }

        public bool Calibrating { get; set; }
        public bool ArePointsVisualizedOn3D { get; private set; } = false;

        public static BitmapImage Convert(System.Drawing.Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
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

        public static double DistanceBetween(Point3D a, Point3D b)
        {
            Point3D direction = new Point3D();
            direction.X = b.X - a.X;
            direction.Y = b.Y - a.Y;
            direction.Z = b.Z - a.Z;

            return Math.Sqrt(direction.X * direction.X +
                             direction.Y * direction.Y +
                             direction.Z * direction.Z);
        }

        public static SharpDX.Point GetMousePositionWindowsForms()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new SharpDX.Point(point.X, point.Y);
        }

        public void AdvancePartialAdvanceStep()
        {
            switch (CurrentCase.Steps[CurrentCase.CurrentStepIndex].Type)
            {
                case Enum_ClinicalCaseStepType.MESSAGE:
                    PrepareAdvanceStudent("Next");

                    #region MESSAGE

                    AdvanceStep();

                    #endregion MESSAGE

                    break;

                case Enum_ClinicalCaseStepType.QUESTIONNAIRE:

                    #region QUESTIONNAIRE MULTIPLE

                    InteractionFrame s3 = (InteractionFrame)StudentFrame;
                    InteractionFrame t3 = (InteractionFrame)TeacherFrame;
                    ClinicalCaseStep_Questionnaire c3 = (ClinicalCaseStep_Questionnaire)caseStepCurrent;
                    switch (c3.Step)
                    {
                        case Enum_StepQuestionnaire.INITIAL:
                            s3.HidePoints();
                            t3.HidePoints();
                            s3.PrepareFor(Enum_StepDynamicAnalysis.ANSWERING);
                            t3.PrepareFor(Enum_StepDynamicAnalysis.ANSWERING);
                            c3.Step = Enum_StepQuestionnaire.ANSWERING;
                            break;

                        case Enum_StepQuestionnaire.ANSWERING:

                            int numErrors = 0;
                            for (int i = 0; i < s3.spOperativity.Children.Count; i++)
                            {
                                MultipleChoiceControl mpc = ((MultipleChoiceControl)(s3.spOperativity.Children[i]));
                                mpc.IsEnabled = false;

                                bool omitted = true;
                                for (int k = 0; k < mpc.spAnswers.Children.Count; k++)
                                {
                                    CustomBorder aa = (CustomBorder)(mpc.spAnswers.Children[k]);
                                    if (aa.YourIntegerProperty == 1)
                                    {
                                        omitted = false;
                                        break;
                                    }
                                }
                                if (omitted)
                                {
                                    numErrors++;
                                }

                                for (int k = 0; k < mpc.spAnswers.Children.Count; k++)
                                {
                                    CustomBorder aa = (CustomBorder)(mpc.spAnswers.Children[k]);

                                    string answer = ((TextBlock)(aa.Child)).Text;

                                    if (aa.YourIntegerProperty == 1)
                                    {
                                        if (c3.Questions[i].CorrectAnswers.Contains(answer))
                                        {
                                            aa.YourIntegerProperty = 2;
                                        }
                                        else
                                        {
                                            aa.YourIntegerProperty = 3;
                                            numErrors++;
                                        }
                                    }
                                    else
                                    {
                                        if (c3.Questions[i].CorrectAnswers.Contains(answer))
                                        {
                                            aa.YourIntegerProperty = 4;
                                        }
                                    }
                                    mpc.OnSomethingChanged(k, aa.YourIntegerProperty);
                                }
                                mpc.UpdateBackgrounds();
                            }
                            c3.NumErrors = numErrors;
                            c3.QuestionnaireScore = ((((double)s3.spOperativity.Children.Count) - (double)numErrors) / (double)s3.spOperativity.Children.Count) * 100f;
                            c3.Score = (float)c3.QuestionnaireScore;
                            c3.Step = Enum_StepQuestionnaire.FEEDBACK;

                            s3.PassScoresQuestionnaire(c3.NumErrors, c3.QuestionnaireScore);
                            t3.PassScoresQuestionnaire(c3.NumErrors, c3.QuestionnaireScore);
                            s3.PassFinalScore(c3.Score);
                            t3.PassFinalScore(c3.Score);
                            s3.tbMessage.Text = AppControl.Instance.EvaluateTheCorrectAnswers;
                            t3.tbMessage.Text = AppControl.Instance.EvaluateTheCorrectAnswersTeacher;
                            break;

                        case Enum_StepQuestionnaire.FEEDBACK:

                            s3.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK_SIMPLE);
                            t3.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK_SIMPLE);
                            s3.tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                            t3.tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                            c3.Step = Enum_StepQuestionnaire.FINALSCORE;

                            break;

                        case Enum_StepQuestionnaire.FINALSCORE:

                            //Save the results

                            ResultPhaseToSave ph2Save = new ResultPhaseToSave();
                            ph2Save.TotalScore = c3.Score;
                            ph2Save.QuestionnaireScore = c3.QuestionnaireScore;
                            ph2Save.NumErrorsQuestionnaire = c3.NumErrors;
                            ph2Save.PhaseName = c3.NameStep;
                            ph2Save.PhaseType = Enum_ClinicalCaseStepType.QUESTIONNAIRE;

                            ResultCaseToSave.ResultPhases.Add(ph2Save);

                            PrepareAdvanceStudent("Next");

                            AdvanceStep();
                            break;

                        default:
                            break;
                    }

                    #endregion QUESTIONNAIRE MULTIPLE

                    break;

                case Enum_ClinicalCaseStepType.ANALYSIS_STATIC_FACE:

                    #region ANALYSIS STATIC FACE

                    InteractionFrame s4 = (InteractionFrame)StudentFrame;
                    InteractionFrame t4 = (InteractionFrame)TeacherFrame;
                    ClinicalCaseStep_AnalysisStaticFace c4 = (ClinicalCaseStep_AnalysisStaticFace)caseStepCurrent;
                    switch (c4.Step)
                    {
                        case Enum_StepQuestionnaire.INITIAL:
                            s4.HidePoints();
                            t4.HidePoints();
                            s4.PrepareFor(Enum_StepDynamicAnalysis.ANSWERING);
                            t4.PrepareFor(Enum_StepDynamicAnalysis.ANSWERING);
                            c4.Step = Enum_StepQuestionnaire.ANSWERING;
                            c4.Step = Enum_StepQuestionnaire.ANSWERING;
                            break;

                        case Enum_StepQuestionnaire.ANSWERING:

                            int numErrors = 0;
                            for (int i = 0; i < s4.spOperativity.Children.Count; i++)
                            {
                                MultipleChoiceControl mpc = ((MultipleChoiceControl)(s4.spOperativity.Children[i]));
                                mpc.IsEnabled = false;

                                bool omitted = true;
                                for (int k = 0; k < mpc.spAnswers.Children.Count; k++)
                                {
                                    CustomBorder aa = (CustomBorder)(mpc.spAnswers.Children[k]);
                                    if (aa.YourIntegerProperty == 1)
                                    {
                                        omitted = false;
                                        break;
                                    }
                                }
                                if (omitted)
                                {
                                    numErrors++;
                                }

                                for (int k = 0; k < mpc.spAnswers.Children.Count; k++)
                                {
                                    CustomBorder aa = (CustomBorder)(mpc.spAnswers.Children[k]);

                                    string answer = ((TextBlock)(aa.Child)).Text;

                                    if (aa.YourIntegerProperty == 1)
                                    {
                                        if (c4.Questions[i].CorrectAnswers.Contains(answer))
                                        {
                                            aa.YourIntegerProperty = 2;
                                        }
                                        else
                                        {
                                            aa.YourIntegerProperty = 3;
                                            numErrors++;
                                        }
                                    }
                                    else
                                    {
                                        if (c4.Questions[i].CorrectAnswers.Contains(answer))
                                        {
                                            aa.YourIntegerProperty = 4;
                                        }
                                    }
                                    mpc.OnSomethingChanged(k, aa.YourIntegerProperty);
                                }
                                mpc.UpdateBackgrounds();
                            }

                            //make things happen on score and inidcations
                            c4.QuestionnaireScore = (((double)s4.spOperativity.Children.Count) - (double)numErrors) / (double)s4.spOperativity.Children.Count * 100f;
                            c4.NumErrors = numErrors;
                            c4.Score = (float)c4.QuestionnaireScore;
                            s4.PassScoresQuestionnaire(c4.NumErrors, c4.QuestionnaireScore);
                            t4.PassScoresQuestionnaire(c4.NumErrors, c4.QuestionnaireScore);
                            s4.PassFinalScore(c4.Score);
                            t4.PassFinalScore(c4.Score);
                            s4.tbMessage.Text = AppControl.Instance.EvaluateTheCorrectAnswers;
                            t4.tbMessage.Text = AppControl.Instance.EvaluateTheCorrectAnswersTeacher;
                            // make things happen with explanations
                            c4.Step = Enum_StepQuestionnaire.FEEDBACK;

                            break;

                        case Enum_StepQuestionnaire.FEEDBACK:

                            s4.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK_SIMPLE);
                            t4.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK_SIMPLE);
                            s4.tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                            t4.tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                            c4.Step = Enum_StepQuestionnaire.FINALSCORE;

                            break;

                        case Enum_StepQuestionnaire.FINALSCORE:
                            PrepareAdvanceStudent("Next");

                            ResultPhaseToSave ph2Save = new ResultPhaseToSave();
                            ph2Save.TotalScore = c4.Score;
                            ph2Save.QuestionnaireScore = c4.QuestionnaireScore;
                            ph2Save.NumErrorsQuestionnaire = c4.NumErrors;
                            ph2Save.PhaseName = c4.NameStep;
                            ph2Save.PhaseType = Enum_ClinicalCaseStepType.ANALYSIS_STATIC_FACE;

                            ResultCaseToSave.ResultPhases.Add(ph2Save);

                            AdvanceStep();
                            break;

                        default:
                            break;
                    }

                    #endregion ANALYSIS STATIC FACE

                    break;

                case Enum_ClinicalCaseStepType.DIDACTIC_DYNAMIC_FACE:

                    #region DIDACTIC DYNAMIC FACE

                    InteractionFrame s5 = (InteractionFrame)StudentFrame;
                    InteractionFrame t5 = (InteractionFrame)TeacherFrame;
                    ClinicalCaseStep_DidacticDynamicFace c5 = (ClinicalCaseStep_DidacticDynamicFace)caseStepCurrent;
                    switch (c5.Step)
                    {
                        case Enum_StepDynamicAnalysis.INITIAL:
                            s5.HidePoints();
                            t5.HidePoints();
                            s5.PrepareFor(Enum_StepDynamicAnalysis.ANSWERING);
                            t5.PrepareFor(Enum_StepDynamicAnalysis.ANSWERING);
                            c5.Step = Enum_StepDynamicAnalysis.ANSWERING;
                            break;

                        case Enum_StepDynamicAnalysis.ANSWERING:

                            int numErrors = 0;
                            for (int i = 0; i < s5.spOperativity.Children.Count; i++)
                            {
                                MultipleChoiceControl mpc = ((MultipleChoiceControl)(s5.spOperativity.Children[i]));
                                mpc.IsEnabled = false;

                                bool omitted = true;
                                for (int k = 0; k < mpc.spAnswers.Children.Count; k++)
                                {
                                    CustomBorder aa = (CustomBorder)(mpc.spAnswers.Children[k]);
                                    if (aa.YourIntegerProperty == 1)
                                    {
                                        omitted = false;
                                        break;
                                    }
                                }
                                if (omitted)
                                {
                                    numErrors++;
                                }

                                for (int k = 0; k < mpc.spAnswers.Children.Count; k++)
                                {
                                    CustomBorder aa = (CustomBorder)(mpc.spAnswers.Children[k]);

                                    string answer = ((TextBlock)(aa.Child)).Text;

                                    if (aa.YourIntegerProperty == 1)
                                    {
                                        if (c5.Questions[i].CorrectAnswers.Contains(answer))
                                        {
                                            aa.YourIntegerProperty = 2;
                                        }
                                        else
                                        {
                                            aa.YourIntegerProperty = 3;
                                            numErrors++;
                                        }
                                    }
                                    else
                                    {
                                        if (c5.Questions[i].CorrectAnswers.Contains(answer))
                                        {
                                            aa.YourIntegerProperty = 4;
                                        }
                                    }
                                    mpc.OnSomethingChanged(k, aa.YourIntegerProperty);
                                }
                                mpc.UpdateBackgrounds();
                                c5.NumErrors = numErrors;
                                c5.QuestionnaireScore = (((double)s5.spOperativity.Children.Count) - (double)numErrors) / (double)s5.spOperativity.Children.Count * 100f;

                                s5.PassScoresQuestionnaire(c5.NumErrors, c5.QuestionnaireScore);
                                t5.PassScoresQuestionnaire(c5.NumErrors, c5.QuestionnaireScore);

                                s5.tbMessage.Text = AppControl.Instance.EvaluateTheCorrectAnswers;
                                t5.tbMessage.Text = AppControl.Instance.EvaluateTheCorrectAnswersTeacher;
                            }

                            //make things happen on score and inidcations

                            // make things happen with explanations
                            c5.Step = Enum_StepDynamicAnalysis.FEEDBACK_ANSWERING;

                            break;

                        case Enum_StepDynamicAnalysis.FEEDBACK_ANSWERING:

                            s5.PrepareFor(Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS);
                            t5.PrepareFor(Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS);
                            t5.OverlayRectangle.Visibility = Visibility.Visible;
                            s5.DrawInjectionPoints(s5.InjectionPoints2D, null);
                            t5.DrawInjectionPoints(s5.InjectionPoints2D, null);
                            s5.SetScrollSynch();
                            s5.tbMessage.Text = c5.MessageToStudentAction;
                            t5.tbMessage.Text = c5.MessageToTeacherAction;
                            c5.Step = Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS;
                            break;

                        case Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS:

                            for (int i = 0; i < s5.InjectionPoints2D.Count; i++)
                            {
                                InjectionPointSpecific2D ijP = s5.InjectionPoints2D[i];
                                ijP.OptimalQuantityVis = ijP.PrescribedQuantity.ToString("00.00");
                                var listViewItem = (ListViewItem)s5.lvInjectionPoints.ItemContainerGenerator.ContainerFromIndex(i);
                                var listViewItemT = (ListViewItem)t5.lvInjectionPoints.ItemContainerGenerator.ContainerFromIndex(i);
                                if (ijP.IsError && ijP.ActuallyChosenOrPerformedQuantity > 0)
                                {
                                    listViewItem.Background = Brushes.Red;
                                    listViewItemT.Background = Brushes.Red;
                                }
                                else
                                {
                                    if (ijP.ActuallyChosenOrPerformedQuantity > ijP.PrescribedQuantity)
                                    {
                                        listViewItem.Background = Brushes.Orange;
                                        listViewItemT.Background = Brushes.Orange;
                                    }
                                    else if (ijP.ActuallyChosenOrPerformedQuantity < ijP.PrescribedQuantity)
                                    {
                                        listViewItem.Background = Brushes.Yellow;
                                        listViewItemT.Background = Brushes.Yellow;
                                    }
                                    else
                                    {
                                        listViewItem.Background = Brushes.Green;
                                        listViewItemT.Background = Brushes.Green;
                                    }
                                }
                            }

                            c5.Step = Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS;
                            s5.DrawInjectionPointsSpecial(s5.InjectionPoints2D);
                            t5.DrawInjectionPointsSpecial(s5.InjectionPoints2D);
                            s5.tbMessage.Text = AppControl.Instance.MessageCheckingFeedbacks;
                            t5.tbMessage.Text = AppControl.Instance.MessageCheckingFeedbacksTeacher;
                            break;

                        case Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS:

                            List<InjectionPointBase> injectionPointBases = new List<InjectionPointBase>();
                            foreach (InjectionPointSpecific2D item in s5.InjectionPoints2D)
                            {
                                injectionPointBases.Add(item);
                            }

                            List<AnalysResult> consequences = PointsManager.Instance.EvaluateWhatHasBeenDone(injectionPointBases, c5.AreaDefinition, false);

                            double scoreFinal = 100;
                            List<string> errors = new List<string>();
                            for (int i = 0; i < consequences.Count; i++)
                            {
                                switch (consequences[i].ScoreEffect)
                                {
                                    case Enum_ScoreEffect.SET0:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: Set to 0.");
                                        scoreFinal = 0;
                                        break;

                                    case Enum_ScoreEffect.MINUS50:
                                        scoreFinal = scoreFinal - 50;
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -50%.");
                                        break;

                                    case Enum_ScoreEffect.MIN20:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -20%.");
                                        scoreFinal = scoreFinal - 20;
                                        break;

                                    case Enum_ScoreEffect.MIN10:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -10%.");
                                        break;

                                    case Enum_ScoreEffect.MIN5:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -5%.");
                                        scoreFinal = scoreFinal - 5;
                                        break;

                                    case Enum_ScoreEffect.NOEFFECT:
                                        break;

                                    default:
                                        break;
                                }
                            }

                            if (scoreFinal < 0)
                            {
                                scoreFinal = 0;
                            }
                            c5.Consequences = consequences;
                            c5.OperativityScore = scoreFinal;
                            c5.Score = (float)(c5.OperativityScore * .75f + c5.QuestionnaireScore * 0.25f);
                            c5.ErrorsActive = errors;
                            s5.PassInfoOperativity(c5.Consequences, errors, c5.OperativityScore);
                            t5.PassInfoOperativity(c5.Consequences, errors, c5.OperativityScore);
                            s5.PassFinalScore(c5.Score);
                            t5.PassFinalScore(c5.Score);
                            s5.PrepareFor(Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS_EFFECTS);
                            t5.PrepareFor(Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS_EFFECTS);

                            s5.UpdateTheConsequence(0);
                            t5.UpdateTheConsequence(0);

                            c5.Step = Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS_EFFECTS;
                            s5.tbMessage.Text = c5.MessageToStudentConsequences;
                            t5.tbMessage.Text = c5.MessageToTeacherConsequences;
                            CaseStudentFrame csf = (CaseStudentFrame)WindowStudent.PageContainer.Content;
                            if (c5.Consequences.Count > 1)
                            {
                                csf.bOk.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                csf.bOk.Visibility = Visibility.Visible;
                                s5.AlreadyCheckedAllConsequencies = true;
                            }

                            break;

                        case Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS_EFFECTS:
                            s5.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK);
                            t5.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK);

                            c5.Step = Enum_StepDynamicAnalysis.FINAL_FEEDBACK;
                            s5.tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                            t5.tbMessage.Text = AppControl.Instance.MessageFinalScoreStep;
                            PrepareAdvanceStudent("Next");
                            break;

                        case Enum_StepDynamicAnalysis.FINAL_FEEDBACK:

                            ResultPhaseToSave ph2Save = new ResultPhaseToSave();
                            ph2Save.TotalScore = c5.Score;
                            ph2Save.ActionScore = c5.OperativityScore;
                            ph2Save.QuestionnaireScore = c5.QuestionnaireScore;
                            ph2Save.NumErrorsQuestionnaire = c5.NumErrors;
                            ph2Save.PhaseName = c5.NameStep;
                            foreach (string vv in c5.ErrorsActive)
                            {
                                ph2Save.ActionsOnError.Add(vv);
                            }
                            ph2Save.PhaseType = Enum_ClinicalCaseStepType.DIDACTIC_DYNAMIC_FACE;

                            ResultCaseToSave.ResultPhases.Add(ph2Save);
                            PrepareAdvanceStudent("Next");

                            AdvanceStep();
                            break;

                        default:
                            break;
                    }

                    #endregion DIDACTIC DYNAMIC FACE

                    break;

                case Enum_ClinicalCaseStepType.FACE3D_INTERACTION:

                    #region FACE3D_INTERACTION

                    Visualization3DFrameStudent s6 = (Visualization3DFrameStudent)StudentFrame;
                    Visualization3DFrame t6 = (Visualization3DFrame)TeacherFrame;

                    AppControl.Instance.CurrentClinicalCaseStep_Face3DInteraction = (ClinicalCaseStep_Face3DInteraction)caseStepCurrent;

                    switch (CurrentClinicalCaseStep_Face3DInteraction.Step)
                    {
                        case Enum_StepFace3DInteraction.LOADING:
                            AppControl.Instance.Feedback3DOn = false;
                            ManageViewPointsOn3D(false);
                            MainTeacherFrame.bViewPoints.Visibility = Visibility.Visible;

                            CurrentClinicalCaseStep_Face3DInteraction.Step = Enum_StepFace3DInteraction.OPERATIVE;
                            t6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            s6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            break;

                        case Enum_StepFace3DInteraction.OPERATIVE:
                            ManageViewPointsOn3D(true);
                            MainTeacherFrame.bViewPoints.Visibility = Visibility.Hidden;
                            for (int i = 0; i < InjectionPoints3DThisStep.Count; i++)
                            {
                                InjectionPoint3D ijP = InjectionPoints3DThisStep[i];
                                ijP.OptimalQuantityVis = ijP.PrescribedQuantity.ToString("00.00");

                                var listViewItemT = (ListViewItem)t6.InjectionPointListView.ItemContainerGenerator.ContainerFromIndex(i);
                                if (ijP.IsError && ijP.ActuallyChosenOrPerformedQuantity > 0)
                                {
                                    listViewItemT.Background = Brushes.Red;
                                }
                                else
                                {
                                    if (ijP.ActuallyChosenOrPerformedQuantity > ijP.PrescribedQuantity)
                                    {
                                        listViewItemT.Background = Brushes.Orange;
                                    }
                                    else if (ijP.ActuallyChosenOrPerformedQuantity < ijP.PrescribedQuantity)
                                    {
                                        listViewItemT.Background = Brushes.Yellow;
                                    }
                                    else
                                    {
                                        listViewItemT.Background = Brushes.Green;
                                    }
                                }
                            }
                            DrawInjectionPointsSpecial();
                            CurrentClinicalCaseStep_Face3DInteraction.Step = Enum_StepFace3DInteraction.FEEDBACK_ON_INJECTIONS_PERFORMED;
                            t6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            s6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            break;

                        case Enum_StepFace3DInteraction.FEEDBACK_ON_INJECTIONS_PERFORMED:
                            CurrentClinicalCaseStep_Face3DInteraction.Step = Enum_StepFace3DInteraction.FEEDBACK_INJECTIONPOINTS_EFFECTS;
                            List<InjectionPointBase> injectionPointBases = new List<InjectionPointBase>();
                            foreach (InjectionPoint3D item in InjectionPoints3DThisStep)
                            {
                                injectionPointBases.Add(item);
                            }

                            List<AnalysResult> consequences = PointsManager.Instance.EvaluateWhatHasBeenDone(injectionPointBases, CurrentClinicalCaseStep_Face3DInteraction.AreaDefinition, false);
                            CurrentClinicalCaseStep_Face3DInteraction.Consequences = consequences;

                            double scoreFinal = 100;
                            List<string> errors = new List<string>();
                            for (int i = 0; i < consequences.Count; i++)
                            {
                                switch (consequences[i].ScoreEffect)
                                {
                                    case Enum_ScoreEffect.SET0:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: Set to 0.");
                                        scoreFinal = 0;
                                        break;

                                    case Enum_ScoreEffect.MINUS50:
                                        scoreFinal = scoreFinal - 50;
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -50%.");
                                        break;

                                    case Enum_ScoreEffect.MIN20:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -20%.");
                                        scoreFinal = scoreFinal - 20;
                                        break;

                                    case Enum_ScoreEffect.MIN10:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -10%.");
                                        break;

                                    case Enum_ScoreEffect.MIN5:
                                        errors.Add(consequences[i].WhatYouDidDescription + " Activity Score: -5%.");
                                        scoreFinal = scoreFinal - 5;
                                        break;

                                    case Enum_ScoreEffect.NOEFFECT:
                                        break;

                                    default:
                                        break;
                                }
                            }

                            if (scoreFinal < 0)
                            {
                                scoreFinal = 0;
                            }
                            CurrentClinicalCaseStep_Face3DInteraction.ErrorsDescription = errors;
                            CurrentClinicalCaseStep_Face3DInteraction.OperativityScore = scoreFinal;
                            CurrentClinicalCaseStep_Face3DInteraction.Score = (float)(CurrentClinicalCaseStep_Face3DInteraction.OperativityScore);

                            t6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            s6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);

                            t6.UpdateTheConsequence();
                            s6.UpdateTheConsequence();

                            CaseStudentFrame csf = (CaseStudentFrame)WindowStudent.PageContainer.Content;
                            if (CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count > 1)
                            {
                                csf.bOk.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                csf.bOk.Visibility = Visibility.Visible;
                                CurrentClinicalCaseStep_Face3DInteraction.AlreadyCheckedAllConsequencies = true;
                            }

                            break;

                        case Enum_StepFace3DInteraction.FEEDBACK_INJECTIONPOINTS_EFFECTS:
                            CurrentClinicalCaseStep_Face3DInteraction.Step = Enum_StepFace3DInteraction.FINAL_FEEDBACK;
                            //PIRINI 20240103 s6.PrepareFor(Enum_StepDynamicAnalysis.FINAL_FEEDBACK);
                            t6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            s6.PrepareFor(CurrentClinicalCaseStep_Face3DInteraction.Step);
                            break;

                        case Enum_StepFace3DInteraction.FINAL_FEEDBACK:

                            ResultPhaseToSave ph2Save = new ResultPhaseToSave();
                            ph2Save.TotalScore = CurrentClinicalCaseStep_Face3DInteraction.Score;
                            ph2Save.ActionScore = CurrentClinicalCaseStep_Face3DInteraction.OperativityScore;
                            ph2Save.PhaseName = CurrentClinicalCaseStep_Face3DInteraction.NameStep;
                            foreach (string vv in CurrentClinicalCaseStep_Face3DInteraction.ErrorsActive)
                            {
                                ph2Save.ActionsOnError.Add(vv);
                            }
                            ph2Save.PhaseType = Enum_ClinicalCaseStepType.FACE3D_INTERACTION;

                            ResultCaseToSave.ResultPhases.Add(ph2Save);

                            PrepareAdvanceStudent("Next");
                            AdvanceStep();
                            break;

                        default:
                            break;
                    }

                    #endregion FACE3D_INTERACTION

                    break;

                default:
                    break;
            }
        }

        public ResultToSave ResultCaseToSave { get; set; }

        public void AdvanceStep()
        {
            MainStudentFrame.bOk.IsEnabled = true;
            MainStudentFrame.bAlarm.Visibility = Visibility.Hidden;
            SetTimer(false);
            if (CurrentCase != null)
            {
                CurrentCase.CurrentStepIndex++;

                for (int i = 0; i < CurrentCase.Steps.Count; i++)
                {
                    CurrentCase.Steps[i].CurrentlySelected = (i == CurrentCase.CurrentStepIndex);
                }

                if (CurrentCase.CurrentStepIndex <= (CurrentCase.Steps.Count - 1))
                {
                    MainTeacherFrame.listItems.SelectedItem = MainTeacherFrame.listItems.Items[CurrentCase.CurrentStepIndex];

                    MainTeacherFrame.listItems.ScrollIntoView(MainTeacherFrame.listItems.SelectedItem);

                    MainStudentFrame.tbStep.Text = "STEP " + (CurrentCase.CurrentStepIndex + 1).ToString() + "/" + CurrentCase.Steps.Count.ToString();

                    if (CurrentCase.CurrentStepIndex == CurrentCase.Steps.Count - 1)
                    {
                        MainTeacherFrame.bNextStep.Content = "Close Case";
                    }

                    switch (CurrentCase.Steps[CurrentCase.CurrentStepIndex].Type)
                    {
                        case Enum_ClinicalCaseStepType.MESSAGE:
                            SetTimer(false);
                            caseStepCurrent = (ClinicalCaseStep_Message)CurrentCase.Steps[CurrentCase.CurrentStepIndex];

                            ClinicalCaseStep_Message mm1 = (ClinicalCaseStep_Message)caseStepCurrent;

                            string messageToStudent = mm1.Message;
                            string messageToTeacher = mm1.Message;

                            if (mm1.MessageType == Enum_MessageType.SCORE)
                            {
                                CurrentCase.CalculateGlobalScore();

                                float globalscore = CurrentCase.GlobalScore;
                                if (globalscore <= 70)
                                {
                                    messageToStudent = "Your global score is " + ((int)globalscore).ToString() + "/100. Try Another time!";
                                }
                                if ((globalscore > 70) && (globalscore <= 90))
                                {
                                    messageToStudent = "Good. Your global score is " + ((int)globalscore).ToString() + "/100. Try Another time!";
                                }
                                if ((globalscore > 90) && (globalscore <= 99))
                                {
                                    messageToStudent = "Very good! Your global score is " + ((int)globalscore).ToString() + "/100. Try Another time!";
                                }
                                if ((globalscore > 99))
                                {
                                    messageToStudent = "Excellent! Your global score is " + ((int)globalscore).ToString() + "/100.";
                                }

                                messageToTeacher = "Global score: " + ((int)globalscore).ToString() + "/100.";
                            }

                            StudentFrame = new MessageFrameS(messageToStudent, ((ClinicalCaseStep_Message)caseStepCurrent).MessageToStudent, ((ClinicalCaseStep_Message)caseStepCurrent).MessageType, false);

                            TeacherFrame = new MessageFrameS(messageToTeacher, ((ClinicalCaseStep_Message)caseStepCurrent).MessageToTeacher, ((ClinicalCaseStep_Message)caseStepCurrent).MessageType, true);

                            //StudentFrame = new MessageFrameS(((ClinicalCaseStep_Message)caseStepCurrent).Message, ((ClinicalCaseStep_Message)caseStepCurrent).MessageInitialToStudent, ((ClinicalCaseStep_Message)caseStepCurrent).MessagType);
                            //TeacherFrame = new MessageFrameS(((ClinicalCaseStep_Message)caseStepCurrent).Message, ((ClinicalCaseStep_Message)caseStepCurrent).MessageInitialToTeacher, ((ClinicalCaseStep_Message)caseStepCurrent).MessagType);

                            MainTeacherFrame.frActivity.Navigate((MessageFrameS)TeacherFrame);
                            MainStudentFrame.frActivity.Navigate((MessageFrameS)StudentFrame);

                            MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bInsertAnaetheticMouse.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bViewImages.Visibility = Visibility.Hidden;
                            ((MessageFrameS)TeacherFrame).tbHeader.FontStyle = FontStyles.Italic;
                            ((MessageFrameS)StudentFrame).tbHeader.FontStyle = FontStyles.Italic;
                            break;

                        case Enum_ClinicalCaseStepType.QUESTIONNAIRE:
                            caseStepCurrent = (ClinicalCaseStep_Questionnaire)CurrentCase.Steps[CurrentCase.CurrentStepIndex];

                            TeacherFrame = new InteractionFrame(true);
                            ((InteractionFrame)TeacherFrame).SetContent((ClinicalCaseStep_Questionnaire)caseStepCurrent);

                            StudentFrame = new InteractionFrame(false);
                            ((InteractionFrame)StudentFrame).ReferredTeacher = (InteractionFrame)TeacherFrame;
                            ((InteractionFrame)StudentFrame).SetContent((ClinicalCaseStep_Questionnaire)caseStepCurrent);

                            MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bInsertAnaetheticMouse.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bViewImages.Visibility = Visibility.Hidden;
                            MainTeacherFrame.frActivity.Navigate(TeacherFrame);
                            MainStudentFrame.frActivity.Navigate(StudentFrame);
                            ((ClinicalCaseStep_Questionnaire)caseStepCurrent).Step = Enum_StepQuestionnaire.INITIAL;
                            AdvancePartialAdvanceStep();
                            break;

                        case Enum_ClinicalCaseStepType.ANALYSIS_STATIC_FACE:
                            caseStepCurrent = (ClinicalCaseStep_AnalysisStaticFace)CurrentCase.Steps[CurrentCase.CurrentStepIndex];

                            TeacherFrame = new InteractionFrame(true);
                            ((InteractionFrame)TeacherFrame).SetContent((ClinicalCaseStep_AnalysisStaticFace)caseStepCurrent);

                            StudentFrame = new InteractionFrame(false);
                            ((InteractionFrame)StudentFrame).ReferredTeacher = (InteractionFrame)TeacherFrame;
                            ((InteractionFrame)StudentFrame).SetContent((ClinicalCaseStep_AnalysisStaticFace)caseStepCurrent);

                            MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bInsertAnaetheticMouse.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bViewImages.Visibility = Visibility.Hidden;
                            MainTeacherFrame.frActivity.Navigate(TeacherFrame);
                            MainStudentFrame.frActivity.Navigate(StudentFrame);
                            ((ClinicalCaseStep_AnalysisStaticFace)caseStepCurrent).Step = Enum_StepQuestionnaire.INITIAL;
                            AdvancePartialAdvanceStep();
                            break;

                        case Enum_ClinicalCaseStepType.DIDACTIC_DYNAMIC_FACE:
                            caseStepCurrent = (ClinicalCaseStep_DidacticDynamicFace)CurrentCase.Steps[CurrentCase.CurrentStepIndex];

                            TeacherFrame = new InteractionFrame(true);
                            ((InteractionFrame)TeacherFrame).SetContent((ClinicalCaseStep_DidacticDynamicFace)caseStepCurrent);

                            StudentFrame = new InteractionFrame(false);
                            ((InteractionFrame)StudentFrame).ReferredTeacher = (InteractionFrame)TeacherFrame;
                            ((InteractionFrame)StudentFrame).SetContent((ClinicalCaseStep_DidacticDynamicFace)caseStepCurrent);

                            StudentFrame.DataContext = StudentFrame;
                            ((InteractionFrame)StudentFrame).ReferredTeacher.DataContext = StudentFrame;

                            MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bInsertAnaetheticMouse.Visibility = Visibility.Hidden;
                            MainTeacherFrame.bViewImages.Visibility = Visibility.Hidden;
                            MainTeacherFrame.frActivity.Navigate(TeacherFrame);
                            MainStudentFrame.frActivity.Navigate(StudentFrame);
                            ((ClinicalCaseStep_DidacticDynamicFace)caseStepCurrent).Step = Enum_StepDynamicAnalysis.INITIAL;
                            AdvancePartialAdvanceStep();
                            break;

                        case Enum_ClinicalCaseStepType.FACE3D_INTERACTION:
                            caseStepCurrent = (ClinicalCaseStep_Face3DInteraction)CurrentCase.Steps[CurrentCase.CurrentStepIndex];

                            if (!(TeacherFrame is Visualization3DFrame))
                            {
                                EntranceDataList = new ObservableCollection<EntranceData>();
                                if (Vis3DFrame == null)
                                {
                                    Vis3DFrame = new Visualization3DFrame();
                                }

                                TeacherFrame = Vis3DFrame;

                                if (Vis3DFrameStudent == null)
                                {
                                    Vis3DFrameStudent = new Visualization3DFrameStudent();
                                }
                                StudentFrame = Vis3DFrameStudent;

                                MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
                            }
                            MainTeacherFrame.frActivity.Navigate(TeacherFrame);
                            MainTeacherFrame.bInsertAnaetheticMouse.Visibility = Properties.Settings.Default.DebugMode ? Visibility.Visible : Visibility.Collapsed;
                            MainTeacherFrame.bViewImages.Visibility = Visibility.Visible;
                            MainStudentFrame.frActivity.Navigate(StudentFrame);
                            AppControl.Instance.SetContent((ClinicalCaseStep_Face3DInteraction)caseStepCurrent);
                            SelectedPoint3DIndex = -1;
                            ((ClinicalCaseStep_Face3DInteraction)caseStepCurrent).Step = Enum_StepFace3DInteraction.LOADING;

                            AdvancePartialAdvanceStep();
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    StopCase(true);
                    AppControl.Instance.WindowStudent.Navigate(new MessageVoid());
                    AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
                }
            }
        }

        public double CalcDistance(Point3D point1, Point3D point2)
        {
            double deltaX = point1.X - point2.X;
            double deltaY = point1.Y - point2.Y;
            double deltaZ = point1.Z - point2.Z;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        public void CheckModulesActivation()
        {
            string[] modulesNames = Enum.GetNames(typeof(Enum_Modules));
            Modules = new ObservableCollection<string>();
            ModulesNames = new ObservableCollection<string>();

            foreach (string s in modulesNames)
            {
                Modules.Add(s);
            }

            if (WindowTeacher != null)
            {
                WindowTeacher.Dispatcher.Invoke(() =>
                {
                    PopulateModulesNames();
                    PopulateCases(false);
                });
            }

            ModulesChecked = true;
        }

        public void CheckSceneNode(SceneNode node, HitTestContext context, SharpDX.Vector3 rayDir, SharpDX.Vector3 tip, ref List<PointHit> pointsHit)//, ref List<string> nodeNames)
        {
            try
            {
                if (node is GroupNode gn)
                {
                    List<HitTestResult> hits = new List<HitTestResult>();
                    gn.HitTest(context, ref hits);
                    if (hits != null)
                    {
                        //List<string> nodeNames = new List<string>();
                        foreach (var hit in hits)
                        {
                            foreach (var item in CollisionItemsGuid)
                            {
                                if (item.Value.Contains(hit.Geometry.GUID))
                                {
                                    SharpDX.Vector3 diff = hit.PointHit - tip;

                                    double distance = SharpDX.Vector3.Dot(diff, rayDir);

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
            //in teoria questo sotto non serve
            //ho visto che i modelli che carichi sono visti come gruppi

            return;
        }

        public void CloseApp(bool forceClose = false)
        {
            if (WindowTeacher != null)
            {
                if (forceClose || MessageBox.Show(BeautySim.Globalization.Language.str_close_beautysim, true, 1000, false))
                {
                    if (CurrentTeacher != null && CurrentTeacher.UserName != ADMIN_USERNAME)
                    {
                        Properties.Settings.Default.LastValidTeacherIndex = (int)CurrentTeacher.Id;
                        if (CurrentStudent != null)
                        {
                            Properties.Settings.Default.LastValidStudentIndex = (int)CurrentStudent.Id;
                        }
                        else
                        {
                            Properties.Settings.Default.LastValidStudentIndex = -1;
                        }
                    }
                    else
                    {
                        Properties.Settings.Default.LastValidTeacherIndex = -1;
                    }
                    if (CurrentEvent != null)
                    {
                        Properties.Settings.Default.LastValidEventIndex = (int)CurrentEvent.Id;
                    }
                    else
                    {
                        Properties.Settings.Default.LastValidEventIndex = -1;
                    }
                    Properties.Settings.Default.Save();
                    WindowTeacher.Close();
                }
            }
        }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D CreateTruncatedConeGeometry(float baseRadius, float topRadius, float height, int thetaDiv, SharpDX.Vector3 normal, SharpDX.Vector3 origin, bool baseCap)
        {
            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, true);
            builder.AddCone(origin, normal, baseRadius, topRadius, height, baseCap, false, thetaDiv);
            var geometry = builder.ToMeshGeometry3D();
            return geometry;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public Point3D GetRotoTransatedPoint(Transform3DGroup tsg, Point3D origin)
        {
            Point3D ptd = tsg.Transform(origin);
            return ptd;
        }

        public void Init()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;
            var hwnd2 = new WindowInteropHelper(WindowTeacher).Handle;
            WindowTeacher.SetWindowLong(hwnd2, WindowTeacher.GWL_STYLE, WindowTeacher.GetWindowLong(hwnd2, WindowTeacher.GWL_STYLE) & ~WindowTeacher.WS_SYSMENU);
            DBConnector.Instance.InitDB(AppControl.DBPath);
            pDIClass = new PDIClass();

            System.Windows.Media.Media3D.Quaternion qy = AppControl.CreateRotationQuaternionAlongAxis(0, 1);
            System.Windows.Media.Media3D.Quaternion qz = AppControl.CreateRotationQuaternionAlongAxis(0, 2);
            System.Windows.Media.Media3D.Quaternion qx = AppControl.CreateRotationQuaternionAlongAxis((float)Properties.Settings.Default.AngleRotationManikinAdjustment, 0);
            AppControl.Instance.Rotation_Manikin = qx;
            UpdateRotationManikin3D();

            ReadCoordinateFile();
            UpdateTranslationManikin3D();

            TransformGroupMankin = new Transform3DGroup();
            TransformGroupMankin.Children.Add(new RotateTransform3D(RotationManikin3D));
            TransformGroupMankin.Children.Add(TranslationManikin3D);
        }

        public void InitializeCoordinates()
        {
            var builder = new LineBuilder();
            builder.AddLine(SharpDX.Vector3.Zero, SharpDX.Vector3.UnitX * 5);
            builder.AddLine(SharpDX.Vector3.Zero, SharpDX.Vector3.UnitY * 5);
            builder.AddLine(SharpDX.Vector3.Zero, SharpDX.Vector3.UnitZ * 5);
            AppControl.Instance.Coordinate = builder.ToLineGeometry3D();
            AppControl.Instance.Coordinate.Colors = new Color4Collection(Enumerable.Repeat<SharpDX.Color4>(SharpDX.Color.White, 6));
            AppControl.Instance.Coordinate.Colors[0] = AppControl.Instance.Coordinate.Colors[1] = SharpDX.Color.Red;
            AppControl.Instance.Coordinate.Colors[2] = AppControl.Instance.Coordinate.Colors[3] = SharpDX.Color.Green;
            AppControl.Instance.Coordinate.Colors[4] = AppControl.Instance.Coordinate.Colors[5] = SharpDX.Color.Blue;

            AppControl.Instance.CoordinateText = new BillboardText3D();
            AppControl.Instance.CoordinateText.TextInfo.Add(new HelixToolkit.Wpf.SharpDX.TextInfo("X", SharpDX.Vector3.UnitX * 6));
            AppControl.Instance.CoordinateText.TextInfo.Add(new HelixToolkit.Wpf.SharpDX.TextInfo("Y", SharpDX.Vector3.UnitY * 6));
            AppControl.Instance.CoordinateText.TextInfo.Add(new HelixToolkit.Wpf.SharpDX.TextInfo("Z", SharpDX.Vector3.UnitZ * 6));
        }

        public void InitializeCurrentTeacherStudentEvent()
        {
            if (Properties.Settings.Default.RememberLastValidTeacher)
            {
                if (Properties.Settings.Default.LastValidTeacherIndex > -1)
                {
                    List<Users> users = DBConnector.Instance.FindAll<Users>().ToList();
                    var teach = from db in users where db.Id == Properties.Settings.Default.LastValidTeacherIndex && db.Role > 0 select db;
                    if (teach.Count() > 0)
                    {
                        SelectTeacher((Users)teach.First());
                        if (Properties.Settings.Default.LastValidStudentIndex > -1)
                        {
                            var stud = from db in users where db.Id == Properties.Settings.Default.LastValidStudentIndex && db.Role == 0 && db.IdParentUser == CurrentTeacher.Id select db;
                            if (stud.Count() > 0)
                            {
                                SelectStudent((Users)stud.First());
                            }
                        }
                    }
                }
            }
            if (Properties.Settings.Default.LastValidEventIndex > -1)
            {
                Events ev = DBConnector.Instance.FindRowById<Events>(new BsonValue((int)Properties.Settings.Default.LastValidEventIndex));
                if (ev != null)
                {
                    SelectEvent(ev);
                }
            }
        }

        public void InitializeModels(string folder)
        {
            bool showSkinComplete = false;
            bool showSkin = true;
            bool showProcerus = false;
            bool showOrbicularisSuperiorLateral = false;
            bool showCorrugator = false;

            var sphere = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere.AddSphere(new SharpDX.Vector3(0, 0, 0), 1);
            AppControl.Instance.GeometrySphere = sphere.ToMeshGeometry3D();
            AppControl.Instance.MaterialSphere = DiffuseMaterials.Green;

            var sphere2 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere2.AddSphere(new SharpDX.Vector3(0, 0, 0), 1);
            AppControl.Instance.GeometrySphere2 = sphere2.ToMeshGeometry3D();
            AppControl.Instance.MaterialSphere2 = DiffuseMaterials.Yellow;

            var antenna = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            antenna.AddBox(SharpDX.Vector3.Zero, 54, 54, 54);
            AppControl.Instance.GeometryAntenna = antenna.ToMeshGeometry3D();
            AppControl.Instance.MaterialAntenna = DiffuseMaterials.Gray;

            AppControl.Instance.GeometrySensor = AppControl.Instance.CreateTruncatedConeGeometry(1, 1, LengthSensor, ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(-LengthSensor / 2.0f, 0, 0), true);
            AppControl.Instance.MaterialSensor = DiffuseMaterials.Black;

            AppControl.Instance.GeometryConnector = AppControl.Instance.CreateTruncatedConeGeometry(3, 3, 21.72f, ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(0, 0, 0), true);
            AppControl.Instance.MaterialConnector = DiffuseMaterials.Gray;

            AppControl.Instance.GeometryTConnector = AppControl.Instance.CreateTruncatedConeGeometry(2, 2, 10, ThetaDivNeedle, new SharpDX.Vector3(0, 1, 0), new SharpDX.Vector3(0, 0, 0), true);

            AppControl.Instance.GeometryNeedle = AppControl.Instance.CreateTruncatedConeGeometry(0.4f, 0.1f, lengthNeedle, ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(LengthSensor / 2.0f, 0, 0), true);
            AppControl.Instance.TipNeedleOrigin = new Point3D(lengthNeedle + LengthSensor / 2.0, 0, 0);
            AppControl.Instance.BaseNeedleOrigin = new Point3D(LengthSensor / 2.0, 0, 0);
            AppControl.Instance.MaterialNeedle = DiffuseMaterials.Blue;

            AppControl.Instance.GeometryCable = AppControl.Instance.CreateTruncatedConeGeometry(0.5f, 0.5f, -LengthCable, ThetaDivNeedle, new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(-LengthSensor, 0, 0), true);
            AppControl.Instance.MaterialCable = DiffuseMaterials.Yellow;

            CollisionItemsGuid = new Dictionary<string, List<Guid>>();
            if (showSkin)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(SharpDX.Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                //var models = reader.Read(folder + "\\Testa pelle mirror3.obj");
                //var models = reader.Read(folder + "\\TestaNew3b.obj");
                var models = reader.Read(folder + "\\Head_cut_base.obj");

                AppControl.Instance.CollisionItemsGuid.Add("SKIN", new List<Guid>());

                var material = new PhongMaterial
                {
                    DiffuseColor = new SharpDX.Color4(1f, 0.5f, 0.5f, 0.3f), // Imposta il quarto valore (alpha) per la trasparenza
                                                                             // Altri parametri del materiale possono essere impostati qui
                };

                for (int i = 0; i < models.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = models[i].Geometry;
                    model.Material = DiffuseMaterials.LightGray;
                    model.CullMode = SharpDX.Direct3D11.CullMode.Back;
                    //model.InvertNormal = true;
                    //model.FrontCounterClockwise = true;

                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    //model.WireframeColor = Colors.AliceBlue;

                    Vis3DFrame.groupSkin.Children.Add(model);

                    var modelS = new MeshGeometryModel3D();
                    modelS.Geometry = models[i].Geometry;
                    modelS.Material = DiffuseMaterials.LightGray;
                    modelS.CullMode = SharpDX.Direct3D11.CullMode.Back;
                    //model.InvertNormal = true;
                    //model.FrontCounterClockwise = true;

                    modelS.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    //model.WireframeColor = Colors.AliceBlue;

                    Vis3DFrameStudent.groupSkin.Children.Add(modelS);

                    AppControl.Instance.CollisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
                }
            }

            if (showSkinComplete)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(SharpDX.Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var models = reader.Read(folder + "\\Pelle combinata5.obj");
                AppControl.Instance.CollisionItemsGuid.Add("SKIN", new List<Guid>());
                for (int i = 0; i < models.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = models[i].Geometry;
                    model.Material = DiffuseMaterials.Gray;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    Vis3DFrame.groupSkin.Children.Add(model);
                    //Vis3DFrameStudent.groupSkin.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
                }
            }

            if (showProcerus)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(SharpDX.Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelProcerus = reader.Read(folder + "\\ProcerusSmoothed.obj");
                AppControl.Instance.CollisionItemsGuid.Add("PROCERUS", new List<Guid>());
                for (int i = 0; i < modelProcerus.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelProcerus[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    Vis3DFrame.groupProcerus.Children.Add(model);
                    //Vis3DFrameStudent.groupProcerus.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["PROCERUS"].Add(model.Geometry.GUID);
                }
            }

            if (showOrbicularisSuperiorLateral)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(SharpDX.Vector3.Zero, 1, 1, 1);
                var readerOrbicularisSuperiorLateral = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelOrbicularisSuperiorLateral = readerOrbicularisSuperiorLateral.Read(folder + "\\OrbicularisSuperiorLateralSmoothed.obj");
                AppControl.Instance.CollisionItemsGuid.Add("ORBICULARIS", new List<Guid>());
                for (int i = 0; i < modelOrbicularisSuperiorLateral.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelOrbicularisSuperiorLateral[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    Vis3DFrame.groupOrbicularisSuperiorLateral.Children.Add(model);
                    //Vis3DFrameStudent.groupOrbicularisSuperiorLateral.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["ORBICULARIS"].Add(model.Geometry.GUID);
                }
            }
            if (showCorrugator)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(SharpDX.Vector3.Zero, 1, 1, 1);
                var readerCorrugator = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelCorrugators = readerCorrugator.Read(folder + "\\CorrugatorSmoothed.obj");
                AppControl.Instance.CollisionItemsGuid.Add("CORRUGATOR", new List<Guid>());
                for (int i = 0; i < modelCorrugators.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelCorrugators[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    Vis3DFrame.groupCorrugators.Children.Add(model);
                    //Vis3DFrameStudent.groupCorrugators.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["CORRUGATOR"].Add(model.Geometry.GUID);
                }
            }

            if (true)
            {
                var readerArteriesSX = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var readerArteriesDX = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelsArteriesSX = readerArteriesSX.Read(folder + "\\red_sx.obj");
                var modelsArteriesDX = readerArteriesDX.Read(folder + "\\red_dx.obj");
                AppControl.Instance.CollisionItemsGuid.Add("ARTERIES", new List<Guid>());
                for (int i = 0; i < modelsArteriesSX.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsArteriesSX[i].Geometry;
                    model.Material = DiffuseMaterials.Red;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Solid;

                    Vis3DFrame.groupArteries.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["ARTERIES"].Add(model.Geometry.GUID);
                }
                for (int i = 0; i < modelsArteriesDX.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsArteriesDX[i].Geometry;
                    model.Material = DiffuseMaterials.Red;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Solid;

                    Vis3DFrame.groupArteries.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["ARTERIES"].Add(model.Geometry.GUID);
                }

                for (int i = 0; i < modelsArteriesSX.Count(); i++)
                {
                    var modelS = new MeshGeometryModel3D();
                    modelS.Geometry = modelsArteriesSX[i].Geometry;
                    modelS.Material = DiffuseMaterials.Red;
                    modelS.FillMode = SharpDX.Direct3D11.FillMode.Solid;

                    Vis3DFrameStudent.groupArteries.Children.Add(modelS);
                }
                for (int i = 0; i < modelsArteriesDX.Count(); i++)
                {
                    var modelS = new MeshGeometryModel3D();
                    modelS.Geometry = modelsArteriesDX[i].Geometry;
                    modelS.Material = DiffuseMaterials.Red;
                    modelS.FillMode = SharpDX.Direct3D11.FillMode.Solid;

                    Vis3DFrameStudent.groupArteries.Children.Add(modelS);
                }
            }

            if (true)
            {
                var readerVeinsSX = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var readerVeinsDX = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelsVeinsSX = readerVeinsSX.Read(folder + "\\blue_sx.obj");
                var modelsVeinsDX = readerVeinsDX.Read(folder + "\\blue_dx.obj");
                AppControl.Instance.CollisionItemsGuid.Add("VEINS", new List<Guid>());
                for (int i = 0; i < modelsVeinsSX.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsVeinsSX[i].Geometry;
                    model.Material = DiffuseMaterials.Blue;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Solid;
                    AppControl.Instance.CollisionItemsGuid["VEINS"].Add(model.Geometry.GUID);
                    Vis3DFrame.groupVeins.Children.Add(model);
                }
                for (int i = 0; i < modelsVeinsDX.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsVeinsDX[i].Geometry;
                    model.Material = DiffuseMaterials.Blue;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Solid;
                    AppControl.Instance.CollisionItemsGuid["VEINS"].Add(model.Geometry.GUID);
                    Vis3DFrame.groupVeins.Children.Add(model);
                }
                for (int i = 0; i < modelsVeinsSX.Count(); i++)
                {
                    var modelS = new MeshGeometryModel3D();
                    modelS.Geometry = modelsVeinsSX[i].Geometry;
                    modelS.Material = DiffuseMaterials.Blue;
                    modelS.FillMode = SharpDX.Direct3D11.FillMode.Solid;
                    Vis3DFrameStudent.groupVeins.Children.Add(modelS);
                }
                for (int i = 0; i < modelsVeinsDX.Count(); i++)
                {
                    var modelS = new MeshGeometryModel3D();
                    modelS.Geometry = modelsVeinsDX[i].Geometry;
                    modelS.Material = DiffuseMaterials.Blue;
                    modelS.FillMode = SharpDX.Direct3D11.FillMode.Solid;
                    Vis3DFrameStudent.groupVeins.Children.Add(modelS);
                }
            }

            AppControl.Instance.StringSkinAreas.Add("ORBICULARIS");
            AppControl.Instance.StringSkinAreas.Add("PROCERUS");
            AppControl.Instance.StringSkinAreas.Add("CORRUGATOR");
        }

        public void InjectAnesthetic()
        {
            if ((TeacherFrame is Visualization3DFrame) && (Model3DInitialized))
            {
                int index = SelectedPoint3DIndex;

                if (index != -1)
                {
                    if (InjectionPoints3DThisStep[index].ActualReleases.Count == 0)
                    {
                        InjectionPoints3DThisStep[index].ActualReleases.Add(new ActualRelease());
                    }
                    InjectionPoints3DThisStep[index].ActualReleases.Last().ArtificiallyInjectedQuantity = InjectionPoints3DThisStep[index].ActualReleases.Last().ArtificiallyInjectedQuantity + 0.5;
                    InjectionPoints3DThisStep[index].RefreshQuantities();
                }
            }
        }

        public void ListCases()
        {
            AvailableCases.Clear();

            List<string> caseFiles = new List<string>();
            DirectoryInfo baseDir = new DirectoryInfo(AppControl.CasesFolder);
            DirectoryInfo[] ff = baseDir.GetDirectories();
            foreach (DirectoryInfo dd in ff)
            {
                FileInfo[] files = dd.GetFiles("*.txt");
                foreach (FileInfo fff in files)
                {
                    if (fff.FullName.EndsWith("CaseDescriptor.txt"))
                    {
                        caseFiles.Add(fff.FullName);
                        break;
                    }
                }
            }
            // LOOK FOR CaseXml in subfolders of Settings.AppPathLocal + "Cases"
            foreach (string s in caseFiles)
            {
                //Load the case
                ClinicalCase caseCl = new ClinicalCase(s);
                //Add the case to AvailableCases
                AvailableCases.Add(caseCl);
            }
        }

        public void LoadCase()
        {
            CurrentCase.ClearAllUserRelatedFields();
            CurrentCase.EvaluateStepsToAddOrNot();
            for (int k = CurrentCase.Steps.Count - 1; k >= 0; k--)
            {
                if (!CurrentCase.Steps[k].PresentToUser)
                {
                    CurrentCase.Steps.RemoveAt(k);
                }
            }

            CurrentCase.CurrentStepIndex = -1;
            CurrentCaseState = Enum_CaseState.LOADING;

            CurrentCaseState = Enum_CaseState.LOADED;
            AppControl.Instance.WindowTeacher.bBack.IsEnabled = true;
            AppControl.Instance.WindowTeacher.SetCase(CurrentCase.Name);
            AppControl.Instance.WindowStudent.Navigate(new CaseStudentFrame());
            AppControl.Instance.WindowTeacher.Navigate(new CaseTeacherFrame());

            MainStudentFrame.tbCaseName.Text = CurrentCase.Name;

            ResultCaseToSave = new ResultToSave();
            ResultCaseToSave.IsBotoxCase = CurrentCase.Module == Enum_Modules.Botox;
            ResultCaseToSave.CaseName = CurrentCase.Name;
            ResultCaseToSave.CaseDescription = CurrentCase.Description;
        }

        public void LogOut()
        {
            SelectStudent(null);
            SelectTeacher(null);
        }

        public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
        {
            string gg = AutomationProperties.GetName(o);

            if ((o.GetType() == typeof(ModelVisual3D)))
            {
                if ((gg == "modelHumanBlock") || (gg == "modelProbePlane") || (gg == "modelProbeBlock") || (gg == "modelAntenna") || (gg == "modelNeedle") || (gg == "modelStartCircle") || (gg == "modelendCircle") || (gg == "modelHitTestProbe") || (gg == "modelHitTestNeedle"))
                {
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                }
                else
                {
                    return HitTestFilterBehavior.Continue;
                }
            }
            return HitTestFilterBehavior.Continue;
        }

        public void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (AppControl.Instance.mouseClickOrMove)
            //{
            //    var viewport = sender as Viewport3DX;
            //    if (viewport == null) { return; }
            //    var point = e.GetPosition(viewport);
            //    var hitTests = viewport.FindHits(point);
            //    if (hitTests != null && hitTests.Count > 0)
            //    {
            //        foreach (var hit in hitTests)
            //        {
            //PIRINI
            //if (hit.Geometry == GeometryHeadSkin)
            //{
            //    Debug.WriteLine("SKIN");
            //}

            //for (int i = 0; i < AppControl.Instance.modelsVeins.Count; i++)
            //{
            //    if (AppControl.Instance.modelsVeins[i].Geometry == hit.Geometry)
            //    {
            //        Debug.WriteLine("VEIN");
            //    }
            //}

            //for (int i = 0; i < AppControl.Instance.modelsArteries.Count; i++)
            //{
            //    if (AppControl.Instance.modelsArteries[i].Geometry == hit.Geometry)
            //    {
            //        Debug.WriteLine("ARTERY");
            //    }
            //}
            //        }
            //    }
            //}
        }

        public void OnMouseMoveHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!AppControl.Instance.mouseClickOrMove)
            {
                if ((DateTime.Now - AppControl.Instance.lastMouseMove).TotalMilliseconds > 100)
                {
                    //if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    //{
                    //DateTime c = DateTime.Now;
                    //var viewport = sender as Viewport3DX;
                    //var cameraPosition = viewport.Camera.Position;
                    //List<PointHit> pointsHit = new List<PointHit>();
                    //if (viewport == null) { return; }
                    //var point = e.GetPosition(viewport);
                    //var hitTests = viewport.FindHits(point);

                    //Vector3D direction = new Vector3D(1, 0, 0); // replace with your vector
                    //Point3D origin = new Point3D(0, 0, 0); // replace with your origin
                    //Ray3D ray = new Ray3D(origin, direction);

                    //if (hitTests != null && hitTests.Count > 0)
                    //{
                    //    foreach (var hit in hitTests)
                    //    {
                    //PIRINI
                    //if (hit.Geometry == GeometryHeadSkin)
                    //{
                    //    pointsHit.Add(new PointHit(hit.PointHit, "SKIN"));
                    //    //Debug.WriteLine("SKIN");
                    //}

                    //for (int i = 0; i < AppControl.Instance.modelsVeins.Count; i++)
                    //{
                    //    if (AppControl.Instance.modelsVeins[i].Geometry == hit.Geometry)
                    //    {
                    //        pointsHit.Add(new PointHit(hit.PointHit, "VEIN"));
                    //        //Debug.WriteLine("VEIN");
                    //    }
                    //}

                    //for (int i = 0; i < AppControl.Instance.modelsArteries.Count; i++)
                    //{
                    //    if (AppControl.Instance.modelsArteries[i].Geometry == hit.Geometry)
                    //    {
                    //        pointsHit.Add(new PointHit(hit.PointHit, "ARTERY"));
                    //        //Debug.WriteLine("ARTERY");
                    //    }
                    //}
                    //        }
                    //    }
                    //    double minDistance = double.MaxValue;
                    //    int indexMin = -1;
                    //    if (pointsHit.Count > 0)
                    //    {
                    //        for (int i = 0; i < pointsHit.Count; i++)
                    //        {
                    //            var distance = AppControl.Instance.CalcDistance(pointsHit[i].Point, cameraPosition);
                    //            if (distance < minDistance)
                    //            {
                    //                minDistance = distance;
                    //                indexMin = i;
                    //            }
                    //        }

                    //        AppControl.Instance.TransformSphere.OffsetX = pointsHit[indexMin].Point.X;
                    //        AppControl.Instance.TransformSphere.OffsetY = pointsHit[indexMin].Point.Y;
                    //        AppControl.Instance.TransformSphere.OffsetZ = pointsHit[indexMin].Point.Z;

                    //        OnPropertyChanged(nameof(AppControl.Instance.TransformSphere));
                    //        AppControl.Instance.HitStructure = pointsHit[indexMin].Name;
                    //        Debug.WriteLine(AppControl.Instance.HitStructure);
                    //    }
                    //    else
                    //    {
                    //        AppControl.Instance.HitStructure = "None";
                    //    }

                    //    AppControl.Instance.lastMouseMove = DateTime.Now;
                    //    Debug.WriteLine((DateTime.Now - c).TotalMilliseconds.ToString());
                    //}
                }
            }
        }

        public void PopulateAvailableCases()
        {
            // find all available cases
            Cases.Clear();

            var directories = Directory.GetDirectories(AppControl.CasesFolder);
            foreach (string s in directories)
            {
                ClinicalCase cs = new ClinicalCase(s);

                Cases.Add(cs);

                cs.LoadCaseDescriptor();
            }
        }

        public void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public void ReadCoordinateFile()
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
                TranslationPointModel = new Point3D(x, y, z);
                LengthNeedle = (float)lengthNeedleR;
            }
        }

        public void SaveResult()
        {
            Results result = new Results();
            result.IdCase = 0;
            result.CaseName = CurrentCase.Name;

            DateTime now = DateTime.Now;
            result.Date = DateTime.Now.ToString(new CultureInfo(16));

            result.Score = CurrentCaseScore;
            result.IdEvent = (CurrentEvent == null ? -1 : CurrentEvent.Id);
            result.IdStudent = (CurrentStudent == null ? -1 : CurrentStudent.Id);
            result.IdTeacher = (CurrentTeacher == null ? -1 : CurrentTeacher.Id);

            DateTime tt = DateTime.Now;
            string baseFolder = AppControl.SaveResultsFolder + result.CaseName + "_" +
                tt.Year.ToString() + tt.Month.ToString() + tt.Day.ToString() + "_" +
                tt.Hour.ToString() + tt.Minute.ToString() + tt.Second.ToString();

            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            string filePath = baseFolder + "\\CaseSaved.xml";

            ResultCaseToSave.Save<ResultToSave>(filePath);
            result.FilePath = filePath;
            DBConnector.Instance.InsertRow<Results>(result);
        }

        public void SetEnabled(UIElement lbl, bool enabled)
        {
            if (!WindowTeacher.Dispatcher.CheckAccess())
            {
                SetEnabledDelegate d = new SetEnabledDelegate(SetEnabled);
                WindowTeacher.Dispatcher.Invoke(d, new object[] { lbl, enabled });
            }
            else
            {
                lbl.IsEnabled = enabled;
            }
        }

        public void SetIndicator(System.Windows.Shapes.Ellipse bb, bool active)
        {
            if (!WindowTeacher.Dispatcher.CheckAccess())
            {
                SetIndicatorDelegate d = new SetIndicatorDelegate(SetIndicator);
                WindowTeacher.Dispatcher.Invoke(d, new object[] { bb, active });
            }
            else
            {
                bb.Fill = active ? ActiveEllipse : Brushes.Gray;
            }
        }

        public void SetText(TextBlock lbl, string text)
        {
            if (!WindowTeacher.Dispatcher.CheckAccess())
            {
                SetTextDelegate d = new SetTextDelegate(SetText);
                WindowTeacher.Dispatcher.Invoke(d, new object[] { lbl, text });
            }
            else
            {
                lbl.Text = text;
            }
        }

        public void SetTimer(bool enabledOrNot)
        {
            caseTimer.Enabled = enabledOrNot;
        }

        public void StopCase(bool IhaveToSaveResults)
        {
            if (CurrentCaseState == Enum_CaseState.LOADED)
            {
                CurrentCaseState = Enum_CaseState.NOTLOADED;

                SetTimer(false);
            }
            if ((IhaveToSaveResults) && (CurrentTeacher != null) && (CurrentStudent != null) && ResultCaseToSave != null)
            {
                AppControl.Instance.SaveResult();
            }
            CurrentCase.ClearAllUserRelatedFields();
            //AppControl.Instance.WindowTeacher.SetCase("");
        }

        public void UpdateVisibilityItems(bool firstLoad, object sender = null)
        {
            if (firstLoad)
            {
                Vis3DFrame.cbSkin.IsChecked = true;
                Vis3DFrame.cbSkinWireframe.IsChecked = false;
                Vis3DFrame.cbVeins.IsEnabled = false;
                Vis3DFrame.cbArteries.IsEnabled = false;
                Vis3DFrame.cbVeins.IsChecked = false;
                Vis3DFrame.cbArteries.IsChecked = false;
            }
            else
            {
                if (sender != null)
                {
                    CheckBox cb = (CheckBox)sender;
                    if ((cb.Name == "cbSkin") && (cb.IsChecked == true))
                    {
                        Vis3DFrame.cbSkinWireframe.IsChecked = false;
                        Vis3DFrame.cbVeins.IsEnabled = false;
                        Vis3DFrame.cbArteries.IsEnabled = false;
                        Vis3DFrame.cbVeins.IsChecked = false;
                        Vis3DFrame.cbArteries.IsChecked = false;
                    }

                    if ((cb.Name == "cbSkinWireframe") && (cb.IsChecked == true))
                    {
                        Vis3DFrame.cbSkin.IsChecked = false;
                        Vis3DFrame.cbVeins.IsEnabled = true;
                        Vis3DFrame.cbArteries.IsEnabled = true;
                    }
                }
            }

            AppControl.Instance.ProcerusVisible = Vis3DFrame.cbProcerus.IsChecked.Value;
            AppControl.Instance.CorrugatorVisible = Vis3DFrame.cbCorrugator.IsChecked.Value;
            AppControl.Instance.OrbicularisVisible = Vis3DFrame.cbOrbicularis.IsChecked.Value;
            AppControl.Instance.VeinsVisible = Vis3DFrame.cbVeins.IsChecked.Value;
            AppControl.Instance.ArteriesVisible = Vis3DFrame.cbArteries.IsChecked.Value;
            AppControl.Instance.SkinVisible = Vis3DFrame.cbSkin.IsChecked.Value || Vis3DFrame.cbSkinWireframe.IsChecked.Value;

            AppControl.Instance.UpdateSkinVisibilityOn3DModels();
        }

        public void UpdateVisizationZoom()
        {
            switch (AppControl.Instance.CurrentArea)
            {
                case Enum_AreaDefinition.FRONTAL:

                    //FRONTAL!!
                    Vis3DFrame.hvView3D.Camera.UpDirection = new Vector3D(.7, .7, 0);
                    Vis3DFrame.hvView3D.Camera.LookDirection = new Vector3D(-.4, .6, 0);

                    Vis3DFrame.hvView3D.Camera.Position = new Point3D(450 + AppControl.Instance.TranslationPointModel.X, -500 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrame.hvView3D.LookAt(new Point3D(360 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z), 3000);

                    break;

                case Enum_AreaDefinition.CENTRAL:

                    //CENTRAL!!
                    Vis3DFrame.hvView3D.Camera.UpDirection = new Vector3D(1, 0, 0);
                    Vis3DFrame.hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, 0);
                    Vis3DFrame.hvView3D.Camera.Position = new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -500 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrame.hvView3D.LookAt(new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z), 3000);
                    break;

                case Enum_AreaDefinition.ORBICULAR_LEFT:
                    //ORBICULAR LEFT!
                    Vis3DFrame.hvView3D.Camera.UpDirection = new Vector3D(.7, .30, 0);
                    Vis3DFrame.hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, .7);
                    Vis3DFrame.hvView3D.Camera.Position = new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, -300 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrame.hvView3D.LookAt(new Point3D(250 + AppControl.Instance.TranslationPointModel.X, -150 + AppControl.Instance.TranslationPointModel.Y, -130 + AppControl.Instance.TranslationPointModel.Z), 3000);
                    break;

                case Enum_AreaDefinition.ORBICULAR_RIGHT:
                    //ORBILULAR RIGHT!
                    Vis3DFrame.hvView3D.Camera.UpDirection = new Vector3D(.7, .30, 0);
                    Vis3DFrame.hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, -.7);
                    Vis3DFrame.hvView3D.Camera.Position = new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -300 + AppControl.Instance.TranslationPointModel.Y, 300 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrame.hvView3D.LookAt(new Point3D(250 + AppControl.Instance.TranslationPointModel.X, -250 + AppControl.Instance.TranslationPointModel.Y, 130 + AppControl.Instance.TranslationPointModel.Z), 3000);
                    break;

                case Enum_AreaDefinition.NASAL:
                    break;

                case Enum_AreaDefinition.ORBICULAR:
                    break;

                default:
                    break;
            }

            switch (AppControl.Instance.CurrentArea)
            {
                case Enum_AreaDefinition.FRONTAL:

                    //FRONTAL!!
                    Vis3DFrameStudent.hvView3D.Camera.UpDirection = new Vector3D(.7, .7, 0);
                    Vis3DFrameStudent.hvView3D.Camera.LookDirection = new Vector3D(-.4, .6, 0);

                    Vis3DFrameStudent.hvView3D.Camera.Position = new Point3D(450 + AppControl.Instance.TranslationPointModel.X, -500 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrameStudent.hvView3D.LookAt(new Point3D(360 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z), 3000);

                    break;

                case Enum_AreaDefinition.CENTRAL:

                    //CENTRAL!!
                    Vis3DFrameStudent.hvView3D.Camera.UpDirection = new Vector3D(1, 0, 0);
                    Vis3DFrameStudent.hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, 0);
                    Vis3DFrameStudent.hvView3D.Camera.Position = new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -500 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrameStudent.hvView3D.LookAt(new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, 0 + AppControl.Instance.TranslationPointModel.Z), 3000);
                    break;

                case Enum_AreaDefinition.ORBICULAR_LEFT:
                    //ORBICULAR LEFT!
                    Vis3DFrameStudent.hvView3D.Camera.UpDirection = new Vector3D(.7, .30, 0);
                    Vis3DFrameStudent.hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, .7);
                    Vis3DFrameStudent.hvView3D.Camera.Position = new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, -300 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrameStudent.hvView3D.LookAt(new Point3D(250 + AppControl.Instance.TranslationPointModel.X, -150 + AppControl.Instance.TranslationPointModel.Y, -130 + AppControl.Instance.TranslationPointModel.Z), 3000);
                    break;

                case Enum_AreaDefinition.ORBICULAR_RIGHT:
                    //ORBILULAR RIGHT!
                    Vis3DFrameStudent.hvView3D.Camera.UpDirection = new Vector3D(.7, .30, 0);
                    Vis3DFrameStudent.hvView3D.Camera.LookDirection = new Vector3D(-.3, .7, -.7);
                    Vis3DFrameStudent.hvView3D.Camera.Position = new Point3D(300 + AppControl.Instance.TranslationPointModel.X, -200 + AppControl.Instance.TranslationPointModel.Y, 300 + AppControl.Instance.TranslationPointModel.Z);
                    Vis3DFrameStudent.hvView3D.LookAt(new Point3D(250 + AppControl.Instance.TranslationPointModel.X, -150 + AppControl.Instance.TranslationPointModel.Y, 130 + AppControl.Instance.TranslationPointModel.Z), 3000);
                    break;

                case Enum_AreaDefinition.NASAL:
                    break;

                case Enum_AreaDefinition.ORBICULAR:
                    break;

                default:
                    break;
            }
        }

        internal void ChangeSelectionPointListView()
        {
            if (!AppControl.Instance.Feedback3DOn)
            {
                for (int i = 0; i < Vis3DFrame.hvView3D.Items.Count; i++)
                {
                    if (Vis3DFrame.hvView3D.Items[i].Tag != null)
                    {
                        if (Vis3DFrame.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                        {
                            string rr = Vis3DFrame.hvView3D.Items[i].Tag.ToString().Replace("ThisIsAPoint", "");
                            MeshGeometryModel3D aa = (MeshGeometryModel3D)Vis3DFrame.hvView3D.Items[i];
                            if (Int32.Parse(rr) == Vis3DFrame.InjectionPointListView.SelectedIndex)
                            {
                                aa.Material = DiffuseMaterials.Blue;
                            }
                            else
                            {
                                aa.Material = DiffuseMaterials.LightBlue;
                            }
                        }
                    }
                }

                for (int i = 0; i < Vis3DFrameStudent.hvView3D.Items.Count; i++)
                {
                    if (Vis3DFrameStudent.hvView3D.Items[i].Tag != null)
                    {
                        if (Vis3DFrameStudent.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                        {
                            string rr = Vis3DFrameStudent.hvView3D.Items[i].Tag.ToString().Replace("ThisIsAPoint", "");
                            MeshGeometryModel3D aa = (MeshGeometryModel3D)Vis3DFrameStudent.hvView3D.Items[i];
                            if (Int32.Parse(rr) == Vis3DFrame.InjectionPointListView.SelectedIndex)
                            {
                                aa.Material = DiffuseMaterials.Blue;
                            }
                            else
                            {
                                aa.Material = DiffuseMaterials.LightBlue;
                            }
                        }
                    }
                }
            }
        }

        internal void ClearTempResults()
        {
            if (!Directory.Exists(AppControl.ResultsTempFolder))
            {
                Directory.CreateDirectory(AppControl.ResultsTempFolder);
            }
            DirectoryInfo di = new DirectoryInfo(AppControl.ResultsTempFolder);

            FileInfo[] files = di.GetFiles("*.pdf");
            foreach (FileInfo ff in files)
            {
                ff.Delete();
            }
        }

        internal bool CloseCurrentCase()
        {
            try
            {
                if (CurrentCase.CurrentStepIndex == CurrentCase.Steps.Count - 1)
                {
                    StopCase(true);
                    AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
                    AppControl.Instance.WindowStudent.Navigate(new MessageVoid());
                }
                else
                {
                    if (MessageBox.Show("Are you sure you want to stop this case? Results will not be saved", true, 1000))
                    {
                        StopCase(false);
                        AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
                        AppControl.Instance.WindowStudent.Navigate(new MessageVoid());
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal void CloseWindowImages()
        {
            if (AppControl.Instance.ShowImagesOn3D != null)
            {
                AppControl.Instance.ShowImagesOn3D.Close();
                AppControl.Instance.ShowImagesOn3D = null;
                WindowStudent.IsEnabled = true;
            }
        }

        internal void ConnectBeautySim()
        {
            HwndSource hwnd = HwndSource.FromHwnd(new WindowInteropHelper(WindowTeacher).Handle);
            BeautySimController.Instance.ScannedSimulatorsEvent += new BeautySimController.ScannedSimulatorsDelegate(ScannedSimulatorListener);
            BeautySimController.Instance.AMessageArrivedEvent += new BeautySimController.AMessageArrivedDelegate(AMessageArriveListener);
            BeautySimController.Instance.SerialNumberAcquired += BeautySim_SerialNumberAcquired;

            BeautySimController.Instance.Go();
            BeautySimController.Instance.StartHookDevices(hwnd);
        }

        internal void ConnectPDI()
        {
            pDIClass.OnConnectionStatusChanged += PDIClass_OnConnectionStatusChanged;
            pDIClass.OnNewFrameAvailable += PDIClass_OnNewFrameAvailable;
            pDIClass.Connect(new WindowInteropHelper(WindowTeacher).Handle);
            timerCheckPolhemus = new DispatcherTimer();
            timerCheckPolhemus.Interval = new TimeSpan(0, 0, 1);
            timerCheckPolhemus.IsEnabled = true;
            timerCheckPolhemus.Tick += new EventHandler(TimerCheckPolhemusTick_Listener);
            timerCheckPolhemus.Start();
        }

        internal void DisconnectBeautySim()
        {
            BeautySimController.Instance.ScannedSimulatorsEvent -= new BeautySimController.ScannedSimulatorsDelegate(ScannedSimulatorListener);
            BeautySimController.Instance.AMessageArrivedEvent -= new BeautySimController.AMessageArrivedDelegate(AMessageArriveListener);
            BeautySimController.Instance.SerialNumberAcquired -= BeautySim_SerialNumberAcquired;
            BeautySimController.Instance.Dispose();
        }

        internal void DisconnectPDI()
        {
            pDIClass.OnConnectionStatusChanged -= PDIClass_OnConnectionStatusChanged;
            pDIClass.OnNewFrameAvailable -= PDIClass_OnNewFrameAvailable;
            pDIClass.Disconnect();
        }

        internal void DrawInjectionPointsSpecial()
        {
            AppControl.Instance.Feedback3DOn = true;
            for (int i = 0; i < Vis3DFrame.hvView3D.Items.Count; i++)
            {
                if (Vis3DFrame.hvView3D.Items[i].Tag != null)
                {
                    if (Vis3DFrame.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                    {
                        string rr = Vis3DFrame.hvView3D.Items[i].Tag.ToString().Replace("ThisIsAPoint", "");
                        int index = Int32.Parse(rr);
                        MeshGeometryModel3D aa = (MeshGeometryModel3D)Vis3DFrame.hvView3D.Items[i];

                        InjectionPoint3D ijP = (InjectionPoint3D)Vis3DFrame.InjectionPointListView.Items[index];

                        if (ijP.IsError && ijP.ActuallyChosenOrPerformedQuantity > 0)
                        {
                            aa.Material = DiffuseMaterials.Red;
                        }
                        else
                        {
                            if (ijP.ActuallyChosenOrPerformedQuantity > ijP.PrescribedQuantity*1.5)
                            {
                                aa.Material = DiffuseMaterials.Orange;
                            }
                            else if (ijP.ActuallyChosenOrPerformedQuantity < ijP.PrescribedQuantity*0.7)
                            {
                                aa.Material = DiffuseMaterials.Yellow;
                            }
                            else
                            {
                                aa.Material = DiffuseMaterials.Green;
                            }
                        }
                    }
                }
            }
            AppControl.Instance.Feedback3DOn = true;
            for (int i = 0; i < Vis3DFrameStudent.hvView3D.Items.Count; i++)
            {
                if (Vis3DFrameStudent.hvView3D.Items[i].Tag != null)
                {
                    if (Vis3DFrameStudent.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                    {
                        string rr = Vis3DFrameStudent.hvView3D.Items[i].Tag.ToString().Replace("ThisIsAPoint", "");
                        int index = Int32.Parse(rr);
                        MeshGeometryModel3D aa = (MeshGeometryModel3D)Vis3DFrameStudent.hvView3D.Items[i];

                        InjectionPoint3D ijP = (InjectionPoint3D)Vis3DFrame.InjectionPointListView.Items[index];

                        if (ijP.IsError && ijP.ActuallyChosenOrPerformedQuantity > 0)
                        {
                            aa.Material = DiffuseMaterials.Red;
                        }
                        else
                        {
                            if (ijP.ActuallyChosenOrPerformedQuantity > ijP.PrescribedQuantity)
                            {
                                aa.Material = DiffuseMaterials.Orange;
                            }
                            else if (ijP.ActuallyChosenOrPerformedQuantity < ijP.PrescribedQuantity)
                            {
                                aa.Material = DiffuseMaterials.Yellow;
                            }
                            else
                            {
                                aa.Material = DiffuseMaterials.Green;
                            }
                        }
                    }
                }
            }
        }

        internal List<string> GetListOfStringCases(string module)
        {
            List<string> toRet = new List<string>();
            foreach (ClinicalCase cs in Cases)
            {
                if (module == Enum.GetName(typeof(Enum_Modules), cs.Module))
                {
                    toRet.Add(cs.Name);
                }
            }

            return toRet;
        }

        internal List<string> GetListOfStringCasesDescription(string module)
        {
            List<string> toRet = new List<string>();
            foreach (ClinicalCase cs in Cases)
            {
                if (module == Enum.GetName(typeof(Enum_Modules), cs.Module))
                {
                    toRet.Add(cs.Description);
                }
            }
            return toRet;
        }

        internal List<string> GetListOfStringCasesImages(string currentModule)
        {
            List<string> toRet = new List<string>();
            foreach (ClinicalCase cs in Cases)
            {
                if (currentModule == Enum.GetName(typeof(Enum_Modules), cs.Module))
                {
                    toRet.Add(cs.Folder + "\\" + cs.ImageName);
                }
            }
            return toRet;
        }

        internal string GiveMeTheEvent(int idEvent)
        {
            Events entries = DBConnector.Instance.FindRowById<Events>(new BsonValue(idEvent));

            if (entries != null)
            {
                return entries.ShortName;
            }
            return "";
        }

        internal string GiveMeTheStudent(int idStudent)
        {
            Users entries = DBConnector.Instance.FindRowById<Users>(new BsonValue(idStudent));

            if (entries != null)
            {
                return entries.Name + " " + entries.Surname;
            }
            return "";
        }

        internal void InitViewModel()
        {
            if (!Model3DInitialized)
            {
                EffectsManager = new DefaultEffectsManager();

                if (TeacherFrame is Visualization3DFrame)
                {
                    InitializeModels(AppControl.CadsFolder);
                    InitializeCoordinates();

                    //((Visualization3DFrameStudent)StudentFrame).bModelView.DataContext = ((Visualization3DFrame)TeacherFrame).MainGrid;

                    double h = ((Visualization3DFrame)TeacherFrame).grInteractive3D.ActualHeight;
                    double w = ((Visualization3DFrame)TeacherFrame).grInteractive3D.ActualWidth;
                    //double hs = ((Visualization3DFrameStudent)StudentFrame).bModelView.ActualHeight;
                    //double ws = ((Visualization3DFrameStudent)StudentFrame).bModelView.ActualWidth;
                    //((Visualization3DFrameStudent)StudentFrame).bModelView.Height = hs;
                    //((Visualization3DFrameStudent)StudentFrame).bModelView.Width = hs * w / h;

                    //((Visualization3DFrameStudent)StudentFrame).MainGrid.ColumnDefinitions[1].Width = new GridLength(hs * w / h);
                }
                Model3DInitialized = true;
            }
        }

        internal void ManageViewImagesOn3D()
        {
            if (AppControl.Instance.AreImagesVisualizedOn3D)
            {
                CloseWindowImages();
            }
            else
            {
                AppControl.Instance.ShowImagesOn3D = new ShowImagesWindow();
                AppControl.Instance.ShowImagesOn3D.Loaded += AppControl.Instance.ShowImagesOn3D_Loaded;

                string nameFileA = AppControl.Instance.CurrentCase.Folder + "\\" + ((ClinicalCaseStep_Face3DInteraction)AppControl.Instance.CurrentCase.Steps[AppControl.Instance.CurrentCase.CurrentStepIndex]).ImageName;
                if (File.Exists(nameFileA))
                {
                    AppControl.Instance.ShowImagesOn3D.imStatic.Source = new BitmapImage(new Uri(nameFileA, UriKind.RelativeOrAbsolute));
                }
                string nameFileB = AppControl.Instance.CurrentCase.Folder + "\\" + ((ClinicalCaseStep_Face3DInteraction)AppControl.Instance.CurrentCase.Steps[AppControl.Instance.CurrentCase.CurrentStepIndex]).ImageNameReference;
                if (File.Exists(nameFileB))
                {
                    AppControl.Instance.ShowImagesOn3D.imDynamic.Source = new BitmapImage(new Uri(nameFileB, UriKind.RelativeOrAbsolute));
                }
                AppControl.Instance.ShowImagesOn3D.Show();
                WindowStudent.IsEnabled = false;
            }
            AppControl.Instance.AreImagesVisualizedOn3D = !AppControl.Instance.AreImagesVisualizedOn3D;

            if ((AppControl.Instance.WindowTeacher.PageContainer.Content is CaseTeacherFrame))
            {
                ((CaseTeacherFrame)(AppControl.Instance.WindowTeacher.PageContainer.Content)).tbViewImages.Text = AppControl.Instance.AreImagesVisualizedOn3D ? BeautySim.Globalization.Language.hide_images : BeautySim.Globalization.Language.view_images;
            }
        }

        internal void RegisterStudent(CaseStudentFrame caseStudentFrame)
        {
            MainStudentFrame = caseStudentFrame;
        }

        internal void RegisterTeacher(CaseTeacherFrame caseTeacherFrame)
        {
            MainTeacherFrame = caseTeacherFrame;
            MainTeacherFrame.listItems.ItemsSource = CurrentCase.Steps;
            foreach (ClinicalCaseStep cl in CurrentCase.Steps)
            {
                cl.Excludable = Properties.Settings.Default.DebugMode;
            }
            AdvanceStep();
        }

        internal void SelectEvent(Events selectedItem)
        {
            CurrentEvent = selectedItem;
            if (CurrentEvent != null)
            {
                WindowTeacher.lEvent.Text = selectedItem.ShortName;
            }
            else
            {
                WindowTeacher.lEvent.Text = "";
            }
        }

        internal void SelectStudent(Users p)
        {
            CurrentStudent = p;
            if (CurrentStudent != null)
            {
                WindowTeacher.lStudent.Text = p.Title + " " + p.Name + " " + p.Surname;
            }
            else
            {
                WindowTeacher.lStudent.Text = "";
            }
        }

        internal void SelectTeacher(Users p)
        {
            CurrentTeacher = p;
            if (CurrentTeacher != null)
            {
                WindowTeacher.lTeacher.Text = p.Title + " " + p.Name + " " + p.Surname;
                WindowTeacher.lTeacher.Foreground = p.Role == 2 ? Brushes.RoyalBlue : Brushes.White;
            }
            else
            {
                WindowTeacher.lTeacher.Text = "";
            }
        }

        internal void SetContent(ClinicalCaseStep_Face3DInteraction caseStepCurrent)
        {
            string filePoints = AppControl.Instance.CurrentCase.Folder + "\\" + caseStepCurrent.PointDefinitionFileName;
            if (File.Exists(filePoints))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<InjectionPointSpecific2D>));

                // Deserialize from file
                using (FileStream fileStream = new FileStream(filePoints, FileMode.Open))
                {
                    List<InjectionPointSpecific2D> anjectionPoints2D = (List<InjectionPointSpecific2D>)deserializer.Deserialize(fileStream);
                    AppControl.Instance.InjectionPoints2D = new ObservableCollection<InjectionPointSpecific2D>(anjectionPoints2D);

                    Console.WriteLine("Deserialized List:");
                }
            }

            if (File.Exists(AppControl.PathFilePointsBase3D))
            {
                AppControl.Instance.InjectionPoints3D = PointsManager.Instance.LoadInjectionPoints3D(AppControl.PathFilePointsBase3D);
            }
            AppControl.Instance.InjectionPoints3DThisStep = new ObservableCollection<InjectionPoint3D>();

            CurrentArea = caseStepCurrent.AreaDefinition;


            List<Enum_AreaDefinition> areasToChek = new List<Enum_AreaDefinition>();
            areasToChek.Add(caseStepCurrent.AreaDefinition);
            if (areasToChek.Contains(Enum_AreaDefinition.CENTRAL))
            {
                areasToChek.Add(Enum_AreaDefinition.NASAL);
            }
            for (int i = 0; i < AppControl.Instance.InjectionPoints3D.Count; i++)
            {
                if (areasToChek.Contains(AppControl.Instance.InjectionPoints3D[i].AreaDef))
                {
                    AppControl.Instance.InjectionPoints3DThisStep.Add(AppControl.Instance.InjectionPoints3D[i]);
                }
            }

            foreach (InjectionPoint3D toCalc in AppControl.Instance.InjectionPoints3DThisStep)
            {
                // Find the nearest point in the grid with the SmartGridAssigner algorithm
                //      Initialize variables to track the nearest and second nearest points
                double minimumDistance = Double.MaxValue; // Smallest distance found
                double secondMinimumDistance = Double.MaxValue; // Second smallest distance found
                foreach (InjectionPoint3D toCalc2 in AppControl.Instance.InjectionPoints3DThisStep)
                {
                    if (toCalc != toCalc2)
                    {
                        double distance = Math.Sqrt(Math.Pow(toCalc2.X - toCalc.X, 2) + Math.Pow(toCalc2.Y - toCalc.Y, 2) + Math.Pow(toCalc2.Z - toCalc.Z, 2));

                        if (distance < minimumDistance)
                        {
                            // Update second nearest with the previous nearest
                            secondMinimumDistance = minimumDistance;

                            // Update nearest with the new nearest
                            minimumDistance = distance;
                        }
                        else if (distance < secondMinimumDistance && distance != minimumDistance)
                        {
                            // Update second nearest if this point is closer than the current second nearest but not equal to the current nearest
                            secondMinimumDistance = distance;
                        }
                    }
                }

                toCalc.MinDistanceFromNeighbours = Math.Max(minimumDistance,secondMinimumDistance);
            }

            foreach (InjectionPoint3D item3D in AppControl.Instance.InjectionPoints3DThisStep)
            {
                var dueDP = AppControl.Instance.InjectionPoints2D.Where(x => x.PointDefinition == item3D.PointDefinition).FirstOrDefault();
                if (dueDP != null)
                {
                    item3D.PrescribedQuantity = dueDP.PrescribedQuantity;
                }
            }
            //WindowMain.pointsListView3D.SelectionChanged += PointsListView3D_SelectionChanged;
            //SaveDynamicInfo3DXML(pathFilePointsBase3D);

            InitializePointsToBeShown();
            UpdateVisizationZoom();

            HasBeen3DPointsLoaded = true;
        }

        internal void ShowImagesOn3D_Loaded(object sender, RoutedEventArgs e)
        {
            if ((AppControl.Instance.ShowImagesOn3D != null) && (AppControl.Instance.WindowStudent != null))
            {
                double left = AppControl.Instance.WindowStudent.Left + (AppControl.Instance.WindowStudent.Width - AppControl.Instance.ShowImagesOn3D.Width) / 2;
                double top = AppControl.Instance.WindowStudent.Top + (AppControl.Instance.WindowStudent.Height - AppControl.Instance.ShowImagesOn3D.Height) / 2;

                AppControl.Instance.ShowImagesOn3D.Left = left;
                AppControl.Instance.ShowImagesOn3D.Top = top;
                AppControl.Instance.ShowImagesOn3D.Topmost = true;
            }
        }

        internal void SwitchMousePosition(CaseTeacherFrame caseTeacherFrame)
        {
            if (CurrentLocateMouseSOrT)
            {
                AppControl.Instance.WindowTeacher.SetCursor((int)AppControl.Instance.WindowTeacher.Width / 2, (int)AppControl.Instance.WindowTeacher.Height / 2);
            }
            else
            {
                AppControl.Instance.WindowStudent.SetCursor((int)AppControl.Instance.WindowStudent.Width / 2, (int)AppControl.Instance.WindowStudent.Height / 2);
            }
            CurrentLocateMouseSOrT = !CurrentLocateMouseSOrT;
            //frame.tbLocateMouse.Text = CurrentLocateMouseSOrT ? "mouse to teacher" : "mouse to student";
        }

        internal void UpdateSkinVisibilityOn3DModels()
        {
            for (int i = 0; i < Vis3DFrame.groupSkin.Children.Count(); i++)
            {
                MeshGeometryModel3D u = (MeshGeometryModel3D)Vis3DFrame.groupSkin.Children[i];
                u.FillMode = Vis3DFrame.cbSkinWireframe.IsChecked.Value ? SharpDX.Direct3D11.FillMode.Wireframe : SharpDX.Direct3D11.FillMode.Solid;
                u.Material = Vis3DFrame.cbSkinWireframe.IsChecked.Value ? DiffuseMaterials.Glass : DiffuseMaterials.Gray;
            }

            for (int i = 0; i < Vis3DFrameStudent.groupSkin.Children.Count(); i++)
            {
                MeshGeometryModel3D u = (MeshGeometryModel3D)Vis3DFrameStudent.groupSkin.Children[i];
                u.FillMode = Vis3DFrame.cbSkinWireframe.IsChecked.Value ? SharpDX.Direct3D11.FillMode.Wireframe : SharpDX.Direct3D11.FillMode.Solid;
                u.Material = Vis3DFrame.cbSkinWireframe.IsChecked.Value ? DiffuseMaterials.Glass : DiffuseMaterials.Gray;
            }
        }

        internal void UpdateTheConsequences(int selectedConsequenceIndex)
        {
            if (StudentFrame is InteractionFrame)
            {
                ((InteractionFrame)StudentFrame).UpdateTheConsequence(selectedConsequenceIndex);
            }
            if (TeacherFrame is InteractionFrame)
            {
                ((InteractionFrame)TeacherFrame).UpdateTheConsequence(selectedConsequenceIndex);
            }

            CaseStudentFrame csf = (CaseStudentFrame)WindowStudent.PageContainer.Content;
            if (selectedConsequenceIndex >= ((InteractionFrame)StudentFrame).ConsequencesToShow.Count - 1)
            {
                csf.bOk.Visibility = Visibility.Visible;
                ((InteractionFrame)StudentFrame).AlreadyCheckedAllConsequencies = true;
            }
            else
            {
                if (!((InteractionFrame)StudentFrame).AlreadyCheckedAllConsequencies)
                {
                    csf.bOk.Visibility = Visibility.Hidden;
                }
                else
                {
                    csf.bOk.Visibility = Visibility.Visible;
                }
            }
        }

        internal void UpdateTheConsequences3D()
        {
            if (StudentFrame is Visualization3DFrameStudent)
            {
                ((Visualization3DFrameStudent)StudentFrame).UpdateTheConsequence();
            }
            if (TeacherFrame is Visualization3DFrame)
            {
                ((Visualization3DFrame)TeacherFrame).UpdateTheConsequence();
            }

            CaseStudentFrame csf = (CaseStudentFrame)WindowStudent.PageContainer.Content;
            if (CurrentClinicalCaseStep_Face3DInteraction.SelectedConsequenceIndex >= CurrentClinicalCaseStep_Face3DInteraction.Consequences.Count - 1)
            {
                csf.bOk.Visibility = Visibility.Visible;
                CurrentClinicalCaseStep_Face3DInteraction.AlreadyCheckedAllConsequencies = true;
            }
            else
            {
                if (!(CurrentClinicalCaseStep_Face3DInteraction.AlreadyCheckedAllConsequencies))
                {
                    csf.bOk.Visibility = Visibility.Hidden;
                }
                else
                {
                    csf.bOk.Visibility = Visibility.Visible;
                }
            }
        }

        public ClinicalCaseStep_Face3DInteraction CurrentClinicalCaseStep_Face3DInteraction { get; private set; }

        internal void UpdateTranslationManikin3D()
        {
            AppControl.Instance.TranslationManikin3D = new System.Windows.Media.Media3D.TranslateTransform3D(AppControl.Instance.TranslationPointModel.X, AppControl.Instance.TranslationPointModel.Y, AppControl.Instance.TranslationPointModel.Z);
        }

        internal void WindowStudentLoaded(object sender, RoutedEventArgs e)
        {
            VisibilityBorderArrowDown = Visibility.Hidden;
            VisibilityBorderArrowLeft = Visibility.Hidden;
            VisibilityBorderArrowRight = Visibility.Hidden;
            VisibilityBorderArrowRotation = Visibility.Hidden;
            VisibilityBorderArrowRotation2 = Visibility.Hidden;
            VisibilityBorderArrowUp = Visibility.Hidden;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                disposedValue = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnCameraModelChanged()
        {
            var eh = CameraModelChanged;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string info = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        private void AMessageArriveListener(string serial, byte type, byte[] message)
        {
            try
            {
                if (!WindowTeacher.Dispatcher.CheckAccess())
                {
                    WindowTeacher.Dispatcher.Invoke(new Action(() =>
                    {
                        if (type == (byte)Enum_SimulatorMessageType.FLUXGYRO)
                        {
                            OrientationBeautySim = (BeautySimController.Instance.Simulator).Orientation;
                            LastOrientationBeautySimArrived = DateTime.Now;
                        }
                        if (type == (byte)Enum_SimulatorMessageType.FLUX)
                        {
                            VolumeLiquid = (BeautySimController.Instance.Simulator).CurrentVolume;
                            FluxLiquid = (BeautySimController.Instance.Simulator).Flux;
                        }
                        //        case (byte)Enum_SimulatorMessageType.FLUX:

                        //            wft.tbFlux.Text = BeautySim.Globalization.Language.str_flux + " " + (BeautySimController.Instance.Simulator).Flux.ToString("00.00");
                        //            wft.tbVolume.Text = BeautySim.Globalization.Language.str_volume + " " + (BeautySimController.Instance.Simulator).CurrentVolume.ToString("00.00");

                        //            break;
                        //if ((WindowTeacher.PageContainer.Content is WorkingFrameTeacher))
                        //{
                        //    WorkingFrameTeacher wft = (WorkingFrameTeacher)WindowTeacher.PageContainer.Content;
                        //    switch (type)
                        //    {
                        //        case (byte)Enum_SimulatorMessageType.FIRMWARE:
                        //            wft.tbFirmware.Text = BeautySim.Globalization.Language.str_firmware + " " + (BeautySimController.Instance.Simulator).FirmwareVersion;
                        //            break;

                        //        case (byte)Enum_SimulatorMessageType.SERIAL:
                        //            wft.tbSerial.Text = BeautySim.Globalization.Language.str_serial + " " + (BeautySimController.Instance.Simulator).SerialNumber;
                        //            break;

                        //        case (byte)Enum_SimulatorMessageType.FLUXGYRO:
                        //            wft.tbFlux.Text = BeautySim.Globalization.Language.str_flux + " " + (BeautySimController.Instance.Simulator).Flux.ToString("00.00");
                        //            wft.tbVolume.Text = BeautySim.Globalization.Language.str_volume + " " + (BeautySimController.Instance.Simulator).CurrentVolume.ToString("00.00");
                        //            wft.tbAcc.Text = BeautySim.Globalization.Language.str_x + " " + (BeautySimController.Instance.Simulator).AccX.ToString() + " " + BeautySim.Globalization.Language.str_y + " " + (BeautySimController.Instance.Simulator).AccY.ToString() + " " + BeautySim.Globalization.Language.str_z + " " + (BeautySimController.Instance.Simulator).AccZ.ToString();
                        //            wft.tbOrientation.Text = BeautySim.Globalization.Language.str_orientation + " " + (BeautySimController.Instance.Simulator).Orientation;

                        //            if ((BeautySimController.Instance.Simulator).Flux > 0.5f)
                        //            {
                        //                if ((DateTime.Now - LastTimeInjection).TotalMilliseconds > 1000)
                        //                {
                        //                    LastTimeInjection = DateTime.Now;
                        //                    InjectAnesthetic();
                        //                }
                        //            }
                        //            break;

                        //        default:
                        //            break;
                        //    }
                        //}
                    }

                        ));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void BeautySim_SerialNumberAcquired(object sender, EventArgs e)
        {
            //if (BTYSimulatorConnected)
            //{
            //    UpdateModules();
            //}
        }

        private void CaseTimeTickListener(object sender, EventArgs e)
        {
            caseTimer.Stop();
            if (CurrentCaseState == Enum_CaseState.LOADED)
            {
                switch (caseStepCurrent.Type)
                {
                    case Enum_ClinicalCaseStepType.MESSAGE:

                        #region MESSAGE

                        MainStudentFrame.bOk.IsEnabled = true;
                        MainStudentFrame.bAlarm.Visibility = Visibility.Hidden;

                        #endregion MESSAGE

                        break;

                    case Enum_ClinicalCaseStepType.QUESTIONNAIRE:
                        break;

                    default:
                        break;
                }
            }
            caseTimer.Start();
        }

        private void EvaluateAvailableSimulators()
        {
            try
            {
                BTYSimulatorConnected = BeautySimController.Instance.Simulator != null;

                SetIndicator(WindowTeacher.elBeautySim, BeautySimController.Instance.Simulator != null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void InitializePointsToBeShown()
        {
            //PIRINI 201231218
            for (int i = Vis3DFrame.hvView3D.Items.Count - 1; i >= 0; i--)
            {
                if (Vis3DFrame.hvView3D.Items[i].Tag != null)
                {
                    if (Vis3DFrame.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                    {
                        Vis3DFrame.hvView3D.Items.RemoveAt(i);
                    }
                }
            }

            for (int i = Vis3DFrameStudent.hvView3D.Items.Count - 1; i >= 0; i--)
            {
                if (Vis3DFrameStudent.hvView3D.Items[i].Tag != null)
                {
                    if (Vis3DFrameStudent.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                    {
                        Vis3DFrameStudent.hvView3D.Items.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < AppControl.Instance.InjectionPoints3DThisStep.Count; i++)
            {
                MeshGeometryModel3D aa = new MeshGeometryModel3D();
                aa.Tag = "ThisIsAPoint" + i.ToString();
                aa.CullMode = SharpDX.Direct3D11.CullMode.Back;

                var sphereA = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                sphereA.AddSphere(new SharpDX.Vector3(0, 0, 0), AppControl.Instance.Diameter3DPoints);
                HelixToolkit.Wpf.SharpDX.Geometry3D geometrySphere = sphereA.ToMeshGeometry3D();
                HelixToolkit.Wpf.SharpDX.Material MaterialSphere = DiffuseMaterials.LightBlue;
                aa.Geometry = geometrySphere;
                aa.Material = MaterialSphere;

                if (AppControl.Instance.InjectionPoints3DThisStep[i].Assigned)
                {
                    Transform3DGroup transform3DGroup = new Transform3DGroup();
                    transform3DGroup.Children.Add(new TranslateTransform3D(
                        AppControl.Instance.InjectionPoints3DThisStep[i].X,
                        AppControl.Instance.InjectionPoints3DThisStep[i].Y,
                        AppControl.Instance.InjectionPoints3DThisStep[i].Z));

                    transform3DGroup.Children.Add(new RotateTransform3D(AppControl.Instance.RotationManikin3D));
                    transform3DGroup.Children.Add(AppControl.Instance.TranslationManikin3D);

                    aa.Transform = transform3DGroup;
                }

                AppControl.Instance.PointsToBeShown.Add(aa);

                Vis3DFrame.hvView3D.Items.Add(aa);
            }

            for (int i = 0; i < AppControl.Instance.InjectionPoints3DThisStep.Count; i++)
            {
                MeshGeometryModel3D aa = new MeshGeometryModel3D();
                aa.Tag = "ThisIsAPoint" + i.ToString();
                aa.CullMode = SharpDX.Direct3D11.CullMode.Back;

                var sphereA = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                sphereA.AddSphere(new SharpDX.Vector3(0, 0, 0), AppControl.Instance.Diameter3DPoints);
                HelixToolkit.Wpf.SharpDX.Geometry3D geometrySphere = sphereA.ToMeshGeometry3D();
                HelixToolkit.Wpf.SharpDX.Material MaterialSphere = DiffuseMaterials.LightBlue;
                aa.Geometry = geometrySphere;
                aa.Material = MaterialSphere;

                if (AppControl.Instance.InjectionPoints3DThisStep[i].Assigned)
                {
                    Transform3DGroup transform3DGroup = new Transform3DGroup();
                    transform3DGroup.Children.Add(new TranslateTransform3D(
                        AppControl.Instance.InjectionPoints3DThisStep[i].X,
                        AppControl.Instance.InjectionPoints3DThisStep[i].Y,
                        AppControl.Instance.InjectionPoints3DThisStep[i].Z));
                    transform3DGroup.Children.Add(new RotateTransform3D(AppControl.Instance.RotationManikin3D));
                    transform3DGroup.Children.Add(AppControl.Instance.TranslationManikin3D);

                    aa.Transform = transform3DGroup;
                }

                AppControl.Instance.PointsToBeShown.Add(aa);
                aa.Visibility = Visibility.Hidden;
                Vis3DFrameStudent.hvView3D.Items.Add(aa);
            }

            Vis3DFrame.InjectionPointListView.ItemsSource = AppControl.Instance.InjectionPoints3DThisStep;
        }

        private void PDIClass_OnConnectionStatusChanged(Device.Motion.CONNECTIONSTATUS status)
        {
            SetIndicator(WindowTeacher.elSensors, status == Device.Motion.CONNECTIONSTATUS.ACQUIRING);
        }

        private void PDIClass_OnNewFrameAvailable(List<Device.Motion.FRAME> frames) // MODIFY EVERYTHING HERE
        {
            if (Calibrating)
            {
                try
                {
                    if (CalibrationWindow != null)
                    {
                        CalibrationWindow.Dispatcher.Invoke((Action)(() =>
                        {
                            if (!alreadyConnectedPolhemus)
                            {
                                alreadyConnectedPolhemus = true;
                                SetIndicator(CalibrationWindow.elSensors, alreadyConnectedPolhemus);
                            }

                            if (frames.Count > 0)
                            {
                                AppControl.Instance.F1 = frames[0];
                                AppControl.Instance.XPosSensor02 = AppControl.Instance.F1.Pos.X * 10;
                                AppControl.Instance.YPosSensor02 = AppControl.Instance.F1.Pos.Y * 10;
                                AppControl.Instance.ZPosSensor02 = AppControl.Instance.F1.Pos.Z * 10;
                                AppControl.Instance.RotationSensor_WRS_real = new System.Windows.Media.Media3D.Quaternion(AppControl.Instance.F1.Ori.X, AppControl.Instance.F1.Ori.Y, AppControl.Instance.F1.Ori.Z, AppControl.Instance.F1.Ori.W);

                                //AxisAngleRotation3D T1pre = new AxisAngleRotation3D(new Vector3D(1, 0, 0), AppControl.Instance.RotationAngleOffsetNeedle);
                                //RotateTransform3D T1 = new RotateTransform3D(T1pre);
                                //TranslateTransform3D T2 = new TranslateTransform3D(0, AppControl.Instance.AxisPositionOffsetNeedle, 0);
                                QuaternionRotation3D rotation = new QuaternionRotation3D(AppControl.Instance.RotationSensor_WRS_real);
                                RotateTransform3D T3 = new RotateTransform3D(rotation);
                                TranslateTransform3D T4 = new TranslateTransform3D(AppControl.Instance.XPosSensor02, AppControl.Instance.YPosSensor02, AppControl.Instance.ZPosSensor02);

                                Transform3DGroup transformGroup = new Transform3DGroup();
                                //transformGroup.Children.Add(T2);
                                //transformGroup.Children.Add(T1);
                                transformGroup.Children.Add(T3);
                                transformGroup.Children.Add(T4);

                                AppControl.Instance.TipNeedle = transformGroup.Transform(AppControl.Instance.TipNeedleOrigin);
                                AppControl.Instance.BaseNeedle = transformGroup.Transform(AppControl.Instance.BaseNeedleOrigin);

                                AppControl.Instance.TransformSphere2.OffsetX = AppControl.Instance.TipNeedle.X;
                                AppControl.Instance.TransformSphere2.OffsetY = AppControl.Instance.TipNeedle.Y;
                                AppControl.Instance.TransformSphere2.OffsetZ = AppControl.Instance.TipNeedle.Z;

                                OnPropertyChanged(nameof(AppControl.Instance.TransformSphere2));

                                Vector3D directionNeedle = AppControl.Instance.TipNeedle - AppControl.Instance.BaseNeedle;
                                directionNeedle.Normalize();

                                AppControl.Instance.PitchNeedle = -Math.Atan2(directionNeedle.X, Math.Sqrt(Math.Pow(directionNeedle.Z, 2) + Math.Pow(directionNeedle.Y, 2))) * 180.0 / Math.PI; ;
                                AppControl.Instance.YawNeedle = Math.Atan2(-directionNeedle.Y, -directionNeedle.X) * 180.0 / Math.PI;
                                var viewport = CalibrationWindow.hvView3D;

                                AppControl.Instance.ActualPointsHit = new List<PointHit>();
                                SharpDX.Ray ray = new SharpDX.Ray(new SharpDX.Vector3((float)AppControl.Instance.BaseNeedle.X, (float)AppControl.Instance.BaseNeedle.Y, (float)AppControl.Instance.BaseNeedle.Z), new SharpDX.Vector3((float)directionNeedle.X, (float)directionNeedle.Y, (float)directionNeedle.Z));

                                var hitContext = new HitTestContext(viewport.RenderHost.RenderContext, ref ray);
                                List<HitTestResult> hits = new List<HitTestResult>();

                                var rend = viewport.Renderables.ToList();

                                foreach (var element in rend)
                                {
                                    if (Calibrating)
                                    {
                                        var t = element.GetType().ToString();
                                        AppControl.Instance.CheckSceneNode(element, hitContext, ray.Direction, new SharpDX.Vector3((float)AppControl.Instance.TipNeedle.X, (float)AppControl.Instance.TipNeedle.Y, (float)AppControl.Instance.TipNeedle.Z), ref AppControl.Instance.ActualPointsHit);//, ref hitNames);
                                    }
                                }

                                double minDistance = double.MaxValue;
                                int indexMin = -1;
                                if (AppControl.Instance.ActualPointsHit.Count > 0)
                                {
                                    for (int i = 0; i < AppControl.Instance.ActualPointsHit.Count; i++)
                                    {
                                        var distance = AppControl.Instance.CalcDistance(AppControl.Instance.ActualPointsHit[i].Point, AppControl.Instance.TipNeedle);
                                        if ((distance >= 0) && (distance < minDistance))
                                        {
                                            minDistance = distance;
                                            indexMin = i;
                                        }
                                    }

                                    AppControl.Instance.TransformSphere.OffsetX = AppControl.Instance.ActualPointsHit[indexMin].Point.X;
                                    AppControl.Instance.TransformSphere.OffsetY = AppControl.Instance.ActualPointsHit[indexMin].Point.Y;
                                    AppControl.Instance.TransformSphere.OffsetZ = AppControl.Instance.ActualPointsHit[indexMin].Point.Z;

                                    OnPropertyChanged(nameof(AppControl.Instance.TransformSphere));
                                }
                                else
                                {
                                }
                            }
                        }));
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    WindowTeacher.Dispatcher.Invoke((Action)(() =>
                    {
                        if (!alreadyConnectedPolhemus)
                        {
                            alreadyConnectedPolhemus = true;
                            SetIndicator(WindowTeacher.elSensors, alreadyConnectedPolhemus);
                        }

                        if (frames.Count > 0)
                        {
                            F1 = frames[0];
                            XPosSensor02 = F1.Pos.X * 10;
                            YPosSensor02 = F1.Pos.Y * 10;
                            ZPosSensor02 = F1.Pos.Z * 10;
                            RotationSensor_WRS_real = new System.Windows.Media.Media3D.Quaternion(F1.Ori.X, F1.Ori.Y, F1.Ori.Z, F1.Ori.W);

                            //AxisAngleRotation3D T1pre = new AxisAngleRotation3D(new Vector3D(1, 0, 0), AppControl.Instance.RotationAngleOffsetNeedle);
                            //RotateTransform3D T1 = new RotateTransform3D(T1pre);
                            //TranslateTransform3D T2 = new TranslateTransform3D(0, AxisPositionOffsetNeedle, 0);
                            QuaternionRotation3D rotation = new QuaternionRotation3D(RotationSensor_WRS_real);
                            RotateTransform3D T3 = new RotateTransform3D(rotation);
                            TranslateTransform3D T4 = new TranslateTransform3D(XPosSensor02, YPosSensor02, ZPosSensor02);

                            Transform3DGroup transformGroup = new Transform3DGroup();
                            //transformGroup.Children.Add(T1);
                            //transformGroup.Children.Add(T2);
                            transformGroup.Children.Add(T3);
                            transformGroup.Children.Add(T4);

                            TipNeedle = transformGroup.Transform(TipNeedleOrigin);
                            BaseNeedle = transformGroup.Transform(BaseNeedleOrigin);

                            TransformSphere2.OffsetX = TipNeedle.X;
                            TransformSphere2.OffsetY = TipNeedle.Y;
                            TransformSphere2.OffsetZ = TipNeedle.Z;

                            OnPropertyChanged(nameof(TransformSphere2));

                            Vector3D directionNeedle = TipNeedle - BaseNeedle;

                            PitchNeedle = Math.Atan2(directionNeedle.X, Math.Sqrt(Math.Pow(directionNeedle.Z, 2) + Math.Pow(directionNeedle.Y, 2))) * 180.0 / Math.PI; ;
                            YawNeedle = Math.Atan2(-directionNeedle.Z, directionNeedle.X) * 180.0 / Math.PI;

                            TextToShowYawPitch = "Elevation: " + PitchNeedle.ToString("00.0") + " Azimuth: " + YawNeedle.ToString("00.0");

                            if ((TeacherFrame is Visualization3DFrame) && (Model3DInitialized))
                            {
                                var viewport = AppControl.Instance.Vis3DFrame.hvView3D;

                                ActualPointsHit = new List<PointHit>();
                                SharpDX.Ray ray = new SharpDX.Ray(new SharpDX.Vector3((float)BaseNeedle.X, (float)BaseNeedle.Y, (float)BaseNeedle.Z), new SharpDX.Vector3((float)directionNeedle.X, (float)directionNeedle.Y, (float)directionNeedle.Z));

                                var hitContext = new HitTestContext(viewport.RenderHost.RenderContext, ref ray);
                                List<HitTestResult> hits = new List<HitTestResult>();

                                var rend = viewport.Renderables.ToList();

                                foreach (var element in rend)
                                {
                                    var t = element.GetType().ToString();
                                    CheckSceneNode(element, hitContext, ray.Direction, new SharpDX.Vector3((float)TipNeedle.X, (float)TipNeedle.Y, (float)TipNeedle.Z), ref ActualPointsHit);//, ref hitNames);
                                }

                                double minDistance = double.MaxValue;
                                int indexMin = -1;
                                if (ActualPointsHit.Count > 0)
                                {
                                    for (int i = 0; i < ActualPointsHit.Count; i++)
                                    {
                                        var distance = CalcDistance(ActualPointsHit[i].Point, TipNeedle);
                                        if ((distance >= 0) && (distance < minDistance))
                                        {
                                            minDistance = distance;
                                            indexMin = i;
                                        }
                                    }

                                    TransformSphere.OffsetX = ActualPointsHit[indexMin].Point.X;
                                    TransformSphere.OffsetY = ActualPointsHit[indexMin].Point.Y;
                                    TransformSphere.OffsetZ = ActualPointsHit[indexMin].Point.Z;

                                    OnPropertyChanged(nameof(TransformSphere));
                                    HitStructure = "next: " + ActualPointsHit[indexMin].Name + " " + ActualPointsHit.Count;
                                    //Debug.WriteLine(HitStructure);
                                }
                                else
                                {
                                    HitStructure = "next: none";
                                }

                                //ELABORATE TO UNDERSTAND
                                for (int i = 0; i < ActualPointsHit.Count; i++)
                                {
                                    PointHit pointHit = ActualPointsHit[i];

                                    bool furtherInvestigateForEntrance = false;
                                    bool furtherInvestigateForExiting = false;
                                    bool furtherInvestigateForUpdate = false;
                                    bool createEntrance = false;
                                    bool closeEntrance = false;
                                    PointHit previousHit = (from db in previousPointsHit where (db.Name == pointHit.Name) select db).LastOrDefault();

                                    //INVESTIGATE FOR ENTRANCE
                                    if (pointHit.Distance < 0) //entered
                                    {
                                        if (previousHit != null)
                                        {
                                            if (previousHit.Distance >= 0) //previously not entered
                                            {
                                                furtherInvestigateForEntrance = true;
                                            }
                                        }

                                        if (furtherInvestigateForEntrance)
                                        {
                                            if (pointHit.Name == "SKIN")
                                            {
                                                bool foundOtherSkinAreas = false;
                                                for (int j = 0; j < ActualPointsHit.Count; j++)
                                                {
                                                    if (j != i)
                                                    {
                                                        if (StringSkinAreas.Contains(ActualPointsHit[j].Name))
                                                        {
                                                            foundOtherSkinAreas = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (!foundOtherSkinAreas)
                                                {
                                                    createEntrance = true;
                                                }
                                            }
                                            else
                                            {
                                                createEntrance = true;
                                            }
                                        }
                                        else
                                        {
                                            //If inside, update depth if needed on the corresponding EntranceData
                                            for (int k = EntranceDataList.Count - 1; k >= 0; k--)
                                            {
                                                if (EntranceDataList[k].StructureEntered == pointHit.Name && (EntranceDataList[k].ToBeClosed))
                                                {
                                                    //Update depth
                                                    double absDepth = Math.Abs(pointHit.Distance);
                                                    if (absDepth > EntranceDataList[k].Depth)
                                                    {
                                                        EntranceDataList[k].Depth = absDepth;
                                                    }
                                                    DepthInjection = absDepth;

                                                    //Update injected volume

                                                    if (SelectedPoint3DIndex != -1)
                                                    {
                                                        if (InjectionPoints3DThisStep[SelectedPoint3DIndex].ActualReleases.Count > 0)
                                                        {
                                                            InjectionPoints3DThisStep[SelectedPoint3DIndex].ActualReleases.Last().InjectedQuantity = BeautySimController.Instance.Simulator.CurrentVolume>=0? (BeautySimController.Instance.Simulator.CurrentVolume * Properties.Settings.Default.MultiplierMlToU):0;

                                                            InjectionPoints3DThisStep[SelectedPoint3DIndex].RefreshQuantities();
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                        if (createEntrance)
                                        {
                                            //Create new EntranceData
                                            EntranceData entData = new EntranceData();
                                            entData.EntranceTime = DateTime.Now;
                                            entData.StructureEntered = pointHit.Name;
                                            entData.CollisionPoint = pointHit.Point;
                                            entData.ToBeClosed = true;
                                            entData.Index = EntranceDataList.Count;
                                            entData.EntryingPointPitch = PitchNeedle;
                                            entData.EntryingPointYaw = YawNeedle;
                                            EntranceDataList.Add(entData);
                                            PointEntrance = new Point3D(EntranceDataList.Last().CollisionPoint.X, EntranceDataList.Last().CollisionPoint.Y, EntranceDataList.Last().CollisionPoint.Z);
                                            ZeroVolume();
                                            AppControl.Instance.Vis3DFrame.entranceListView.ScrollIntoView(entData);

                                            Debug.WriteLine(entData.ToReadableString());

                                            ImpactPointToShow = pointHit.Point;

                                            
                                            //      Initialize variables to track the nearest and second nearest points
                                            double minimumDistancePoint = Double.MaxValue; // Smallest distance found
                                            int minimumDistancePointIndex = -1; // Index of the nearest point
                                            
                                           

                                            for (int ww = 0; ww < InjectionPoints3DThisStep.Count; ww++)
                                            {
                                                Point3D toCheck = InjectionPoints3DThisStep[ww].GetRotoTransatedPoint(TransformGroupMankin);
                                                double distance = CalcDistance(toCheck, PointEntrance);

                                                if (distance < minimumDistancePoint)
                                                {
                                                    // Update second nearest with the previous nearest


                                                    // Update nearest with the new nearest
                                                    minimumDistancePoint = distance;
                                                    minimumDistancePointIndex = ww;
                                                }
                                            }

                                            //  Check if the nearest point is close enough to the second nearest point to be considered a good match and if so, select it
                                            if (minimumDistancePoint < (InjectionPoints3DThisStep[minimumDistancePointIndex].MinDistanceFromNeighbours*0.8))
                                            {
                                                //Point3D toCheck = new Point3D(toEval.X + TranslationPointModel.X, toEval.Y + TranslationPointModel.Y, toEval.Z + TranslationPointModel.Z);
                                                SelectedPoint3DIndex = minimumDistancePointIndex;
                                                AppControl.instance.Vis3DFrame.InjectionPointListView.ScrollIntoView(InjectionPoints3DThisStep[minimumDistancePointIndex]);
                                                AppControl.instance.Vis3DFrame.InjectionPointListView.SelectedIndex = minimumDistancePointIndex;

                                                ActualRelease actualRelease = new ActualRelease();
                                                actualRelease.PitchEntrance = EntranceDataList.Last().EntryingPointPitch;
                                                actualRelease.YawEntrance = EntranceDataList.Last().EntryingPointYaw;

                                                actualRelease.ActualX = EntranceDataList.Last().CollisionPoint.X;
                                                actualRelease.ActualY = EntranceDataList.Last().CollisionPoint.Y;
                                                actualRelease.ActualZ = EntranceDataList.Last().CollisionPoint.Z;

                                                InjectionPoints3DThisStep[minimumDistancePointIndex].ActualReleases.Add(actualRelease);
                                                break;
                                            }
                                        }
                                    }
                                    if (pointHit.Distance > 0) //exited
                                    {
                                        if (previousHit != null)
                                        {
                                            if (previousHit.Distance < 0) //previous was inside
                                            {
                                                furtherInvestigateForExiting = true;
                                            }
                                        }

                                        if (furtherInvestigateForExiting)
                                        {
                                            if (pointHit.Name == "SKIN")
                                            {
                                                bool foundOtherSkinAreas = false;
                                                for (int j = 0; j < ActualPointsHit.Count; j++)
                                                {
                                                    if (j != i)
                                                    {
                                                        if (StringSkinAreas.Contains(ActualPointsHit[j].Name))
                                                        {
                                                            foundOtherSkinAreas = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (!foundOtherSkinAreas)
                                                {
                                                    closeEntrance = true;
                                                }
                                            }
                                            else
                                            {
                                                closeEntrance = true;
                                            }
                                        }
                                        if (closeEntrance)
                                        {
                                            for (int k = EntranceDataList.Count - 1; k >= 0; k--)

                                            {
                                                if (EntranceDataList[k].StructureEntered == pointHit.Name && (EntranceDataList[k].ToBeClosed))
                                                {
                                                    EntranceDataList[k].ToBeClosed = false;
                                                    EntranceDataList[k].ExitingTime = DateTime.Now;

                                                    if (SelectedPoint3DIndex != -1)
                                                    {
                                                        AppControl.instance.Vis3DFrame.InjectionPointListView.SelectedIndex = -1;
                                                        SelectedPoint3DIndex = -1;
                                                        ImpactPointToShow = new Point3D(0, 0, 0);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        DepthInjection = 0;
                                        ZeroVolume();
                                    }
                                }

                                if (EntranceDataList.Count > 0)
                                {
                                    if (EntranceDataList.Last().ToBeClosed)
                                    {
                                        PointEntrance = new Point3D(EntranceDataList.Last().CollisionPoint.X, EntranceDataList.Last().CollisionPoint.Y, EntranceDataList.Last().CollisionPoint.Z);
                                    }
                                }

                                //UPDATE THE PREVIOUS POINTS HITS
                                previousPointsHit.Clear();
                                for (int nn = 0; nn < ActualPointsHit.Count; nn++)
                                {
                                    previousPointsHit.Add(ActualPointsHit[nn].DeepCopy());
                                }
                            }
                        }
                    }));
                }
                catch (Exception)
                {
                }
            }
        }

        private void PopulateCases(bool checkLicense)
        {
            PopulateAvailableCases();

            for (int i = Cases.Count - 1; i >= 0; i--)
            {
                if (checkLicense)
                {
                    if (!ModulesActivation[(int)Cases[i].Module])
                    {
                        Cases.Remove(Cases[i]);
                    }
                }
            }

            Cases.OrderBy(p => p.Module).ThenBy(p => p.Name);
        }

        private void PopulateModulesNames()
        {
            foreach (string s in Modules)
            {
                switch (s)
                {
                    case "Botox":
                        ModulesNames.Add("Botox");
                        break;

                    case "Filler":
                        ModulesNames.Add("Filler");
                        break;

                    default:
                        break;
                }
            }
        }

        private void PrepareAdvanceStudent(string v)
        {
            switch (v)
            {
                case "Hide":
                    MainStudentFrame.bOk.Visibility = Visibility.Hidden;
                    break;

                case "Start":
                    MainStudentFrame.tbStudentOk.Text = "start";
                    MainStudentFrame.iconStudentOk.Kind = MaterialDesignThemes.Wpf.PackIconKind.PlayCircle;
                    break;

                case "Validate":
                    MainStudentFrame.tbStudentOk.Text = "validate";
                    MainStudentFrame.iconStudentOk.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                    break;

                case "Next":
                    MainStudentFrame.tbStudentOk.Text = "next";
                    MainStudentFrame.iconStudentOk.Kind = MaterialDesignThemes.Wpf.PackIconKind.StepForward;
                    break;

                case "Stop":
                    MainStudentFrame.tbStudentOk.Text = "stop";
                    MainStudentFrame.iconStudentOk.Kind = MaterialDesignThemes.Wpf.PackIconKind.StopCircle;
                    break;

                case "Balloon":
                    MainStudentFrame.tbStudentOk.Text = "inflate";
                    MainStudentFrame.iconStudentOk.Kind = MaterialDesignThemes.Wpf.PackIconKind.Airballoon;
                    break;

                default:
                    break;
            }

            if (v != "Hide")
            {
                MainStudentFrame.bOk.Visibility = Visibility.Visible;
            }
        }

        private void ScannedSimulatorListener()
        {
            try
            {
                EvaluateAvailableSimulators();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(ex.StackTrace + ex.Message);
            }
        }

        private void TimerCheckPolhemusTick_Listener(object sender, EventArgs e)
        {
            if ((DateTime.Now - lastArrivedInput).TotalSeconds > 0.5)
            {
                alreadyConnectedPolhemus = false;
                SetIndicator(WindowTeacher.elSensors, alreadyConnectedPolhemus);
            }
        }

        private void UpdateRotationManikin3D()
        {
            RotationManikin3D = new QuaternionRotation3D(Rotation_Manikin);
        }

        internal void ZeroVolume()
        {
            BeautySimController.Instance.Simulator.ZeroVolume();
        }

        internal void ManageViewPointsOn3D(bool destination)
        {
            AppControl.Instance.ArePointsVisualizedOn3D = destination;

            if ((AppControl.Instance.WindowTeacher.PageContainer.Content is CaseTeacherFrame))
            {
                ((CaseTeacherFrame)(AppControl.Instance.WindowTeacher.PageContainer.Content)).tbViewPoints.Text = AppControl.Instance.ArePointsVisualizedOn3D ? BeautySim.Globalization.Language.hide_points : BeautySim.Globalization.Language.view_points;
            }

            ManageVisibilityPointsStudent(AppControl.Instance.ArePointsVisualizedOn3D);
        }

        internal void ManageVisibilityPointsStudent(bool visibleOrHidden)
        {
            for (int i = 0; i < Vis3DFrameStudent.hvView3D.Items.Count; i++)
            {
                MeshGeometryModel3D aa = new MeshGeometryModel3D();
                if (Vis3DFrameStudent.hvView3D.Items[i].Tag != null)
                {
                    if (Vis3DFrameStudent.hvView3D.Items[i].Tag.ToString().StartsWith("ThisIsAPoint"))
                    {
                        aa = (MeshGeometryModel3D)Vis3DFrameStudent.hvView3D.Items[i];
                        if (visibleOrHidden)
                        {
                            aa.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            aa.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }
    }
}