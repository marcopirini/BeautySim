using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace BeautySimStartingApp
{
    public class MainWindowViewModel : DemoCore.BaseViewModel, INotifyPropertyChanged
    {
        private List<Object3D> modelsVeins;
        private List<Object3D> modelsArteries;
        private DateTime lastMouseMove;
        private bool mouseClickOrMove = false;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HelixToolkit.Wpf.SharpDX.Geometry3D GeometryHeadSkin { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Geometry3D RectGeometry { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Material Material { private set; get; }

        public HelixToolkit.Wpf.SharpDX.Material MaterialSphere { private set; get; }
        public HelixToolkit.Wpf.SharpDX.Material ViewCubeMaterial2 { private set; get; }

        public HelixToolkit.Wpf.SharpDX.Geometry3D Sphere { private set; get; }

        public LineGeometry3D Coordinate { private set; get; }
        public BillboardText3D CoordinateText { private set; get; }
       


        private string hitStructure;
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

        public System.Windows.Media.Media3D.Transform3D ViewCubeTransform3 { private set; get; }

        public MainWindowViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera()
            {
                Position = new System.Windows.Media.Media3D.Point3D(0, 0, 10),
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, -10),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0)
            };

            InitializeModels("C:\\Lavoro\\BeautySim");
            InitializeViewCubes();
            InitializeCoordinates();
        }

        public ObservableElement3DCollection GroupModelSourceArteries { private set; get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection GroupModelSourceVeins { private set; get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection GroupModelSourceNerves { private set; get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection GroupModelSourceUnderSkin { private set; get; } = new ObservableElement3DCollection();
        public bool HitThrough { set; get; }

        public void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mouseClickOrMove)
            {
                //foreach (var item in HighlightItems)
                //{
                //    item.Highlight = false;
                //}
                //HighlightItems.Clear();
                //Material = PhongMaterials.White;
                var viewport = sender as Viewport3DX;
                if (viewport == null) { return; }
                var point = e.GetPosition(viewport);
                var hitTests = viewport.FindHits(point);
                if (hitTests != null && hitTests.Count > 0)
                {
                    //if (HitThrough)
                    //{
                    foreach (var hit in hitTests)
                    {
                        if (hit.Geometry == GeometryHeadSkin)
                        {
                            Debug.WriteLine("SKIN");
                        }

                        for (int i = 0; i < modelsVeins.Count; i++)
                        {
                            if (modelsVeins[i].Geometry == hit.Geometry)
                            {
                                Debug.WriteLine("VEIN");
                            }
                        }

                        for (int i = 0; i < modelsArteries.Count; i++)
                        {
                            if (modelsArteries[i].Geometry == hit.Geometry)
                            {
                                Debug.WriteLine("ARTERY");
                            }
                        }
                    }
                }
            }
        }

        public Vector3D UpDirection { set; get; } = new Vector3D(0, 0, 1);

        //if ((hit.ModelHit as Element3D).DataContext is DataModel)
        //{
        //    var model = (hit.ModelHit as Element3D).DataContext as DataModel;
        //    model.Highlight = true;
        //    HighlightItems.Add(model);
        //}
        //else if ((hit.ModelHit as Element3D).DataContext == this)
        //{
        //    if (hit.TriangleIndices != null)
        //    {
        //        Material = PhongMaterials.Yellow;
        //    }
        //    else
        //    {
        //        var v = new Vector3Collection();
        //        v.Add(hit.PointHit);
        //        PointsHitModel.Positions = v;
        //        var idx = new IntCollection();
        //        idx.Add(0);
        //        PointsHitModel = new PointGeometry3D() { Positions = v, Indices = idx };
        //    }
        //}
        //}
        //}
        //else
        //{
        //var hit = hitTests[0];
        //if (hit.ModelHit is Element3D elem)
        //{
        //    if (elem.DataContext is DataModel)
        //    {
        //        var model = elem.DataContext as DataModel;
        //        model.Highlight = true;
        //        HighlightItems.Add(model);
        //    }
        //    else if (elem.DataContext == this)
        //    {
        //        if (hit.TriangleIndices != null)
        //        {
        //            Material = PhongMaterials.Yellow;
        //        }
        //        else
        //        {
        //            var v = new Vector3Collection();
        //            v.Add(hit.PointHit);
        //            PointsHitModel.Positions = v;
        //            var idx = new IntCollection();
        //            idx.Add(0);
        //            PointsHitModel = new PointGeometry3D() { Positions = v, Indices = idx };
        //        }
        //    }
        //}
        //        }
        //    }
        //}

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
                        var hitTests = viewport.FindHits(point);
                        if (hitTests != null && hitTests.Count > 0)
                        {
                            //if (HitThrough)
                            //{
                            foreach (var hit in hitTests)
                            {
                                if (hit.Geometry == GeometryHeadSkin)
                                {
                                    pointsHit.Add(new PointHit(hit.PointHit, "SKIN"));
                                    //Debug.WriteLine("SKIN");
                                }

                                for (int i = 0; i < modelsVeins.Count; i++)
                                {
                                    if (modelsVeins[i].Geometry == hit.Geometry)
                                    {
                                        pointsHit.Add(new PointHit(hit.PointHit, "VEIN"));
                                        //Debug.WriteLine("VEIN");
                                    }
                                }

                                for (int i = 0; i < modelsArteries.Count; i++)
                                {
                                    if (modelsArteries[i].Geometry == hit.Geometry)
                                    {
                                        pointsHit.Add(new PointHit(hit.PointHit, "ARTERY"));
                                        //Debug.WriteLine("ARTERY");
                                    }
                                }
                            }
                        }
                        double minDistance = double.MaxValue;
                        int indexMin = -1;
                        if (pointsHit.Count > 0)
                        {
                            for (int i=0; i<pointsHit.Count; i++)
                            {
                                var distance = CalcDistance(pointsHit[i].Point, cameraPosition);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    indexMin = i;
                                }
                            }
                            
                            SphereTransform.OffsetX = pointsHit[indexMin].Point.X;
                            SphereTransform.OffsetY = pointsHit[indexMin].Point.Y;
                            SphereTransform.OffsetZ = pointsHit[indexMin].Point.Z;

                            OnPropertyChanged(nameof(SphereTransform));
                            HitStructure = pointsHit[indexMin].Name;
                            Debug.WriteLine(HitStructure);
                        }
                        else
                        {
                            HitStructure = "None";
                            
                        }
                        
                        lastMouseMove = DateTime.Now;
                        Debug.WriteLine((DateTime.Now - c).TotalMilliseconds.ToString());
                    }
                }
            }


            
            //foreach (var item in HighlightItems)
            //{
            //    item.Highlight = false;
            //}
            //HighlightItems.Clear();
        }

        private double CalcDistance(Point3D point1, Point3D point2)
        {
            double deltaX = point1.X - point2.X;
            double deltaY = point1.Y - point2.Y;
            double deltaZ = point1.Z - point2.Z;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }
        public TranslateTransform3D SphereTransform { private set; get; } = new System.Windows.Media.Media3D.TranslateTransform3D(5, 0, 0);
        

        private void InitializeModels(string folder)
        {
            bool showSkin = true;
            bool showNoSkin = false;
            bool showArteries = true;
            bool showVeins = true;
            bool showNerves = false;

            var sphere = new MeshBuilder();
            sphere.AddSphere(new Vector3(0, 0, 0), 2);
            Sphere = sphere.ToMeshGeometry3D();
            MaterialSphere = DiffuseMaterials.Green;

            if (showSkin)
            {
                var builder = new MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var reader = new ObjReader();
                var models = reader.Read(folder + "\\Testa pelle mirror3.obj");
                GeometryHeadSkin = models[0].Geometry;
                Material = DiffuseMaterials.Gray;

                builder = new MeshBuilder();
                builder.AddBox(new Vector3(0, 0, -4), 2, 2, 6);
                RectGeometry = builder.ToMeshGeometry3D();
            }

            if (showArteries)
            {
                GroupModelSourceArteries = new ObservableElement3DCollection();
                var readerArteries = new ObjReader();
                modelsArteries = readerArteries.Read(folder + "\\Arteries_Cleaned.obj");
                for (int i = 0; i < modelsArteries.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsArteries[i].Geometry;
                    model.Material = DiffuseMaterials.Red;

                    GroupModelSourceArteries.Add(model);
                }
            }

            if (showVeins)
            {
                GroupModelSourceVeins = new ObservableElement3DCollection();
                var readerVeins = new ObjReader();
                modelsVeins = readerVeins.Read(folder + "\\Veins_Cleaned.obj");
                for (int i = 0; i < modelsVeins.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsVeins[i].Geometry;
                    model.Material = DiffuseMaterials.Blue;

                    GroupModelSourceVeins.Add(model);
                }
            }

            //GroupModelSourceNerves = new ObservableElement3DCollection();
            //var readerNerves = new ObjReader();
            //var modelsNerves = readerNerves.Read(folder + "\\Vene blu mirror3.obj");
            //for (int i = 0; i < modelsNerves.Count(); i++)
            //{
            //    var model = new MeshGeometryModel3D();
            //    model.Geometry = modelsVeins[i].Geometry;
            //    model.Material = DiffuseMaterials.Yellow;

            //    GroupModelSourceNerves.Add(model);
            //}

            if (showNoSkin)
            {
                GroupModelSourceUnderSkin = new ObservableElement3DCollection();
                var readerUnderSkin = new ObjReader();
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

        private void InitializeCoordinates()
        {
            var builder = new LineBuilder();
            builder.AddLine(Vector3.Zero, Vector3.UnitX * 5);
            builder.AddLine(Vector3.Zero, Vector3.UnitY * 5);
            builder.AddLine(Vector3.Zero, Vector3.UnitZ * 5);
            Coordinate = builder.ToLineGeometry3D();
            Coordinate.Colors = new Color4Collection(Enumerable.Repeat<Color4>(Color.White, 6));
            Coordinate.Colors[0] = Coordinate.Colors[1] = Color.Red;
            Coordinate.Colors[2] = Coordinate.Colors[3] = Color.Green;
            Coordinate.Colors[4] = Coordinate.Colors[5] = Color.Blue;

            CoordinateText = new BillboardText3D();
            CoordinateText.TextInfo.Add(new TextInfo("X", Vector3.UnitX * 6));
            CoordinateText.TextInfo.Add(new TextInfo("Y", Vector3.UnitY * 6));
            CoordinateText.TextInfo.Add(new TextInfo("Z", Vector3.UnitZ * 6));
        }
    }
}