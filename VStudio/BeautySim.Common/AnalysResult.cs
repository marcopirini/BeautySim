using System;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    [Serializable]
    public class AnalysResult
    {
        [XmlArray]
        public Enum_AnaysisVariablesToCheck AnalyisCondition { get; set; }

        [XmlArray]
        public Enum_AnalysisConsequences AnalysisConsequence { get; set; }

        [XmlAttribute]
        public string WhatYouDidDescription { get; set; }

        [XmlAttribute]
        public string WhatWillBeTheConsequence { get; set; }

        [XmlAttribute]
        public string AnalysisImageName { get; set; }

        [XmlAttribute]
        public Enum_AreaDefinition AnalysisArea { get; set; }

        [XmlAttribute]
        public Enum_ScoreEffect ScoreEffect { get; set; }

        public AnalysResult(Enum_AnaysisVariablesToCheck it)
        {
            AnalyisCondition = it;
            switch (it)
            {
                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R;
                    WhatYouDidDescription = "You injected botox too low on right frontal muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_R";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_FRONTAL_L1_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L;
                    WhatYouDidDescription = "You injected botox too low on left frontal muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_L";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L;
                    WhatYouDidDescription = "You injected botox too lateral on left corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_L";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R;
                    WhatYouDidDescription = "You injected botox too lateral on right corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_R";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_UpperEyelid_R;
                    WhatYouDidDescription = "You injected botox too proximal on right upper orbicularis muscle area.";
                    AnalysisImageName = "Error_Ptosis_UpperEyelid_R";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_RIGHT;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_LowerEyelid_R;
                    WhatYouDidDescription = "You injected botox too proximal on right lower orbicularis muscle area.";
                    AnalysisImageName = "Error_Ptosis_LowerEyelid_R";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_RIGHT;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_UpperEyelid_L;
                    WhatYouDidDescription = "You injected botox too proximal on left upper orbicularis muscle area.";
                    AnalysisImageName = "Error_Ptosis_UpperEyelid_L";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_LEFT;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_LowerEyelid_L;
                    WhatYouDidDescription = "You injected botox too proximal on left lower orbicularis muscle area.";
                    AnalysisImageName = "Error_Ptosis_LowerEyelid_L";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_LEFT;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L_Proximal;
                    WhatYouDidDescription = "You injected botox on procerus with the wrong angle, injecting on left corrugator.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_L_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R_Proximal;
                    WhatYouDidDescription = "You injected botox on procerus with the wrong angle, injecting on right corrugator.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_R_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.H_ANGULAR_INJECTION_PROCERUS_TOWARD_UP:
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Frontal_AsymR;
                    WhatYouDidDescription = "You injected botox asymmetrically, with much more on the left frontal area than on the right frontal area.";
                    AnalysisImageName = "Error_Frontal_AsymR";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.M_ASIMMETRY_INJECTIONS_FRONTAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Frontal_AsymL;
                    WhatYouDidDescription = "You injected botox asymmetrically, with much more on the right frontal area than on the left frontal area.";
                    AnalysisImageName = "Error_Frontal_AsymL";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R_Proximal;
                    WhatYouDidDescription = "You injected botox too much on right corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_R_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.M_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L_Proximal;
                    WhatYouDidDescription = "You injected botox too much on left corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_L_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.O_FRONTAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Frontal_OmisR;
                    WhatYouDidDescription = "You did not inject enough botox on right frontal muscle area.";
                    AnalysisImageName = "DynamicFrontal";
                    AnalysisArea= Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.O_FRONTAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Frontal_OmisL;
                    WhatYouDidDescription = "You did not inject enough botox on left frontal muscle area.";
                    AnalysisImageName = "DynamicFrontal";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Corrugator_OmisL;
                    WhatYouDidDescription= "You did not inject enough botox on left corrugators muscle area.";
                    AnalysisImageName = "DynamicCorrugators";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.O_CORRUGATORS_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Corrugator_OmisR;
                    WhatYouDidDescription = "You did not inject enough botox on right corrugators muscle area.";
                    AnalysisImageName = "DynamicCorrugators";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.O_PROCERUS:
                    AnalysisConsequence= Enum_AnalysisConsequences.Error_Procerus_Omis;
                    WhatYouDidDescription = "You did not inject enough botox on procerus muscle area.";
                    AnalysisImageName = "DynamicCorrugators";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Orbicularis_OmisR;
                    WhatYouDidDescription = "You did not inject enough botox on right orbicularis muscle area.";
                    AnalysisImageName = "DynamicOrbicularRight";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_RIGHT;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.O_ORBICULARIS_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Orbicularis_OmisL;
                    WhatYouDidDescription = "You did not inject enough botox on left orbicularis muscle area.";
                    AnalysisImageName = "DynamicOrbicularLeft";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_LEFT;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_FRONTAL_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Frontal_ComisR;
                    WhatYouDidDescription = "You injected botox quantities on right frontal muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_FRONTAL_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Frontal_ComisL;
                    WhatYouDidDescription = "You injected botox quantities on left frontal muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Corrugator_ComisL;
                    WhatYouDidDescription = "You injected botox quantities on left corrugators muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_CORRUGATORS_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Corrugator_ComisR;
                    WhatYouDidDescription = "You injected botox quantities on right corrugators muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_PROCERUS:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Procerus_Comis;
                    WhatYouDidDescription = "You injected botox quantities on procerus muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Orbicularis_ComisR;
                    WhatYouDidDescription = "You injected botox quantities on right orbicularis muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_RIGHT;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.C_ORBICULARIS_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Orbicularis_ComisL;
                    WhatYouDidDescription = "You injected botox quantities on left orbicularis muscle area in unnecessary areas.";
                    AnalysisImageName = "";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_LEFT;
                    ScoreEffect = Enum_ScoreEffect.MIN20;
                    break;

                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_RIGHT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrowUpperEyeLid_R;
                    WhatYouDidDescription = "You injected a large quantity of botox too low on right frontal muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrowUpperEyeLid_R";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_FRONTAL_L1_LEFT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrowUpperEyeLid_L;
                    WhatYouDidDescription = "You injected a large quantity of botox too low on left frontal muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrowUpperEyeLid_L";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_LEFT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrowUpperEyeLid_L;
                    WhatYouDidDescription = "You injected a large quantity of botox too lateral on left corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrowUpperEyeLid_L";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_CORRUGATORS_LATERAL_RIGHT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrowUpperEyeLid_R;
                    WhatYouDidDescription = "You injected a large quantity of botox too lateral on right corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrowUpperEyeLid_R";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_RIGHT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_UpperLowerEyelids_R;
                    WhatYouDidDescription = "You injected a large quantity of botox too proximal on right upper orbicularis muscle area.";
                    AnalysisImageName = "Error_Ptosis_UpperLowerEyelids_R";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_RIGHT;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HH_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_LEFT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_UpperLowerEyelids_L;
                    WhatYouDidDescription = "You injected a large quantity of botox too proximal on left upper orbicularis muscle area.";
                    AnalysisImageName = "Error_Ptosis_UpperLowerEyelids_L";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_LEFT;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_RIGHT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R_Proximal;
                    WhatYouDidDescription = "You injected a large quantity of botox too much on right corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_R_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.MH_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_LEFT_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L_Proximal;
                    WhatYouDidDescription = "You injected a large quantity of botox too much on left corrugators muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrow_L_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_FRONTAL:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrows;
                    WhatYouDidDescription = "You injected botox too low on both sides of frontal muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrows";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_CORRUGATORS_LATERAL:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrows;
                    WhatYouDidDescription = "You injected botox too lateral on both corrugators muscle areas.";
                    AnalysisImageName = "Error_Ptosis_EyeBrows";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_UpperEyelids;
                    WhatYouDidDescription = "You injected botox too proximal on both upper orbicularis muscle areas.";
                    AnalysisImageName = "Error_Ptosis_UpperEyelids";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HB_INJECTIONS_ON_ORBICULAR_LOWER_PROXIMAL:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_LowerEyelids;
                    WhatYouDidDescription = "You injected botox too proximal on both lower orbicularis muscle areas.";
                    AnalysisImageName = "Error_Ptosis_LowerEyelids";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.MB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL:

                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrows_Proximal;
                    WhatYouDidDescription = "You injected too much botox on both corrugators muscle areas.";
                    AnalysisImageName = "Error_Ptosis_EyeBrows_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_FRONTAL_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrowsUpperEyeLids;
                    WhatYouDidDescription = "You injected a large quantity of botox too low on both sides of frontal muscle area.";
                    AnalysisImageName = "Error_Ptosis_EyeBrowsUpperEyeLids";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_CORRUGATORS_LATERAL_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrowsUpperEyeLids;
                    WhatYouDidDescription = "You injected a large quantity of botox too lateral on both corrugators muscle areas.";
                    AnalysisImageName = "Error_Ptosis_EyeBrowsUpperEyeLids";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.HHB_INJECTIONS_ON_ORBICULAR_UPPER_PROXIMAL_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_UpperLowerEyelids;
                    WhatYouDidDescription = "You injected a large quantity of botox too proximal on both upper orbicularis muscle areas.";
                    AnalysisImageName = "Error_Ptosis_UpperLowerEyelids";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR;
                    ScoreEffect = Enum_ScoreEffect.SET0;
                    break;

                case Enum_AnaysisVariablesToCheck.MHB_EXCESSIVE_INJECTIONS_ON_CORRUGATORS_PROXIMAL_MAX:
                    AnalysisConsequence = Enum_AnalysisConsequences.Error_Ptosis_EyeBrows_Proximal;
                    WhatYouDidDescription = "You injected too much botox on both corrugators muscle areas.";
                    AnalysisImageName = "Error_Ptosis_EyeBrows_Proximal";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.MINUS50;
                    break;

                case Enum_AnaysisVariablesToCheck.COR_FRONTAL:
                    AnalysisConsequence = Enum_AnalysisConsequences.Right_Frontal;
                    WhatYouDidDescription = "You injected the correct botox quantities on frontal muscle areas.";
                    AnalysisImageName = "Right_Frontal";
                    AnalysisArea = Enum_AreaDefinition.FRONTAL;
                    ScoreEffect = Enum_ScoreEffect.NOEFFECT;
                    break;

                case Enum_AnaysisVariablesToCheck.COR_CENTRAL:
                    AnalysisConsequence = Enum_AnalysisConsequences.Right_Corrugators;
                    WhatYouDidDescription = "You injected the correct botox quantities on central muscle areas.";
                    AnalysisImageName = "Right_Corrugators";
                    AnalysisArea = Enum_AreaDefinition.CENTRAL;
                    ScoreEffect = Enum_ScoreEffect.NOEFFECT;
                    break;

                case Enum_AnaysisVariablesToCheck.COR_ORBICULAR_LEFT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Right_Lateral_L;
                    WhatYouDidDescription = "You injected the correct botox quantities on left orbicularis muscle areas.";
                    AnalysisImageName = "Right_Lateral_L";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_LEFT;
                    ScoreEffect = Enum_ScoreEffect.NOEFFECT;
                    break;

                case Enum_AnaysisVariablesToCheck.COR_ORBICULAR_RIGHT:
                    AnalysisConsequence = Enum_AnalysisConsequences.Right_Lateral_R;
                    WhatYouDidDescription = "You injected the correct botox quantities on right orbicularis muscle areas.";
                    AnalysisImageName = "Right_Lateral_R";
                    AnalysisArea = Enum_AreaDefinition.ORBICULAR_RIGHT;
                    ScoreEffect = Enum_ScoreEffect.NOEFFECT;
                    break;

                default:
                    break;
            }
            WhatWillBeTheConsequence = GiveMeTextualConsequence(AnalysisConsequence);
        }

        private string GiveMeTextualConsequence(Enum_AnalysisConsequences analysisConsequence)
        {
            string toReturn = "";
            switch (analysisConsequence)
            {
                case Enum_AnalysisConsequences.Right_Frontal:
                    toReturn = "You did a great job!";
                    break;
                case Enum_AnalysisConsequences.Right_Corrugators:
                    toReturn = "You did a great job!";
                    break;
                case Enum_AnalysisConsequences.Right_Lateral_L:
                    toReturn = "You did a great job!";
                    break;
                case Enum_AnalysisConsequences.Right_Lateral_R:
                    toReturn = "You did a great job!";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R:
                    toReturn = "You induced a ptosis of the right eyebrow.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L:
                    toReturn = "You induced a ptosis of the left eyebrow.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrows:
                    toReturn = "You induced a ptosis of both eyebrows.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrowUpperEyeLid_R:
                    toReturn = "You induced a ptosis of the right eyebrow and upper eyelid.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrowUpperEyeLid_L:
                    toReturn = "You induced a ptosis of the left eyebrow and upper eyelid.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrowsUpperEyeLids:
                    toReturn = "You induced a ptosis of both eyebrows and upper eyelids.";
                    break;
                case Enum_AnalysisConsequences.Error_Frontal_AsymR:
                    toReturn = "You induced an asymmetry of the right frontal area.";
                    break;
                case Enum_AnalysisConsequences.Error_Frontal_AsymL:
                    toReturn = "You induced an asymmetry of the left frontal area.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_UpperEyelid_R:
                    toReturn = "You induced a ptosis of the right upper eyelid.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_LowerEyelid_R:
                    toReturn = "You induced a ptosis of the right lower eyelid.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_UpperEyelid_L:
                    toReturn = "You induced a ptosis of the left upper eyelid.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_LowerEyelid_L:
                    toReturn = "You induced a ptosis of the left lower eyelid.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_UpperLowerEyelids_R:
                    toReturn = "You induced a ptosis of the right upper and lower eyelids.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_UpperLowerEyelids_L:
                    toReturn = "You induced a ptosis of the left upper and lower eyelids.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_UpperEyelids:
                    toReturn = "You induced a ptosis of both upper eyelids.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_LowerEyelids:
                    toReturn = "You induced a ptosis of both lower eyelids.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_UpperLowerEyelids:
                    toReturn = "You induced a ptosis of both upper and lower eyelids.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrows_Proximal:
                    toReturn = "You induced a ptosis of both proximal eyebrows.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_L_Proximal:
                    toReturn = "You induced a ptosis of the left proximal eyebrow.";
                    break;
                case Enum_AnalysisConsequences.Error_Ptosis_EyeBrow_R_Proximal:
                    toReturn = "You induced a ptosis of the right proximal eyebrow";
                    break;
                case Enum_AnalysisConsequences.No_Effects_Frontal:
                    toReturn = "Insufficient effects on frontal area.";
                    break;
                case Enum_AnalysisConsequences.No_Effects_Orbicularis_Left:
                    toReturn = "Insufficient effects on left periocular area.";
                    break;
                case Enum_AnalysisConsequences.No_Effects_Orbicularis_Right:
                    toReturn = "Insufficient effects on right periocular area.";
                    break;
                case Enum_AnalysisConsequences.No_Effects_Central:
                    toReturn = "Insufficient effects on corrugators and procerus.";
                    break;
                case Enum_AnalysisConsequences.Error_Frontal_OmisR:
                    toReturn = "Insufficient effects on right frontal area.";
                    break;
                case Enum_AnalysisConsequences.Error_Orbicularis_ComisR:
                    toReturn= "Irregular effects on right periocular area.";
                    break;
                case Enum_AnalysisConsequences.Error_Procerus_Comis:
                    toReturn = "Irregular effects on procerus area.";

                    break;
                case Enum_AnalysisConsequences.Error_Corrugator_ComisR:
                    toReturn = "Irregular effects on right central area.";
                    break;
                case Enum_AnalysisConsequences.Error_Frontal_OmisL:
                    toReturn = "Insufficient effects on left frontal area.";
                    break;
                case Enum_AnalysisConsequences.Error_Frontal_ComisR:
                    toReturn = "Irregular effects on right frontal area.";
                    break;
                case Enum_AnalysisConsequences.Error_Orbicularis_ComisL:
                    toReturn = "Irregular effects on left periocular area.";
                    break;
                case Enum_AnalysisConsequences.Error_Procerus_Omis:
                    toReturn = "Insufficient effects on procerus area.";
                    break;
                case Enum_AnalysisConsequences.Error_Corrugator_OmisR:
                    toReturn = "Insufficient effects on right central area.";
                    break;
                case Enum_AnalysisConsequences.Error_Orbicularis_OmisR:
                    toReturn = "Insufficient effects on right periocular area.";
                    break;
                case Enum_AnalysisConsequences.Error_Orbicularis_OmisL:
                    toReturn = "Insufficient effects on left periocular area.";
                    break;
                case Enum_AnalysisConsequences.Error_Corrugator_OmisL:
                    toReturn = "Insufficient effects on left central area.";
                    break;
                case Enum_AnalysisConsequences.Error_Frontal_ComisL:
                    toReturn = "Irregular effects on left frontal area.";
                    break;
                case Enum_AnalysisConsequences.Error_Corrugator_ComisL:
                    toReturn= "Irregular effects on left periocular area.";
                    break;
                default:
                    toReturn = "";
                    break;
            }
            return toReturn;
        }
    }
}