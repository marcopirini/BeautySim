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

using VectorMath;
using System;
using System.Runtime.InteropServices;

namespace Device.Motion
{
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

    [Serializable]
    public class FRAME
    {
        public uint FrameCount;
        public byte Sensor;     //zero based number
        public Vector3 Pos;
        public Quaternion Ori;
        public Matrix Matrix => Matrix.RotationQuaternion(Ori);
        public byte Digital;
        public float ExtraData;
        public int BatteryLevel;
        public FRAME()
        {
            Ori = Quaternion.Identity;
        }

        public YPR YPR(EULER_CALC_MODE mode)
        {
            Vector3 ea = EulerUtils.DCMtoEA(Matrix, mode);
            return new YPR() { Yaw = ea.Z, Pitch = ea.Y, Roll = ea.X };
        }

    }

    public class YPR
    {
        public double Yaw, Pitch, Roll;
        public double YawDegree => Yaw.ToDegree();
        public double PitchDegree => Pitch.ToDegree();
        public double RollDegree => Roll.ToDegree();
    }

    /// <summary>
    /// Data reference mode
    /// WORLD: the system reference is relative to  antenna orientation
    /// SENSOR: the system reference is relative to Sensor Zero position
    /// </summary>
    public enum REFERENCE_MODE
    {
        SENSOR = 0,
        WORLD = 1,
    };


    public enum STOP_SAVE_REASON
    {
        USER_REQUEST,
        RECORD_RESTART,
        DEVICES_STATUS_CHANGE
    }

    ///file structure

    //200 byte header
    [StructLayout(LayoutKind.Sequential, Size = 200)]
    public struct MOTIONFILEHEADER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]       //10
        public byte[] Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]       //30
        public byte[] DeviceType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]       //50
        public byte[] DeviceID;

        public float FrameRate;                                     //54
        public int NumDevices;                                         //58
        public int NumSensors;                                      //62
        public DateTime Date;                                       //66
        //134 bytes left for future use

        public void SetVersion(string value)
        {
            Version = StringHelper.StringToBuffer(value, 10);
        }

        public void SetDeviceType(string value)
        {
            DeviceType = StringHelper.StringToBuffer(value, 20);
        }

        public void SetDeviceID(string value)
        {
            DeviceID = StringHelper.StringToBuffer(value, 20);
        }

        public string DeviceIDStr
        {
            get
            {
                return StringHelper.BufferToString(DeviceID);
            }
        }

        public string DeviceTypeStr
        {
            get
            {
                return StringHelper.BufferToString(DeviceType);
            }
        }

        public string VersionStr
        {
            get
            {
                return StringHelper.BufferToString(Version);
            }
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public class GENERALSETTINGS
    {
        public Matrix AxisTransform;    //Axis translation
        public Vector3 OriginTranslation;   //origin translation
        public REFERENCE_MODE ReferenceMode;
        public EULER_CALC_MODE EulerCalcMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public class SENSORSETTINGS
    {
        public int sensor_number;
        public Vector3 position_offset;
        public Quaternion rotation_offset=Quaternion.Identity;

        public bool compensation_enabled = false;
        public int compensation_sensor;

        public Vector3 scaler;

        public void SetRotationOffset(Quaternion q)
        {
            rotation_offset = q;
        }

        public void SetPositionOffset(Vector3 p)
        {
            position_offset = p;
        }
    };

    /// <summary>
    /// VRRS Transformed data frame.
    /// </summary>
    public struct MOTIONFRAME
    {
        public int Sensor;
        public uint FrameCount;
        public Vector3 Pos, X, Y, Z;
        public Quaternion Ori;
        public Matrix Mat;
        public double Yaw, Pitch, Roll;
        public int BatteryLevel;

        public byte Digital;

        public double YawDegree => Yaw.ToDegree();


        public double PitchDegree => Pitch.ToDegree();


        public double RollDegree => Roll.ToDegree();
       
    };
}