using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class CaseStudentFrame : Page
    {
        public CaseStudentFrame()
        {
            InitializeComponent();
            AppControl.Instance.RegisterStudent(this);

            animation = new ColorAnimation();
            animation.From = Colors.Yellow;
            animation.To = Colors.Transparent;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.RepeatBehavior = RepeatBehavior.Forever;
            
        }
        ColorAnimation animation;
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.AdvancePartialAdvanceStep();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bAlarm.Visibility = Visibility.Hidden;
            //this.bAlarm.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
    }
}
