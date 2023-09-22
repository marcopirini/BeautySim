using BeautySim2023.DataModel;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class UsersManagerFrame : Page
    {
        private Users CurrentSelectedUser = null;

        public UsersManagerFrame(Enum_UserVisualizationMode usersManagerFrameMode)
        {
            InitializeComponent();

            AppControl.Instance.WindowTeacher.bBack.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.bClose.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Collapsed;

            UsersManagerFrameMode = usersManagerFrameMode;
            UpdateControlButtons(false);
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.STUDENTS) //Students
            {
                gMainGrid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
                PopulateList();
                grFields.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                tbModify.Text = BeautySim.Globalization.Language.str_modify;
                spModify.Visibility = Visibility.Collapsed;
                PopulateList();
            }

            if (UsersManagerFrameMode == Enum_UserVisualizationMode.TEACHER) //Teachers management
            {
                gMainGrid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
                grFields.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                tbModify.Text = BeautySim.Globalization.Language.str_view;
                spModify.Visibility = Visibility.Collapsed;
                PopulateList();
            }

            if (UsersManagerFrameMode == Enum_UserVisualizationMode.EDITTEACHER) //Edit Teacher Profile
            {
                gMainGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                spPresent.Visibility = Visibility.Collapsed;
                spModify.Visibility = Visibility.Visible;
                grFields.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                cbRoleSelector.IsEnabled = false;

                if (AppControl.Instance.CurrentTeacher != null)
                {
                    cbRoleSelector.IsEnabled = false;
                    tbName.Text = AppControl.Instance.CurrentTeacher.Name;
                    tbSurname.Text = AppControl.Instance.CurrentTeacher.Surname;
                    tbOrganization.Text = AppControl.Instance.CurrentTeacher.Organization;
                    tbTitle.Text = AppControl.Instance.CurrentTeacher.Title;
                    tbUserName.Text = AppControl.Instance.CurrentTeacher.UserName;
                    tbPassword.Password = AppControl.Instance.CurrentTeacher.Password;
                    tbConfirmPassword.Password = AppControl.Instance.CurrentTeacher.Password;
                    cbRoleSelector.IsChecked = AppControl.Instance.CurrentTeacher.Role == 2;
                    cbRoleSelector.IsEnabled = false;
                }
            }

            UpdateControlButtons(false);
        }

        public bool AddOrModify { get; private set; }

        public Enum_UserVisualizationMode UsersManagerFrameMode { get; private set; }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedUser = new Users();
            AddOrModify = true;
            spModify.Visibility = Visibility.Visible;
            ListUsers.IsEnabled = false;
            spCommands.IsEnabled = false;
            tbUserName.IsEnabled = false;
            tbPassword.IsEnabled = false;
            tbConfirmPassword.IsEnabled = false;
            cbRoleSelector.IsEnabled = true;
            grStandardFields.IsEnabled = true;
            grAdminFields.IsEnabled = true;
            bSave.Visibility = Visibility.Visible;
            bCancel.Visibility = Visibility.Visible;

            tbName.Text = "";
            tbSurname.Text = "";
            tbOrganization.Text = "";
            tbTitle.Text = "";
            tbUserName.Text = "";
            tbPassword.Password = "";
            tbConfirmPassword.Password = "";
            cbRoleSelector.IsChecked = false;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            spModify.Visibility = Visibility.Collapsed;
            ListUsers.IsEnabled = true;
            spCommands.IsEnabled = true;
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.EDITTEACHER)
            {
                AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
            }
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            Users user = ((ViewUser)ListUsers.SelectedItem).User;
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.TEACHER)
            {
                if (false)
               // if (user.UserName == AppControl.BeautySim_USERNAME || user.UserName == AppControl.ADMIN_USERNAME)
                {
                    //MessageBox.Show(BeautySim.Globalization.Language.str_cant_del_user);
                }
                else
                {
                    if (MessageBox.Show(BeautySim.Globalization.Language.str_sure_cancel_teacher, true, 1000, false))
                    {
                        
                        var students = from db in DBConnector.Instance.FindAll<Users>() where ((db.Role == 0) && (db.IdParentUser==user.Id))  select db;
                        foreach (Users us in students)
                        {
                            DBConnector.Instance.DeleteRow<Users>(new BsonValue(us.Id));
                        }
                        
                        var results = from db in DBConnector.Instance.FindAll<Results>() where db.IdTeacher == user.Id select db;
                        foreach (Results us in results)
                        {
                            DBConnector.Instance.DeleteRow<Results>(new BsonValue(us.Id));
                        }
                        DBConnector.Instance.DeleteRow<Users>(new BsonValue(user.Id));
                        PopulateList();
                        if (AppControl.Instance.CurrentTeacher == user)
                        {
                            AppControl.Instance.SelectStudent(null);
                            AppControl.Instance.SelectTeacher(null);
                        }
                    }
                }
            }
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.STUDENTS)
            {
                if (MessageBox.Show(BeautySim.Globalization.Language.str_sure_cancel_student, true, 1000, false))
                {
                    var results = from db in DBConnector.Instance.FindAll<Results>() where db.IdStudent == user.Id select db;
                    foreach (Results us in results)
                    {
                        DBConnector.Instance.DeleteRow<Results>(new BsonValue(us.Id));
                    }
                    DBConnector.Instance.DeleteRow<Users>(new BsonValue(user.Id));
                    PopulateList();
                    if (AppControl.Instance.CurrentStudent == user)
                    {
                        AppControl.Instance.SelectStudent(null);
                    }
                }
            }
        }

        private void bModify_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedUser = ((ViewUser)ListUsers.SelectedItem).User;
            AddOrModify = false;
            grAdminFields.IsEnabled = false;
            spCommands.IsEnabled = false;
            ListUsers.IsEnabled = false;

            bCancel.Visibility = Visibility.Visible;

            // Populate the spModify
            switch (UsersManagerFrameMode)
            {
                case Enum_UserVisualizationMode.STUDENTS:
                    bSave.Visibility = Visibility.Visible;
                    grStandardFields.IsEnabled = true;
                    tbName.Text = CurrentSelectedUser.Name;
                    tbSurname.Text = CurrentSelectedUser.Surname;
                    tbOrganization.Text = CurrentSelectedUser.Organization;
                    tbTitle.Text = CurrentSelectedUser.Title;
                    break;

                case Enum_UserVisualizationMode.TEACHER:
                    bSave.Visibility = Visibility.Collapsed;
                    grStandardFields.IsEnabled = false;
                    tbName.Text = CurrentSelectedUser.Name;
                    tbSurname.Text = CurrentSelectedUser.Surname;
                    tbOrganization.Text = CurrentSelectedUser.Organization;
                    tbTitle.Text = CurrentSelectedUser.Title;
                    tbUserName.Text = CurrentSelectedUser.UserName;
                    tbPassword.Password = CurrentSelectedUser.Password;
                    tbConfirmPassword.Password = CurrentSelectedUser.Password;
                    cbRoleSelector.IsChecked = CurrentSelectedUser.Role == 2;

                    break;

                case Enum_UserVisualizationMode.EDITTEACHER:
                    tbName.Text = AppControl.Instance.CurrentTeacher.Name;
                    tbSurname.Text = AppControl.Instance.CurrentTeacher.Surname;
                    tbOrganization.Text = AppControl.Instance.CurrentTeacher.Organization;
                    tbTitle.Text = AppControl.Instance.CurrentTeacher.Title;
                    tbUserName.Text = AppControl.Instance.CurrentTeacher.UserName;
                    tbPassword.Password = AppControl.Instance.CurrentTeacher.Password;
                    tbConfirmPassword.Password = AppControl.Instance.CurrentTeacher.Password;
                    cbRoleSelector.IsChecked = AppControl.Instance.CurrentTeacher.Role == 2;
                    cbRoleSelector.IsEnabled = false;
                    break;

                default:
                    break;
            }
            spModify.Visibility = Visibility.Visible;
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            // SAVE
            bool collapse = true;

            switch (UsersManagerFrameMode)
            {
                case Enum_UserVisualizationMode.STUDENTS:
                    string name = tbName.Text;
                    string surname = tbSurname.Text;
                    string title = tbTitle.Text;
                    string organization = tbOrganization.Text;

                    if (name == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_name);
                    }

                    if (surname == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_surname);
                    }

                    if (collapse)
                    {
                        //if (AddOrModify)
                        //{
                        //    CurrentSelectedUser.Id = AppControl.Instance.GetNextPrimayKey("Users", DBConnector.Instance.Context);
                        //}

                        CurrentSelectedUser.Name = name;
                        CurrentSelectedUser.Surname = surname;
                        CurrentSelectedUser.Title = title;
                        CurrentSelectedUser.Organization = organization;
                        CurrentSelectedUser.Role = 0;

                        CurrentSelectedUser.IdParentUser = AppControl.Instance.CurrentTeacher.Id;
                        if (AddOrModify)
                        {
                            DBConnector.Instance.InsertRow<Users>(CurrentSelectedUser);
                        }
                        else
                        {
                            DBConnector.Instance.UpdateRow<Users>(CurrentSelectedUser);
                        }
                        PopulateList();
                        spCommands.IsEnabled = true;
                        spModify.Visibility = Visibility.Hidden;
                        ListUsers.IsEnabled = true;
                    }

                    break;

                case Enum_UserVisualizationMode.TEACHER:
                    string name1 = tbName.Text;
                    string surname1 = tbSurname.Text;
                    string title1 = tbTitle.Text;
                    string organization1 = tbOrganization.Text;
                    string password1 = BeautySim.Globalization.Language.str_password_lower;
                    string username1 = name1 + "." + surname1;
                    username1.Replace(" ", "").Replace("'", "");
                    bool isAdmin = (bool)cbRoleSelector.IsChecked;
                    if (name1 == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_name);
                    }

                    if (surname1 == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_surname);
                    }
                    if (collapse)
                    {
                        if (AddOrModify)
                        {
                            MessageBox.Show(BeautySim.Globalization.Language.str_a_new_teacher + " " + (isAdmin ? BeautySim.Globalization.Language.str_with_admin_rights + " " : "") + BeautySim.Globalization.Language.str_created + " " + BeautySim.Globalization.Language.str_def_username + " " + username1 + "; " + BeautySim.Globalization.Language.str_def_password + " " + password1);
                            Users us = new Users();
                            us.Name = name1;
                            us.Surname = surname1;
                            us.Title = title1;
                            us.Organization = organization1;
                            us.Role = isAdmin ? 2 : 1;
                            us.UserName = username1;
                            us.Password = password1;
                            //us.Id = AppControl.Instance.GetNextPrimayKey("Users", DBConnector.Instance.Context);
                            DBConnector.Instance.InsertRow<Users>(us);
                            spCommands.IsEnabled = true;
                            spModify.Visibility = Visibility.Hidden;
                            ListUsers.IsEnabled = true;
                            PopulateList();
                        }
                    }
                    break;

                case Enum_UserVisualizationMode.EDITTEACHER:
                    string name2 = tbName.Text;
                    string surname2 = tbSurname.Text;
                    string title2 = tbTitle.Text;
                    string organization2 = tbOrganization.Text;
                    string password2 = tbPassword.Password;
                    string password2Confirm = tbConfirmPassword.Password;
                    string username2 = tbUserName.Text;
                    username2.Replace(" ", "").Replace("'", "");
                    if (name2 == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_name);
                    }
                    if (surname2 == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_surname);
                    }
                    if (username2 == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_username);
                    }
                    if (password2 == "")
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_password);
                    }
                    if (password2 != password2Confirm)
                    {
                        collapse = false;
                        MessageBox.Show(BeautySim.Globalization.Language.str_pwd_not_confirmed);
                    }
                    if (collapse)
                    {
                        Users us = DBConnector.Instance.FindRowById<Users>(new BsonValue(AppControl.Instance.CurrentTeacher.Id));
                        if (us !=null)
                        {
                            us.Name = name2;
                            us.Surname = surname2;
                            us.Title = title2;
                            us.Organization = organization2;

                            us.UserName = username2;
                            us.Password = password2;
                        }
                        AppControl.Instance.WindowTeacher.Navigate(new FunctionalitiesFrame());
                    }

                    break;

                default:
                    break;
            }
        }

        private void bSelect_Click(object sender, RoutedEventArgs e)
        {
            switch (UsersManagerFrameMode)
            {
                case Enum_UserVisualizationMode.STUDENTS:
                    if (((ViewUser)ListUsers.SelectedItem).User == AppControl.Instance.CurrentStudent)
                    {
                        AppControl.Instance.SelectStudent(null);
                    }
                    else
                    {
                        AppControl.Instance.SelectStudent(((ViewUser)ListUsers.SelectedItem).User);
                    }
                    UpdateBackgroundSelected();
                    UpdateSelectUnselect();
                    break;

                case Enum_UserVisualizationMode.TEACHER:

                    AppControl.Instance.SelectTeacher(((ViewUser)ListUsers.SelectedItem).User);
                    break;

                default:
                    break;
            }
        }

        private void ListUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateControlButtons(ListUsers.SelectedIndex >= 0);

            UpdateSelectUnselect();
        }

        private void PopulateList()
        {
            List<Users> users = new List<Users>();
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.TEACHER && AppControl.Instance.CurrentTeacher != null)
            {
                List<Users> entries2 = DBConnector.Instance.FindAll<Users>().ToList();

                users = (from db in entries2 where ((db.Role == 1) || (db.Role == 2)) && (db.Id != AppControl.Instance.CurrentTeacher.Id) select db).ToList();

                ListUsers.Items.Clear();
                foreach (Users us in users)
                {
                    ListUsers.Items.Add(new ViewUser(us));
                }
                spCommands.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Star);
                bSelect.IsEnabled = false;
            }
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.STUDENTS && AppControl.Instance.CurrentTeacher != null)
            {
                List<Users> entries = DBConnector.Instance.FindAll<Users>().ToList();
                List<Users> entries2 = (from db in entries where ((db.Role == 0) && (db.IdParentUser == AppControl.Instance.CurrentTeacher.Id)) select db).ToList();
                users = entries2.ToList();
                ListUsers.Items.Clear();
                foreach (Users us in users)
                {
                    ListUsers.Items.Add(new ViewUser(us));
                }
                spCommands.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                bSelect.IsEnabled = true;
                UpdateBackgroundSelected();
            }
        }
        private void UpdateBackgroundSelected()
        {
            if (UsersManagerFrameMode == Enum_UserVisualizationMode.STUDENTS)
            {
                foreach (ViewUser us in ListUsers.Items)
                {
                    us.Selected = (us.User == AppControl.Instance.CurrentStudent);
                }
            }
        }
        private void UpdateControlButtons(bool v)
        {
            if (v)
            {
                bModify.IsEnabled = true;
                bSelect.IsEnabled = true;
                bDelete.IsEnabled = true;
            }
            else
            {
                bModify.IsEnabled = false;
                bSelect.IsEnabled = false;
                bDelete.IsEnabled = false;
            }
        }

        private void UpdateSelectUnselect()
        {
            if (AppControl.Instance.CurrentStudent != null)
            {
                if (UsersManagerFrameMode == Enum_UserVisualizationMode.STUDENTS)
                {
                    if (((ViewUser)ListUsers.SelectedItem) != null)
                    {
                        if (((ViewUser)ListUsers.SelectedItem).User == AppControl.Instance.CurrentStudent)
                        {
                            tbSelect.Text = BeautySim.Globalization.Language.str_unselect;
                        }
                        else
                        {
                            tbSelect.Text = BeautySim.Globalization.Language.str_select;
                        }
                    }
                    else
                    {
                        tbSelect.Text = BeautySim.Globalization.Language.str_select;
                    }
                }
            }
            else
            {
                tbSelect.Text = BeautySim.Globalization.Language.str_select;
            }
        }
    }
}