
using BeautySim2023.DataModel;

namespace BeautySim2023
{
    public class ViewEvent : NotifyClass
    {
        private bool selected;
        private Events eventOrg;

        public Events EventOrg
        {
            get
            {
                return eventOrg;
            }
            set
            {
                if (value != eventOrg)
                {
                    SendPropertyChanging();
                    eventOrg = value;
                    SendPropertyChanged("EventOrg");
                }
            }
        }

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value != selected)
                {
                    SendPropertyChanging();
                    selected = value;
                    SendPropertyChanged("Selected");
                }
            }
        }

        public ViewEvent (Events _eventOrg)
        {
            EventOrg = _eventOrg;
            Selected = false;
        }
    }
}
