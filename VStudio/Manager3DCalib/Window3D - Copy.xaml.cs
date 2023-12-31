using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace BeautySimStartingApp
{
    /// <summary>
    /// Logica di interazione per MAIN.xaml
    /// </summary>
    public partial class Window3D : Window
    {
        private List<string> arteriesStrings = new List<string>() { "Arteries" };

        //private HelixToolkit.Wpf.SharpDX.Camera Camera;

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

        //private HelixToolkit.Wpf.SharpDX.Material materialProbeActive = MaterialHelper.CreateMaterial(Colors.Green, .5);
        //private HelixToolkit.Wpf.SharpDX.Material materialProbeInactive = MaterialHelper.CreateMaterial(Colors.Yellow, .5);
        //private HelixToolkit.Wpf.SharpDX.Material materialProbeWrong = MaterialHelper.CreateMaterial(Colors.Red, .5);
        private Model3DGroup modelVeins;

        private List<string> nervesStrings = new List<string>() { "Nerves" };

        private List<string> veinsStrings = new List<string>() { "Veins" };
        public Window3D()
        {
            //Screen screen0 = Screen.AllScreens[0];
            //System.Drawing.Rectangle rStudent;
            //if (Screen.AllScreens.Length > 1)
            //{
            //    Screen screen1 = Screen.AllScreens[1];
            //    rStudent = screen1.WorkingArea;
            //}
            //else
            //{
            //    rStudent = screen0.WorkingArea;
            //}

            //System.Drawing.Rectangle rTeacher = screen0.WorkingArea;
            //this.Top = rTeacher.Top;
            //this.Left = rTeacher.Left;

            InitializeComponent();

            this.Loaded += new RoutedEventHandler(WindowsTeacher_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(WindowsTeacher_Closing);
        }

        public delegate void LoadingModel_Delegate(bool loading);

        public delegate void ManikinLoaded_Delegate();

        private delegate void MethodInvoker();

        public event LoadingModel_Delegate LoadingModel_Event;

        public event ManikinLoaded_Delegate ManikinLoadedEvent;

        public double XAngleProbe { get; set; } = 0;

        public double XPosProbe { get; set; } = 0;

        public double YAngleProbe { get; set; } = 0;

        public double YPosProbe { get; set; } = 0;

        public double ZAngleProbe { get; set; } = 0;

        public double ZPosProbe { get; set; } = 0;

        //public void LoadModels(string folder, bool updateCamera = true)
        //{
        //    try
        //    {
        //        ClearIndexes();
        //        SetLoading(true);
        //        this.UpdateLayout();

        //        //READ THE HEAD
        //        HelixToolkit.Wpf.SharpDX.ObjReader objReaderSkin = new HelixToolkit.Wpf.SharpDX.ObjReader();
        //        HelixToolkit.Wpf.SharpDX.ObjReader objReaderUnderSkin = new HelixToolkit.Wpf.SharpDX.ObjReader();
        //        HelixToolkit.Wpf.SharpDX.ObjReader objReaderArteries = new HelixToolkit.Wpf.SharpDX.ObjReader();
        //        HelixToolkit.Wpf.SharpDX.ObjReader objReaderVeins = new HelixToolkit.Wpf.SharpDX.ObjReader();
        //        HelixToolkit.Wpf.SharpDX.ObjReader objReaderNerves = new HelixToolkit.Wpf.SharpDX.ObjReader();

        //        bool loadSkin = true;
        //        bool loadUnderSkin = false;
        //        bool loadArteries = true;
        //        bool loadVeins = true;
        //        bool loadNerves = false;

        //        // PIRINI
        //        //modelGroupGeneral = new Model3DGroup();

        //        //var models = objReaderSkin.Read(folder + "\\Testa no pelle mirror4.obj");
        //        var models = objReaderSkin.Read(folder + "\\bunny.obj");//\\Testa pelle mirror3.obj");

        //        noSkinHead.Geometry = models[0].Geometry;
        //        noSkinHead.Material = PhongMaterials.Red;
        //        //Geometry = models[0].Geometry;
        //        //Material = PhongMaterials.Red;

        //        //if (loadSkin)
        //        //{
        //        //    string skinPath = folder + "\\Testa pelle mirror3.obj";

        //        //    modelSkin = objReaderSkin.Read(skinPath);
        //        //    for (int i = 0; i < modelSkin.Children.Count; i++)
        //        //    {
        //        //        if (modelSkin.Children[i] is GeometryModel3D)
        //        //        {
        //        //            GeometryModel3D geometryModel = (GeometryModel3D)modelSkin.Children[i];
        //        //            geometryModel.Material = materialSkin;
        //        //            geometryModel.BackMaterial = materialSkin;

        //        //            geometryModel.SetName("Skin");
        //        //            modelGroupGeneral.Children.Add(geometryModel);
        //        //        }
        //        //    }
        //        //}

        //        //if (loadUnderSkin)
        //        //{
        //        //    string underSkinPath = folder + "\\Testa no pelle mirror5.obj";

        //        //    modelUnderSkin = objReaderUnderSkin.Read(underSkinPath);
        //        //    for (int i = 0; i < modelUnderSkin.Children.Count; i++)
        //        //    {
        //        //        if (modelUnderSkin.Children[i] is GeometryModel3D)
        //        //        {
        //        //            GeometryModel3D geometryModel = (GeometryModel3D)modelUnderSkin.Children[i];
        //        //            geometryModel.Material = materialUnderSkin;
        //        //            geometryModel.BackMaterial = materialUnderSkin;

        //        //            geometryModel.SetName("UnderSkin");
        //        //            modelGroupGeneral.Children.Add(geometryModel);
        //        //        }
        //        //    }
        //        //}

        //        //if (loadArteries)
        //        //{
        //        //    string arteriesPath = folder + "\\Vene rosse mirror4.obj";

        //        //    modelArteries = objReaderArteries.Read(arteriesPath);
        //        //    for (int i = 0; i < modelArteries.Children.Count; i++)
        //        //    {
        //        //        if (modelArteries.Children[i] is GeometryModel3D)
        //        //        {
        //        //            GeometryModel3D geometryModel = (GeometryModel3D)modelArteries.Children[i];
        //        //            geometryModel.Material = materialArteries;
        //        //            geometryModel.BackMaterial = materialArteries;

        //        //            geometryModel.SetName("Arteries");
        //        //            modelGroupGeneral.Children.Add(geometryModel);
        //        //        }
        //        //    }
        //        //}

        //        //if (loadVeins)
        //        //{
        //        //    string veinsStrings = folder + "\\Vene blu mirror3.obj";

        //        //    modelVeins = objReaderVeins.Read(veinsStrings);
        //        //    for (int i = 0; i < modelVeins.Children.Count; i++)
        //        //    {
        //        //        if (modelVeins.Children[i] is GeometryModel3D)
        //        //        {
        //        //            GeometryModel3D geometryModel = (GeometryModel3D)modelVeins.Children[i];
        //        //            geometryModel.Material = materialVeins;
        //        //            geometryModel.BackMaterial = materialVeins;

        //        //            geometryModel.SetName("Veins");
        //        //            modelGroupGeneral.Children.Add(geometryModel);
        //        //        }
        //        //    }
        //        //}

        //        //if (loadNerves)
        //        //{
        //        //    string nervesPath = folder + "\\Testa no pelle mirror3.obj";

        //        //    modelNerves = objReaderNerves.Read(nervesPath);
        //        //    for (int i = 0; i < modelNerves.Children.Count; i++)
        //        //    {
        //        //        if (modelNerves.Children[i] is GeometryModel3D)
        //        //        {
        //        //            GeometryModel3D geometryModel = (GeometryModel3D)modelNerves.Children[i];
        //        //            geometryModel.Material = materialNerves;
        //        //            geometryModel.BackMaterial = materialNerves;

        //        //            geometryModel.SetName("Nerves");
        //        //            modelGroupGeneral.Children.Add(geometryModel);
        //        //        }
        //        //    }
        //        //}
        //        //modelGroupGeneral.Freeze();
        //        //modelHumanBlock.Content = modelGroupGeneral;

        //        //READ THE PROBE
        //        //HelixToolkit.Wpf.ObjReader objReader2 = new HelixToolkit.Wpf.ObjReader();

        //        //string probePath= folder + "\\Probe\\MergedProbe.obj";

        //        //modelProbe=objReader2.Read(probePath);
        //        //for (int i = 0; i < modelProbe.Children.Count; i++)
        //        //{
        //        //    if (modelProbe.Children[i] is GeometryModel3D)
        //        //    {
        //        //        GeometryModel3D geometryModel = (GeometryModel3D)modelProbe.Children[i];
        //        //        geometryModel.Material = materialProbeInactive;
        //        //        geometryModel.BackMaterial = materialProbeInactive;
        //        //    }
        //        //}
        //        //modelProbeBlock.Content = modelProbe;

        //        //READ THE FEAMLE BODY
        //        //HelixToolkit.Wpf.ObjReader objReader3 = new HelixToolkit.Wpf.ObjReader();

        //        //{
        //        //    yrotmodel.Angle = 0;
        //        //    Material mat3 = MaterialHelper.CreateMaterial(Colors.Pink, .5);
        //        //    Material mat4 = MaterialHelper.CreateMaterial(Colors.RosyBrown, .1);
        //        //    Material mat5 = MaterialHelper.CreateMaterial(Colors.Brown, .25);

        //        //    modelFemale = objReader3.Read(folder);

        //        //    List<string> objects = new List<string>();

        //        //    for (int i = 0; i < modelFemale.Children.Count; i++)
        //        //    {
        //        //        if (modelFemale.Children[i] is GeometryModel3D)
        //        //        {
        //        //            GeometryModel3D geometryModel = (GeometryModel3D)modelFemale.Children[i];

        //        //            string c = geometryModel.GetName();

        //        //            objects.Add(c);

        //        //            if (IsInsideList(c, femaleStrings))
        //        //            {
        //        //                indexesFemale.Add(i);
        //        //            }
        //        //            else
        //        //            {
        //        //                if (IsInsideList(c, uterusStrings))
        //        //                {
        //        //                    indexesUterus.Add(i);
        //        //                }
        //        //            }

        //        //            if (c.Contains("Body"))
        //        //            {
        //        //                geometryModel.Material = mat3;
        //        //                geometryModel.BackMaterial = mat3;
        //        //            }

        //        //            if (c.Contains("Uterus"))
        //        //            {
        //        //                geometryModel.Material = mat5;
        //        //                geometryModel.BackMaterial = mat5;
        //        //            }
        //        //        }
        //        //    }

        //        //    this.Dispatcher.Invoke(new Action(() =>
        //        //    {
        //        //        spControls.Visibility = Visibility.Collapsed;
        //        //    }));
        //        //}

        //        this.Dispatcher.Invoke(new Action(() =>
        //        {
        //            loadedModel = true;

        //            UpdateVisualizationModel();
        //        }));

        //        SetLoading(false);
        //        if (!firstEvent)
        //        {
        //            if (ManikinLoadedEvent != null)
        //            {
        //                ManikinLoadedEvent();
        //            }
        //            firstEvent = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    //if (updateCamera)
        //    //{
        //    //    UpdateCamera();
        //    //}
        //}

        //public void MoveCameraToTarget(double psxval, double psyval, double pszval, int time)
        //{
        //    this.Dispatcher.Invoke(new Action(() =>
        //    {
        //        hvView3D.LookAt(new Point3D(psxval, psyval, pszval), 1000);
        //    }));
        //}

        //public void SetCursor(int x, int y)
        //{
        //    // Left boundary
        //    var xL = (int)this.Left;
        //    // Top boundary
        //    var yT = (int)this.Top;

        //    SetCursorPos(x + xL, y + yT);
        //}

//        public void UpdateProbePosition(Point3D pointMove)
//        {
//            this.Dispatcher.Invoke(new Action(() =>
//            {
//                try
//                {
//                    //PIRINI
//                    //probePos.OffsetX = pointMove.X;
//                    //probePos.OffsetY = pointMove.Y;
//                    //probePos.OffsetZ = pointMove.Z;
//                }
//                catch (Exception)
//                {
//                }
//            }
//));
//        }

        internal void CenterView()
        {
        }

        internal void HideEverything()
        {
            //spControls.Visibility = Visibility.Collapsed;
        }

        internal void SetProbeActive()
        {
            //this.Dispatcher.Invoke(new Action(() =>
            //{
            //    Model3DCollection m3dc = ((Model3DGroup)modelProbeBlock.Content).Children;
            //    for (int i = 0; i < m3dc.Count; i++)
            //    {
            //        if (m3dc[i] is GeometryModel3D)
            //        {
            //            GeometryModel3D geometryModel = (GeometryModel3D)m3dc[i];

            //            geometryModel.Material = materialProbeActive;
            //            geometryModel.BackMaterial = materialProbeActive;
            //        }
            //    }
            //}));
        }

        internal void ShowControls(bool v)
        {
            //spSystems.Visibility = v ? Visibility.Visible : Visibility.Hidden;
        }

        //internal void UpdateProbeRotation()
        //{
        //    this.Dispatcher.Invoke(new Action(() =>
        //    {
        //        try
        //        {
        //            //PIRINI
        //            //xrot.Angle = XAngleProbe;
        //            //yrot.Angle = YAngleProbe;
        //            //zrot.Angle = ZAngleProbe;
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //    ));
        //}

        //[DllImport("User32.dll")]
        //private static extern bool SetCursorPos(int X, int Y);
        private void BReset_Click(object sender, RoutedEventArgs e)
        {
            //UpdateCamera();
        }

        private void BUpdate_Click(object sender, RoutedEventArgs e)
        {
            //UpdateVisualizationModel();
        }

        private void cbsModified_Checked(object sender, RoutedEventArgs e)
        {
            //UpdateVisualizationModel();
        }

        private void ClearIndexes()
        {
            //indexesMannikin.Clear();
            //indexesProbe.Clear();
        }

        private void HvView3D_Loaded(object sender, RoutedEventArgs e)
        {
            //loadedViewer = true;
            //UpdateVisualizationModel();
            //UpdateCamera();
        }

        //private void hvView3D_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (hvView3D.Camera.LookDirection.Length >= 3000 && e.Delta < 0)
        //    {
        //        e.Handled = true;
        //    }

        //    if (hvView3D.Camera.LookDirection.Length <= 5 && e.Delta > 0)
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private bool IsInsideList(string k, List<string> stringsToCompare)
        //{
        //    foreach (string t in stringsToCompare)
        //    {
        //        if (k.Contains(t))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        private void manikinControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
        }

        private void MeshGeometryModel3D_Mouse3DDown(object sender, MouseDown3DEventArgs e)
        {
        }

        private void PageContainer_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            //if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Refresh)
            //    e.Cancel = true;
        }

        //private void SetLoading(bool v)
        //{
        //    if (LoadingModel_Event != null)
        //    {
        //        LoadingModel_Event(v);
        //    }
        //}

        private void SetManikinType(string folder)
        {
        }

        //private void UpdateCamera()
        //{
        //    hvView3D.Camera.LookDirection = new Vector3D(1, .2, -1);
        //    hvView3D.Camera.UpDirection = new Vector3D(1, 1, 2);

        //    hvView3D.ZoomExtents();
        //}

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void WindowsTeacher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void WindowsTeacher_Loaded(object sender, RoutedEventArgs e)
        {
            //BrushConverter bc = new BrushConverter();
            //LoadModels("C:\\Lavoro\\BeautySim");
        }
    }
}