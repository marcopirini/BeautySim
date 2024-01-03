using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows.Markup;
using System.Windows.Media.Media3D.Converters;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

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

        private double minDistanceFromNeighbours;
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
        private double numReleases;

        [XmlAttribute]
        public double NumReleases
        {
            get { return numReleases; }
            set
            {
                if (numReleases != value)
                {
                    numReleases = value;
                    OnPropertyChanged(nameof(NumReleases));
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

        [XmlAttribute]
        public double MinDistanceFromNeighbours
        {
            get { return minDistanceFromNeighbours; }
            set
            {
                if (minDistanceFromNeighbours != value)
                {
                    minDistanceFromNeighbours = value;
                    OnPropertyChanged(nameof(MinDistanceFromNeighbours));
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
            MinDistanceFromNeighbours = 0;
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
            YawMax = bb.YawMax;
            YawMin = bb.YawMin;
            MinDistanceFromNeighbours = 0;
        }

        public void RefreshQuantities()
        {
            double num = 0;
            foreach (ActualRelease actualRelease in ActualReleases)
            {
                num += (actualRelease.InjectedQuantity + actualRelease.ArtificiallyInjectedQuantity);
            }
            
            ActuallyChosenOrPerformedQuantity = num;
            NumReleases = ActualReleases.Count;
        }

        public double GiveMeMinEntranceYaw()
        {
            double minYaw = 1000;
            foreach (ActualRelease actualRelease in ActualReleases)
            {
                if ((actualRelease.YawEntrance < minYaw) && ((actualRelease.InjectedQuantity + actualRelease.ArtificiallyInjectedQuantity) > 0))    
                {
                    minYaw = actualRelease.YawEntrance;
                }
            }
            return minYaw;  
        }

        public double GiveMeMaxEntranceYaw()
        {
            double maxYaw = -1000;
            foreach (ActualRelease actualRelease in ActualReleases)
            {
                if ((actualRelease.YawEntrance > maxYaw) && ((actualRelease.InjectedQuantity + actualRelease.ArtificiallyInjectedQuantity) > 0))
                {
                    maxYaw = actualRelease.YawEntrance;
                }
            }
            return maxYaw;
        }

        public double GiveMeMinEntrancePitch()
        {
            double minPitch = 1000;
            foreach (ActualRelease actualRelease in ActualReleases)
            {
                if ((actualRelease.PitchEntrance < minPitch) && ((actualRelease.InjectedQuantity + actualRelease.ArtificiallyInjectedQuantity) > 0))
                {
                    minPitch = actualRelease.PitchEntrance;
                }
            }
            return minPitch;
        }

        public double GiveMeMaxEntrancePitch()
        {
            double maxPitch = -1000;
            foreach (ActualRelease actualRelease in ActualReleases)
            {
                if ((actualRelease.PitchEntrance > maxPitch) && ((actualRelease.InjectedQuantity + actualRelease.ArtificiallyInjectedQuantity) > 0))
                {
                    maxPitch = actualRelease.PitchEntrance;
                }
            }
            return maxPitch;
        }

        public bool HasBeedTargeted()
        {
            return actualReleases.Count > 0;
        }

        public System.Windows.Media.Media3D.Point3D GetPoint()
        {
            return new System.Windows.Media.Media3D.Point3D(X, Y, Z);
        }

        public Point3D GetRotoTransatedPoint(Transform3DGroup tsg)
        {
            Point3D ptd=tsg.Transform(GetPoint());
            return ptd;
        }
    }
}