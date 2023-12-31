using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

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
            tbNeedleLength.Text = Properties.Settings.Default.LengthNeedle.ToString("00.00", new CultureInfo("en-US"));
            tbXTrasl.Text = Properties.Settings.Default.TranslationX.ToString("00.00", new CultureInfo("en-US"));
            tbYTrasl.Text = Properties.Settings.Default.TranslationY.ToString("00.00", new CultureInfo("en-US"));
            tbZTrasl.Text = Properties.Settings.Default.TranslationZ.ToString("00.00", new CultureInfo("en-US"));
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            //if (float.TryParse(tbNeedleLength.Text, out float converted))
            //{
            //    if ((converted >= 20) && (converted <= 300))
            //    {
            //        Properties.Settings.Default.LengthNeedle = converted;
            //        Properties.Settings.Default.Save();
            //        AppControl.Instance.LengthNeedle = converted;
            //    }
            //    else
            //    {
            //        tbNeedleLength.Text = Properties.Settings.Default.LengthNeedle.ToString("00.00", new CultureInfo("en-US"));
            //    }
            //}
            //else
            //{
            //    tbNeedleLength.Text = Properties.Settings.Default.LengthNeedle.ToString("00.00", new CultureInfo("en-US"));
            //}

            //if (float.TryParse(tbXTrasl.Text, out float convertedX))
            //{
            //    if ((convertedX >= -200) && (convertedX <= 200))
            //    {
            //        Properties.Settings.Default.TranslationX = convertedX;
            //        Properties.Settings.Default.Save();
            //    }
            //    else
            //    {
            //        tbXTrasl.Text = Properties.Settings.Default.TranslationX.ToString("00.00", new CultureInfo("en-US"));
            //    }
            //}
            //else
            //{
            //    tbXTrasl.Text = Properties.Settings.Default.TranslationX.ToString("00.00", new CultureInfo("en-US"));
            //}

            //if (float.TryParse(tbYTrasl.Text, out float convertedY))
            //{
            //    if ((convertedY >= -200) && (convertedY <= 200))
            //    {
            //        Properties.Settings.Default.TranslationY = convertedY;
            //        Properties.Settings.Default.Save();
            //    }
            //    else
            //    {
            //        tbYTrasl.Text = Properties.Settings.Default.TranslationY.ToString("00.00", new CultureInfo("en-US"));
            //    }
            //}
            //else
            //{
            //    tbYTrasl.Text = Properties.Settings.Default.TranslationY.ToString("00.00", new CultureInfo("en-US"));
            //}

            //if (float.TryParse(tbZTrasl.Text, out float convertedZ))
            //{
            //    if ((convertedZ >= -200) && (convertedZ <= 200))
            //    {
            //        Properties.Settings.Default.TranslationZ = convertedZ;
            //        Properties.Settings.Default.Save();
            //    }
            //    else
            //    {
            //        tbZTrasl.Text = Properties.Settings.Default.TranslationZ.ToString("00.00", new CultureInfo("en-US"));
            //    }
            //}
            //else
            //{
            //    tbZTrasl.Text = Properties.Settings.Default.TranslationZ.ToString("00.00", new CultureInfo("en-US"));
            //}

            //AppControl.Instance.TranslationPointModel= new Point3D(Properties.Settings.Default.TranslationX, Properties.Settings.Default.TranslationY, Properties.Settings.Default.TranslationZ);
        }
    }
}