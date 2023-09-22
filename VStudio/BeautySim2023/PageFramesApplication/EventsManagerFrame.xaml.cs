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
    public partial class EventsManagerFrame : Page
    {
        private Events CurrentSelectedEvent = null;

        public EventsManagerFrame()
        {
            InitializeComponent();

            UpdateControlButtons(false);

            tbModify.Text = BeautySim.Globalization.Language.str_modify;
            // hide the details
            spModify.Visibility = Visibility.Collapsed;

            PopulateList();

            UpdateControlButtons(false);

            AppControl.Instance.WindowTeacher.bBack.Visibility = Visibility.Visible;
            AppControl.Instance.WindowTeacher.bClose.Visibility = Visibility.Collapsed;
            AppControl.Instance.WindowTeacher.bLogOut.Visibility = Visibility.Collapsed;
        }

        public bool AddOrModify { get; private set; }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedEvent = new Events();
            AddOrModify = true;
            spModify.Visibility = Visibility.Visible;
            tbShortName.IsEnabled = true;
            ListEvents.IsEnabled = false;
            spCommands.IsEnabled = false;
            bSave.Visibility = Visibility.Visible;
            bCancel.Visibility = Visibility.Visible;

            tbShortName.Text = "";
            tbCompleteName.Text = "";
            tbOrganisedBy.Text = "";
            tbSponsoredBy.Text = "";
            tbLocation.Text = "";
            tbStartFrom.Text = "";
            tbFinishAt.Text = "";
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            spModify.Visibility = Visibility.Collapsed;
            ListEvents.IsEnabled = true;
            spCommands.IsEnabled = true;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            Events ev = ((ViewEvent)ListEvents.SelectedItem).EventOrg;
            if (MessageBox.Show(BeautySim.Globalization.Language.str_sure_cancel_evt, true, 1000, false))
            {
                var results = from db in DBConnector.Instance.FindAll<Results>() where db.IdEvent==ev.Id select db;
                foreach (Results us in results)
                {
                    DBConnector.Instance.DeleteRow<Results>(new BsonValue(us.Id));
                }
                DBConnector.Instance.DeleteRow<Events>(new BsonValue(ev.Id));
                if (AppControl.Instance.CurrentEvent == ev)
                {
                    AppControl.Instance.SelectEvent(null);
                }
                PopulateList();
            }
        }

        private void bModify_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedEvent = ((ViewEvent)ListEvents.SelectedItem).EventOrg;
            AddOrModify = false;
            spCommands.IsEnabled = false;
            ListEvents.IsEnabled = false;
            bSave.Visibility = Visibility.Visible;
            bCancel.Visibility = Visibility.Visible;
            spModify.Visibility = Visibility.Visible;
            spModify.IsEnabled = true;
            tbShortName.IsEnabled = false;

            tbShortName.Text = CurrentSelectedEvent.ShortName;
            tbCompleteName.Text = CurrentSelectedEvent.CompleteName;
            tbOrganisedBy.Text = CurrentSelectedEvent.OrganizedBy;
            tbSponsoredBy.Text = CurrentSelectedEvent.SponsoredBy;
            tbLocation.Text = CurrentSelectedEvent.Location;
            tbStartFrom.Text = CurrentSelectedEvent.FromDate;
            tbFinishAt.Text = CurrentSelectedEvent.ToDate;
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            bool collapse = true;

            spModify.Visibility = Visibility.Collapsed;

            string name = tbShortName.Text;
            string completeName = tbCompleteName.Text;
            string location = tbLocation.Text;
            string organizedBy = tbOrganisedBy.Text;
            string sponsoredBy = tbSponsoredBy.Text;
            string startFrom = tbStartFrom.Text;
            string finisheAt = tbFinishAt.Text;

            if (name == "")
            {
                collapse = false;
                MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_sname);
            }
            if (completeName == "")
            {
                collapse = false;
                MessageBox.Show(BeautySim.Globalization.Language.str_mandatory_cname);
            }
            if (collapse)
            {

                //if (AddOrModify)
                //{
                //    CurrentSelectedEvent.Id = AppControl.Instance.GetNextPrimayKey("Events", DBConnector.Instance.Context);
                //}

                CurrentSelectedEvent.ShortName = name;
                CurrentSelectedEvent.CompleteName = completeName;
                CurrentSelectedEvent.Location = location;
                CurrentSelectedEvent.OrganizedBy = organizedBy;
                CurrentSelectedEvent.SponsoredBy = sponsoredBy;
                CurrentSelectedEvent.FromDate = startFrom;
                CurrentSelectedEvent.ToDate = finisheAt;
                if (AddOrModify)
                {
                    DBConnector.Instance.InsertRow<Events>(CurrentSelectedEvent);
                }
                else
                {
                    DBConnector.Instance.UpdateRow<Events>(CurrentSelectedEvent);
                }

                PopulateList();
                spCommands.IsEnabled = true;
                ListEvents.IsEnabled = true;
            }
        }

        private void bSelect_Click(object sender, RoutedEventArgs e)
        {
            if (((ViewEvent)ListEvents.SelectedItem).EventOrg == AppControl.Instance.CurrentEvent)
            {
                AppControl.Instance.SelectEvent(null);
            }
            else
            {
                AppControl.Instance.SelectEvent(((ViewEvent)ListEvents.SelectedItem).EventOrg);
            }
            UpdateSelectUnselect();
            UpdateBackgroundSelected();
        }

        private void ListEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateControlButtons(ListEvents.SelectedIndex >= 0);

            UpdateSelectUnselect();
        }

        private void PopulateList()
        {
            List<Events> events = DBConnector.Instance.FindAll<Events>().ToList();
            ListEvents.Items.Clear();
            foreach (Events us in events)
            {
                ListEvents.Items.Add(new ViewEvent(us));
            }
            spCommands.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
            bSelect.IsEnabled = true;
            UpdateBackgroundSelected();
        }
        private void UpdateBackgroundSelected()
        {
            foreach (ViewEvent us in ListEvents.Items)
            {
                us.Selected = (us.EventOrg == AppControl.Instance.CurrentEvent);
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
            if (AppControl.Instance.CurrentEvent != null)
            {
                if (((ViewEvent)ListEvents.SelectedItem) != null)
                {
                    if (((ViewEvent)ListEvents.SelectedItem).EventOrg == AppControl.Instance.CurrentEvent)
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
            else
            {
                tbSelect.Text = BeautySim.Globalization.Language.str_select;
            }
        }
    }
}