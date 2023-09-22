using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class InjectionPoint : INotifyPropertyChanged
    {
        [XmlAttribute]
        public bool ToTarget { get; set; }

        [XmlAttribute]
        public int PointNumber { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlArray]
        public List<double> DepthOptions { get; set; }

        [XmlAttribute]
        public double ChosenDepth { get; set; }

        [XmlArray]
        public List<double> QuantityOptions { get; set; }

        [XmlAttribute]
        public double ChosenQuantity { get; set; }

        [XmlAttribute]
        public double YawMin { get; set; }

        [XmlAttribute]
        public double YawMax { get; set; }

        [XmlAttribute]
        public double PitchMin { get; set; }

        [XmlAttribute]
        public double PitchMax { get; set; }

        [XmlAttribute]
        public string Explanation { get; set; }

        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ErrorCase> ErrorCases { get; set; }

        public InjectionPoint()
        {
            ErrorCases = new List<ErrorCase>();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string coordinates;

        public string Coordinates
        {
            get { return coordinates; }
            set
            {
                if (coordinates != value)
                {
                    coordinates = value;
                    OnPropertyChanged(nameof(Coordinates));
                }
            }
        }

        [XmlAttribute]
        public string DepthOptionsAsString { get; set; }

        [XmlAttribute]
        public string QuantityOptionsAsString { get; set; }
    }
}