
using BeautySim2023.DataModel;

namespace BeautySim2023
{
    public class ViewUser : NotifyClass
    {
        private bool selected;
        private Users user;

        public Users User
        {
            get
            {
                return user;
            }
            set
            {
                if (value != user)
                {
                    SendPropertyChanging();
                    user = value;
                    SendPropertyChanged("User");
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

        public ViewUser (Users _user)
        {
            User = _user;
            Selected = false;
        }
    }
}
