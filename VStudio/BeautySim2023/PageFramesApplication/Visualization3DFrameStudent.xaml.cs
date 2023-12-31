using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

//using NEUROWAVE.Data;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class Visualization3DFrameStudent : Page, INotifyPropertyChanged
    {
        public Visualization3DFrameStudent()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = AppControl.Instance;
        }
    }
}