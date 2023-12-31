using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeautySim2023.Windows
{
    /// <summary>
    /// Logica di interazione per ShowImagesWindow.xaml
    /// </summary>
    public partial class ShowImagesWindow : Window
    {
        public ShowImagesWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.ManageViewImagesOn3D();
        }
    }
}
