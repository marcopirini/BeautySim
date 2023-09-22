using BeautySim2023.DataModel;
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
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using VectorMath;
using HelixToolkitException = HelixToolkit.Wpf.SharpDX.HelixToolkitException;
using HitTestResult = HelixToolkit.Wpf.SharpDX.HitTestResult;

namespace BeautySim2023
{
    public class AppControl : IDisposable, INotifyPropertyChanged
    {
        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryAntenna {  set; get; }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryCable { get;  set; }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryConnector { get;  set; }
        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryHeadSkin {  set; get; }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryNeedle { get;  set; }

        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometrySensor { get;  set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere { get;  set; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometrySphere2 {  set; get; }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D GeometryTConnector { get;  set; }


        public HelixToolkit.Wpf.SharpDX.Material MaterialAntenna {  set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialCable { get;  set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialConnector { get;  set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialHeadSkin {  set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialNeedle { get;  set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialProcerus {  set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSensor { get;  set; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere {  set; get; }
        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere2 {  set; get; }
        public LineGeometry3D Coordinate { set; get; }
        public BillboardText3D CoordinateText { set; get; }
        public HelixToolkit.Wpf.SharpDX.Material ViewCubeMaterial2 { set; get; }

        public System.Windows.Media.Media3D.Transform3D ViewCubeTransform3 { set; get; }
        public TranslateTransform3D TransformSphere {  set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(5, 0, 0);

        public TranslateTransform3D TransformSphere2 {  set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(5, 0, 0);

        public TranslateTransform3D TrasformAntenna {  set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);

        public static System.Windows.Media.Media3D.Quaternion CreateQuaternionFromAxisAngle(float angleInDegrees, float axisX, float axisY, float axisZ)
        {
            float angleInRadians = (float)(Math.PI * angleInDegrees / 180.0);
            float sinHalfAngle = (float)Math.Sin(angleInRadians / 2.0);
            float cosHalfAngle = (float)Math.Cos(angleInRadians / 2.0);

            return new System.Windows.Media.Media3D.Quaternion(sinHalfAngle * axisX, sinHalfAngle * axisY, sinHalfAngle * axisZ, cosHalfAngle);
        }

        private float axisPositionOffsetNeedle = 20;
        private Point3D baseNeedle;
        public Point3D BaseNeedleOrigin;

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

        private string hitStructure;

        public bool mouseClickOrMove = false;

        public DateTime lastMouseMove;

        private System.Windows.Media.Media3D.Quaternion rotationSensor_WRS;
        public int rotationTimer;
        private DispatcherTimer timerCheckPolhemus;
        private DispatcherTimer timerRotation;
        private Point3D tipNeedle;
        public Point3D TipNeedleOrigin;
        private Vector3D upDirection;
        private float xPosSensor02 = 50;
        private float yPosSensor02;
        private float zPosSensor02;

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

        public double CalcDistance(Point3D point1, Point3D point2)
        {
            double deltaX = point1.X - point2.X;
            double deltaY = point1.Y - point2.Y;
            double deltaZ = point1.Z - point2.Z;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        //private PhongMaterial CreatePhongMaterial(SharpDX.Color4 color)
        //{
        //    return new PhongMaterial
        //    {
        //        DiffuseColor = color,
        //        SpecularColor = new SharpDX.Color4(1.0f, 1.0f, 1.0f, 1.0f),
        //        SpecularShininess = 32f
        //    };
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

        public Vector3D UpDirection
        {
            get { return upDirection; }
            set
            {
                upDirection = value;
                OnPropertyChanged(nameof(UpDirection));
            }
        }

        private float rotationAngleOffsetNeedle = 20;
        private System.Windows.Media.Media3D.Quaternion rotation_Manikin;

        private bool procerusVisible = true;

        public bool ProcerusVisible
        {
            get { return procerusVisible; }
            set
            {
                procerusVisible = value;
                OnPropertyChanged(nameof(ProcerusVisible));
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

        public float RotationAngleOffsetNeedle
        {
            get { return rotationAngleOffsetNeedle; }
            set
            {
                rotationAngleOffsetNeedle = value;
                OnPropertyChanged(nameof(RotationAngleOffsetNeedle));
            }
        }

        public const string ADMIN_USERNAME = "Accurate.Admin";
        public const string BeautySim_USERNAME = "Accurate.BeautySim";
        public const string CadsFolder = "C:\\BeautySim\\Models3D\\";

        public const string CasesFolder = "C:\\BeautySim\\Cases\\";

        public const string Orthographic = "Orthographic Camera";

        public const string Perspective = "Perspective Camera";
        public Brush ActiveEllipse = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 150, 136));

        public List<ClinicalCase> AvailableCases;
        public float BlockX;
        public float BlockY;
        public ObservableCollection<ClinicalCase> Cases;

        public bool CheckCollisions = false;
        public ClinicalCase CurrentCase = null;
        public CurrentCaseStateChangedDelegate CurrentCaseStateChangedEvent;
        public Events CurrentEvent = null;
        public string CurrentModule = null;
        public ClinicalCaseStep CurrentStep;
        public Users CurrentStudent = null;
        public Users CurrentTeacher = null;
        public VectorMath.Vector3 D_ANTICOLLISION = new VectorMath.Vector3(140f, 0, 0);
        public VectorMath.Vector3 D_ANTICOLLISION_NEEDLE = new VectorMath.Vector3(-140f, 0, 0);
        public VectorMath.Vector3 D_COLLISION = new VectorMath.Vector3(-140f, 0, 0);
        public VectorMath.Vector3 D_COLLISION_NEEDLE = new VectorMath.Vector3(-100f, 0, 0);
        public VectorMath.Vector3 D_NEdge_CollisionEdge = new VectorMath.Vector3(13f, 0, 0);
        public bool DemoMode = true;
        public float Height = 100;
        public int intervalMsTimer = 30;
        public string LastSerial;
        public double LimitAngleDeg = 10;
        public ObservableCollection<string> Modules;

        public ObservableCollection<string> ModulesNames;
        public SharpDX.Vector3 NedleAreaEnd = new SharpDX.Vector3(0, 0, 0);
        public List<VectorMath.Vector3> needlePoints_NRS;
        public List<VectorMath.Vector3> needlePoints_WRS;
        public int numPointsNeedle;
        public int NumPointsX = 100;
        public int NumPointsY = 100;
        public Users OldTeacher = null;
        public VectorMath.Vector3 originalPositionSkinBlockCenter_WRS;
        public float PhysicalLengthNeedle = 131;

        public bool StopLoadCommand = false;
        public string StringCurrentCase = BeautySim.Globalization.Language.str_curr_case;

        public string StringLoadCase = BeautySim.Globalization.Language.str_sel_load_case;

        public string StringNeedleInjCorrectTarget = BeautySim.Globalization.Language.str_great;

        public string StringNeedleInjTakeAim = BeautySim.Globalization.Language.str_go_ahead;

        public string StringNeedleInjWrongTarget = BeautySim.Globalization.Language.str_oh_no;

        public Point3D TraslationFromSensor02ToNeedleStart = new Point3D(10, 0, 0);

        public float Width = 100;
        private static AppControl instance = null;

        private float additionalCorrectionLengthNeedle;
        private bool alreadyConnectedPolhemus;

        private HelixToolkit.Wpf.SharpDX.Camera camera;

        private string cameraModel;

        private ClinicalCaseStep caseStepCurrent;
        private System.Windows.Forms.Timer caseTimer;
        private Point3D collidePointNeedle;
        private int counter;
        private int counterCollider;
        private int counterProgressCalibration;
        private decimal CurrentCaseScore = 0;
        private Enum_CaseState currentCaseState;
        private HelixToolkit.Wpf.SharpDX.OrthographicCamera defaultOrthographicCamera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 1, FarPlaneDistance = 100 };

        private HelixToolkit.Wpf.SharpDX.PerspectiveCamera defaultPerspectiveCamera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 0.5, FarPlaneDistance = 150 };

        private float deltaViewFar = 1500;

        private float deltaViewNear = 600;

        private double depthImage;

        private bool disposedValue = false;

        private double distanceNeedleHits;
        private List<double> distancesNeedleCollision = new List<double>();
        private FRAME f1;

        private FRAME f2;

        private FunctionalitiesFrame frameControl;
        private int fsamp = 20;
        private float hOrigin;

        private float hScaled;

        private DateTime initialDataTime;
        private int insertionNeedle;

        private string insertionNeedleText;

        private DateTime lastArrivedInput;

        private float lengthNeedle;

        private bool loadedCase;

        private CaseStudentFrame MainStudentFrame;
        private CaseTeacherFrame MainTeacherFrame;
        private float minValueAlpha = 0.35f;

        private HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelMask;
        private HelixToolkit.Wpf.SharpDX.DiffuseMaterial modelMaterialMask;
        private string nameFileDefinitionSave;

        private bool needleAlignedCorrectly;

        private bool needleAngleOk;

        //private LineGeometry3D needleArea;

        //private LineGeometry3D needleAreaIndication;

        private System.Windows.Media.Color needleColor;

        private Point3D needleEdgeCollider;
        private System.Windows.Media.Color needleIndicationColor;

        private bool needleInSkin;

        private VectorMath.Vector3 needlePointFinish_WRS_real;

        private VectorMath.Vector3 needlePointFinish_WRS_realHalf;

        private VectorMath.Vector3 needlePointFinish_WRS_toBeUsed;

        private VectorMath.Vector3 needlePointStart_WRS_real;

        private VectorMath.Vector3 needlePointStart_WRS_toBeUsed;

        private System.Windows.Media.Color needleTipIndicationColor;

        private YPR needleYPR_WRS_real;
        private List<SharpDX.Vector3> originalEffectPOints;

        private Point3D originPlaneImage;

        private PDIClass pDIClass;

        private double pitchSensor02;

        private List<OrderedPoint> PointCollideNeedle3Ds = new List<OrderedPoint>();
        private VectorMath.Vector3 pointEnd_WRS;

        private List<SharpDX.Vector3> pointsCircle = new List<SharpDX.Vector3>();
        private SharpDX.Vector3[] pointsDX;

        private SharpDX.Vector3[] pointsDXMask;

        private VectorMath.Vector3 pointStart_WRS;

        private VectorMath.Vector3 positionSensor02_WRS_real;
        private VectorMath.Vector3 positionSensor02_WRS_toBeUsed;
        private double rollSensor02;
        private VectorMath.Quaternion rotationFromSensor02ToNeedle = VectorMath.Quaternion.RotationAxis(new VectorMath.Vector3(1, 0, 0), 100f / 180f * (float)Math.PI);
        private double[,] rotationMatrixTrasposed_Needle_WRS_real;
        private double[,] rotationMatrixTrasposed_Needle_WRS_toBeUsed;
        private double[,] rotationMatrixTrasposed_Needle_WRS_WhenInsertion;
        private System.Windows.Media.Media3D.Quaternion rotationNeedle_WRS_m;
        private VectorMath.Quaternion rotationNeedle_WRS_real;
        private System.Windows.Media.Media3D.Quaternion rotationOperableEnsamble;
        private VectorMath.Quaternion rotationSensor02_WRS_real;
        private YPR sensor02ypr_WRS_real;
        private bool showIndicationNeedle;
        private bool showTargetPosition;
        private Page StudentFrame;
        private string subTitle;

        private Color targetSpecialNerveColor;
        public Visualization3DFrame Vis3DFrame { get; private set; }
        public Visualization3DFrameStudent Vis3DFrameStudent { get; private set; }
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
        private string textVascularWrong;
        private string textVascularWrongExp;
        private string textVascularWrongIntro;
        private string textWrongInjections;
        private string textWrongInjectionsExp;

        private int timeStepSimulation = 50;
        private string title;
        private bool toContinueProgressCalibration;
        private bool toPlot;
        private bool toUpdateControls;
        private Vector3 vecCollisionNeedle_WRS;
        private VectorMath.Vector3 versorNeedle_WhenInsertion;

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

        private float xTipNeedle = 0;

        private double yawSensor02;

        private float yPosOperableEnsamble;

        private float yTipNeedle = 0;

        private float zPosOperableEnsamble;

        private float zTipNeedle = 0;
        private IEffectsManager effectsManager;
        private string textToShowYawPitch;

        private AppControl()
        {
            Cases = new ObservableCollection<ClinicalCase>();

            nameFileDefinitionSave = "DefinitionFramesEffect.xml";
            originalEffectPOints = new List<SharpDX.Vector3>();

            originalPositionSkinBlockCenter_WRS = new VectorMath.Vector3(117.5f, 2, -30.5f);

            PhysicalLengthNeedle = Properties.Settings.Default.DistanceSensorNeedleTip;
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
            initialDataTime = DateTime.Now;
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

        public TimeSpan DurationNeedlePhase { get; private set; }

        public int IndexRealFinishNeedle { get; internal set; }

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

        public double MinutesNeedlePhase { get; private set; }

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

        //public LineGeometry3D NeedleArea
        //{
        //    get
        //    {
        //        return needleArea;
        //    }
        //    set
        //    {
        //        SetValue(ref needleArea, value, "NeedleArea");
        //    }
        //}

        //public LineGeometry3D NeedleAreaIndication
        //{
        //    get
        //    {
        //        return needleAreaIndication;
        //    }
        //    set
        //    {
        //        SetValue(ref needleAreaIndication, value, "NeedleAreaIndication");
        //    }
        //}

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

        public double PitchSensor02
        {
            get { return pitchSensor02; }
            set
            {
                pitchSensor02 = value;
                RaisePropertyChanged("PitchSensor02");
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

                            c3.Step = Enum_StepQuestionnaire.ANSWERING;
                            break;

                        case Enum_StepQuestionnaire.ANSWERING:
                            c3.NumErrors = 0;

                            for (int i = 0; i < s3.spOperativity.Children.Count; i++)
                            {
                                MultipleChoiceControl mpc = ((MultipleChoiceControl)(s3.spOperativity.Children[i]));
                                mpc.IsEnabled = false;
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

                            //make things happen on score and inidcations

                            // make things happen with explanations
                            c3.Step = Enum_StepQuestionnaire.FEEDBACK;

                            break;
                        //s3.Step = Enum_QuestionnaireMultipleFrameStep.FEEDBACK;

                        //PrepareAdvanceStudent("Next");
                        //s3.listView.Background = Brushes.Transparent;
                        //c3.NumErrors = 0;
                        //if (c3.MultipleSelectionAllowed)
                        //{
                        //    for (int i = 0; i < s3.items.Count; i++)
                        //    {
                        //        if (s3.items[i].IHaveToBeSelected != s3.items[i].IHaveBeenSelected)
                        //        {
                        //            c3.NumErrors++;
                        //        }
                        //        //s3.items[i].SituationSelection = (s3.items[i].IHaveToBeSelected != s3.items[i].IHaveBeenSelected) ? 0 : 1;
                        //        s3.items[i].SituationSelection = (s3.items[i].IHaveToBeSelected) ? 1 : -1;
                        //    }
                        //}
                        //else
                        //{
                        //    for (int i = 0; i < s3.items.Count; i++)
                        //    {
                        //        if (s3.items[i].IHaveToBeSelected)
                        //        {
                        //            s3.items[i].SituationSelection = 1;
                        //            if (!s3.items[i].IHaveBeenSelected)
                        //            {
                        //                c3.NumErrors++;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (s3.items[i].IHaveBeenSelected)
                        //            {
                        //                s3.items[i].SituationSelection = -1;
                        //            }
                        //        }
                        //    }
                        //}
                        ////if (c3.ShowFeedback)
                        ////{
                        ////    if (c3.MultipleSelectionAllowed)
                        ////    {
                        ////        s3.tbMessage.Text = c3.NumErrors == 0 ? "Correct! - You made no errors" : "You made " + c3.NumErrors.ToString() + (c3.NumErrors == 1 ? " error" : " errors.");
                        ////    }
                        ////    else
                        ////    {
                        ////        s3.tbMessage.Text = c3.NumErrors == 0 ? "Correct!" : "Wrong Answer";
                        ////    }
                        ////    s3.bMessage.BorderBrush = c3.NumErrors == 0 ? AppControl.Instance.GiveMeColorOk() : AppControl.Instance.GiveMeColorWrong();

                        ////    //s3.listView.IsEnabled = false;
                        ////    s3.listView.Background = Brushes.Transparent;
                        ////}
                        ////else
                        ////{
                        ////    s3.tbMessage.Text = "Let's verify your hypothesis.";
                        ////    s3.spOperativity.Visibility = Visibility.Collapsed;
                        ////    s3.grMain.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                        ////    s3.listView.Background = Brushes.Transparent;
                        ////}
                        //foreach (QuestionnaireItemSelection item in s3.listView.Items)
                        //{
                        //    item.ImEnabled = false;
                        //}

                        ////if (c3.AssociatedContent)
                        ////{
                        ////    s3.gridAnswers.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        ////    t3.gridAnswers.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        ////}

                        //List<QuestionnaireItemSelection> showToTeacher = new List<QuestionnaireItemSelection>();
                        //foreach (QuestionnaireItemSelection item in s3.items)
                        //{
                        //    showToTeacher.Add(new QuestionnaireItemSelection(item.ItemAssociatedText, item.IHaveToBeSelected, item.IHaveBeenSelected, item.SituationSelection));
                        //}
                        //t3.grMain.RowDefinitions[1].Height = new GridLength(4, GridUnitType.Star);
                        //t3.listView.Visibility = Visibility.Visible;
                        //t3.listView.ItemsSource = showToTeacher;
                        //foreach (QuestionnaireItemSelection item in t3.listView.Items)
                        //{
                        //    item.ImEnabled = false;
                        //}
                        //if (c3.MultipleSelectionAllowed)
                        //{
                        //    t3.tbMessage.Text = c3.NumErrors == 0 ? "Correct! - The student made no errors" : "The student made " + c3.NumErrors.ToString() + (c3.NumErrors == 1 ? " error." : " errors.");
                        //}
                        //else
                        //{
                        //    t3.tbMessage.Text = c3.NumErrors == 0 ? "Correct!" : "Wrong Answer";
                        //}
                        //t3.bMessage.BorderBrush = c3.NumErrors == 0 ? AppControl.Instance.GiveMeColorOk() : AppControl.Instance.GiveMeColorWrong();

                        //break;

                        case Enum_StepQuestionnaire.FEEDBACK:

                            //Save the results

                            //if (c3.MultipleSelectionAllowed)
                            //{
                            //    c3.Score = ((float)c3.SelectableItems.Count - (float)c3.NumErrors) / (float)c3.SelectableItems.Count * 100;
                            //}
                            //else
                            //{
                            //    c3.Score = c3.NumErrors > 0 ? 0 : 100;
                            //}
                            //s3.Step = Enum_QuestionnaireMultipleFrameStep.FINISHED;
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

                            c4.Step = Enum_StepQuestionnaire.ANSWERING;
                            break;

                        case Enum_StepQuestionnaire.ANSWERING:
                            c4.NumErrors = 0;

                            for (int i = 0; i < s4.spOperativity.Children.Count; i++)
                            {
                                MultipleChoiceControl mpc = ((MultipleChoiceControl)(s4.spOperativity.Children[i]));
                                mpc.IsEnabled = false;
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

                            // make things happen with explanations
                            c4.Step = Enum_StepQuestionnaire.FEEDBACK;

                            break;
                        //s3.Step = Enum_QuestionnaireMultipleFrameStep.FEEDBACK;

                        //PrepareAdvanceStudent("Next");
                        //s3.listView.Background = Brushes.Transparent;
                        //c3.NumErrors = 0;
                        //if (c3.MultipleSelectionAllowed)
                        //{
                        //    for (int i = 0; i < s3.items.Count; i++)
                        //    {
                        //        if (s3.items[i].IHaveToBeSelected != s3.items[i].IHaveBeenSelected)
                        //        {
                        //            c3.NumErrors++;
                        //        }
                        //        //s3.items[i].SituationSelection = (s3.items[i].IHaveToBeSelected != s3.items[i].IHaveBeenSelected) ? 0 : 1;
                        //        s3.items[i].SituationSelection = (s3.items[i].IHaveToBeSelected) ? 1 : -1;
                        //    }
                        //}
                        //else
                        //{
                        //    for (int i = 0; i < s3.items.Count; i++)
                        //    {
                        //        if (s3.items[i].IHaveToBeSelected)
                        //        {
                        //            s3.items[i].SituationSelection = 1;
                        //            if (!s3.items[i].IHaveBeenSelected)
                        //            {
                        //                c3.NumErrors++;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (s3.items[i].IHaveBeenSelected)
                        //            {
                        //                s3.items[i].SituationSelection = -1;
                        //            }
                        //        }
                        //    }
                        //}
                        ////if (c3.ShowFeedback)
                        ////{
                        ////    if (c3.MultipleSelectionAllowed)
                        ////    {
                        ////        s3.tbMessage.Text = c3.NumErrors == 0 ? "Correct! - You made no errors" : "You made " + c3.NumErrors.ToString() + (c3.NumErrors == 1 ? " error" : " errors.");
                        ////    }
                        ////    else
                        ////    {
                        ////        s3.tbMessage.Text = c3.NumErrors == 0 ? "Correct!" : "Wrong Answer";
                        ////    }
                        ////    s3.bMessage.BorderBrush = c3.NumErrors == 0 ? AppControl.Instance.GiveMeColorOk() : AppControl.Instance.GiveMeColorWrong();

                        ////    //s3.listView.IsEnabled = false;
                        ////    s3.listView.Background = Brushes.Transparent;
                        ////}
                        ////else
                        ////{
                        ////    s3.tbMessage.Text = "Let's verify your hypothesis.";
                        ////    s3.spOperativity.Visibility = Visibility.Collapsed;
                        ////    s3.grMain.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                        ////    s3.listView.Background = Brushes.Transparent;
                        ////}
                        //foreach (QuestionnaireItemSelection item in s3.listView.Items)
                        //{
                        //    item.ImEnabled = false;
                        //}

                        ////if (c3.AssociatedContent)
                        ////{
                        ////    s3.gridAnswers.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        ////    t3.gridAnswers.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        ////}

                        //List<QuestionnaireItemSelection> showToTeacher = new List<QuestionnaireItemSelection>();
                        //foreach (QuestionnaireItemSelection item in s3.items)
                        //{
                        //    showToTeacher.Add(new QuestionnaireItemSelection(item.ItemAssociatedText, item.IHaveToBeSelected, item.IHaveBeenSelected, item.SituationSelection));
                        //}
                        //t3.grMain.RowDefinitions[1].Height = new GridLength(4, GridUnitType.Star);
                        //t3.listView.Visibility = Visibility.Visible;
                        //t3.listView.ItemsSource = showToTeacher;
                        //foreach (QuestionnaireItemSelection item in t3.listView.Items)
                        //{
                        //    item.ImEnabled = false;
                        //}
                        //if (c3.MultipleSelectionAllowed)
                        //{
                        //    t3.tbMessage.Text = c3.NumErrors == 0 ? "Correct! - The student made no errors" : "The student made " + c3.NumErrors.ToString() + (c3.NumErrors == 1 ? " error." : " errors.");
                        //}
                        //else
                        //{
                        //    t3.tbMessage.Text = c3.NumErrors == 0 ? "Correct!" : "Wrong Answer";
                        //}
                        //t3.bMessage.BorderBrush = c3.NumErrors == 0 ? AppControl.Instance.GiveMeColorOk() : AppControl.Instance.GiveMeColorWrong();

                        //break;

                        case Enum_StepQuestionnaire.FEEDBACK:

                            //Save the results

                            //if (c3.MultipleSelectionAllowed)
                            //{
                            //    c3.Score = ((float)c3.SelectableItems.Count - (float)c3.NumErrors) / (float)c3.SelectableItems.Count * 100;
                            //}
                            //else
                            //{
                            //    c3.Score = c3.NumErrors > 0 ? 0 : 100;
                            //}
                            //s3.Step = Enum_QuestionnaireMultipleFrameStep.FINISHED;
                            PrepareAdvanceStudent("Next");

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

                            c5.Step = Enum_StepDynamicAnalysis.ANSWERING;
                            break;

                        case Enum_StepDynamicAnalysis.ANSWERING:
                            c5.NumErrors = 0;

                            for (int i = 0; i < s5.spOperativity.Children.Count; i++)
                            {
                                MultipleChoiceControl mpc = ((MultipleChoiceControl)(s5.spOperativity.Children[i]));
                                mpc.IsEnabled = false;
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
                            }

                            //make things happen on score and inidcations

                            // make things happen with explanations
                            c5.Step = Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS;

                            break;
                        //s3.Step = Enum_QuestionnaireMultipleFrameStep.FEEDBACK;

                        //PrepareAdvanceStudent("Next");
                        //s3.listView.Background = Brushes.Transparent;
                        //c3.NumErrors = 0;
                        //if (c3.MultipleSelectionAllowed)
                        //{
                        //    for (int i = 0; i < s3.items.Count; i++)
                        //    {
                        //        if (s3.items[i].IHaveToBeSelected != s3.items[i].IHaveBeenSelected)
                        //        {
                        //            c3.NumErrors++;
                        //        }
                        //        //s3.items[i].SituationSelection = (s3.items[i].IHaveToBeSelected != s3.items[i].IHaveBeenSelected) ? 0 : 1;
                        //        s3.items[i].SituationSelection = (s3.items[i].IHaveToBeSelected) ? 1 : -1;
                        //    }
                        //}
                        //else
                        //{
                        //    for (int i = 0; i < s3.items.Count; i++)
                        //    {
                        //        if (s3.items[i].IHaveToBeSelected)
                        //        {
                        //            s3.items[i].SituationSelection = 1;
                        //            if (!s3.items[i].IHaveBeenSelected)
                        //            {
                        //                c3.NumErrors++;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (s3.items[i].IHaveBeenSelected)
                        //            {
                        //                s3.items[i].SituationSelection = -1;
                        //            }
                        //        }
                        //    }
                        //}
                        ////if (c3.ShowFeedback)
                        ////{
                        ////    if (c3.MultipleSelectionAllowed)
                        ////    {
                        ////        s3.tbMessage.Text = c3.NumErrors == 0 ? "Correct! - You made no errors" : "You made " + c3.NumErrors.ToString() + (c3.NumErrors == 1 ? " error" : " errors.");
                        ////    }
                        ////    else
                        ////    {
                        ////        s3.tbMessage.Text = c3.NumErrors == 0 ? "Correct!" : "Wrong Answer";
                        ////    }
                        ////    s3.bMessage.BorderBrush = c3.NumErrors == 0 ? AppControl.Instance.GiveMeColorOk() : AppControl.Instance.GiveMeColorWrong();

                        ////    //s3.listView.IsEnabled = false;
                        ////    s3.listView.Background = Brushes.Transparent;
                        ////}
                        ////else
                        ////{
                        ////    s3.tbMessage.Text = "Let's verify your hypothesis.";
                        ////    s3.spOperativity.Visibility = Visibility.Collapsed;
                        ////    s3.grMain.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                        ////    s3.listView.Background = Brushes.Transparent;
                        ////}
                        //foreach (QuestionnaireItemSelection item in s3.listView.Items)
                        //{
                        //    item.ImEnabled = false;
                        //}

                        ////if (c3.AssociatedContent)
                        ////{
                        ////    s3.gridAnswers.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        ////    t3.gridAnswers.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        ////}

                        //List<QuestionnaireItemSelection> showToTeacher = new List<QuestionnaireItemSelection>();
                        //foreach (QuestionnaireItemSelection item in s3.items)
                        //{
                        //    showToTeacher.Add(new QuestionnaireItemSelection(item.ItemAssociatedText, item.IHaveToBeSelected, item.IHaveBeenSelected, item.SituationSelection));
                        //}
                        //t3.grMain.RowDefinitions[1].Height = new GridLength(4, GridUnitType.Star);
                        //t3.listView.Visibility = Visibility.Visible;
                        //t3.listView.ItemsSource = showToTeacher;
                        //foreach (QuestionnaireItemSelection item in t3.listView.Items)
                        //{
                        //    item.ImEnabled = false;
                        //}
                        //if (c3.MultipleSelectionAllowed)
                        //{
                        //    t3.tbMessage.Text = c3.NumErrors == 0 ? "Correct! - The student made no errors" : "The student made " + c3.NumErrors.ToString() + (c3.NumErrors == 1 ? " error." : " errors.");
                        //}
                        //else
                        //{
                        //    t3.tbMessage.Text = c3.NumErrors == 0 ? "Correct!" : "Wrong Answer";
                        //}
                        //t3.bMessage.BorderBrush = c3.NumErrors == 0 ? AppControl.Instance.GiveMeColorOk() : AppControl.Instance.GiveMeColorWrong();

                        //break;

                        case Enum_StepDynamicAnalysis.HYPOTHESIS_INJECTIONPOINTS:

                            //Save the results

                            //if (c3.MultipleSelectionAllowed)
                            //{
                            //    c3.Score = ((float)c3.SelectableItems.Count - (float)c3.NumErrors) / (float)c3.SelectableItems.Count * 100;
                            //}
                            //else
                            //{
                            //    c3.Score = c3.NumErrors > 0 ? 0 : 100;
                            //}
                            //s3.Step = Enum_QuestionnaireMultipleFrameStep.FINISHED;
                            c5.Step = Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS;
                            PrepareAdvanceStudent("Next");
                            break;

                        case Enum_StepDynamicAnalysis.FEEDBACK_INJECTIONPOINTS:

                            //Save the results

                            //if (c3.MultipleSelectionAllowed)
                            //{
                            //    c3.Score = ((float)c3.SelectableItems.Count - (float)c3.NumErrors) / (float)c3.SelectableItems.Count * 100;
                            //}
                            //else
                            //{
                            //    c3.Score = c3.NumErrors > 0 ? 0 : 100;
                            //}
                            //s3.Step = Enum_QuestionnaireMultipleFrameStep.FINISHED;
                            c5.Step = Enum_StepDynamicAnalysis.FEEDBACK;
                            PrepareAdvanceStudent("Next");
                            break;

                        case Enum_StepDynamicAnalysis.FEEDBACK:

                            //Save the results

                            //if (c3.MultipleSelectionAllowed)
                            //{
                            //    c3.Score = ((float)c3.SelectableItems.Count - (float)c3.NumErrors) / (float)c3.SelectableItems.Count * 100;
                            //}
                            //else
                            //{
                            //    c3.Score = c3.NumErrors > 0 ? 0 : 100;
                            //}
                            //s3.Step = Enum_QuestionnaireMultipleFrameStep.FINISHED;
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
                    ClinicalCaseStep_Face3DInteraction c6 = (ClinicalCaseStep_Face3DInteraction)caseStepCurrent;
                    switch (c6.Step)
                    {
                        case Enum_StepFace3DInteraction.LOADING:

                            c6.Step = Enum_StepFace3DInteraction.OPERATIVE;
                            break;

                        case Enum_StepFace3DInteraction.OPERATIVE:

                            c6.Step = Enum_StepFace3DInteraction.FEEDBACK;

                            break;

                        case Enum_StepFace3DInteraction.FEEDBACK:
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
                    if (CurrentCase.Steps[CurrentCase.CurrentStepIndex].ToBeExcluded)
                    {
                        AdvanceStep();
                        return;
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
                                int counterScoreable = 0;

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

                            MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
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
                                Vis3DFrame.SetContent((ClinicalCaseStep_Face3DInteraction)caseStepCurrent);

                                if (Vis3DFrameStudent == null)
                                {
                                    Vis3DFrameStudent = new Visualization3DFrameStudent();
                                }
                                StudentFrame = Vis3DFrameStudent;

                                MainTeacherFrame.spFluidControls.Visibility = Visibility.Hidden;
                                MainTeacherFrame.frActivity.Navigate(TeacherFrame);
                                MainStudentFrame.frActivity.Navigate(StudentFrame);
                            }
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        internal void StartRotation()
        {
            AppControl.Instance.UpDirection = new Vector3D(0, 0, -1);
            //timerRotation.Start();
        }

        public void Init()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;
            var hwnd2 = new WindowInteropHelper(WindowTeacher).Handle;
            WindowTeacher.SetWindowLong(hwnd2, WindowTeacher.GWL_STYLE, WindowTeacher.GetWindowLong(hwnd2, WindowTeacher.GWL_STYLE) & ~WindowTeacher.WS_SYSMENU);
            DBConnector.Instance.InitDB("C:\\BeautySim\\Database\\BeautySimDB.db");
            pDIClass = new PDIClass();
            System.Windows.Media.Media3D.Quaternion qy = AppControl.CreateRotationQuaternionAlongAxis(180, 1);
            System.Windows.Media.Media3D.Quaternion qz = AppControl.CreateRotationQuaternionAlongAxis(0, 2);

            AppControl.Instance.Rotation_Manikin = qy * qz;
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

        public void InjectAnesthetic(bool dummy = false)
        {
        }

        public void ListCases()
        {
            AvailableCases.Clear();

            List<string> caseFiles = new List<string>();
            DirectoryInfo baseDir = new DirectoryInfo("c:\\BeautySim\\Cases");
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

            CurrentCase.CurrentStepIndex = -1;
            CurrentCaseState = Enum_CaseState.LOADING;

            CurrentCaseState = Enum_CaseState.LOADED;
            AppControl.Instance.WindowTeacher.bBack.IsEnabled = true;
            AppControl.Instance.WindowTeacher.SetCase(CurrentCase.Name);
            AppControl.Instance.WindowStudent.Navigate(new CaseStudentFrame());
            AppControl.Instance.WindowTeacher.Navigate(new CaseTeacherFrame());

            MainStudentFrame.tbCaseName.Text = CurrentCase.Name;
        }

        public Stream LoadTexture(string file)
        {
            var bytecode = global::SharpDX.IO.NativeFile.ReadAllBytes(file);
            return new MemoryStream(bytecode);
        }

        public Stream LoadTextureCorrected(string file)
        {
            System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fs.CopyTo(ms);
            return fs;
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

        public void ResetPoints()
        {
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
            string baseFolder = "C:\\BeautySim\\DataBase\\Results\\" + result.CaseName + "_" +
                tt.Year.ToString() + tt.Month.ToString() + tt.Day.ToString() + "_" +
                tt.Hour.ToString() + tt.Minute.ToString() + tt.Second.ToString();

            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            string filePath = baseFolder + "\\CaseSaved.xml";
            ResultToSave resToSave = new ResultToSave();
            resToSave.NumberOfNeedleInsertions = InsertionNeedle;

            resToSave.IsMultipleNeedleInjections = true;
            resToSave.WrongInjections = TargetMissedNerve_Injections;

            resToSave.IsMultipleNeedleInjections = false;

            resToSave.Save<ResultToSave>(filePath);
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
            if (IhaveToSaveResults)
            {
                Results resultToSave = new Results();
                //AppControl.Instance.SaveResult(SelectedCase);
            }
            CurrentCase.ClearAllUserRelatedFields();
            //AppControl.Instance.WindowTeacher.SetCase("");
        }

        public void WindowTeacherLoaded(object sender, RoutedEventArgs e)
        {
        }

        public void ZeroCaseIndications()
        {
            if ((WindowTeacher.PageContainer.Content is FunctionalitiesFrame))
            {
                FunctionalitiesFrame wft = (FunctionalitiesFrame)WindowTeacher.PageContainer.Content;
                if (CurrentCase != null)
                {
                    if (CurrentCase.Steps != null)
                    {
                        foreach (ClinicalCaseStep f in CurrentCase.Steps)
                        {
                            f.Selected = false;
                        }
                    }
                }
                foreach (ClinicalCase f in Cases)
                {
                    f.Selected = false;
                }
                wft.pbLoadCaseSingleStep.Value = 0;
                SetText(wft.tbInfoLoadCase, StringLoadCase);
                //SetText(wft.tCurrentCase, StringCurrentCase);
            }
        }

        internal void ClearTempResults()
        {
            if (!Directory.Exists("C:\\BeautySim\\ResultsTemp"))
            {
                Directory.CreateDirectory("C:\\BeautySim\\ResultsTemp");
            }
            DirectoryInfo di = new DirectoryInfo("C:\\BeautySim\\ResultsTemp");

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

        internal void ConnectBeautySim()
        {
            HwndSource hwnd = HwndSource.FromHwnd(new WindowInteropHelper(WindowTeacher).Handle);
            BeautySimController.Instance.ScannedSimulatorsEvent += new BeautySimController.ScannedSimulatorsDelegate(ScannedSimulatorListener);
            BeautySimController.Instance.AMessageArrivedEvent += new BeautySimController.AMessageArrivedDelegate(AMessageArriveListener);
            BeautySimController.Instance.SerialNumberAcquired += BeautySim_SerialNumberAcquired;

            BeautySimController.Instance.Go();
            BeautySimController.Instance.StartHookDevices(hwnd);
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

        internal List<string> GetListOfStringCases()
        {
            List<string> toRet = new List<string>();
            foreach (ClinicalCase item in AvailableCases)
            {
                toRet.Add(item.Name);
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

        internal Brush GiveMeColorOk()
        {
            BrushConverter bc = new BrushConverter();
            return (Brush)bc.ConvertFrom("#009688");
        }

        internal Brush GiveMeColorWrong()
        {
            BrushConverter bc = new BrushConverter();
            return (Brush)bc.ConvertFrom("#DB4C4C");
            //BrushConverter bc = new BrushConverter();
            //return Brushes.DarkRed;
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

        internal void ShowNeedleIndication(object sender)
        {
            ShowIndicationNeedle = (bool)(sender as ToggleButton).IsChecked;
            Properties.Settings.Default.ShowNeedleTrajectory = ShowIndicationNeedle;
            Properties.Settings.Default.Save();
            NeedleIndicationColor = System.Windows.Media.Color.FromArgb((ShowIndicationNeedle ? (byte)255 : (byte)0), 150, 150, 150);
            if ((bool)(sender as ToggleButton).IsChecked)
            {
                // Code for Checked state
            }
            else
            {
                // Code for Un-Checked state
            }
        }

        internal void StopLoadingCase()
        {
            if (CurrentCaseState == Enum_CaseState.LOADING)
            {
                StopLoadCommand = true;
            }
        }

        internal void TbShowFasciaPosition(object sender)
        {
            //ShowTargetPosition = (bool)(sender as ToggleButton).IsChecked;
            //Properties.Settings.Default.ShowTargetPosition = ShowTargetPosition;
            //Properties.Settings.Default.Save();
            //if ((!EffectFasciaHitActivated) && (ModelMaterialDeformableArea != null))
            //{
            //    ModelMaterialDeformableArea.DiffuseColor = new SharpDX.Color4(0, 255, 255, ((ShowTargetPosition && TargetStructureAvailableInFrame && !CurrentPhase.MuscularBlock) ? 0.5f : 0));
            //}

            //if ((bool)(sender as ToggleButton).IsChecked)
            //{
            //    // Code for Checked state
            //}
            //else
            //{
            //    // Code for Un-Checked state
            //}

            //WindowStudent.spLegenda.Visibility = (bool)(sender as ToggleButton).IsChecked ? Visibility.Visible : Visibility.Hidden;
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

        internal void WorkingFrameLoaded(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.VisibilityResults = Visibility.Hidden;
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

                        //        case (byte)Enum_SimulatorMessageType.FLUX:

                        //            wft.tbFlux.Text = BeautySim.Globalization.Language.str_flux + " " + (BeautySimController.Instance.Simulator).Flux.ToString("00.00");
                        //            wft.tbVolume.Text = BeautySim.Globalization.Language.str_volume + " " + (BeautySimController.Instance.Simulator).CurrentVolume.ToString("00.00");

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

        private void AnimationTick(object sender, EventArgs e)
        {
        }

        //private bool AreModulesChecked()
        //{
        //    bool areChecked = false;

        //    if (CurrentTeacher != null && CurrentTeacher.UserName == ADMIN_USERNAME)
        //    {
        //        areChecked = true;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < ModulesActivation.Length; i++)
        //        {
        //            if (ModulesActivation[i])
        //            {
        //                areChecked = true;
        //                break;
        //            }
        //        }
        //    }

        //    return areChecked;
        //}

        private void BeautySim_SerialNumberAcquired(object sender, EventArgs e)
        {
            //if (BTYSimulatorConnected)
            //{
            //    UpdateModules();
            //}
        }

        private void CalculateAndShowResults()
        {
            decimal offset = 0;
            DurationNeedlePhase = DateTime.Now - StartNeedlePhaseTime;
            MinutesNeedlePhase = DurationNeedlePhase.Minutes;
            TextTime = DurationNeedlePhase.Minutes.ToString() + ":" + DurationNeedlePhase.Seconds.ToString();
            TextNeedlePuntures = insertionNeedle.ToString();

            string comment = BeautySim.Globalization.Language.str_ok;
            if (insertionNeedle == 0)
            {
                comment = BeautySim.Globalization.Language.str_anomaly;
            }

            string comment2 = BeautySim.Globalization.Language.str_lt_3min;
            if (DurationNeedlePhase.Minutes > 2)
            {
                comment2 = BeautySim.Globalization.Language.str_too_lengthy + " " + (5 * (DurationNeedlePhase.Minutes - 2)).ToString();
            }

            TextNeedlePunturesExp = comment;
            TextTimeExp = comment2;

            //TextNerveWrongIntro = "";
            //TextNerveWrong = "";
            //TextNerveWrongExp = "";
            //TextVascularWrongIntro = "";
            //TextVascularWrong = "";
            //TextVascularWrongExp = "";

            //if (CurrentPhase.NerveMultipleInjections)
            //{
            //    int hitCorrectTarget = CurrentPhase.TargetPointsNerve.Where(x => ((x.Injections.Count > 0) && (x.T == 0))).Count();
            //    int hitWrongNerve = CurrentPhase.TargetPointsNerve.Where(x => ((x.AreaEnteredFromNeedle > 0) && (x.T == 1))).Count();
            //    int hitWrongVascular = CurrentPhase.TargetPointsNerve.Where(x => ((x.AreaEnteredFromNeedle > 0) && (x.T == 2))).Count();

            //    int totalCorrectTargets = CurrentPhase.TargetPointsNerve.Where(x => x.T == 0).Count();

            //    TextNerveTarget = hitCorrectTarget.ToString() + " / " + totalCorrectTargets.ToString();
            //    TextWrongInjections = TargetMissedNerve_Injections.ToString();

            //    TextNerveTargetExp = "hit " + hitCorrectTarget.ToString() + " / " + totalCorrectTargets.ToString() + " targets";
            //    TextWrongInjectionsExp = TargetMissedNerve_Injections.ToString() + " injections. Score diminished by 10 for each wrong hit.";

            //    if (hitWrongNerve > 0)
            //    {
            //        TextNerveWrongIntro = "wrong nerve hits";
            //        TextNerveWrong = hitWrongNerve.ToString();
            //        TextNerveWrongExp = "hit " + hitWrongNerve.ToString() + " wrong nerve areas. Score dimished by 50 for each wrong hit.";
            //    }

            //    if (hitWrongVascular > 0)
            //    {
            //        TextVascularWrongIntro = "wrong vascular hits";
            //        TextVascularWrong = hitWrongVascular.ToString();
            //        TextVascularWrongExp = "hit " + hitWrongVascular.ToString() + " vascular areas. Score set to 0.";
            //    }

            //    if (hitCorrectTarget < totalCorrectTargets)
            //    {
            //        offset = offset - (100 * (decimal)(totalCorrectTargets - hitCorrectTarget) / (decimal)totalCorrectTargets);
            //    }

            //    if (TargetMissedNerve_Injections > 0)
            //    {
            //        offset = offset - (10 * (TargetMissedNerve_Injections));
            //    }

            //    offset = offset - (50 * hitWrongNerve);
            //    offset = offset - (100 * hitWrongVascular);
            //    CurrentCaseScore = 100 + offset;
            //}
            //else
            //{
            //    if (insertionNeedle > 1)
            //    {
            //        offset = offset - (10 * (insertionNeedle - 1));
            //    }

            //    if (MinutesNeedlePhase > 2)
            //    {
            //        offset = offset - (5 * ((int)MinutesNeedlePhase - 2));
            //    }
            //    CurrentCaseScore = NeedleTargetedFascia ? (100 + offset) : 0;

            //    TextFasciaTarget = NeedleTargetedFascia ? BeautySim.Globalization.Language.str_yes : BeautySim.Globalization.Language.str_no;

            //    TextFasciaTargetExp = NeedleTargetedFascia ? BeautySim.Globalization.Language.str_ok : BeautySim.Globalization.Language.str_score_set_to_zero;
            //}
            if (CurrentCaseScore < 0)
            {
                CurrentCaseScore = 0;
            }
            TextScore = CurrentCaseScore.ToString("00.00");

            VisibilityResults = Visibility.Visible;
        }

        private float CalculateDistanceNeedleToTarget(SharpDX.Vector3 vector3, out float t, out bool upOrLow, out SharpDX.Vector3 vectorToCentral)
        {
            t = 0;
            upOrLow = false;
            vectorToCentral = new SharpDX.Vector3();

            return 1000f;
        }

        public IEffectsManager EffectsManager
        {
            get { return effectsManager; }
            protected set
            {
                SetValue(ref effectsManager, value);
            }
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

        private bool CheckInSkin_WRS(VectorMath.Vector3 position, double histUp, double histDown, double histUpPadD, double histDownPadD, bool valuePreviousStep, bool imPadD)
        {
            return false;
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

        private void PDIClass_OnConnectionStatusChanged(Device.Motion.CONNECTIONSTATUS status)
        {
            SetIndicator(WindowTeacher.elSensors, status == Device.Motion.CONNECTIONSTATUS.ACQUIRING);
        }

        private void PDIClass_OnNewFrameAvailable(List<Device.Motion.FRAME> frames) // MODIFY EVERYTHING HERE
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
                        f1 = frames[0];
                        XPosSensor02 = f1.Pos.X * 10;
                        YPosSensor02 = f1.Pos.Y * 10;
                        ZPosSensor02 = f1.Pos.Z * 10;
                        RotationSensor_WRS_real = new System.Windows.Media.Media3D.Quaternion(f1.Ori.X, f1.Ori.Y, f1.Ori.Z, f1.Ori.W);

                        AxisAngleRotation3D T1pre = new AxisAngleRotation3D(new Vector3D(1, 0, 0), AppControl.Instance.RotationAngleOffsetNeedle);
                        RotateTransform3D T1 = new RotateTransform3D(T1pre);
                        TranslateTransform3D T2 = new TranslateTransform3D(0, AxisPositionOffsetNeedle, 0);
                        QuaternionRotation3D rotation = new QuaternionRotation3D(RotationSensor_WRS_real);
                        RotateTransform3D T3 = new RotateTransform3D(rotation);
                        TranslateTransform3D T4 = new TranslateTransform3D(XPosSensor02, YPosSensor02, ZPosSensor02);

                        Transform3DGroup transformGroup = new Transform3DGroup();
                        transformGroup.Children.Add(T1);
                        transformGroup.Children.Add(T2);
                        transformGroup.Children.Add(T3);
                        transformGroup.Children.Add(T4);

                        TipNeedle = transformGroup.Transform(TipNeedleOrigin);
                        BaseNeedle = transformGroup.Transform(BaseNeedleOrigin);

                        TransformSphere2.OffsetX = TipNeedle.X;
                        TransformSphere2.OffsetY = TipNeedle.Y;
                        TransformSphere2.OffsetZ = TipNeedle.Z;

                        OnPropertyChanged(nameof(TransformSphere2));

                        Vector3D directionNeedle = TipNeedle - BaseNeedle;

                        PitchNeedle = -Math.Atan2(directionNeedle.Z, Math.Sqrt(Math.Pow(directionNeedle.X, 2) + Math.Pow(directionNeedle.Y, 2))) * 180.0 / Math.PI; ;
                        YawNeedle = Math.Atan2(-directionNeedle.Y, -directionNeedle.X) * 180.0 / Math.PI;

                        TextToShowYawPitch = "Pitch: " + PitchNeedle.ToString("00.0") + " Yaw: " + YawNeedle.ToString("00.0");


                        if ((TeacherFrame is Visualization3DFrame) && (Model3DInitialized))
                        {



                            var viewport = AppControl.Instance.Vis3DFrame.hvView3D;

                            actualPointsHit = new List<PointHit>();
                            //SharpDX.Ray ray = new Ray(new Vector3((float)TipNeedle.X, (float)TipNeedle.Y, (float)TipNeedle.Z), new SharpDX.Vector3((float)directionNeedle.X, (float)directionNeedle.Y, (float)directionNeedle.Z));
                            SharpDX.Ray ray = new SharpDX.Ray(new SharpDX.Vector3((float)BaseNeedle.X, (float)BaseNeedle.Y, (float)BaseNeedle.Z), new SharpDX.Vector3((float)directionNeedle.X, (float)directionNeedle.Y, (float)directionNeedle.Z));

                            var hitContext = new HitTestContext(viewport.RenderHost.RenderContext, ref ray);
                            List<HitTestResult> hits = new List<HitTestResult>();

                            var rend = viewport.Renderables.ToList();

                            stopwatch.Restart();

                            foreach (var element in rend)
                            {
                                var t = element.GetType().ToString();
                                CheckSceneNode(element, hitContext, ray.Direction, new SharpDX.Vector3((float)TipNeedle.X, (float)TipNeedle.Y, (float)TipNeedle.Z), ref actualPointsHit);//, ref hitNames);
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
                                HitStructure = "next: " + actualPointsHit[indexMin].Name + " " + actualPointsHit.Count;
                                //Debug.WriteLine(HitStructure);
                            }
                            else
                            {
                                HitStructure = "next: none";
                            }

                            //ELABORATE TO UNDERSTAND
                            for (int i = 0; i < actualPointsHit.Count; i++)
                            {
                                PointHit pointHit = actualPointsHit[i];

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
                                        if (previousHit.Distance >= 0) //entered
                                        {
                                            furtherInvestigateForEntrance = true;
                                        }
                                    }

                                    if (furtherInvestigateForEntrance)
                                    {
                                        //CREA
                                        if (pointHit.Name == "SKIN")
                                        {
                                            bool foundOtherSkinAreas = false;
                                            for (int j = 0; j < actualPointsHit.Count; j++)
                                            {
                                                if (j != i)
                                                {
                                                    if (StringSkinAreas.Contains(actualPointsHit[j].Name))
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
                                        for (int k = EntranceDataList.Count - 1; k >= 0; k--)

                                        {
                                            if (EntranceDataList[k].StructureEntered == pointHit.Name && (EntranceDataList[k].ToBeClosed))
                                            {
                                                double absDepth = Math.Abs(pointHit.Distance);
                                                if (absDepth > EntranceDataList[k].Depth)
                                                {
                                                    EntranceDataList[k].Depth = absDepth;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    if (createEntrance)
                                    {
                                        EntranceData entData = new EntranceData();
                                        entData.EntranceTime = DateTime.Now;
                                        entData.StructureEntered = pointHit.Name;
                                        entData.EntryingPointPitch = 0;
                                        entData.EntryingPointYaw = 0;
                                        entData.CollisionPoint = pointHit.Point;
                                        entData.ToBeClosed = true;
                                        entData.Index = EntranceDataList.Count;
                                        entData.EntryingPointPitch = PitchNeedle;
                                        entData.EntryingPointYaw = YawNeedle;
                                        EntranceDataList.Add(entData);
                                        AppControl.Instance.Vis3DFrame.entranceListView.ScrollIntoView(entData);

                                        Debug.WriteLine(entData.ToReadableString());
                                    }
                                }
                                if (pointHit.Distance > 0) //exited
                                {
                                    if (previousHit != null)
                                    {
                                        if (previousHit.Distance < 0) //exited
                                        {
                                            furtherInvestigateForExiting = true;
                                        }
                                    }

                                    if (furtherInvestigateForExiting)
                                    {
                                        if (pointHit.Name == "SKIN")
                                        {
                                            bool foundOtherSkinAreas = false;
                                            for (int j = 0; j < actualPointsHit.Count; j++)
                                            {
                                                if (j != i)
                                                {
                                                    if (StringSkinAreas.Contains(actualPointsHit[j].Name))
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
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            previousPointsHit.Clear();
                            for (int nn = 0; nn < actualPointsHit.Count; nn++)
                            {
                                previousPointsHit.Add(actualPointsHit[nn].DeepCopy());
                            }
                        }
                    }
                }));
            }
            
            catch (Exception)
            {
            }

       
        }
        private List<PointHit> previousPointsHit = new List<PointHit>();
        public List<string> StringSkinAreas = new List<string>();
        public Dictionary<string, List<Guid>> CollisionItemsGuid = new Dictionary<string, List<Guid>>();
        private Stopwatch stopwatch = new Stopwatch();
        private List<long> averageCollisionTime = new List<long>();
        private void CheckSceneNode(SceneNode node, HitTestContext context,SharpDX.Vector3 rayDir, SharpDX.Vector3 tip, ref List<PointHit> pointsHit)//, ref List<string> nodeNames)
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
            //if (node is MeshNode ms)
            //{
            //    if (ms.Geometry is MeshGeometry3D geom)
            //    {
            //        List<HitTestResult> hits = new List<HitTestResult>();

            //        ms.HitTest(context, ref hits);

            //        foreach (var hit in hits)
            //        {
            //            pointsHit.Add(new PointHit(hit.PointHit, ms.Name));

            //            foreach (var item in collisionItemsGuid)
            //            {
            //                if (item.Value.Contains(hit.Geometry.GUID))
            //                {
            //                    nodeNames.Add(item.Key);
            //                }
            //            }
            //        }
            //    }
            //}
        }
        public Point3D BaseNeedle
        {
            get { return baseNeedle; }
            set
            {
                baseNeedle = value;
                OnPropertyChanged(nameof(BaseNeedle));
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

        public float AxisPositionOffsetNeedle
        {
            get { return axisPositionOffsetNeedle; }
            set
            {
                axisPositionOffsetNeedle = value;
                OnPropertyChanged(nameof(AxisPositionOffsetNeedle));
            }
        }

        public bool Model3DInitialized { get; private set; }
        public double PitchNeedle { get; private set; }
        public double YawNeedle { get; private set; }
        private bool corrugatorVsible;
        private bool orbicularisVisible;
        private bool veinsVisible;
        private bool arteriesVisible;
        private bool skinVisible;
        public ObservableCollection<EntranceData> EntranceDataList { get; set; }
        private List<PointHit> actualPointsHit = new List<PointHit>();
        public bool CorrugatorVisible
        {
            get { return corrugatorVsible; }
            set
            {
                corrugatorVsible = value;
                OnPropertyChanged(nameof(CorrugatorVisible));
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


        public bool VeinsVisible
        {
            get { return veinsVisible; }
            set
            {
                veinsVisible = value;
                OnPropertyChanged(nameof(VeinsVisible));
            }
        }

        public bool ArteriesVisible
        {
            get { return arteriesVisible; }
            set
            {
                arteriesVisible = value;
                OnPropertyChanged(nameof(ArteriesVisible));
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

        //private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    //CurrentCaseState = Enum_CaseState.LOADED;
        //    //AppControl.Instance.WindowTeacher.bBack.IsEnabled = true;
        //    //AppControl.Instance.WindowTeacher.lCase.Text=SelectedCase.Name;
        //    //AppControl.Instance.WindowStudent.Navigate(new CaseStudentFrame());
        //    //AppControl.Instance.WindowTeacher.Navigate(new CaseTeacherFrame());

        //    //MainStudentFrame.tbCaseName.Text = SelectedCase.Name;
        //}

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

        //private void UpdateModules()
        //{
        //    string currentSerial = BeautySimController.Instance != null && BeautySimController.Instance.Simulator != null ? BeautySimController.Instance.Simulator.SerialNumber : string.Empty;

        //    if (!ModulesChecked || (!string.IsNullOrWhiteSpace(currentSerial) && currentSerial != LastSerial))
        //    {
        //        CheckModulesActivation();

        //        LastSerial = currentSerial;
        //    }

        //    if (WindowTeacher != null)
        //    {
        //        WindowTeacher.Dispatcher.Invoke(() =>
        //        {
        //            if (WindowTeacher.PageContainer.Content is FunctionalitiesFrame)
        //            {
        //                ((FunctionalitiesFrame)WindowTeacher.PageContainer.Content).InitModulesItemSource();

        //                ((FunctionalitiesFrame)WindowTeacher.PageContainer.Content).InitFrame();
        //            }
        //        });
        //    }
        //}

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        internal void InitViewModel()
        {
            if (!Model3DInitialized)
            {
                EffectsManager = new DefaultEffectsManager();

                if (TeacherFrame is Visualization3DFrame)
                {
                    ((Visualization3DFrame)TeacherFrame).InitializeModels("C:\\Lavoro\\Lavoro_A\\BeautySIM\\BeautySim_MODELLI2");
                    ((Visualization3DFrame)TeacherFrame).InitializeCoordinates();

                    ((Visualization3DFrameStudent)StudentFrame).bModelView.DataContext = ((Visualization3DFrame)TeacherFrame).hvView3D;

                    double h = ((Visualization3DFrame)TeacherFrame).hvView3D.ActualHeight;
                    double w = ((Visualization3DFrame)TeacherFrame).hvView3D.ActualWidth;
                    double hs = ((Visualization3DFrameStudent)StudentFrame).bModelView.ActualHeight;
                    double ws = ((Visualization3DFrameStudent)StudentFrame).bModelView.ActualWidth;
                    ((Visualization3DFrameStudent)StudentFrame).bModelView.Height = hs;
                    ((Visualization3DFrameStudent)StudentFrame).bModelView.Width = hs * w / h;
                }
                Model3DInitialized = true;
            }
        }
    }
}