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
    public class InjectionPoint3DCalib : InjectionPoint3D
    {


        public InjectionPoint3DCalib() : base()
        {
            
        }

        public InjectionPoint3DCalib(InjectionPointBase baseDef, double chosenQuantity, double x, double y, double z, bool assigned) : base(baseDef, chosenQuantity, x, y, z, assigned)
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

        public InjectionPoint3DCalib(InjectionPoint3D basePoint)
        {
            PrescribedQuantity= basePoint.PrescribedQuantity;
            X = basePoint.X;
            Y = basePoint.Y;
            Z = basePoint.Z;
            Assigned = basePoint.Assigned;
            PointDefinition = basePoint.PointDefinition;
            PrescribedDepth = basePoint.PrescribedDepth;
            DepthOptions = basePoint.DepthOptions;
            DepthOptionsAsString = basePoint.DepthOptionsAsString;
            IsError = basePoint.IsError;
            PitchMax = basePoint.PitchMax;
            PitchMin = basePoint.PitchMin;
            AreaDef = basePoint.AreaDef;
            QuantityOptions = basePoint.QuantityOptions;
            QuantityOptionsAsString = basePoint.QuantityOptionsAsString;
            YawMax = basePoint.YawMax;
            YawMin = basePoint.YawMin;
            AreaDef = basePoint.AreaDef;
            QuantityOptions = basePoint.QuantityOptions;
            QuantityOptionsAsString = basePoint.QuantityOptionsAsString;
            YawMax = basePoint.YawMax;
            YawMin = basePoint.YawMin;
        }

        private bool assignedCalibration;
        private double zAssigned;
        private double yAssigned;
        private double xAssigned;


        [XmlAttribute]
        public double XAssigned
        {
            get { return xAssigned; }
            set
            {
                if (xAssigned != value)
                {
                    xAssigned = value;
                    OnPropertyChanged(nameof(XAssigned));
                }
            }
        }

        [XmlAttribute]
        public double YAssigned
        {
            get { return yAssigned; }
            set
            {
                if (yAssigned != value)
                {
                    yAssigned = value;
                    OnPropertyChanged(nameof(YAssigned));
                }
            }
        }

        [XmlAttribute]
        public double ZAssigned
        {
            get { return zAssigned; }
            set
            {
                if (zAssigned != value)
                {
                    zAssigned = value;
                    OnPropertyChanged(nameof(ZAssigned));
                }
            }
        }


        [XmlAttribute]
        public bool AssignedCalibration
        {
            get { return assignedCalibration; }
            set
            {
                if (assignedCalibration != value)
                {
                    assignedCalibration = value;
                    OnPropertyChanged(nameof(AssignedCalibration));
                }
            }
        }

        public System.Windows.Media.Media3D.Point3D GetPointAssigned()
        {
            return new System.Windows.Media.Media3D.Point3D(XAssigned, YAssigned, ZAssigned);
        }

    }
}