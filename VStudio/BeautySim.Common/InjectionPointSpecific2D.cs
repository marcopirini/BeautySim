using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class InjectionPointSpecific2D : InjectionPointBase
    {
        public InjectionPointSpecific2D()
        {
        }

        private string coordinates;

        public InjectionPointSpecific2D(InjectionPointBase baseDef, double prescribedQuantity, double x, double y, bool assigned)
        {
            PrescribedQuantity = prescribedQuantity;
            X = x;
            Y = y;
            Assigned = assigned;
            PointDefinition = baseDef.PointDefinition;
            PrescribedDepth = baseDef.PrescribedDepth;
            DepthOptions = baseDef.DepthOptions;
            DepthOptionsAsString = baseDef.DepthOptionsAsString;
            IsError = baseDef.IsError;
            PitchMax = baseDef.PitchMax;
            PitchMin = baseDef.PitchMin;
            AreaDef = baseDef.AreaDef;
            QuantityOptions = baseDef.QuantityOptions;
            QuantityOptionsAsString = baseDef.QuantityOptionsAsString;
            YawMax = baseDef.YawMax;
            YawMin = baseDef.YawMin;
        }

        [XmlAttribute]
        public double ActuallyChosenDepth { get; set; }

        [XmlAttribute]

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
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
        public void CopyAllPropertiesFromBaseExcptPrescribedQuantity(InjectionPointBase bb)
        {
            PointDefinition = bb.PointDefinition;
            PrescribedDepth = bb.PrescribedDepth;
            DepthOptions = bb.DepthOptions;
            DepthOptionsAsString = bb.DepthOptionsAsString;
            IsError = bb.IsError;
            PitchMax = bb.PitchMax;
            PitchMin = bb.PitchMin;
            AreaDef = bb.AreaDef;
            QuantityOptions = bb.QuantityOptions;
            QuantityOptionsAsString = bb.QuantityOptionsAsString;
            YawMax = bb.YawMax;
            YawMin = bb.YawMin;
        }
    }
}