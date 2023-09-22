using Device.BeautySim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class FunctionalitiesFrame : Page
    {
        private List<string> listCases = new List<string>();

        private List<string> listCasesDescription = new List<string>();

        private List<string> listCasesImages = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public FunctionalitiesFrame()
        {
            InitializeComponent();
            AppControl.Instance.WindowTeacher.bBack.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.bClose.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.cbSelectLanguage.Visibility = Visibility.Collapsed;

            //if (AppControl.Instance != null &&  ((AppControl.Instance.CurrentTeacher != null && AppControl.Instance.CurrentTeacher.UserName == AppControl.ADMIN_USERNAME) ||
            //    AppControl.Instance.OldTeacher != null && AppControl.Instance.OldTeacher.UserName == AppControl.ADMIN_USERNAME))
            //{
            //    AppControl.Instance.CheckModulesActivation();
            //}
            AppControl.Instance.CheckModulesActivation();
            AppControl.Instance.OldTeacher = AppControl.Instance.CurrentTeacher;
            AppControl.Instance.WindowStudent.spLegenda.Visibility = Visibility.Hidden;

            InitModulesItemSource();

            InitFrame();

            //PrepareBackGroundWorker();
        }

        public void InitFrame()
        {
            if (AppControl.Instance.Modules != null && AppControl.Instance.Modules.Count > 0)
            {
                cbSelectModule.SelectedIndex = AppControl.Instance.LastIndexSelectedModule >= 0 ? AppControl.Instance.LastIndexSelectedModule : 0;

                PrepareCasesSelectedModule(false);
            }

            SetUpThisFrame(true);
        }

        /// <summary>
        ///  
        /// </summary>
        public void InitModulesItemSource()
        {
            cbSelectModule.ItemsSource = AppControl.Instance.ModulesNames;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new StartPageFrame());
        }

        //internal void PrepareBackGroundWorker()
        //{
        //    workerLoadCase = new BackgroundWorker();
        //    workerLoadCase.WorkerReportsProgress = true;
        //    workerLoadCase.DoWork += WorkerLoadCase_DoWork;
        //    workerLoadCase.ProgressChanged += WorkerLoadCase_ProgressChanged;
        //    workerLoadCase.RunWorkerCompleted += WorkerLoadCase_Completed;
        //}
        private void bLoadCase_Click(object sender, RoutedEventArgs e)
        {
            if ((AppControl.Instance.CurrentTeacher != null) && (AppControl.Instance.CurrentStudent == null))
            {
                if (MessageBox.Show(BeautySim.Globalization.Language.str_no_student_sel, true, 1000, false) == false)
                {
                    return;
                }
            }

            var item = cbSelectCase.SelectedItem;
            AppControl.Instance.LastIndexSelectedModule = cbSelectModule.SelectedIndex;
            AppControl.Instance.LastIndexSelectedCase = cbSelectCase.SelectedIndex;
            if (item != null)
            {
                foreach (ClinicalCase f in AppControl.Instance.Cases)
                {
                    if (f != item)
                    {
                        f.Selected = false;
                    }
                    else
                    {
                        f.Selected = true;
                    }
                }

                AppControl.Instance.LoadingCase = true;
                AppControl.Instance.LoadedCase = false;
                SetUpThisFrame(false);

                AppControl.Instance.VisibilityLogoOnStudentPage = Visibility.Hidden;

                AppControl.Instance.CurrentCase = AppControl.Instance.Cases.Where(x => x.Name == cbSelectCase.SelectedItem).FirstOrDefault();

                AppControl.Instance.LoadCase();
            }
        }

        private void cbSelectCase_DropDownClosed(object sender, EventArgs e)
        {
            cbSelectCase.Background.Opacity = 0;
        }

        private void cbSelectCase_DropDownOpened(object sender, EventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (System.Windows.Media.Brush)converter.ConvertFromString("#01FFFF90");
            cbSelectCase.Background = brush;
        }

        private void cbSelectCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listCasesDescription.Count > 0 && cbSelectCase.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(listCasesDescription[cbSelectCase.SelectedIndex]))
            {
                tbCaseInfo.Text = listCasesDescription[cbSelectCase.SelectedIndex].Replace('*', '\n');
                imCaseImage.Source = new BitmapImage(new Uri(listCasesImages[cbSelectCase.SelectedIndex], UriKind.RelativeOrAbsolute));
            }
        }

        private void cbSelectModule_DropDownClosed(object sender, EventArgs e)
        {
            tbSelectCase.Visibility = Visibility.Visible;
            cbSelectCase.Visibility = Visibility.Visible;
            cbSelectModule.Background.Opacity = 0;
        }

        /// <summary>
        /// Prepare cases for selected module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelectModule_DropDownOpened(object sender, EventArgs e)
        {
            tbSelectCase.Visibility = Visibility.Hidden;
            cbSelectCase.Visibility = Visibility.Hidden;

            var converter = new System.Windows.Media.BrushConverter();
            var brush = (System.Windows.Media.Brush)converter.ConvertFromString("#01FFFF90");

            cbSelectModule.Background = brush;
        }

        private void cbSelectModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrepareCasesSelectedModule(true);
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new UsersManagerFrame(Enum_UserVisualizationMode.EDITTEACHER));
        }

        /// <summary>
        /// Manage the events of the teacher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageEvents_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new EventsManagerFrame());
        }

        private void ManageStudents_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new UsersManagerFrame(0));
        }

        private void ManageTeachers_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new UsersManagerFrame(Enum_UserVisualizationMode.TEACHER));
        }

        /// <summary>
        /// Prepare the list of cases for the selected module
        /// </summary>
        /// <param name="moduleChanged"></param>
        private void PrepareCasesSelectedModule(bool moduleChanged)
        {
            if (cbSelectModule.SelectedIndex >= 0)
            {
                AppControl.Instance.CurrentModule = AppControl.Instance.Modules[cbSelectModule.SelectedIndex];

                listCases = AppControl.Instance.GetListOfStringCases(AppControl.Instance.CurrentModule);
                listCasesDescription = AppControl.Instance.GetListOfStringCasesDescription(AppControl.Instance.CurrentModule);
                listCasesImages = AppControl.Instance.GetListOfStringCasesImages(AppControl.Instance.CurrentModule);
                cbSelectCase.ItemsSource = listCases;
                if (listCases.Count > 0)
                {
                    cbSelectCase.SelectedIndex = moduleChanged ? 0 : AppControl.Instance.LastIndexSelectedCase;
                }
            }
        }

        /// <summary>
        /// Prepare the list of modules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionsArchive_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new ResultsManagerFrame());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            AppControl.Instance.WindowTeacher.Navigate(new SettingsFrame());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        private void SetUpThisFrame(bool v)
        {
            try
            {
                if (v)
                {
                    bManageStudents.IsEnabled = AppControl.Instance.CurrentTeacher != null;
                    bSessionsArchive.IsEnabled = AppControl.Instance.CurrentTeacher != null;
                    bEditMyProfile.IsEnabled = AppControl.Instance.CurrentTeacher != null && AppControl.Instance.CurrentTeacher.UserName != AppControl.BeautySim_USERNAME && AppControl.Instance.CurrentTeacher.UserName != AppControl.ADMIN_USERNAME;
                    if (AppControl.Instance.CurrentTeacher != null)
                    {
                        //grMain.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        if (AppControl.Instance.CurrentTeacher.Role == 2)
                        {
                            bManageTeachers.IsEnabled = true;
                            bManageEvents.IsEnabled = true;
                        }
                        else
                        {
                            bManageTeachers.IsEnabled = false;
                            bManageEvents.IsEnabled = false;
                        }
                    }
                    else
                    {
                        bManageTeachers.IsEnabled = false;
                        bManageEvents.IsEnabled = false;
                    }
                    grSelectCase.IsEnabled = true;
                    bLoadCaseText.Text = BeautySim.Globalization.Language.str_load_case;
                    grLoadCase.Visibility = Visibility.Hidden;

                    tbSerial.Text = AppControl.Instance.BTYSimulatorConnected ? "SN BTY: " + BeautySimController.Instance.Simulator.SerialNumber : "SN BTY:";
                }
                else
                {
                    bManageStudents.IsEnabled = false;
                    bSessionsArchive.IsEnabled = false;
                    bManageTeachers.IsEnabled = false;
                    bManageEvents.IsEnabled = false;
                    cbSelectCase.IsEnabled = false;
                    grSelectCase.IsEnabled = false;
                    bEditMyProfile.IsEnabled = false;
                    bSettings.IsEnabled = false;
                    bLoadCaseText.Text = BeautySim.Globalization.Language.str_loading_case;
                    grLoadCase.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}