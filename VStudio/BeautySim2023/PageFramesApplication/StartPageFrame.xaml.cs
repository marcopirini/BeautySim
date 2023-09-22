using BeautySim2023.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class StartPageFrame : Page
    {
        public StartPageFrame()
        {
            InitializeComponent();
            AppControl.Instance.WindowTeacher.bBack.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.bClose.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.cbSelectLanguage.Visibility = Visibility.Visible;
            lVersion.Content= "v. " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

            if (AppControl.Instance.CurrentTeacher != null && AppControl.Instance.CurrentTeacher.UserName == AppControl.ADMIN_USERNAME)
            {
                AppControl.Instance.CurrentTeacher = null;
                AppControl.Instance.ModulesChecked = false;
                AppControl.Instance.LastSerial = string.Empty;
            }

            if (AppControl.Instance.CurrentTeacher != null)
            {
                PreparePage("Proceed");

            }
            else
            {
                PreparePage("Login");
            }
        }

        private void GuestMode_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (AppControl.Instance.CurrentTeacher == null)
            {
                List<Users> items = DBConnector.Instance.FindAll<Users>().ToList();
                List<Users> items2 = (from db in items where db.Role > 0 && db.UserName == tbUserName.Text && db.Password == tbPassword.Password select db).ToList();
                if (items2.Count() > 0)
                {
                    AppControl.Instance.SelectTeacher((Users)items2.First());
                    AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
                }
                else
                {
                    MessageBox.Show(BeautySim.Globalization.Language.str_no_teachers);
                }
            }
            else
            {
                AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
            }
            
        }

        internal void PreparePage(string v)
        {
            if (v == "Proceed")
            {
                AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Visible;
                this.tbPassword.Password = AppControl.Instance.CurrentTeacher.Password;
                this.tbUserName.Text = AppControl.Instance.CurrentTeacher.UserName;
                this.tbPassword.IsEnabled = false;
                this.tbUserName.IsEnabled = false;
                this.tbLogin.Text = BeautySim.Globalization.Language.str_proceed;
                this.bGuestMode.Visibility = Visibility.Hidden;
            }
            if(v=="Login")
            {
                AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Collapsed;
                this.tbPassword.IsEnabled = true;
                this.tbUserName.IsEnabled = true;
                this.tbPassword.Password = "";
                this.tbUserName.Text = "";
                this.tbLogin.Text = BeautySim.Globalization.Language.str_log_in;
                this.bGuestMode.Visibility = Visibility.Visible;
            }
        }
    }
}
