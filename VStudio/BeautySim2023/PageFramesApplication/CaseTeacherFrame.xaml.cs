using BeautySim2023.Windows;
using System;
using System.IO;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class CaseTeacherFrame : Page
    {
        public bool CurrentLocateMouseSOrT { get; private set; }

        public CaseTeacherFrame()
        {
            InitializeComponent();
            AppControl.Instance.RegisterTeacher(this);
            //listItems.ItemsSource = CaseController.Instance.SelectedCase.Steps;
            AppControl.Instance.WindowTeacher.bBack.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.bClose.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Collapsed;

            AppControl.Instance.AreImagesVisualizedOn3D = false;
        }

         private void bNextStep_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.CloseWindowImages();
            AppControl.Instance.AdvanceStep();
        }

        private void bLocateMouse_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.SwitchMousePosition(this);
            
        }

        private void bZeroVolume_Click(object sender, RoutedEventArgs e)
        {
            //AppControl.Instance.ZeroVolume();
        }

        private void bFlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            //AppControl.Instance.FlipHorizontal();
        }

        private void bFlipVertical_Click(object sender, RoutedEventArgs e)
        {
            //AppControl.Instance.FlipVertical();
        }

        private void bOffsetPosition_Click(object sender, RoutedEventArgs e)
        {
            //AppControl.Instance.OffsetPositions();
            //AppControl.Instance.OffsetPositions();
        }

        private void bInsertAnaethetic_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.InjectAnesthetic();
        }

        private void bViewImages_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.ManageViewImagesOn3D();

        }

        private void bViewPoints_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.ManageViewPointsOn3D(!AppControl.Instance.ArePointsVisualizedOn3D);
        }
    }
}
