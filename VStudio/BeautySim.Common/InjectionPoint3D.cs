using System.Collections.Generic;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class InjectionPoint3D : InjectionPointBase
    {
        private List<ActualRelease> actualReleases;

        private string coordinates;

        private double targetedX;

        private double targetedY;

        private double targetedZ;

        private double x;

        private double y;

        private double z;

        public InjectionPoint3D() : base()
        {
            actualReleases = new List<ActualRelease>();
        }

        public InjectionPoint3D(InjectionPointBase baseDef, double chosenQuantity, double x, double y, double z, bool assigned)
        {
            PrescribedQuantity = chosenQuantity;
            X = x;
            Y = y;
            Z = z;
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

        [XmlArray]
        public List<ActualRelease> ActualReleases
        {
            get { return actualReleases; }
            set
            {
                if (actualReleases != value)
                {
                    actualReleases = value;
                    OnPropertyChanged(nameof(ActualReleases));
                }
            }
        }

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
        public double X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        [XmlAttribute]
        public double Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        [XmlAttribute]
        public double Z
        {
            get { return z; }
            set
            {
                if (z != value)
                {
                    z = value;
                    OnPropertyChanged(nameof(Z));
                }
            }
        }
        public void CopyAllPropertiesFrom2DSpecific(InjectionPointSpecific2D bb)
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
            Assigned = bb.Assigned;
        }

        public void CopyAllPropertiesFromBase(InjectionPointBase bb)
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
            PrescribedQuantity = bb.PrescribedQuantity;
            YawMax = bb.YawMax;
            YawMin = bb.YawMin;
        }

        public void CalculateTotalQuantity()
        {
            double num = 0;
            foreach (ActualRelease actualRelease in ActualReleases)
            {
                num += actualRelease.InjectedQuantity;
            }
            
            ActuallyChosenOrPerformedQuantity = num;
        }

        public bool HasBeedTargeted()
        {
            return actualReleases.Count > 0;
        }
    }
}