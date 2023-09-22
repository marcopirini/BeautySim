using System;
using System.Collections.Generic;
using System.Linq;
using VectorMath;

namespace Device.Motion
{
    public delegate void ConnectionStatusChanged(CONNECTIONSTATUS status);

    public delegate void NewFrameAvailable(List<FRAME> frames);

    public delegate void StopSaveDelegate(STOP_SAVE_REASON reason);

    public class MotionClass
    {
        public List<FRAME> lastFrames = new List<FRAME>();
        private CONNECTIONSTATUS connectionStatus = CONNECTIONSTATUS.NOT_INITIALIZED;
        private string currentFileName = "";
        private DataTransformer dataTransformer;
        private object frameLockObject = new object();
        private CONNECTIONSTATUS oldConnectionStatus = CONNECTIONSTATUS.NOT_INITIALIZED;
        public MotionClass()
        {
            dataTransformer = new DataTransformer();

            AutoFileNaming = false;
            AutoNameFileFormat = "track_{0:000}.dat";
            AutoFileNameIndex = 0;
            SaveDataFolder = "";
        }

       
        public event ConnectionStatusChanged OnConnectionStatusChanged;

        /// <summary>
        /// New frame event generated at each frame reception
        /// </summary>
        public event NewFrameAvailable OnNewFrameAvailable;

        public event StopSaveDelegate OnStopSave;

        public virtual int AutoFileNameIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Enable file autonaming
        /// </summary>
        public virtual bool AutoFileNaming
        {
            get;
            set;
        }

        /// <summary>
        /// File name format for autonaming (es: track_{000}.dat)
        /// </summary>
        public virtual string AutoNameFileFormat
        {
            get;
            set;
        }

        public bool CanGetData { get; private set; }
        /// <summary>
        /// Connection status
        /// </summary>
        public CONNECTIONSTATUS ConnectionStatus
        {
            get
            {
                if (connectionStatus != oldConnectionStatus)
                {
                    RaiseOnConnectionStatusChanged(connectionStatus);

                }

                oldConnectionStatus = connectionStatus;
                return connectionStatus;
            }
            set
            {
                connectionStatus = value;
                if (connectionStatus == CONNECTIONSTATUS.ACQUIRING)
                {
                    CanGetData = true;
                }
                else
                {
                    CanGetData = false;
                }
            }
        }

        public virtual string CurrentFileName
        {
            get { return currentFileName; }
            set { currentFileName = value; }
        }

        public virtual string DeviceType
        {
            get;
            set;
        }

        /// <summary>
        /// Get last frame raw data
        /// </summary>
        public List<FRAME> GetRawData
        {
            get
            {
                lock (frameLockObject)
                {
                    return lastFrames.OrderBy(x => x.Sensor).ToList();
                }
            }
        }

        /// <summary>
        /// Real frame rate
        /// </summary>
        public virtual float RealFrameRate
        {
            get { return 0; }
        }

        public virtual bool Recording
        {
            get { return false; }
        }

        /// <summary>
        /// Folder where save file
        /// </summary>
        public virtual string SaveDataFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Mask of sensors connected
        /// </summary>
        public bool[] SensorsConnectedMask
        {
            get;
            set;
        }

        /// <summary>
        /// Get online data transformer
        /// </summary>
        public virtual DataTransformer Transformer
        {
            get { return dataTransformer; }
        }


        public virtual void Connect()
        {
        }



        public virtual void Connect(IntPtr hWND)
        {
        }

        /// <summary>
        /// Disconnect from G4
        /// </summary>
        public virtual void Dispose()
        {
        }

        public virtual bool IShowAcquisitionProperties
        {
            get;
            set;
        }

        public void RaiseOnConnectionStatusChanged(CONNECTIONSTATUS status)
        {
            if (OnConnectionStatusChanged != null)
                OnConnectionStatusChanged(status);
        }
        public void RaiseOnNewFrameAvailable(List<FRAME> frames)
        {
            if (OnNewFrameAvailable != null)
                OnNewFrameAvailable(frames);
        }
        public void RaiseOnStopSave(STOP_SAVE_REASON reason)
        {
            if (OnStopSave != null)
                OnStopSave(reason);
        }

        public virtual void ReadSettingsFromRegistry()
        {
        }

        /// <summary>
        /// Reset offset settings for all sensors
        /// </summary>
        /// <returns></returns>
        public void ResetAbsoluteOffset()
        {
            for (int i = 0; i < DataTransformer.MAX_SENSORS; i++)
            {
                dataTransformer.SensorSettings(i).SetRotationOffset(Quaternion.Identity);
                dataTransformer.SensorSettings(i).SetPositionOffset(Vector3.Zero);
            }
        }

        /// <summary>
        /// set sensor offset values for all sensor relative to reference sensor
        /// </summary>
        /// <param name="sensor"></param>
        public bool SetAbsoluteOffset(int reference_sensor)
        {
            if (Recording)
                return false;

            List<FRAME> frames = GetRawData;

            FRAME ref_frame = null;
            foreach (FRAME f in frames)
            {
                if (reference_sensor == f.Sensor)
                {
                    ref_frame = f;
                    break;
                }
            }
            if (ref_frame != null)
            {
                foreach (FRAME f in frames)
                {
                    dataTransformer.SensorSettings(f.Sensor).SetRotationOffset(ref_frame.Ori);
                    dataTransformer.SensorSettings(f.Sensor).SetPositionOffset(ref_frame.Pos);
                }
            }

            return true;
        }

        public void SetRawData(List<FRAME> data)
        {
            lock (frameLockObject)
            {
                lastFrames.Clear();
                foreach (FRAME f in data)
                {
                    FRAME cf = ObjectCopier.Clone<FRAME>(f);
                    lastFrames.Add(cf);
                }
            }
            //generate sync event
            RaiseOnNewFrameAvailable(lastFrames);
        }

        /// <summary>
        /// set sensor offset values from current position for all sensors
        /// </summary>
        /// <param name="sensor"></param>
        public bool SetRelativeOffset()
        {
            if (Recording)
                return false;

            List<FRAME> frames = GetRawData;

            foreach (FRAME f in frames)
            {
                dataTransformer.SensorSettings(f.Sensor).SetRotationOffset(f.Ori);
                dataTransformer.SensorSettings(f.Sensor).SetPositionOffset(f.Pos);
            }

            return true;
        }

        /// <summary>
        /// set sensor offset values from current position for single sensor
        /// </summary>
        /// <param name="sensor"></param>
        public bool SetRelativeOffsetTo(int sensor)
        {
            if (Recording)
                return false;

            List<FRAME> frames = GetRawData;
            Quaternion ori = Quaternion.Identity;
            Vector3 pos = Vector3.Zero;
            foreach (FRAME f in frames)
            {
                if (sensor == f.Sensor)
                {
                    ori = f.Ori;
                    pos = f.Pos;
                }
            }
            foreach (FRAME f in frames)
            {
                dataTransformer.SensorSettings(sensor).SetRotationOffset(ori);
                dataTransformer.SensorSettings(sensor).SetPositionOffset(pos);
            }
            return true;
        }

        public virtual void SetupDevice()
        {
        }

        public virtual bool StartSaveData()
        {
            return false;
        }

        /// <summary>
        /// Start save data. If recoding, current recordin will be stopped
        /// </summary>
        /// <param name="_filename"></param>
        public virtual bool StartSaveData(string _filename)
        {
            return false;
        }

        /// <summary>
        /// Stop save data and delete last recording
        /// </summary>
        public virtual void StopSaveAndDiscard()
        {
        }

        /// <summary>
        /// Stop save data
        /// </summary>
        public virtual string StopSaveData()
        {
            return string.Empty;
        }

        public virtual void WriteSettingsOnRegistry()
        {
        }

        
    }
}