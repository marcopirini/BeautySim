using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BeautySim2023.DataModel;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MessageBox.xaml
    /// </summary>
    public partial class SelectStudent : Window
    {
        public bool ApplyFilter = false;

        public List<Users> FilteredStudents = new List<Users>();

        public SelectStudent(Users teacher, List<Users> alreadySelected)
        {
            InitializeComponent();

            Teacher = teacher;
            //var entries = DBConnector.Instance.FindRowsByAttribute<Users>("IDParentUser", Teacher.Id.ToString());
            var entries = from db in DBConnector.Instance.FindAll<Users>() where db.IdParentUser == Teacher.Id select db;
            foreach (Users item in entries)
            {
                ViewUser vu = new ViewUser(item);
                if(alreadySelected.Contains(item))
                {
                    vu.Selected = true;
                }
                ListUsers.Items.Add(vu);
            }
        }

        public List<ViewUser>  SelectedStudents { get; private set; }
        public DateTime DateStart { get; private set; }
        public Users Teacher { get; private set; }

        private void bApplyOk_Click(object sender, RoutedEventArgs e)
        {
            FilteredStudents.Clear();
            ApplyFilter = false;
            foreach (ViewUser vu in ListUsers.Items)
            {
                if (vu.Selected)
                {
                    FilteredStudents.Add(vu.User);
                    ApplyFilter = true;
                }
            }
            Close();
        }

        private void bRemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            SelectedStudents = new List<ViewUser>();
            ApplyFilter = false;
            Close();
        }
    }
}
