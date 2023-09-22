using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Model3D
{
    public class EcoViewModel : BaseViewModel
    {
        private DeformableArea deformableArea;
        private DispatcherTimer dispatcherTimerAnimation = new DispatcherTimer();
        private DispatcherTimer dispatcherTimerChangeBackGround = new DispatcherTimer();

        private float Height = 100;
        private List<int> index_points_to_move = new List<int>();
        private int numPointsX = 100;
        private int numPointsY = 100;

        private Vector3[] points;
        private float Width = 100;

        public delegate void ReportProgressDelegate(int percentage);

        public event ReportProgressDelegate ReportProgressEvent;

        public delegate void ReportCompletedDelegate();

        public event ReportCompletedDelegate ReportCompletedEvent;

        public List<Stream> imagesBackGround = new List<Stream>();
        private int counterImage;
        private MeshGeometry3D model;
        private DiffuseMaterial modelMaterial;
        private MeshGeometry3D modelDeformableArea;
        private LineGeometry3D needleArea;

        public EcoViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            dispatcherTimerAnimation.Interval = TimeSpan.FromMilliseconds(60);
            dispatcherTimerAnimation.Tick += new EventHandler(AnimationTick);
            imagesBackGround.Clear();
            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += worker_DoWork;
            Worker.ProgressChanged += worker_ProgressChanged;
            Worker.RunWorkerCompleted += worker_Completed;
            Worker.RunWorkerAsync();

            counterImage = 0;
            dispatcherTimerChangeBackGround.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimerChangeBackGround.Tick += new EventHandler(ChangeBackgroundTick);
            dispatcherTimerChangeBackGround.Start();
            imagesBackGround.Clear();
        }

        private void ChangeBackgroundTick(object sender, EventArgs e)
        {
            counterImage++;
            if (counterImage >= imagesBackGround.Count)
            {
                counterImage = 0;
            }
            ModelMaterial.DiffuseMap = imagesBackGround[counterImage];
        }

        private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ReportCompletedEvent != null)
            {
                ReportCompletedEvent();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ReportProgressEvent != null)
            {
                ReportProgressEvent(e.ProgressPercentage);
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int globalNumberOfImages = 210;

            string folderName = @"C:\BlockSim2\Cases\_PARAVERT\01";
            int counterGlobal = 0;
            for (int i = 0; i < globalNumberOfImages; i++)
            {
                string fileName = folderName + "\\Im_" + (i + 1).ToString() + ".png";
                imagesBackGround.Add(LoadTexture(fileName));
                counterGlobal++;
                Worker.ReportProgress((int)((float)counterGlobal / (float)globalNumberOfImages * 100f));
            }
        }

        public delegate void ChangedTimerStateDelegate(bool active);

        public event ChangedTimerStateDelegate ChangeTimerStateEvent;

        public bool AutoRun { get; set; } = false;
        public Enum_DeformableAreaType DeformableAreaType { get; private set; } = Enum_DeformableAreaType.LOZANGE_ARC;

        public MeshGeometry3D Model
        {
            get
            {
                return model;
            }
            set
            {
                SetValue(ref model, value, "Model");
            }
        }

        public MeshGeometry3D ModelDeformableArea
        {
            get
            {
                return modelDeformableArea;
            }
            set
            {
                SetValue(ref modelDeformableArea, value, "ModelDeformableArea");
            }
        }

        public LineGeometry3D NeedleArea
        {
            get
            {
                return needleArea;
            }
            set
            {
                SetValue(ref needleArea, value, "NeedleArea");
            }
        }

        public DiffuseMaterial ModelMaterial
        {
            get
            {
                return modelMaterial;
            }
            set
            {
                SetValue(ref modelMaterial, value, "ModelMaterial");
            }
        }

        //public MeshGeometry3D Model { get; private set; }

        //public MeshGeometry3D ModelDeformableArea { get; private set; }

        //public DiffuseMaterial ModelMaterial { private set; get; } = new DiffuseMaterial();

        public DiffuseMaterial ModelMaterialDeformableArea { private set; get; }
        public BackgroundWorker Worker { get; private set; }

        public void SetUpModel(Enum_DeformableAreaType defAreaType, string fileBackground, int height, int width, float ratioDeformUp, float ratioDeformDown, Enum_StartingSurface startingSurface, List<Vector3> pointsStartingSurface, float ratioDeformation)
        {
            ModelMaterial = new DiffuseMaterial();
            if (dispatcherTimerAnimation.IsEnabled)
            {
                StopAnimation();
            }
            var builderDeformableArea = new MeshBuilder(true);
            var builder = new MeshBuilder(true);

            var lineBuilder = new LineBuilder();
            Width = width;
            Height = height; //impostare proporzioni texture

            // camera setup
            Camera = new OrthographicCamera
            {
                Position = new Point3D(0, 0, 1),
                Width = this.Width,
                LookDirection = new Vector3D(0, 0, 1),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 25000
            };

            points = new Vector3[numPointsX * numPointsY];
            for (int i = 0; i < numPointsX; ++i)
            {
                for (int j = 0; j < numPointsY; ++j)
                {
                    points[i * numPointsX + j] = new Vector3(i * Width / numPointsX - Width / 2, j * Height / numPointsY - Height / 2, 0);
                }
            }
            builder.AddRectangularMesh(points, numPointsX);

            Model = builder.ToMesh();
            Model.IsDynamic = true;
            for (int i = 0; i < Model.TextureCoordinates.Count; i++)
            {
                Model.TextureCoordinates[i] = new Vector2(1 - Model.TextureCoordinates[i].Y, 1 - Model.TextureCoordinates[i].X);
            }

            //ModelMaterial.DiffuseMap = LoadTexture("scacchiera.jpg");
            ModelMaterial.DiffuseMap = LoadTexture(fileBackground);

            ModelMaterial.EnableUnLit = true;
            DeformableAreaType = defAreaType;
            switch (DeformableAreaType)
            {
                case Enum_DeformableAreaType.LOZANGE_ARC:
                case Enum_DeformableAreaType.LOZANGE_DOUBLEV:

                    //losanga
                    deformableArea = new LozangeArea();
                    deformableArea.DeformableAreaType = DeformableAreaType;
                    deformableArea.StartingSurface = startingSurface;
                    deformableArea.DefiningPointsSurface = pointsStartingSurface;
                    deformableArea.tAppliedDeformation = ratioDeformation;
                    deformableArea.RatioDeformUp = ratioDeformUp;
                    deformableArea.RatioDeformDown = ratioDeformDown;
                    break;

                case Enum_DeformableAreaType.CIRCULAR_ENLARGEMENT:
                    break;

                default:
                    break;
            }
            index_points_to_move.Clear();
            deformableArea.Create();

            builderDeformableArea.AddRectangularMesh(deformableArea.GiveMeBorderPoints(), DeformableArea.NUMPOINTS_SIDE);
            ModelDeformableArea = builderDeformableArea.ToMesh();
            modelDeformableArea.CalculateNormals();
            ModelDeformableArea.IsDynamic = true;

            lineBuilder.AddLine(new Vector3(100, 100, -3), new Vector3(200, 200, -3));
            NeedleArea = lineBuilder.ToLineGeometry3D();

            ModelMaterialDeformableArea = new DiffuseMaterial();
            ModelMaterialDeformableArea.DiffuseMap = LoadTexture("filler.png");

            UpdatePoints(false);
        }

        public void StartAnimation()
        {
            dispatcherTimerAnimation.Start();
            if (ChangeTimerStateEvent != null)
            {
                ChangeTimerStateEvent(true);
            }
        }

        public void StopAnimation()
        {
            if (dispatcherTimerAnimation.IsEnabled)
            {
                dispatcherTimerAnimation.Stop();
                if (ChangeTimerStateEvent != null)
                {
                    ChangeTimerStateEvent(false);
                }
            }
        }

        public void UpdatePoints(bool increment)
        {
            deformableArea.UpdatePositions();
            deformableArea.UpdateCurves();

            if (increment)
            {
                deformableArea.IncrementEffect();
            }

            if (index_points_to_move.Count == 0)
            {
                for (int i = 0; i < points.Count(); i++)
                {
                    Vector3 vertex = points[i];

                    bool res = deformableArea.GetMovedPoint(vertex, out Vector3 newpoint);

                    if (res)
                    {
                        index_points_to_move.Add(i);
                    }

                    Model.Positions[i] = newpoint;
                }
            }
            else
            {
                foreach (int index in index_points_to_move)
                {
                    Vector3 vertex = points[index];

                    bool res = deformableArea.GetMovedPoint(vertex, out Vector3 newpoint);

                    Model.Positions[index] = newpoint;
                }
            }
            Model.UpdateVertices();
            Vector3[] pointsNew = deformableArea.GiveMeBorderPoints();
            for (int i = 0; i < pointsNew.Length; i++)
            {
                ModelDeformableArea.Positions[i] = pointsNew[i];
            }
            ModelDeformableArea.UpdateVertices();
        }

        private Stream LoadTexture(string file)
        {
            var bytecode = global::SharpDX.IO.NativeFile.ReadAllBytes(file);
            return new MemoryStream(bytecode);
        }

        private Stream LoadTextureCorrected(string file)
        {
            System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fs.CopyTo(ms);
            return fs;
        }

        private void AnimationTick(object sender, EventArgs e)
        {
            UpdatePoints(true);
        }
    }
}