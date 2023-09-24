namespace Device.Motion
{
    public abstract class BasicBalanceClass
    {
        public delegate void DataReadyEventHandler(double X, double Y, double sumWeight);

        public event DataReadyEventHandler DataReadyEvent;

        public delegate void CanGetDataChanged(CONNECTIONSTATUS status);

        public event CanGetDataChanged OnCanGetDataChanged;

        protected virtual void LauchEventDataReady(double x, double y, double totalWeight)
        {
            if (DataReadyEvent!=null)
            {
                DataReadyEvent(x, y, totalWeight);
            }
        }

        protected virtual void LaunchConnectionStatusChange()
        {
            if (OnCanGetDataChanged != null)
            {
                OnCanGetDataChanged(CanGetData?CONNECTIONSTATUS.ACQUIRING:CONNECTIONSTATUS.SEARCHING_DEVICE);
            }
        }

        public void SetRotate(int rotate)
        {
            Rotate = rotate;
        }

        public int Rotate { get; private set; }

        public virtual void Dispose()
        {

        }

        protected bool canGetData;

        public bool CanGetData
        {
            get { return canGetData; }
            set
            {
                canGetData = value;
                LaunchConnectionStatusChange();
            }
        }
    }
}