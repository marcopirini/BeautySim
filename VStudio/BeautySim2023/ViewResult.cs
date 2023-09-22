
using BeautySim2023.DataModel;

namespace BeautySim2023
{
    public class ViewResult : NotifyClass
    {
        public string Student
        {
            get
            {
                return student;
            }
            set
            {
                if (value != student)
                {
                    SendPropertyChanging();
                    student = value;
                    SendPropertyChanged("Student");
                }
            }
        }

        public string EventOrg
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


        public string DateTimeResult
        {
            get
            {
                return dateTimeResult;
            }
            set
            {
                if (value != dateTimeResult)
                {
                    SendPropertyChanging();
                    dateTimeResult = value;
                    SendPropertyChanged("DateTimeResult");
                }
            }
        }

        public string CaseResult
        {
            get
            {
                return caseResult;
            }
            set
            {
                if (value != caseResult)
                {
                    SendPropertyChanging();
                    caseResult = value;
                    SendPropertyChanged("CaseResult");
                }
            }
        }

        public int Score 
        {
            get
            {
                return score;
            }
            set
            {
                if (value != score)
                {
                    SendPropertyChanging();
                    score = value;
                    SendPropertyChanged("Score");
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



        public Results Tag;
        private int score;
        private string caseResult;
        private string dateTimeResult;
        private string eventOrg;
        private string student;
        private bool selected;

        public void Populate(Results tag)
        {
            Tag = tag;

            DateTimeResult = tag.Date;
            Score = (int)tag.Score;

            Student = AppControl.Instance.GiveMeTheStudent(tag.IdStudent);
            EventOrg = AppControl.Instance.GiveMeTheEvent(tag.IdEvent);

            CaseResult = tag.CaseName;
        }
    }
}
