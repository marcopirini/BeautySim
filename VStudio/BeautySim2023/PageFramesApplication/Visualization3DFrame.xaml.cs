using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

//using NEUROWAVE.Data;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class Visualization3DFrame : Page, INotifyPropertyChanged
    {
        private bool hasBeenLoaded = false;

        public Visualization3DFrame()
        {
            InitializeComponent();

            this.Loaded += ThisPage_Loaded;
            this.Unloaded += Visualization3DFrame_Unloaded;
        }

        private void Visualization3DFrame_Unloaded(object sender, RoutedEventArgs e)
        {
            //Utilities.Dispose(ref indicationSphere);

            //Utilities.Dispose(ref hvView3D);

            //Utilities.Dispose(ref indicationSphere2);

            //Utilities.Dispose(ref modelAntenna);

            //RemoveAndDispose(groupProcerus);
            //RemoveAndDispose(groupArteries);
            //RemoveAndDispose(groupVeins);
            //RemoveAndDispose(groupNerves);
            //RemoveAndDispose(groupCorrugators);
            //RemoveAndDispose(groupOrbicularisSuperiorLateral);
            //RemoveAndDispose(groupUnderSkin);
            //RemoveAndDispose(groupSkin);

            //RemoveAndDisposeGroup(modelBody);
            //Utilities.Dispose(ref modelSensor);

            //Utilities.Dispose(ref modelNeedle);

            //this.Dispose();
        }

        //private void RemoveAndDisposeGroup(GroupModel3D modelBody)
        //{
        //    for (int i = 0; i < modelBody.Children.Count(); i++)
        //    {
        //        ItemsModel3D u = (ItemsModel3D)modelBody.Children[i];
        //        Disposer.RemoveAndDispose(ref u);
        //        u = null;
        //    }

        //    Disposer.RemoveAndDispose(ref modelBody);
        //}

        //private void RemoveAndDispose(ItemsModel3D groupProcerus)
        //{
        //    for (int i = 0; i < groupProcerus.Children.Count(); i++)
        //    {
        //        MeshGeometryModel3D u = (MeshGeometryModel3D)groupProcerus.Children[i];
        //        Disposer.RemoveAndDispose(ref u);
        //        u = null;
        //    }

        //    Disposer.RemoveAndDispose(ref groupProcerus);
        //}

        //public void Dispose()
        //{
        //    if (!disposedValue)
        //    {
        //        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //        // TODO: set large fields to null.
        //        if (AppControl.Instance.EffectsManager != null)
        //        {
        //            var effectManager = AppControl.Instance.EffectsManager as IDisposable;
        //            Disposer.RemoveAndDispose(ref effectManager);
        //        }
        //        disposedValue = true;
        //        GC.SuppressFinalize(this);
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;


        public ClinicalCaseStep CurrentClinicalStep { get; private set; }

        public bool HitThrough { set; get; }

        //public float RotationAngleOffsetNeedle
        //{
        //    get { return rotationAngleOffsetNeedle; }
        //    set
        //    {
        //        rotationAngleOffsetNeedle = value;
        //        OnPropertyChanged(nameof(RotationAngleOffsetNeedle));
        //    }
        //}



        public Visibility VisibilityAntenna { private set; get; } = Visibility.Visible;

        public HitTestResultBehavior ResultCallbackNeedle(System.Windows.Media.HitTestResult result)
        {
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
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

        internal void SetContent(ClinicalCaseStep_Face3DInteraction caseStepCurrent)
        {
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private HelixToolkit.Wpf.SharpDX.MeshGeometry3D CreateTruncatedConeGeometry(float baseRadius, float topRadius, float height, int thetaDiv, Vector3 normal, Vector3 origin, bool baseCap)
        {
            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, true);
            builder.AddCone(origin, normal, baseRadius, topRadius, height, baseCap, false, thetaDiv);
            var geometry = builder.ToMeshGeometry3D();
            return geometry;
        }

        private void hvView3D_Loaded_1(object sender, RoutedEventArgs e)
        {
            hvView3D.Camera.LookDirection = new Vector3D(-1, 0, 0);
            hvView3D.Camera.UpDirection = new Vector3D(0, 0, -1);
            hvView3D.ZoomExtents();
        }

        public void InitializeCoordinates()
        {
            var builder = new LineBuilder();
            builder.AddLine(Vector3.Zero, Vector3.UnitX * 5);
            builder.AddLine(Vector3.Zero, Vector3.UnitY * 5);
            builder.AddLine(Vector3.Zero, Vector3.UnitZ * 5);
            AppControl.Instance.Coordinate = builder.ToLineGeometry3D();
            AppControl.Instance.Coordinate.Colors = new Color4Collection(Enumerable.Repeat<Color4>(SharpDX.Color.White, 6));
            AppControl.Instance.Coordinate.Colors[0] = AppControl.Instance.Coordinate.Colors[1] = SharpDX.Color.Red;
            AppControl.Instance.Coordinate.Colors[2] = AppControl.Instance.Coordinate.Colors[3] = SharpDX.Color.Green;
            AppControl.Instance.Coordinate.Colors[4] = AppControl.Instance.Coordinate.Colors[5] = SharpDX.Color.Blue;

            AppControl.Instance.CoordinateText = new BillboardText3D();
            AppControl.Instance.CoordinateText.TextInfo.Add(new TextInfo("X", Vector3.UnitX * 6));
            AppControl.Instance.CoordinateText.TextInfo.Add(new TextInfo("Y", Vector3.UnitY * 6));
            AppControl.Instance.CoordinateText.TextInfo.Add(new TextInfo("Z", Vector3.UnitZ * 6));
        }

        [Obsolete]
        public void InitializeModels(string folder)
        {
            bool showSkinComplete = false;
            bool showSkin = true;
            bool showNoSkin = false;
            bool showArteries = true;
            bool showVeins = true;
            bool showNerves = false;
            bool showProcerus = true;
            bool showOrbicularisSuperiorLateral = true;
            bool showCorrugator = true;

            var sphere = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere.AddSphere(new Vector3(0, 0, 0), 2);
            AppControl.Instance.GeometrySphere = sphere.ToMeshGeometry3D();
            AppControl.Instance.MaterialSphere = DiffuseMaterials.Green;

            var sphere2 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            sphere2.AddSphere(new Vector3(0, 0, 0), 2);
            AppControl.Instance.GeometrySphere2 = sphere2.ToMeshGeometry3D();
            AppControl.Instance.MaterialSphere2 = DiffuseMaterials.Yellow;

            var antenna = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            antenna.AddBox(Vector3.Zero, 54, 54, 54);
            AppControl.Instance.GeometryAntenna = antenna.ToMeshGeometry3D();
            AppControl.Instance.MaterialAntenna = DiffuseMaterials.Gray;

            AppControl.Instance.GeometrySensor = CreateTruncatedConeGeometry(1, 1, 10, 18, new Vector3(1, 0, 0), new Vector3(0, 0, 0), true);
            AppControl.Instance.MaterialSensor = DiffuseMaterials.Black;

            AppControl.Instance.GeometryConnector = CreateTruncatedConeGeometry(3, 3, 15, 18, new Vector3(1, 0, 0), new Vector3(0, 0, 0), true);
            AppControl.Instance.MaterialConnector = DiffuseMaterials.Gray;
            AppControl.Instance.GeometryTConnector = CreateTruncatedConeGeometry(2, 2, 10, 18, new Vector3(0, 1, 0), new Vector3(0, 0, 0), true);

            AppControl.Instance.GeometryNeedle = CreateTruncatedConeGeometry(0.4f, 0.1f, 50, 18, new Vector3(1, 0, 0), new Vector3(15, 0, 0), true);
            AppControl.Instance.TipNeedleOrigin = new Point3D(65, 0, 0);
            AppControl.Instance.BaseNeedleOrigin = new Point3D(0, 0, 0);
            AppControl.Instance.MaterialNeedle = DiffuseMaterials.Blue;

            AppControl.Instance.GeometryCable = CreateTruncatedConeGeometry(0.5f, 0.5f, -50, 18, new Vector3(1, 0, 0), new Vector3(0, 0, 0), true);
            AppControl.Instance.MaterialCable = DiffuseMaterials.Yellow;

            if (showSkin)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var models = reader.Read(folder + "\\Testa pelle mirror3.obj");
                AppControl.Instance.CollisionItemsGuid.Add("SKIN", new List<Guid>());
                for (int i = 0; i < models.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = models[i].Geometry;
                    model.Material = DiffuseMaterials.Gray;
                    //model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    groupSkin.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
                }
            }

            if (showSkinComplete)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var models = reader.Read(folder + "\\Pelle combinata5.obj");
                AppControl.Instance.CollisionItemsGuid.Add("SKIN", new List<Guid>());
                for (int i = 0; i < models.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = models[i].Geometry;
                    model.Material = DiffuseMaterials.Gray;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    groupSkin.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["SKIN"].Add(model.Geometry.GUID);
                }
            }

            if (showProcerus)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var reader = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelProcerus = reader.Read(folder + "\\ProcerusSmoothed.obj");
                AppControl.Instance.CollisionItemsGuid.Add("PROCERUS", new List<Guid>());
                for (int i = 0; i < modelProcerus.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelProcerus[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    groupProcerus.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["PROCERUS"].Add(model.Geometry.GUID);
                }
            }

            if (showOrbicularisSuperiorLateral)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var readerOrbicularisSuperiorLateral = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelOrbicularisSuperiorLateral = readerOrbicularisSuperiorLateral.Read(folder + "\\OrbicularisSuperiorLateralSmoothed.obj");
                AppControl.Instance.CollisionItemsGuid.Add("ORBICULARIS", new List<Guid>());
                for (int i = 0; i < modelOrbicularisSuperiorLateral.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelOrbicularisSuperiorLateral[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    groupOrbicularisSuperiorLateral.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["ORBICULARIS"].Add(model.Geometry.GUID);
                }
            }
            if (showCorrugator)
            {
                var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                builder.AddBox(Vector3.Zero, 1, 1, 1);
                var readerCorrugator = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelCorrugators = readerCorrugator.Read(folder + "\\CorrugatorSmoothed.obj");
                AppControl.Instance.CollisionItemsGuid.Add("CORRUGATOR", new List<Guid>());
                for (int i = 0; i < modelCorrugators.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelCorrugators[i].Geometry;
                    model.Material = DiffuseMaterials.Orange;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    groupCorrugators.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["CORRUGATOR"].Add(model.Geometry.GUID);
                }
            }

            if (showArteries)
            {
                var readerArteries = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelsArteries = readerArteries.Read(folder + "\\Arteries_Cleaned.obj");
                AppControl.Instance.CollisionItemsGuid.Add("ARTERIES", new List<Guid>());
                for (int i = 0; i < modelsArteries.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsArteries[i].Geometry;
                    model.Material = DiffuseMaterials.Red;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;

                    groupArteries.Children.Add(model);
                    AppControl.Instance.CollisionItemsGuid["ARTERIES"].Add(model.Geometry.GUID);
                }
            }

            if (showVeins)
            {
                var readerVeins = new HelixToolkit.Wpf.SharpDX.ObjReader();
                var modelsVeins = readerVeins.Read(folder + "\\Veins_Cleaned.obj");
                AppControl.Instance.CollisionItemsGuid.Add("VEINS", new List<Guid>());
                for (int i = 0; i < modelsVeins.Count(); i++)
                {
                    var model = new MeshGeometryModel3D();
                    model.Geometry = modelsVeins[i].Geometry;
                    model.Material = DiffuseMaterials.Blue;
                    model.FillMode = SharpDX.Direct3D11.FillMode.Wireframe;
                    AppControl.Instance.CollisionItemsGuid["VEINS"].Add(model.Geometry.GUID);
                    groupVeins.Children.Add(model);
                }
            }

            AppControl.Instance.StringSkinAreas.Add("ORBICULARIS");
            AppControl.Instance.StringSkinAreas.Add("PROCERUS");
            AppControl.Instance.StringSkinAreas.Add("CORRUGATOR");
        }

        private void ThisPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.InitViewModel();
            DataContext = AppControl.Instance;
            hasBeenLoaded = true;
            UpdateVisibilityItems();
        }
        private void entranceListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (entranceListView.Items.Count > 0)
            {
                entranceListView.ScrollIntoView(entranceListView.Items[entranceListView.Items.Count - 1]);
            }
        }
        private void TimerRotation_Tick1(object sender, EventArgs e)
        {
            AppControl.Instance.rotationTimer += 10;
        }

        private void hvView3D_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void cbSomething_Changed(object sender, RoutedEventArgs e)
        {
            if (hasBeenLoaded)
            {
                UpdateVisibilityItems();

            }
        }

        private void UpdateVisibilityItems()
        {
            AppControl.Instance.ProcerusVisible = cbProcerus.IsChecked.Value;
            AppControl.Instance.CorrugatorVisible = cbCorrugator.IsChecked.Value;
            AppControl.Instance.OrbicularisVisible = cbOrbicularis.IsChecked.Value;
            AppControl.Instance.VeinsVisible = cbVeins.IsChecked.Value;
            AppControl.Instance.ArteriesVisible = cbArteries.IsChecked.Value;
            AppControl.Instance.SkinVisible = cbSkin.IsChecked.Value;
        }
    }
}