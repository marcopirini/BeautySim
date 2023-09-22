using BeautySim.Globalization;
using System.Windows;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Language.Culture = new System.Globalization.CultureInfo(BeautySim2023.Properties.Settings.Default.Language);
        }
    }
}
