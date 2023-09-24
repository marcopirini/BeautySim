using System.ComponentModel;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class ActualRelease : INotifyPropertyChanged
    {
        private double pitchEntrance;
        private double yawEntrance;
        private double actualZ;
        private double injectedQuantity;
        private double depthInjection;
        private double actualY;
        private double actualX;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [XmlAttribute]
        public double InjectedQuantity
        {
            get { return injectedQuantity; }
            set
            {
                if (injectedQuantity != value)
                {
                    injectedQuantity = value;
                    OnPropertyChanged(nameof(InjectedQuantity));
                }
            }
        }

        [XmlAttribute]
        public double DepthInjection
        {
            get { return depthInjection; }
            set
            {
                if (depthInjection != value)
                {
                    depthInjection = value;
                    OnPropertyChanged(nameof(DepthInjection));
                }
            }
        }

        [XmlAttribute]
        public double ActualX
        {
            get { return actualX; }
            set
            {
                if (actualX != value)
                {
                    actualX = value;
                    OnPropertyChanged(nameof(ActualX));
                }
            }
        }

        [XmlAttribute]
        public double ActualY
        {
            get { return actualY; }
            set
            {
                if (actualY != value)
                {
                    actualY = value;
                    OnPropertyChanged(nameof(ActualY));
                }
            }
        }

        [XmlAttribute]
         public double ActualZ
        {
            get { return actualZ; }
            set
            {
                if (actualZ != value)
                {
                    actualZ = value;
                    OnPropertyChanged(nameof(ActualZ));
                }
            }
        }

        [XmlAttribute]
        public double YawEntrance
        {
            get { return yawEntrance; }
            set
            {
                if (yawEntrance != value)
                {
                    yawEntrance = value;
                    OnPropertyChanged(nameof(YawEntrance));
                }
            }
        }   

        public double PitchEntrance
        {
            get { return pitchEntrance; }
            set
            {
                if (pitchEntrance != value)
                {
                    pitchEntrance = value;
                    OnPropertyChanged(nameof(PitchEntrance));
                }
            }
        }
    }
}