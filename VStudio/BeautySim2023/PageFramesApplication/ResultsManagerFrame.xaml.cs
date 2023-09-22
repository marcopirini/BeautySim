using BeautySim2023.DataModel;
using GemBox.Pdf;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;

namespace BeautySim2023
{
    /// <summary>
    /// Logica di interazione per MainPage.xaml
    /// </summary>
    public partial class ResultsManagerFrame : Page
    {
        public List<ViewResult> viewResults = new List<ViewResult>();

        public bool ApplyDateFilter { get; private set; }
        public DateTime StartDateFilter { get; private set; }
        public DateTime FinishDateFilter { get; private set; }
        public bool ApplyStudentFilter { get; private set; }
        private Spire.PdfViewer.Forms.PdfViewer pdfW;
        public ResultsManagerFrame()
        {
            InitializeComponent();
            AppControl.Instance.WindowTeacher.bBack.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.bClose.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Collapsed;

            PopulateList(AppControl.Instance.CurrentTeacher);

            pdfW = new Spire.PdfViewer.Forms.PdfViewer();

            // Add the Windows Forms ListBox to the host control
            wfHost.Child = pdfW;
            timerShowMessage = new DispatcherTimer();
            timerShowMessage.Tick += TimerShowMessage_Tick;
            timerShowMessage.Interval = new TimeSpan(0, 0, 0, 3);
            //pdfWebViewer.Navigate(new Uri("about:blank"));
        }

        private void TimerShowMessage_Tick(object sender, EventArgs e)
        {
            timerShowMessage.Stop();
            ShowMessageTenPages(false);

        }

        private List<Users> students = new List<Users>();

        private void PopulateList(Users teacher)
        {
            viewResults.Clear();
            List<Results> results = DBConnector.Instance.FindAll<Results>().ToList();

            foreach (Results item in results)
            {
                DateTime c = DateTime.Parse(item.Date, new CultureInfo(16));
                long IdTeacher = (long)item.IdTeacher;
                long IdStudent = (long)item.IdStudent;

                bool toAdd = false;
                if ((IdTeacher == teacher.Id))// && (item.Deleted == 0))
                {
                    if (ApplyStudentFilter)
                    {
                        if (students.Select(e => e.Id).ToList().Contains(item.IdStudent))
                        {
                            if (ApplyDateFilter)
                            {
                                if ((c >= StartDateFilter) && (c <= FinishDateFilter))
                                {
                                    toAdd = true;
                                }
                            }
                            else
                            {
                                toAdd = true;
                            }
                        }
                    }
                    else
                    {
                        if (ApplyDateFilter)
                        {
                            if ((c >= StartDateFilter) && (c <= FinishDateFilter))
                            {
                                toAdd = true;
                            }
                        }
                        else
                        {
                            toAdd = true;
                        }
                    }

                    if (toAdd)
                    {
                        ViewResult viewRes = new ViewResult();
                        viewRes.Populate(item);
                        viewResults.Add(viewRes);
                    }
                }
            }

            ListUsers.Items.Clear();
            foreach (ViewResult res in viewResults)
            {
                ListUsers.Items.Add(res);
            }
        }

        private void bFilterByStudent_Click(object sender, RoutedEventArgs e)
        {
            SelectStudent slStudent = new SelectStudent(AppControl.Instance.CurrentTeacher, students);
            slStudent.ShowDialog();

            ApplyStudentFilter = slStudent.ApplyFilter;
            BrushConverter bc = new BrushConverter();

            bFilterByStudent.Background = ApplyStudentFilter ? Brushes.Violet : (Brush)bc.ConvertFrom("#009688");

            students = slStudent.FilteredStudents;

            PopulateList(AppControl.Instance.CurrentTeacher);
        }

        private void bFilterByDate_Click(object sender, RoutedEventArgs e)
        {
            SelectDate slDate = new SelectDate(StartDateFilter, FinishDateFilter);
            slDate.ShowDialog();

            ApplyDateFilter = slDate.ApplyFilter;
            BrushConverter bc = new BrushConverter();

            bFilterByDate.Background = ApplyDateFilter ? Brushes.Violet : (Brush)bc.ConvertFrom("#009688");

            StartDateFilter = ApplyDateFilter ? slDate.DateStart : DateTime.MinValue;
            FinishDateFilter = ApplyDateFilter ? slDate.DateFinish : DateTime.MinValue;

            TimeSpan ts = new TimeSpan(23, 59, 59);
            FinishDateFilter = FinishDateFilter.Date + ts;

            PopulateList(AppControl.Instance.CurrentTeacher);
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(BeautySim.Globalization.Language.str_sure_delete_res, true, 1000, false))
            {
                List<ViewResult> selectedesults = new List<ViewResult>();
                for (int i = 0; i < ListUsers.Items.Count; i++)
                {
                    if (((ViewResult)ListUsers.Items[i]).Selected)
                    {
                        selectedesults.Add(((ViewResult)ListUsers.Items[i]));
                    }
                }
                foreach (ViewResult item in selectedesults)
                {

                        DBConnector.Instance.DeleteRow<Results>(new BsonValue(item.Tag.Id));

                }
                PopulateList(AppControl.Instance.CurrentTeacher);
            }
        }

        private void bSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Results> toCompileResults = new List<Results>();
                foreach (ViewResult vres in ListUsers.Items)
                {
                    if (vres.Selected)
                    {
                        toCompileResults.Add(vres.Tag);
                    }
                }

                if (toCompileResults.Count > 10)
                {
                    for (int i = toCompileResults.Count - 1; i >= 10; i--)
                    {
                        toCompileResults.RemoveAt(i);
                    }
                }

                AppControl.Instance.ClearTempResults();

                ResultManager.CreateReportPDF(toCompileResults, "C:\\BeautySim\\ResultsTemp");

                DirectoryInfo di = new DirectoryInfo("C:\\BeautySim\\ResultsTemp");

                FileInfo[] files = di.GetFiles("*.pdf");
                if (files.Count() > 0)
                {
                    pdfW.LoadFromFile(files.First().FullName);
                    //using (var document = PdfDocument.Load(files.First().FullName))
                    //{
                    //    XpsDocument xpsDocument = document.ConvertToXpsDocument(SaveOptions.Xps);
                    //    this.DocumentViewer.Document = xpsDocument.GetFixedDocumentSequence();
                    //}

                    //pdfWebViewer.Navigate(files.First().FullName);
                    //moonPdfPanel.OpenFile(files.First().FullName);
                    //moonPdfPanel.ZoomToWidth();
                    //moonPdfPanel.Zoom(0.9);
                    //moonPdfPanel.ZoomOut();
                }
                grViewer.IsEnabled = true;
            }
            catch (Exception)
            {
                int gg = 0;
                gg++;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo("C:\\BeautySim\\ResultsTemp");
            FileInfo[] files = di.GetFiles("*.pdf");
            if (files.Count() > 0)
            {
               
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = files.First().Name;
                string saveDir = "C:\\BeautySim_SavedReports";
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                saveFileDialog1.InitialDirectory = saveDir;
                saveFileDialog1.Filter = BeautySim.Globalization.Language.str_pdf_files + " (*.pdf)|*.pdf";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(files.First().FullName, saveFileDialog1.FileName);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewResult vr = ((System.Windows.Controls.CheckBox)sender).DataContext as ViewResult;

            int count = 0;
            foreach (ViewResult vres in ListUsers.Items)
            {
                if (vres.Selected)
                {
                    count++;
                }
            }

            if (count>10)
            {
                timerShowMessage.Start();
                ShowMessageTenPages(true);
                vr.Selected = false;
            }


        }

        DispatcherTimer timerShowMessage;

        private void ShowMessageTenPages(bool v)
        {
            tbUserSelect.Visibility = v ? Visibility.Visible : Visibility.Hidden;
        }
    }
}