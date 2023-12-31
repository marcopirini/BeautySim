/******************************************
 * Class name:
 * Author:
 * Creation:
 * Last modify:
 * Version:
 * 
 * DESCRIPTION
 * 
 * 
 * *****************************************/

using System;

namespace Device.Polhemus
{
     
    internal struct SENSORDATA
    {				// per sensor P&O data

        public int nSnsID;
        public VECTOR3 pos;
        public QUATERNION ori;
    };//32 bytes

    internal struct HUBDATA
    {				// per hub P&O data

        public int nHubID;
        public uint nFrameCount;
        public int dwSensorMap;
        public int dwDigIO;
    };		// 112 bytes	


     [Serializable]
    public struct QUATERNION
    {
        public float W;
        public float X;
        public float Y;
        public float Z;
       

       
    }
    public struct VECTOR3
    {
        public float X;
        public float Y;
        public float Z;

        public VECTOR3(float _x, float _y, float _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }
    }

    
   

}
