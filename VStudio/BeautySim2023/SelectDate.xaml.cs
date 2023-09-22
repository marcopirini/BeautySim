using System;
using System.Windows;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MessageBox.xaml
    /// </summary>
    public partial class SelectDate : Window
    {
        public bool ApplyFilter = false;

        public SelectDate(DateTime start, DateTime finish)
        {
            InitializeComponent();

            if (start!=DateTime.MinValue)
            {
                dpStart.SelectedDate = start;
            }
            else
            {
                dpStart.DisplayDate = DateTime.Now; 
            }
            if (finish != DateTime.MinValue)
            {
                dpFinish.SelectedDate = finish;
            }
            else
            {
                dpFinish.DisplayDate = DateTime.Now;
            }
        }

        public DateTime DateFinish { get; private set; }
        public DateTime DateStart { get; private set; }

        private void bApplyOk_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter = (dpStart.SelectedDate != null && dpFinish.SelectedDate != null);

            DateStart = dpStart.SelectedDate == null ? DateTime.MinValue : (DateTime)dpStart.SelectedDate;
            DateFinish = dpFinish.SelectedDate == null ? DateTime.MinValue : (DateTime)dpFinish.SelectedDate;

            Close();
        }

        private void bRemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter = false;
            Close();
        }
    }
}
