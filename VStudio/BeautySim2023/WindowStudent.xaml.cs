using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per WindowStudent.xaml
    /// </summary>
    public partial class WindowStudent : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);

        public WindowStudent()
        {
            InitializeComponent();
        }

        private delegate void SetImageDelegate(System.Windows.Controls.Image lbl, BitmapSource c);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        public void Navigate(Page page)
        {
            Dispatcher.BeginInvoke(new MethodInvoker(() =>
            {
                this.PageContainer.Content = page;
                //if (page is StartPageFrame)
                //{
                //}
            }));
        }
    }
}