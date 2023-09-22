using System.Windows;
using System.Windows.Controls;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class SettingsFrame : Page
    {
        public SettingsFrame()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbNeedleLength.Text = Properties.Settings.Default.DistanceSensorNeedleTip.ToString("00.00");
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(tbNeedleLength.Text, out float converted))
            {
                if ((converted >= 80) && (converted <= 300))
                {
                    Properties.Settings.Default.DistanceSensorNeedleTip = converted;
                    Properties.Settings.Default.Save();
                    AppControl.Instance.PhysicalLengthNeedle = converted;
                }
                else
                {
                    tbNeedleLength.Text = Properties.Settings.Default.DistanceSensorNeedleTip.ToString("00.00");
                }
            }
            else
            {
                tbNeedleLength.Text = Properties.Settings.Default.DistanceSensorNeedleTip.ToString("00.00");
            }
        }
    }
}