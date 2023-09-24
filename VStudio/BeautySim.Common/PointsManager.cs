using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class PointsManager
    {
        private static PointsManager instance;

        public static PointsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PointsManager();
                }
                return instance;
            }
        }

        private PointsManager()
        {
            InjectionPoints = new List<InjectionPointBase>();
        }

        public List<InjectionPointBase> InjectionPoints { get; set; }

        public void AddInjectionPoint(InjectionPointBase point)
        {
            InjectionPoints.Add(point);
        }
        
        public void PopulateInjectionPointsAndSaveThem(string fileName)
        {
            List<double> qtOptions = new List<double>() { 0, 0.5, 1, 1.5, 2, 2.5, 3 };
            List<double> dpOptions = new List<double>() { 0, 0.5, 1, 1.5, 2, 3, 4 };
            double standardYawMin = -20;
            double standardYawMax = 20;
            double standardPitchMin = -30;
            double standardPitchMax = 30;

            double procerusYawMin = -10;
            double procerusYawMax = 10;
            double procerusPitchMin = -30;
            double procerusPitchMax = 30;

            double corrugatorLeftYawMin = -10;
            double corrugatorLeftYawMax = 10;
            double corrugatorLeftPitchMin = -30;
            double corrugatorLeftPitchMax = 30;

            double corrugatorRightYawMin = -10;
            double corrugatorRightYawMax = 10;
            double corrugatorRightPitchMin = -30;
            double corrugatorRightPitchMax = 30;

            double orbicLeftYawMin = -20;
            double orbicLeftYawMax = 20;
            double orbicLeftPitchMin = 40;
            double orbicLeftPitchMax = 80;

            double orbicRightYawMin = -20;
            double orbicRightYawMax = 20;
            double orbicRightPitchMin = -40;
            double orbicRightPitchMax = -80;

            double noDepth = 0;
            double stDepth = 1;

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_C0, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_L1, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_R1, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_L2, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_R2, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_L3, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_R3, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_L4, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_1_R4, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_C0, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_C0, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L3, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R3, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L4, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R4, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_C0, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L3, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R3, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L4, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R4, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_C0, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_L1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_R1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_L2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_R2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));


            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.P_U0, stDepth, dpOptions, false, procerusYawMin, procerusYawMax, procerusPitchMin, procerusPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.P_L0, stDepth, dpOptions, false, procerusYawMin, procerusYawMax, procerusPitchMin, procerusPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_L1, stDepth, dpOptions, false, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_L2, stDepth, dpOptions, false, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_L3, noDepth, dpOptions, true, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_L4, noDepth, dpOptions, true, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_L5, noDepth, dpOptions, true, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_R1, stDepth, dpOptions, false, corrugatorRightYawMin, corrugatorRightYawMax, corrugatorRightPitchMin, corrugatorRightPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_R2, stDepth, dpOptions, false, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_R3, noDepth, dpOptions, true, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_R4, noDepth, dpOptions, true, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.C_R5, noDepth, dpOptions, true, corrugatorLeftYawMin, corrugatorLeftYawMax, corrugatorLeftPitchMin, corrugatorLeftPitchMax, Enum_AreaDefinition.CENTRAL, qtOptions));


            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.E_L1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.NASAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.E_R1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.NASAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_C0, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_U1, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_U2, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_U3, noDepth, dpOptions, true, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_U4, noDepth, dpOptions, true, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_L1, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_L2, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_L3, noDepth, dpOptions, true, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_1_L4, noDepth, dpOptions, true, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_2_C0, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_2_U1, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_2_U2, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_2_L1, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OR_2_L2, stDepth, dpOptions, false, orbicRightYawMin, orbicRightYawMax, orbicRightPitchMin, orbicRightPitchMax, Enum_AreaDefinition.ORBICULAR_RIGHT, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_C0, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_U1, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_U2, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_U3, noDepth, dpOptions, true, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_U4, noDepth, dpOptions, true, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_L1, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_L2, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_L3, noDepth, dpOptions, true, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_1_L4, noDepth, dpOptions, true, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_2_C0, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_2_U1, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_2_U2, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_2_L1, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.OL_2_L2, stDepth, dpOptions, false, orbicLeftYawMin, orbicLeftYawMax, orbicLeftPitchMin, orbicLeftPitchMax, Enum_AreaDefinition.ORBICULAR_LEFT, qtOptions));

            XmlSerializer serializer = new XmlSerializer(typeof(List<InjectionPointBase>));

            // Serialize to file
            
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(fileStream, InjectionPoints);
                Console.WriteLine($"Serialized to XML file: {fileName}");
            }

        }

        public static List<Enum_PointDefinition> GetEnumSubList(int skip, int take)
        {
            // Convert the enum to an array
            Enum_PointDefinition[] enumValues = (Enum_PointDefinition[])Enum.GetValues(typeof(Enum_PointDefinition));

            // Validate indices
            if (skip < 0 || (skip+take) > enumValues.Length)
            {
                throw new ArgumentException("Invalid indices");
            }

            // Get the sublist
            Enum_PointDefinition[] subArray = enumValues.Skip(skip).Take(take).ToArray();

            return new List<Enum_PointDefinition>(subArray);
        }

        public List<Enum_PointDefinition> GiveMePointsBasedOnArea(Enum_AreaDefinition area)
        {
            switch (area)
            {
                case Enum_AreaDefinition.FRONTAL:
                    return GetEnumSubList(0, 32);
                    break;
                case Enum_AreaDefinition.CENTRAL:
                    return GetEnumSubList(32, 12);
                    break;
                case Enum_AreaDefinition.ORBICULAR_LEFT:
                    return GetEnumSubList(60, 14);
                    break;
                case Enum_AreaDefinition.ORBICULAR_RIGHT:
                    return GetEnumSubList(46, 14);
                    break;
                case Enum_AreaDefinition.NASAL:
                    return GetEnumSubList(44, 2);
                    break;
            }

            return new List<Enum_PointDefinition>();
        }

        public List<InjectionPointBase> LoadInjectionPoints(string pathFilePointsBase)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<InjectionPointBase>));

            // Deserialize from file
            using (FileStream fileStream = new FileStream(pathFilePointsBase, FileMode.Open))
            {
                List<InjectionPointBase> injectionPoints = (List<InjectionPointBase>)deserializer.Deserialize(fileStream);
                return injectionPoints;
            }
        }

        public ObservableCollection<InjectionPoint3D> LoadInjectionPoints3D(string pathFilePointsBase3D)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<InjectionPoint3D>));

            // Deserialize from file
            using (FileStream fileStream = new FileStream(pathFilePointsBase3D, FileMode.Open))
            {
                List<InjectionPoint3D> injectionPoints3D = (List<InjectionPoint3D>)deserializer.Deserialize(fileStream);
                ObservableCollection<InjectionPoint3D> injectionPoints3DColl = new ObservableCollection<InjectionPoint3D>();
                foreach (InjectionPoint3D item in injectionPoints3D)
                {
                    injectionPoints3DColl.Add(item);
                }
                return injectionPoints3DColl;
            }
        }



        public List<StructuredErrors> EvaluateWhatHasBeenDone(List<InjectionPointBase> injectionPointsWorked)
        {

            bool isThis2D = injectionPointsWorked[0] is InjectionPointSpecific2D;
            bool isTHis3D = injectionPointsWorked[0] is InjectionPoint3D;


            if (injectionPointsWorked == null || injectionPointsWorked.Count == 0)
            {
                return null;
            }

            return null;
        }


    }
}
