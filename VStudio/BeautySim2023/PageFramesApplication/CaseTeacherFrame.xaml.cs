using System.Windows;
using System.Windows.Controls;

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

        }

         private void bNextStep_Click(object sender, RoutedEventArgs e)
        {
            //CaseController.Instance.AdvanceStep();
        }

        private void bLocateMouse_Click(object sender, RoutedEventArgs e)
        {
            //AppControl.Instance.SwitchMousePosition(this);
            
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


    }
}
