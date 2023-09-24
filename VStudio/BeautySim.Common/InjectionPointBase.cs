using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class InjectionPointBase : INotifyPropertyChanged
    {
        private double actuallyChosenOrPerformedQuantity;
        private bool assigned;
        private double prescribedQuantity;
        public InjectionPointBase()
        {
        }

        public InjectionPointBase(Enum_PointDefinition pointDefinition, double chosenDepth, List<double> depthOptions, bool isError, double pitchMin, double pitchMax, double yawMin, double yawMax, Enum_AreaDefinition areaDef, List<double> quantityOptions)
        {
            PointDefinition = pointDefinition;
            PrescribedDepth = chosenDepth;
            DepthOptions = depthOptions;
            IsError = isError;
            PitchMax = pitchMax;
            PitchMin = pitchMin;
            AreaDef = areaDef;
            QuantityOptions = quantityOptions;
            YawMax = yawMax;
            YawMin = yawMin;
            Assigned = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute]
        public double ActuallyChosenOrPerformedQuantity
        {
            get { return actuallyChosenOrPerformedQuantity; }
            set
            {
                if (actuallyChosenOrPerformedQuantity != value)
                {
                    actuallyChosenOrPerformedQuantity = value;
                    OnPropertyChanged(nameof(ActuallyChosenOrPerformedQuantity));
                }
            }
        }

        [XmlAttribute]
        public Enum_AreaDefinition AreaDef { get; set; }

        [XmlAttribute]
        public bool Assigned
        {
            get { return assigned; }
            set
            {
                if (assigned != value)
                {
                    assigned = value;
                    OnPropertyChanged(nameof(Assigned));
                }
            }
        }

        [XmlArray]
        public List<double> DepthOptions { get; set; }

        [XmlAttribute]
        public string DepthOptionsAsString { get; set; }

        [XmlAttribute]
        public bool IsError { get; set; }

        [XmlAttribute]
        public double PitchMax { get; set; }

        [XmlAttribute]
        public double PitchMin { get; set; }

        [XmlAttribute]
        public Enum_PointDefinition PointDefinition { get; set; }
        [XmlAttribute]
        public double PrescribedDepth { get; set; }
        [XmlAttribute]
        public double PrescribedQuantity
        {
            get { return prescribedQuantity; }
            set
            {
                if (prescribedQuantity != value)
                {
                    prescribedQuantity = value;
                    OnPropertyChanged(nameof(PrescribedQuantity));
                }
            }
        }

        [XmlArray]
        public List<double> QuantityOptions { get; set; }

        [XmlAttribute]
        public string QuantityOptionsAsString { get; set; }

        [XmlAttribute]
        public double YawMax { get; set; }

        [XmlAttribute]
        public double YawMin { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}