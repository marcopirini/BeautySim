
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per WindowTeacher.xaml
    /// </summary>
    public partial class WindowTeacher : Window
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;


        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public WindowTeacher()
        {
            InitializeComponent();

            try
            {
                tbDebugMode.Visibility = Properties.Settings.Default.DebugMode ? Visibility.Visible : Visibility.Hidden;
               
                AppControl.Instance.WindowTeacher = this;
                AppControl.Instance.Init();
                //LANGUAGES
                foreach (Enum_Languages language in (Enum_Languages[])Enum.GetValues(typeof(Enum_Languages)))
                {
                    InitLanguageComboBox(cbSelectLanguage, GetLanguageFlagIconUrl(language), GetLanguageName(language), GetLanguageCode(language));
                }

                AppControl.Instance.WindowStudent = new WindowStudent();

                Screen[] listScreens = Screen.AllScreens;
                Screen screen0 = Screen.AllScreens[0];
                Screen screen1 = Screen.AllScreens[0];
                if (Screen.AllScreens.Length == 1)
                {
                    screen0 = Screen.AllScreens[0];
                    screen1 = Screen.AllScreens[0];
                }
                else
                {
                    if (Screen.AllScreens[0].Primary)
                    {
                        screen0 = Screen.AllScreens[0];
                        screen1 = Screen.AllScreens[1];
                    }
                    else
                    {
                        screen1 = Screen.AllScreens[0];
                        screen0 = Screen.AllScreens[1];
                    }
                }
                System.Drawing.Rectangle rTeacher = screen0.WorkingArea;
                System.Drawing.Rectangle rStudent = screen1.WorkingArea;
                //Screen screen0 = Screen.AllScreens[0];
                
                //if (Screen.AllScreens.Length > 1)
                //{
                //    Screen screen1 = Screen.AllScreens[1];
                //    rStudent = screen1.WorkingArea;
                //}
                //else
                //{
                //    rStudent = screen0.WorkingArea;
                //}

                
                AppControl.Instance.WindowTeacher.Top = rTeacher.Top;
                AppControl.Instance.WindowTeacher.Left = rTeacher.Left;
                AppControl.Instance.WindowStudent.Top = rStudent.Top;
                AppControl.Instance.WindowStudent.Left = rStudent.Left;
                AppControl.Instance.WindowStudent.Width = rStudent.Width;
                AppControl.Instance.WindowStudent.Height = rStudent.Height;

                AppControl.Instance.WindowStudent.DataContext = AppControl.Instance;
                AppControl.Instance.WindowStudent.Loaded += AppControl.Instance.WindowStudentLoaded;
                AppControl.Instance.ChangeTimerStateEvent += new AppControl.ChangedTimerStateDelegate(ChangeTimerStateEvent_Listener);
                this.PageContainer.Navigating += new System.Windows.Navigation.NavigatingCancelEventHandler(PageContainer_Navigating);
                this.DataContext = AppControl.Instance;
                //AppControl.Instance.Points.Add(new System.Windows.Media.Media3D.Point3D(0, 0, 0));


                AppControl.Instance.WindowStudent.Show();
                AppControl.Instance.WindowStudent.Navigate(new MessageVoid());
            }
            catch (Exception ex)
            {
                MessageBox.Show(BeautySim.Globalization.Language.str_inst_err);
                App.Current.Shutdown();
            }
        }



        private void PageContainer_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Refresh)
                e.Cancel = true;
        }

        private void ChangeTimerStateEvent_Listener(bool active)
        {
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if ((AppControl.Instance.WindowTeacher.PageContainer.Content is UsersManagerFrame) || (AppControl.Instance.WindowTeacher.PageContainer.Content is EventsManagerFrame) || (AppControl.Instance.WindowTeacher.PageContainer.Content is ResultsManagerFrame) || (AppControl.Instance.WindowTeacher.PageContainer.Content is SettingsFrame))
            {
                AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
            }
            if ((AppControl.Instance.WindowTeacher.PageContainer.Content is FunctionalitiesFrame))
            {
                AppControl.Instance.WindowTeacher.Navigate(new StartPageFrame());
            }
            if ((AppControl.Instance.WindowTeacher.PageContainer.Content is CaseTeacherFrame))
            {
                if (AppControl.Instance.CloseCurrentCase())
                {
                    AppControl.Instance.WindowStudent.Navigate(new MessageVoid());
                    AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
                    AppControl.Instance.SetText(AppControl.Instance.WindowTeacher.lCase, "");
                }
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.LogOut();
            if (AppControl.Instance.WindowTeacher.PageContainer.Content is StartPageFrame)
            {
                ((StartPageFrame)AppControl.Instance.WindowTeacher.PageContainer.Content).PreparePage("Login");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.CloseApp();
        }

        private delegate void MethodInvoker();

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


        private void Window_Closed(object sender, EventArgs e)
        {
            //if (DataContext is IDisposable)
            //{
            //    (DataContext as IDisposable).Dispose();
            //}
            //System.Windows.Application.Current.ShutdownMode=ShutdownMode.OnExplicitShutdown;
            //System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
            //try
            //{

            //    AppControl.Instance.DisconnectBeautySim();
            //    AppControl.Instance.DisconnectPDI();
            //}
            //catch (Exception ex)
            //{
            //}
            //AppControl.Instance.WindowStudent.Close();
            //AppControl.Instance.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            AppControl.Instance.InitializeCurrentTeacherStudentEvent();
            Version version = assembly.GetName().Version;
            this.Navigate(new StartPageFrame());

            var hwnd2 = new WindowInteropHelper(this).Handle;
            WindowTeacher.SetWindowLong(hwnd2, WindowTeacher.GWL_STYLE, WindowTeacher.GetWindowLong(hwnd2, WindowTeacher.GWL_STYLE) & ~WindowTeacher.WS_SYSMENU);

            //WindowTeacher.lVersion.Content = "v. " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + ".0";
            //SetWindowLong(hwnd2, GWL_STYLE, GetWindowLong(hwnd2, GWL_STYLE) & (0xFFFF ^ WS_SYSMENU));

            AppControl.Instance.LoadedCase = false;


            //PIRINI
            AppControl.Instance.ConnectPDI();
            AppControl.Instance.ConnectBeautySim();
            AppControl.Instance.WindowTeacher.Navigate(new StartPageFrame());
        }

        internal void Zero()
        {
        
        }

        private void InitLanguageComboBox(System.Windows.Controls.ComboBox comboBox, string itemImageSource, string itemText, string itemTag)
        {
            ComboBoxItem item = new ComboBoxItem();
            DockPanel dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(20, 0, -10, 0);
            dockPanel.Width = comboBox.Width - 20;
            Image icon = new Image();
            icon.Height = icon.Width = 25;
            BitmapImage flag = new BitmapImage();
            flag.BeginInit();
            flag.UriSource = new Uri(itemImageSource);
            flag.EndInit();
            icon.Source = flag;
            icon.Margin = new Thickness(0, 0, 2, 0);
            dockPanel.Children.Add(icon);
            TextBlock tBlock = new TextBlock();
            tBlock.FontSize = 16;
            tBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            tBlock.VerticalAlignment = VerticalAlignment.Center;
            tBlock.Text = itemText;
            dockPanel.Children.Add(tBlock);
            item.Content = dockPanel;
            item.VerticalContentAlignment = VerticalAlignment.Center;
            item.Tag = itemTag;
            comboBox.Items.Add(item);

            comboBox.SelectedIndex = (int)GetLanguageFromCode(Properties.Settings.Default.Language);
        }

        private string GetLanguageName(Enum_Languages language)
        {
            switch (language)
            {
                case Enum_Languages.English:
                    return BeautySim.Globalization.Language.str_english;
                case Enum_Languages.Italian:
                    return BeautySim.Globalization.Language.str_italian;
                case Enum_Languages.Chinese:
                    return BeautySim.Globalization.Language.str_chinese;
                case Enum_Languages.French:
                    return BeautySim.Globalization.Language.str_french;
                default:
                    return BeautySim.Globalization.Language.str_english;
            }
        }

        private string GetLanguageCode(Enum_Languages language)
        {
            switch (language)
            {
                case Enum_Languages.English:
                    return "en-EN";
                case Enum_Languages.Italian:
                    return "it-IT";
                case Enum_Languages.Chinese:
                    return "zh-CN";
                case Enum_Languages.French:
                    return "fr-FR";
                default:
                    return "en-EN";
            }
        }

        private string GetLanguageFlagIconUrl(Enum_Languages language)
        {
            switch (language)
            {
                case Enum_Languages.English:
                    return "pack://application:,,,/BeautySim2023;component/Images/en-EN.png";
                case Enum_Languages.Italian:
                    return "pack://application:,,,/BeautySim2023;component/Images/it-IT.png";
                case Enum_Languages.Chinese:
                    return "pack://application:,,,/BeautySim2023;component/Images/zh-CN.png";
                case Enum_Languages.French:
                    return "pack://application:,,,/BeautySim2023;component/Images/fr-FR.png";
                default:
                    return "pack://application:,,,/BeautySim2023;component/Images/en-EN.png";
            }
        }

        private Enum_Languages GetLanguageFromCode(string code)
        {
            switch (code)
            {
                case "en-EN":
                    return Enum_Languages.English;
                case "it-IT":
                    return Enum_Languages.Italian;
                case "zh-CN":
                    return Enum_Languages.Chinese;
                case "fr-FR":
                    return Enum_Languages.French;
                default:
                    return Enum_Languages.English;
            }
        }

        private void CbSelectLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLanguage = GetLanguageCode((Enum_Languages)cbSelectLanguage.SelectedIndex);
            if (selectedLanguage != Properties.Settings.Default.Language)
            {
                if (MessageBox.Show(BeautySim.Globalization.Language.str_change_lang, true, 1000, false))
                {
                    Properties.Settings.Default.Language = selectedLanguage;

                    Properties.Settings.Default.Save();

                    RestartApplication();
                }
                else
                {
                    cbSelectLanguage.SelectedIndex = (int)GetLanguageFromCode(Properties.Settings.Default.Language);
                }
            }
        }

        private void RestartApplication()
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetEntryAssembly().Location + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            AppControl.Instance.CloseApp(true);
        }

        internal void SetCase(string v)
        {
            lCase.Text = v;
        }
    }
}