using BeautySim.Common;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class Visualization3DFrame : Page, INotifyPropertyChanged
    {
        public Visualization3DFrame()
        {
            InitializeComponent();

            this.Loaded += ThisPage_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasBeen3DPointsLoaded { get; private set; }

        public Visibility VisibilityAntenna { private set; get; } = Visibility.Hidden;
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

        //    Disposer.RemoveAndDispose(ref modelBody);
        //}

        

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cbSomething_Changed(object sender, RoutedEventArgs e)
        {
            if (AppControl.Instance.HasBeenLoadedVis3DFrame)
            {
                AppControl.Instance.UpdateVisibilityItems(false, sender);
            }
        }

        private void entranceListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (entranceListView.Items.Count > 0)
            {
                entranceListView.ScrollIntoView(entranceListView.Items[entranceListView.Items.Count - 1]);
            }
        }

        private void InjectionPointListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppControl.Instance.ChangeSelectionPointListView();

        }

        private void ThisPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.InitViewModel();
            DataContext = AppControl.Instance;
            AppControl.Instance.HasBeenLoadedVis3DFrame = true;
            AppControl.Instance.UpdateVisibilityItems(true);
            InjectionPointListView.ItemsSource = AppControl.Instance.InjectionPoints3DThisStep;
        }
    }
}