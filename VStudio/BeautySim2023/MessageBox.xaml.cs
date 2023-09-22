using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public MessageBox()
        {
            InitializeComponent();
            this.Ok.PreviewMouseUp += new MouseButtonEventHandler(Ok_PreviewMouseUp);
            this.Cancel.PreviewMouseUp += new MouseButtonEventHandler(Cancel_PreviewMouseUp);            
            this.Closed += new EventHandler(MessageBox_Closed);
        }

        void Cancel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;
                DialogResult = false;
                this.Close();
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message + ee.StackTrace);
            }
        }

        void Ok_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;
                try
                {
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.Message + ee.StackTrace);
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message + ee.StackTrace);
            }
        }

        void MessageBox_Closed(object sender, EventArgs e)
        {
            if (_timer != null)
                _timer.Stop();
        }

       

        internal bool DefaultValue
        {
            get;
            set;
        }
        internal void SetText(string Text)
        {
            this.messageText.Text = Text;
        }
        internal void HideButtons()
        {
            this.Ok.Visibility = Visibility.Hidden;
        }
        internal void SetYesNo()
        {
            this.Ok.HorizontalAlignment = HorizontalAlignment.Left;           
            this.Cancel.Visibility = Visibility.Visible;
        }
        System.Windows.Threading.DispatcherTimer _timer = null;
        internal void SetTimer(int seconds)
        {
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, seconds);
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            try
            {
                DialogResult = DefaultValue;
                this.Close();
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message + ee.StackTrace);
            }
            
        }
        
        public static bool Show(string Text, bool YesNo, int Seconds)
        {

            return Show(Text,YesNo, Seconds,true);
            

        
        }
      
        public static bool Show(string Text)
        {
           
            return Show(Text,  false, 0);
        }
        public static bool Show(string Text,int Seconds)
        {
            return Show(Text, false, Seconds);
        }
        delegate void MethodInvoker();

        public static bool Show(string Text, bool YesNo, int Seconds,bool default_value)
        {

            //Logger.AddLog(false, "Message box", Text);
            try
            {
                MessageBox message = new MessageBox();
                message.SetText(Text);
                message.DefaultValue = default_value;
                if (YesNo) message.SetYesNo();
                if (Seconds > 0)
                {
                    message.SetTimer(Seconds);
                }
                return (bool)message.ShowDialog();
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message + ee.StackTrace);
                //Logger.AddLog(ee);
                return false;
            }
        }
        static MessageBox _message = null;
        public static void ShowMessage(string Text)
        {
            if (_message != null)
                _message.Close();
            _message = new MessageBox();
            _message.SetText(Text);
            _message.Show();

            
        }
        public static void CloseMessage()
        {
            if (_message != null)
                _message.Close();
            _message = null;
        }
        
    }
}
