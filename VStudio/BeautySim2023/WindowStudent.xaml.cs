using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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


        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public void SetCursor(int x, int y)
        {
            // Left boundary
            var xL = (int)this.Left;
            // Top boundary
            var yT = (int)this.Top;

            SetCursorPos(x + xL, y + yT);
        }

        public WindowStudent()
        {
            InitializeComponent();
            this.KeyDown += WindowStudent_KeyDown;
        }

        private void WindowStudent_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key== System.Windows.Input.Key.M)
            {
                AppControl.Instance.InjectAnesthetic();
            }
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