using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    public class PointsManager
    {
        private static PointsManager instance;

        private double limitBigErrorOnCorrugators = 6;
        private double limitBigErrorOnFrontal = 6;
        private double limitBigErrorOnOrbicolar = 6;
        private double limitTooMuchOnCorrugators = 6;
        private PointsManager()
        {
            InjectionPoints = new List<InjectionPointBase>();
        }

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

        public List<InjectionPointBase> InjectionPoints { get; set; }

        public static List<Enum_PointDefinition> GetEnumSubList(int skip, int take)
        {
            // Convert the enum to an array
            Enum_PointDefinition[] enumValues = (Enum_PointDefinition[])Enum.GetValues(typeof(Enum_PointDefinition));

            // Validate indices
            if (skip < 0 || (skip + take) > enumValues.Length)
            {
                throw new ArgumentException("Invalid indices");
            }

            // Get the sublist
            Enum_PointDefinition[] subArray = enumValues.Skip(skip).Take(take).ToArray();

            return new List<Enum_PointDefinition>(subArray);
        }

        public void AddInjectionPoint(InjectionPointBase point)
        {
            InjectionPoints.Add(point);
        }

        public List<AnalysResult> EvaluateWhatHasBeenDone(List<InjectionPointBase> injectionPointsWorked, Enum_AreaDefinition area, bool forceall)
        {
            bool isThis2D = injectionPointsWorked[0] is InjectionPointSpecific2D;
            bool isThis3D = injectionPointsWorked[0] is InjectionPoint3D;
            List<Enum_AnaysisVariablesToCheck> verifiedErrorsSecondPass = new List<Enum_AnaysisVariablesToCheck>();
            if (!forceall)
            {

                if (injectionPointsWorked == null || injectionPointsWorked.Count == 0)
                {
                    return null;
                }

                List<Enum_AnaysisVariablesToCheck> verifiedErrors = new List<Enum_AnaysisVariablesToCheck>();
                double error_inj_frontal_l1_right = 0;
                double error_inj_frontal_l1_left = 0;
                double error_inj_corrugators_left = 0;
                double error_inj_corrugators_right = 0;
                double error_inj_orbicolar_upperRight = 0;
                double error_inj_orbicolar_lowerRight = 0;
                double error_inj_orbicolar_upperLeft = 0;
                double error_inj_orbicolar_lowerLeft = 0;

                double prescribed_inj_frontal_right = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            //Enum_PointDefinition.F_2_R1,
                            //Enum_PointDefinition.F_2_R2,
                            //Enum_PointDefinition.F_2_R3,
                            //Enum_PointDefinition.F_2_R4,
                            Enum_PointDefinition.F_3_R1,
                            //Enum_PointDefinition.F_3_R2,
                            //Enum_PointDefinition.F_3_R3,
                            //Enum_PointDefinition.F_3_R4,
                            Enum_PointDefinition.F_4_R1,
                            Enum_PointDefinition.F_4_R2
            });

                double prescribed_inj_frontal_left = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                            //Enum_PointDefinition.F_2_L1,
                            //Enum_PointDefinition.F_2_L2,
                            //Enum_PointDefinition.F_2_L3,
                            //Enum_PointDefinition.F_2_L4,
                            Enum_PointDefinition.F_3_L1,
                            //Enum_PointDefinition.F_3_L2,
                            //Enum_PointDefinition.F_3_L3,
                            //Enum_PointDefinition.F_3_L4,
                            Enum_PointDefinition.F_4_L1,
                            Enum_PointDefinition.F_4_L2
            });

                double inj_frontal_right = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            //Enum_PointDefinition.F_2_R1,
                            //Enum_PointDefinition.F_2_R2,
                            //Enum_PointDefinition.F_2_R3,
                            //Enum_PointDefinition.F_2_R4,
                            Enum_PointDefinition.F_3_R1,
                            //Enum_PointDefinition.F_3_R2,
                            //Enum_PointDefinition.F_3_R3,
                            //Enum_PointDefinition.F_3_R4,
                            Enum_PointDefinition.F_4_R1,
                            Enum_PointDefinition.F_4_R2
            });

                double inj_frontal_left = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                            //Enum_PointDefinition.F_2_L1,
                            //Enum_PointDefinition.F_2_L2,
                            //Enum_PointDefinition.F_2_L3,
                            //Enum_PointDefinition.F_2_L4,
                            Enum_PointDefinition.F_3_L1,
                            //Enum_PointDefinition.F_3_L2,
                            //Enum_PointDefinition.F_3_L3,
                            //Enum_PointDefinition.F_3_L4,
                            Enum_PointDefinition.F_4_L1,
                            Enum_PointDefinition.F_4_L2
            });

                double prescribed_inj_corrugators_left = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                            Enum_PointDefinition.C_L1,
                            Enum_PointDefinition.C_L2,
                            Enum_PointDefinition.C_L3});

                double prescribed_inj_corrugators_right = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {               Enum_PointDefinition.C_R1,
                            Enum_PointDefinition.C_R2,
                            Enum_PointDefinition.C_R3});

                double inj_corrugators_left = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                            Enum_PointDefinition.C_L1,
                            Enum_PointDefinition.C_L2,
                            Enum_PointDefinition.C_L3});

                double inj_corrugators_right = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
             {
                            Enum_PointDefinition.C_R1,
                            Enum_PointDefinition.C_R2,
                            Enum_PointDefinition.C_R3});

                double prescribed_inj_orbicolar_upperRight = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OR_1_U1,
              Enum_PointDefinition.OR_1_U2});

                double prescribed_inj_orbicolar_lowerRight = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OR_1_L1,
              Enum_PointDefinition.OR_1_L2});

                double prescribed_inj_orbicolar_upperLeft = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OL_1_U1,
              Enum_PointDefinition.OL_1_U2});

                double prescribed_inj_orbicolar_lowerLeft = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
             { Enum_PointDefinition.OL_1_L1,
               Enum_PointDefinition.OL_1_L2});

                double inj_orbicolar_upperRight = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OR_1_U1,
              Enum_PointDefinition.OR_1_U2});

                double inj_orbicolar_lowerRight = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OR_1_L1,
              Enum_PointDefinition.OR_1_L2});

                double inj_orbicolar_upperLeft = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OL_1_U1,
              Enum_PointDefinition.OL_1_U2});

                double inj_orbicolar_lowerLeft = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.OL_1_L1,
              Enum_PointDefinition.OL_1_L2});

                double prescribed_inj_procerus = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.P_L0,
                         Enum_PointDefinition.P_U0});

                double inj_procerus = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            { Enum_PointDefinition.P_L0,
                                    Enum_PointDefinition.P_U0});

                double prescribed_inj_orbicolar_right = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                Enum_PointDefinition.OR_1_C0,
                Enum_PointDefinition.OR_1_U1,
                         Enum_PointDefinition.OR_1_U2,
                         Enum_PointDefinition.OR_1_L1,
                         Enum_PointDefinition.OR_1_L2});

                double prescribed_inj_orbicolar_left = GiveMePrescribedInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                Enum_PointDefinition.OL_1_C0,
                Enum_PointDefinition.OL_1_U1,
                         Enum_PointDefinition.OL_1_U2,
                         Enum_PointDefinition.OL_1_L1,
                         Enum_PointDefinition.OL_1_L2});
                double inj_orbicolar_right = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                Enum_PointDefinition.OR_1_C0,
                Enum_PointDefinition.OR_1_U1,
                         Enum_PointDefinition.OR_1_U2,
                         Enum_PointDefinition.OR_1_L1,
                         Enum_PointDefinition.OR_1_L2});
                double inj_orbicolar_left = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
            {
                Enum_PointDefinition.OL_1_C0,
                Enum_PointDefinition.OL_1_U1,
                         Enum_PointDefinition.OL_1_U2,
                         Enum_PointDefinition.OL_1_L1,
                         Enum_PointDefinition.OL_1_L2});

                foreach (Enum_AnaysisVariablesToCheck errorVariable in Enum.GetValues(typeof(Enum_AnaysisVariablesToCheck)))
                {
                    switch (errorVariable)
                    {
                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_RIGHT:

                            error_inj_frontal_l1_right = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.F_1_C0,
                            Enum_PointDefinition.F_1_R1,
                            Enum_PointDefinition.F_1_R2,
                            Enum_PointDefinition.F_1_R3,
                            Enum_PointDefinition.F_1_R4,
                            Enum_PointDefinition.F_2_C0,
                            Enum_PointDefinition.F_2_R1,
                            Enum_PointDefinition.F_2_R2,
                            Enum_PointDefinition.F_2_R3,
                            Enum_PointDefinition.F_2_R4,
                            Enum_PointDefinition.F_3_R2,
                            Enum_PointDefinition.F_3_R3,
                            Enum_PointDefinition.F_3_R4







                        });
                            if (error_inj_frontal_l1_right > 0)
                            {
                                if (error_inj_frontal_l1_right > limitBigErrorOnFrontal)
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_RIGHT);
                                }
                                else
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_RIGHT_MAX);
                                }
                            }

                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_LEFT:

                            error_inj_frontal_l1_left = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.F_1_C0,
                            Enum_PointDefinition.F_1_L1,
                            Enum_PointDefinition.F_1_L2,
                            Enum_PointDefinition.F_1_L3,
                            Enum_PointDefinition.F_1_L4,
                            Enum_PointDefinition.F_2_C0,
                            Enum_PointDefinition.F_2_L1,
                            Enum_PointDefinition.F_2_L2,
                            Enum_PointDefinition.F_2_L3,
                            Enum_PointDefinition.F_2_L4,
                            Enum_PointDefinition.F_3_L2,
                            Enum_PointDefinition.F_3_L3,
                            Enum_PointDefinition.F_3_L4




                        });
                            if (error_inj_frontal_l1_left > 0)
                            {
                                if (error_inj_frontal_l1_left > limitBigErrorOnFrontal)
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_LEFT);
                                }
                                else
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_LEFT_MAX);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT:
                            error_inj_corrugators_left = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.C_L3,
                            Enum_PointDefinition.C_L4,
                            Enum_PointDefinition.C_L5});
                            if (error_inj_corrugators_left > 0)
                            {
                                if (error_inj_corrugators_left > limitBigErrorOnCorrugators)
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT_MAX);
                                }
                                else
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT);
                                }
                            }

                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT:
                            error_inj_corrugators_right = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.C_R3,
                            Enum_PointDefinition.C_R4,
                            Enum_PointDefinition.C_R5});
                            if (error_inj_corrugators_right > 0)
                            {
                                if (error_inj_corrugators_right > limitBigErrorOnCorrugators)
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT_MAX);
                                }
                                else
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT:
                            error_inj_orbicolar_upperRight = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.OR_1_U3,
                            Enum_PointDefinition.OR_1_U4});
                            if (error_inj_orbicolar_upperRight > 0)
                            {
                                if (error_inj_orbicolar_upperRight > limitBigErrorOnOrbicolar)
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT_MAX);
                                }
                                else
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_RIGHT:
                            error_inj_orbicolar_lowerRight = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.OR_1_L3,
                            Enum_PointDefinition.OR_1_L4});
                            if (error_inj_orbicolar_lowerRight > 0)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT:
                            error_inj_orbicolar_upperLeft = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.OL_1_U3,
                            Enum_PointDefinition.OL_1_U4});
                            if (error_inj_orbicolar_upperLeft > 0)
                            {
                                if (error_inj_orbicolar_upperLeft > limitBigErrorOnOrbicolar)
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT_MAX);
                                }
                                else
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_LEFT:
                            error_inj_orbicolar_lowerLeft = GiveMeSumInjection(injectionPointsWorked, new List<Enum_PointDefinition>
                        {
                            Enum_PointDefinition.OL_1_L3,
                            Enum_PointDefinition.OL_1_L4});
                            if (error_inj_orbicolar_lowerLeft > 0)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_LEFT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_LEFT:
                            if ((isThis3D) && (area == Enum_AreaDefinition.CENTRAL))
                            {
                                InjectionPoint3D injectionPoint3D1 = injectionPointsWorked.Where(x => x.PointDefinition == Enum_PointDefinition.P_L0).FirstOrDefault() as InjectionPoint3D;
                                InjectionPoint3D injectionPoint3D2 = injectionPointsWorked.Where(x => x.PointDefinition == Enum_PointDefinition.P_U0).FirstOrDefault() as InjectionPoint3D;
                                if ((injectionPoint3D1.GiveMeMinEntranceYaw() < injectionPoint3D1.YawMin) || (injectionPoint3D2.GiveMeMinEntranceYaw() < injectionPoint3D2.YawMin))
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_LEFT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_RIGHT:
                            if ((isThis3D) && (area == Enum_AreaDefinition.CENTRAL))
                            {
                                InjectionPoint3D injectionPoint3D1 = injectionPointsWorked.Where(x => x.PointDefinition == Enum_PointDefinition.P_L0).FirstOrDefault() as InjectionPoint3D;
                                InjectionPoint3D injectionPoint3D2 = injectionPointsWorked.Where(x => x.PointDefinition == Enum_PointDefinition.P_U0).FirstOrDefault() as InjectionPoint3D;
                                if ((injectionPoint3D1.GiveMeMaxEntranceYaw() > injectionPoint3D1.YawMax) || (injectionPoint3D2.GiveMeMaxEntranceYaw() > injectionPoint3D2.YawMax))
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_RIGHT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_UP:
                            if ((isThis3D) && (area == Enum_AreaDefinition.CENTRAL))
                            {
                                InjectionPoint3D injectionPoint3D2 = injectionPointsWorked.Where(x => x.PointDefinition == Enum_PointDefinition.P_U0).FirstOrDefault() as InjectionPoint3D;
                                if ((injectionPoint3D2.GiveMeMaxEntrancePitch() > injectionPoint3D2.PitchMax))
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_UP);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_RIGHT:

                            if ((prescribed_inj_frontal_right > 0) && (prescribed_inj_frontal_left > 0))
                            {
                                if (inj_frontal_right / prescribed_inj_frontal_right > (1.5 * inj_frontal_left / prescribed_inj_frontal_left))
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_RIGHT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_LEFT:
                            if ((prescribed_inj_frontal_right > 0) && (prescribed_inj_frontal_left > 0))
                            {
                                if (inj_frontal_left / prescribed_inj_frontal_left > (1.5 * inj_frontal_right / prescribed_inj_frontal_right))
                                {
                                    verifiedErrors.Add(Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_LEFT);
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT:

                            if (prescribed_inj_corrugators_right > 0)
                            {
                                if (inj_corrugators_right / prescribed_inj_corrugators_right > 1.5)
                                {
                                    if (inj_corrugators_right / prescribed_inj_corrugators_right < 2)
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT);
                                    }
                                    else
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT_MAX);
                                    }
                                }
                            }
                            else
                            {
                                if (inj_corrugators_right > 0)
                                {
                                    if (inj_corrugators_right < limitTooMuchOnCorrugators)
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT);
                                    }
                                    else
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT_MAX);
                                    }
                                }
                            }

                            break;

                        case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT:
                            if (prescribed_inj_corrugators_left > 0)
                            {
                                if (inj_corrugators_left / prescribed_inj_corrugators_left > 1.5)
                                {
                                    if (inj_corrugators_left / prescribed_inj_corrugators_left < 2)
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT);
                                    }
                                    else
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT_MAX);
                                    }
                                }
                            }
                            else
                            {
                                if (inj_corrugators_left > 0)
                                {
                                    if (inj_corrugators_left < limitTooMuchOnCorrugators)
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT);
                                    }
                                    else
                                    {
                                        verifiedErrors.Add(Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT_MAX);
                                    }
                                }
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_FRONTAL_RIGHT:
                            if (inj_frontal_right < 0.5 * prescribed_inj_frontal_right)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_FRONTAL_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_FRONTAL_LEFT:
                            if (inj_frontal_left < 0.5 * prescribed_inj_frontal_left)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_FRONTAL_LEFT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_LEFT:
                            if (inj_corrugators_left < 0.5 * prescribed_inj_corrugators_left)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_CORRUGATORS_LEFT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_RIGHT:
                            if (inj_corrugators_right < 0.5 * prescribed_inj_corrugators_right)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_CORRUGATORS_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_PROCERUS:
                            if (inj_procerus < 0.5 * prescribed_inj_procerus)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_PROCERUS);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_RIGHT:
                            if (inj_orbicolar_right < 0.5 * prescribed_inj_orbicolar_right)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_ORBICULARIS_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_LEFT:
                            if (inj_orbicolar_left < 0.5 * prescribed_inj_orbicolar_left)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.O_ORBICULARIS_LEFT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_FRONTAL_RIGHT:
                            if (inj_frontal_right > 1.5 * prescribed_inj_frontal_right)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_FRONTAL_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_FRONTAL_LEFT:
                            if (inj_frontal_left > 1.5 * prescribed_inj_frontal_left)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_FRONTAL_LEFT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_LEFT:
                            if (inj_corrugators_left > 1.5 * prescribed_inj_corrugators_left)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_CORRUGATORS_LEFT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_RIGHT:
                            if (inj_corrugators_right > 1.5 * prescribed_inj_corrugators_right)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_CORRUGATORS_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_PROCERUS:
                            if (inj_procerus > 1.5 * prescribed_inj_procerus)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_PROCERUS);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_RIGHT:
                            if (inj_orbicolar_right > 1.5 * prescribed_inj_orbicolar_right)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_ORBICULARIS_RIGHT);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_LEFT:
                            if (inj_orbicolar_left > 1.5 * prescribed_inj_orbicolar_left)
                            {
                                verifiedErrors.Add(Enum_AnaysisVariablesToCheck.C_ORBICULARIS_LEFT);
                            }
                            break;

                        default:
                            break;
                    }
                }

                
                foreach (Enum_AnaysisVariablesToCheck errFirst in verifiedErrors)
                {
                    switch (errFirst)
                    {
                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_RIGHT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_LEFT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_FRONTAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }

                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_LEFT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_RIGHT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_FRONTAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_RIGHT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_LEFT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_FRONTAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_LEFT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_RIGHT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_FRONTAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_CORRUGATORS_LATERAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_CORRUGATORS_LATERAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_CORRUGATORS_LATERAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_CORRUGATORS_LATERAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_RIGHT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_LEFT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_LEFT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_RIGHT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_UP:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.MB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.MB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.MHB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT_MAX:
                            if (verifiedErrors.Contains(Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT_MAX))
                            {
                                verifiedErrorsSecondPass.Add(Enum_AnaysisVariablesToCheck.MHB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_MAX);
                            }
                            else
                            {
                                verifiedErrorsSecondPass.Add(errFirst);
                            }
                            break;

                        case Enum_AnaysisVariablesToCheck.O_FRONTAL_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.O_FRONTAL_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.O_PROCERUS:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_FRONTAL_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_FRONTAL_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_PROCERUS:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_RIGHT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_LEFT:
                            verifiedErrorsSecondPass.Add(errFirst);
                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                Array aaa = Enum.GetValues(typeof(Enum_AnaysisVariablesToCheck));
                verifiedErrorsSecondPass = aaa.Cast<Enum_AnaysisVariablesToCheck>().ToList();
            }





            // Using HashSet to remove duplicates
            HashSet<Enum_AnaysisVariablesToCheck> uniqueCases = new HashSet<Enum_AnaysisVariablesToCheck>(verifiedErrorsSecondPass);

            // Converting back to List if needed
            List<Enum_AnaysisVariablesToCheck> verifiedErrorsSecondPassNoDuplicates = new List<Enum_AnaysisVariablesToCheck>(uniqueCases);

            List<AnalysResult> errorCases = new List<AnalysResult>();
            foreach (Enum_AnaysisVariablesToCheck it in verifiedErrorsSecondPassNoDuplicates)
            {
                AnalysResult errorCase = new AnalysResult(it);
                if (errorCase.WhatYouDidDescription != "")
                {
                    errorCases.Add(errorCase);
                }
            }

            List<AnalysResult> errorCasesFiltered = FilterErrorCasesBasedOnArea(errorCases, area);

            if (errorCasesFiltered.Count == 0)
            {
                errorCasesFiltered.Add(GiveMeNoError(errorCasesFiltered, area));
            }
            //PIRINI 20240103
            //return errorCases;
            return errorCasesFiltered;
            
        }

        public List<Enum_PointDefinition> GiveMePointsBasedOnArea(List<Enum_AreaDefinition> areas)
        {
            List<Enum_PointDefinition> toRet = new List<Enum_PointDefinition>();
            List<Enum_PointDefinition> toRetGlobal = new List<Enum_PointDefinition>();
            foreach (Enum_AreaDefinition area in areas)
            {

                switch (area)
                {
                    case Enum_AreaDefinition.FRONTAL:
                        toRet = GetEnumSubList(0, 34);
                        break;

                    case Enum_AreaDefinition.CENTRAL:
                        toRet = GetEnumSubList(34, 12);
                        break;

                    case Enum_AreaDefinition.ORBICULAR_LEFT:
                        toRet = GetEnumSubList(62, 14);
                        break;

                    case Enum_AreaDefinition.ORBICULAR_RIGHT:
                        toRet = GetEnumSubList(48, 14);
                        break;

                    case Enum_AreaDefinition.NASAL:
                        toRet = GetEnumSubList(46, 2);
                        break;
                }

                foreach (Enum_PointDefinition item in toRet)
                {
                    toRetGlobal.Add(item);
                }
            }

            return toRetGlobal;
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

        public void PopulateInjectionPointsAndSaveThem(string fileName)
        {
            List<double> qtOptions = new List<double>() { 0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6 };
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

            double noDepth = 1;
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

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_C0, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L1, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R1, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L2, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R2, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L3, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R3, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_L4, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_2_R4, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_C0, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L2, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R2, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L3, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R3, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_L4, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_3_R4, stDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_C0, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_L1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_R1, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_L2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_R2, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_L3, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.F_4_R3, stDepth, dpOptions, false, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.FRONTAL, qtOptions));

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

        public void PopulateInjectionPointsCalibAndSaveThem(string pathFilePointsBaseCalib)
        {
            List<double> qtOptions = new List<double>() { 0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6 };
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

            double noDepth = 1;
            double stDepth = 1;

            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.RE_EXT, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.RE_INT, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.LE_EXT, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.LE_INT, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.NOSEPOINT, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.RL_CORNER, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));
            InjectionPoints.Add(new InjectionPointBase(Enum_PointDefinition.LL_CORNER, noDepth, dpOptions, true, standardYawMin, standardYawMax, standardPitchMin, standardPitchMax, Enum_AreaDefinition.CALIB, qtOptions));

            XmlSerializer serializer = new XmlSerializer(typeof(List<InjectionPointBase>));

            // Serialize to file

            using (FileStream fileStream = new FileStream(pathFilePointsBaseCalib, FileMode.Create))
            {
                serializer.Serialize(fileStream, InjectionPoints);
                Console.WriteLine($"Serialized to XML file: {pathFilePointsBaseCalib}");
            }

        }

        private List<AnalysResult> FilterErrorCasesBasedOnArea(List<AnalysResult> errors, Enum_AreaDefinition area)
        {
            List<AnalysResult> errorCasesFiltered = new List<AnalysResult>();
            for (int i = 0; i < errors.Count; i++)
            {
                if ((area == Enum_AreaDefinition.FRONTAL) || (area == Enum_AreaDefinition.CENTRAL))
                {
                    if (errors[i].AnalysisArea == area)
                    {
                        errorCasesFiltered.Add(errors[i]);
                    }
                }
                if (area == Enum_AreaDefinition.ORBICULAR_LEFT)
                {
                    if ((errors[i].AnalysisArea == Enum_AreaDefinition.ORBICULAR_LEFT) || (errors[i].AnalysisArea == Enum_AreaDefinition.ORBICULAR))
                    {
                        errorCasesFiltered.Add(errors[i]);
                    }
                }
                if (area == Enum_AreaDefinition.ORBICULAR_RIGHT)
                {
                    if ((errors[i].AnalysisArea == Enum_AreaDefinition.ORBICULAR_RIGHT) || (errors[i].AnalysisArea == Enum_AreaDefinition.ORBICULAR))
                    {
                        errorCasesFiltered.Add(errors[i]);
                    }
                }
            }

            return errorCasesFiltered;
        }

        private AnalysResult GiveMeNoError(List<AnalysResult> errorCasesFiltered, Enum_AreaDefinition area)
        {
            switch (area)
            {
                case Enum_AreaDefinition.FRONTAL:

                    AnalysResult result = new AnalysResult(Enum_AnaysisVariablesToCheck.COR_FRONTAL);
                    return result;

                case Enum_AreaDefinition.CENTRAL:

                    AnalysResult result1 = new AnalysResult(Enum_AnaysisVariablesToCheck.COR_CENTRAL);
                    return result1;

                    break;

                case Enum_AreaDefinition.ORBICULAR_LEFT:

                    AnalysResult result2 = new AnalysResult(Enum_AnaysisVariablesToCheck.COR_ORBICULAR_LEFT);
                    return result2;

                    break;

                case Enum_AreaDefinition.ORBICULAR_RIGHT:

                    AnalysResult result3 = new AnalysResult(Enum_AnaysisVariablesToCheck.COR_ORBICULAR_RIGHT);
                    return result3;

                    break;

                case Enum_AreaDefinition.NASAL:
                    return null;
                    break;

                case Enum_AreaDefinition.ORBICULAR:
                    return null;
                    break;

                default:
                    return null;
                    break;
            }
        }
        private double GiveMePrescribedInjection(List<InjectionPointBase> injectionPointsWorked, List<Enum_PointDefinition> enum_PointDefinitions)
        {
            double sum = 0;
            foreach (Enum_PointDefinition item in enum_PointDefinitions)
            {
                var c = from db in injectionPointsWorked
                        where db.PointDefinition == item
                        select db;
                if (c.ToList().Count > 0)
                {
                    sum += c.ToList()[0].PrescribedQuantity;
                }
            }
            return sum;
        }

        private double GiveMeSumInjection(List<InjectionPointBase> injectionPointsWorked, List<Enum_PointDefinition> enum_PointDefinitions)
        {
            double sum = 0;
            foreach (Enum_PointDefinition item in enum_PointDefinitions)
            {
                var c = from db in injectionPointsWorked
                        where db.PointDefinition == item
                        select db;
                if (c.ToList().Count > 0)
                {
                    sum += c.ToList()[0].ActuallyChosenOrPerformedQuantity;
                }
            }
            return sum;
        }
    }
}