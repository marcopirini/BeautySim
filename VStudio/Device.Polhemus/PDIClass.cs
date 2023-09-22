using Device.Motion;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using VectorMath;

namespace Device.Polhemus
{
    //public delegate void ConnectionStatusChanged(G4CONNECTIONSTATUS status);

    //public delegate void NewFrameAvailable(List<FRAME> frames);

    //public delegate void StopSaveDelegate(STOP_SAVE_REASON reason);
    public class PDIClass : MotionClass
    {
        private const int G4BUFFERLEN = 0x1FA400;
        private const string MODULEID = "PATRIOTDEVICE";

        private const int NUM_MAX_SENSORS = 2;
        private ReaderWriterLockSlim _data_read_lock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim _data_save_lock = new ReaderWriterLockSlim();
        private DateTime _date_last_frame = DateTime.MinValue;
        private int _dwMap = 0;
        private DateTime _last_connection = DateTime.Now;
        private int _last_frame_count = 0;
        private DateTime _last_frame_received = DateTime.Now;
        private int _num_thread = 0;
        private int _number_save_hubs = 0;
        private PDICONNECTIONSTATUS _old_connection_status = PDICONNECTIONSTATUS.END;
        private int _old_frame_count = 0;
        private float _real_frame_rate = 0;
        private bool _recording_data = false;

        // new byte[0x1AF280];
        private int _retry_counter = 0;

        private bool _start_record_request = false;
        private IntPtr hWND;
        private bool m_close = false;
        private System.Timers.Timer m_connection_timer = null;
        private IntPtr patriotBuffer = IntPtr.Zero;
        private PDICONNECTIONSTATUS patriotConnectionStatus = PDICONNECTIONSTATUS.NOT_INITIALIZED;
        private string patriotPID = "PID_EF20";
        private string patriotVID = "VID_0F44";
        private Thread PDIThread = null;

        public PDIClass() : base()
        {
            AddDllsFolder();
            Transformer.TransformFunction = TransformFunction;

            SensorsConnectedMask = new bool[NUM_MAX_SENSORS] { false, false };
        }

        ///// <summary>
        ///// Device present state
        ///// </summary>
        //public static bool DevicePresent
        //{
        //    get
        //    {
        //        return USBDevice.DeviceConnected(patrio, patriotPID);
        //    }
        //}

        public override string DeviceType
        {
            get { return "PATRIOT"; }
        }

        /// <summary>
        /// Real frame rate
        /// </summary>
        public override float RealFrameRate
        {
            get { return _real_frame_rate; }
        }

        public override bool Recording
        {
            get { return _recording_data; }
        }

        /// <summary>
        /// Connect to Patriot.
        /// </summary>
        public override void Connect(IntPtr hwnd)
        {
            try
            {
                hWND = hwnd;
                ConnectInternal();
            }
            catch (Exception)
            {
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        /// <summary>
        /// Disconnect from G4
        /// </summary>
        public override void Dispose()
        {
            try
            {
                if (patriotConnectionStatus <= PDICONNECTIONSTATUS.SEARCHING_DEVICE)
                    return;
                if (patriotConnectionStatus >= PDICONNECTIONSTATUS.CLOSE_THREAD)
                    return;

                patriotConnectionStatus = PDICONNECTIONSTATUS.CLOSE_THREAD;

                DateTime start_discconnect = DateTime.Now;
                while (patriotConnectionStatus != PDICONNECTIONSTATUS.END)
                {
                    System.Threading.Thread.Sleep(100);

                    TimeSpan elapsed = DateTime.Now - start_discconnect;
                    if (elapsed.TotalSeconds >= 2)
                    {
                        break;
                    }
                }

                if (m_connection_timer != null)
                {
                    m_connection_timer.Enabled = false;
                    patriotConnectionStatus = 0;
                    m_connection_timer.Elapsed -= new System.Timers.ElapsedEventHandler(m_connection_timer_Elapsed);
                    m_connection_timer = null;
                }
                Log("EXIT DISPOSE");
                GC.Collect();
            }
            catch (Exception)
            {
            }
        }


        public void ReadAxisTranslationFromRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Accurate Software\\BlockSim", true);

                string AxisTranslation = (string)key.GetValue("AxisTranslation", null);

                if (AxisTranslation == null || AxisTranslation == "")
                {
                    //Default value
                    AxisTranslation = "0,0,-1,1,0,0,0,-1,0";
                    key.SetValue("AxisTranslation", AxisTranslation);
                }

                string[] components = AxisTranslation.Split(',');
                Matrix m = Matrix.Identity;
                m.M11 = float.Parse(components[0], CultureInfo.InvariantCulture);
                m.M12 = float.Parse(components[1], CultureInfo.InvariantCulture);
                m.M13 = float.Parse(components[2], CultureInfo.InvariantCulture);
                m.M21 = float.Parse(components[3], CultureInfo.InvariantCulture);
                m.M22 = float.Parse(components[4], CultureInfo.InvariantCulture);
                m.M23 = float.Parse(components[5], CultureInfo.InvariantCulture);
                m.M31 = float.Parse(components[6], CultureInfo.InvariantCulture);
                m.M32 = float.Parse(components[7], CultureInfo.InvariantCulture);
                m.M33 = float.Parse(components[8], CultureInfo.InvariantCulture);

                Transformer.GeneralSetting.AxisTransform = m;
            }
            catch (Exception)
            {
                Transformer.GeneralSetting.AxisTransform = Matrix.Identity;
            }
        }

        public void SetAxisTranslationFromString(string AxisTranslation)
        {
            try
            {
                string[] components = AxisTranslation.Split(',');
                Matrix m = Matrix.Identity;
                m.M11 = float.Parse(components[0], CultureInfo.InvariantCulture);
                m.M12 = float.Parse(components[1], CultureInfo.InvariantCulture);
                m.M13 = float.Parse(components[2], CultureInfo.InvariantCulture);
                m.M21 = float.Parse(components[3], CultureInfo.InvariantCulture);
                m.M22 = float.Parse(components[4], CultureInfo.InvariantCulture);
                m.M23 = float.Parse(components[5], CultureInfo.InvariantCulture);
                m.M31 = float.Parse(components[6], CultureInfo.InvariantCulture);
                m.M32 = float.Parse(components[7], CultureInfo.InvariantCulture);
                m.M33 = float.Parse(components[8], CultureInfo.InvariantCulture);

                Transformer.GeneralSetting.AxisTransform = m;
            }
            catch (Exception)
            {
                Transformer.GeneralSetting.AxisTransform = Matrix.Identity;
            }
        }

        /// <summary>
        /// Start save data. If recoding, current recordin will be stopped
        /// </summary>
        /// <param name="_filename"></param>
        public override bool StartSaveData(string _filename)
        {
            if (patriotConnectionStatus == PDICONNECTIONSTATUS.ACQUIRING)
            {
                //the real start save is asyncronous.
                CurrentFileName = _filename;

                _start_record_request = true;
                return true;
            }

            return false;
        }


        public void WriteAxisTranslationToRegistry(Matrix m)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Accurate\\BlockSim", true);

                string AxisTranslation = string.Format(CultureInfo.InvariantCulture,
                    "{0},{1},{2}," +
                    "{3},{4},{5}," +
                    "{6},{7},{8}", m.M11, m.M12, m.M13, m.M21, m.M22, m.M23, m.M31, m.M32, m.M33);

                key.SetValue("AxisTranslation", AxisTranslation, RegistryValueKind.String);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Write calibration file name to registry
        /// </summary>
        /// <param name="path"></param>
        public void WriteCalibrationFilePathToRegistry(string path)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Accurate\\BlockSim", true);
                key.SetValue("AntennaCalibrationFile", path, RegistryValueKind.String);
            }
            catch (Exception)
            {
            }
        }


        public override void WriteSettingsOnRegistry()
        {
            WriteAxisTranslationToRegistry(Transformer.GeneralSetting.AxisTransform);
        }

        private static void Log(string message)
        {
            
        }

        private static void Log(Exception ex)
        {
        }

        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int uPeriod);

        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int uPeriod);

        /// <summary>
        /// Transform frames from raw data according to General and Sensors settings
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        private static MOTIONFRAMEList TransformFunction(List<FRAME> frames, DataTransformer Transformer)
        {
            List<FRAME> clone = new List<FRAME>();
            //Copy frames
            foreach (FRAME f in frames)
            {
                FRAME c = new FRAME();
                c.Sensor = f.Sensor;
                c.FrameCount = f.FrameCount;
                c.Pos = f.Pos;
                c.Ori = f.Ori;
                c.FrameCount = f.FrameCount;
                c.Digital = f.Digital;
                clone.Add(c);
            }

            MOTIONFRAMEList vrrs_frames = new MOTIONFRAMEList();

            #region GENERAL TRANSLATION AND SCALER

            //transform frames world axis orientation
            for (int i = 0; i < clone.Count; i++)
            {
                FRAME f = clone[i];
                f.Pos -= Transformer.SensorSettings(f.Sensor).position_offset;

                //Position scaler
                Vector3 scaler = new Vector3(1, 1, 1) + Transformer.SensorSettings(f.Sensor).scaler;
                f.Pos = new Vector3(scaler.X * f.Pos.X, scaler.Y * f.Pos.Y, scaler.Z * f.Pos.Z);

                Quaternion compensation = Quaternion.Identity;
                Vector3 pos_compensation = Vector3.Zero;

                #region Compensation

                if (Transformer.SensorSettings(f.Sensor).compensation_enabled)
                {
                    //search compensation sensor
                    foreach (FRAME item in frames)
                    {
                        if (item.Sensor == Transformer.SensorSettings(f.Sensor).compensation_sensor && item.FrameCount >= f.FrameCount)
                        {
                            compensation = VectorUtils.RelativeRotation(Transformer.SensorSettings(item.Sensor).rotation_offset, item.Ori, Matrix.Identity);
                            compensation.Invert();
                            pos_compensation = -(item.Pos - Transformer.SensorSettings(item.Sensor).position_offset);
                            break;
                        }
                    }
                }

                #endregion Compensation

                //orientation calc
                switch (Transformer.GeneralSetting.ReferenceMode)
                {
                    case REFERENCE_MODE.SENSOR:
                        Quaternion inv_offset = Transformer.SensorSettings(f.Sensor).rotation_offset;
                        inv_offset.Invert();
                        f.Ori = (f.Ori * compensation) * (inv_offset);
                        break;

                    case REFERENCE_MODE.WORLD:

                        f.Ori = VectorUtils.RelativeRotation(Transformer.SensorSettings(f.Sensor).rotation_offset, f.Ori * compensation, Transformer.GeneralSetting.AxisTransform);
                        f.Pos = Vector3.TransformCoordinate(f.Pos + pos_compensation, Transformer.GeneralSetting.AxisTransform);
                        break;
                }

                f.Pos -= Transformer.GeneralSetting.OriginTranslation;
            }

            #endregion GENERAL TRANSLATION AND SCALER

            #region COPY TO VRRSFRAME CLASS

            for (int i = 0; i < clone.Count; i++)
            {
                FRAME f = clone[i];
                MOTIONFRAME v = new MOTIONFRAME();
                v.FrameCount = f.FrameCount;
                v.Pos = f.Pos;
                v.Ori = f.Ori;
                v.Mat = Matrix.RotationQuaternion(f.Ori);
                v.Sensor = f.Sensor;
                v.Digital = f.Digital;

                VectorUtils.DCMFromMatrix(v.Mat, ref v.X, ref v.Y, ref v.Z);

                Vector3 euler = EulerUtils.DCMtoEA(v.Mat, Transformer.GeneralSetting.EulerCalcMode);

                v.Roll = euler.X;
                v.Pitch = euler.Y;
                v.Yaw = euler.Z;
                vrrs_frames.Add(v);
            }

            #endregion COPY TO VRRSFRAME CLASS

            return vrrs_frames;
        }

        /// <summary>
        /// Marge buffers
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        private byte[] AddBuf(byte[] b1, byte[] b2)
        {
            if (b2 == null)
                return b1;

            if (b1 == null)
                return b2;

            if (b1 == null && b2 == null)
                return null;

            byte[] res_buf = new byte[b1.Length + b2.Length];

            Buffer.BlockCopy(b1, 0, res_buf, 0, b1.Length);
            Buffer.BlockCopy(b2, 0, res_buf, b1.Length, b2.Length);

            return res_buf;
        }

        /// <summary>
        /// Add static dll folder
        /// </summary>
        private void AddDllsFolder()
        {
            try
            {
                string current_environment_var = Environment.GetEnvironmentVariable("PATH");

                string g4_dll_path = IntPtr.Size == 8 ? "C:\\ACCURATE_DEVICES\\G4DLLs_x64" : "C:\\ACCURATE_DEVICES\\G4DLLs_x86";
                Log(g4_dll_path);
                //MessageBox.Show(g4_dll_path);
                if (!Directory.Exists(g4_dll_path))
                {
                    throw new System.Exception(BeautySim.Globalization.Language.str_g4_dll_path + " " + g4_dll_path);
                }

                if (!current_environment_var.Contains(g4_dll_path))
                {
                    current_environment_var = g4_dll_path + ";" + current_environment_var;
                    Environment.SetEnvironmentVariable("PATH", current_environment_var);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void CloseThread()
        {
            m_close = true;
            if (PDIThread != null)
            {
                PDIThread.Join(2000);
                try
                {
                    PDIThread.Abort();
                }
                catch (Exception)
                {
                }

                PDIThread = null;
            }
        }

        /// <summary>
        /// Connect to G4
        /// </summary>
        /// <param name="config_file">.g4c configuration file</param>
        private void ConnectInternal()
        {
            //intialize connection timer
            if (m_connection_timer == null)
            {
                patriotConnectionStatus = PDICONNECTIONSTATUS.SEARCHING_DEVICE;
                m_connection_timer = new System.Timers.Timer(100);
                m_connection_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_connection_timer_Elapsed);
                m_connection_timer.Enabled = true;
            }
        }

        private void m_connection_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_connection_timer.Enabled = false;

            if (patriotConnectionStatus != _old_connection_status)
            {
                Log("STATUS:" + patriotConnectionStatus.ToString());
                _old_connection_status = patriotConnectionStatus;
            }

            switch (patriotConnectionStatus)
            {
                case PDICONNECTIONSTATUS.SEARCHING_DEVICE: //try to discover if any G4 doungle is connected...

                    m_connection_timer.Interval = 500;
                    ////USB\VID_0F44&PID_FF20

                    if (!USBDevice.DeviceConnected(patriotVID, patriotPID))
                    {
                        break;
                    }

                    Log("Device Found");

                    Log("Connect:");
                    try
                    {
                        if (PDIDLLWrapper.Instance.Connect())
                        {
                            m_connection_timer.Interval = 20;
                            patriotConnectionStatus++;
                            _last_connection = DateTime.Now;
                            _retry_counter = 0;

                            m_connection_timer_Elapsed(sender, e);
                        }
                        else
                        {
                            //connect method fail call Disconnect and ResetTracker
                            try
                            {
                                Log("connect method fail ResetTracker " + PDIDLLWrapper.Instance.GetLastResult().ToString());
                                bool res = PDIDLLWrapper.Instance.ResetTracker();
                                _retry_counter++;
                                m_connection_timer_Elapsed(sender, e);
                            }
                            catch (Exception ex)
                            {
                                Log(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                        MessageBox.Show(ex.Message+ex.StackTrace);
                    }

                    break;

                case PDICONNECTIONSTATUS.GET_STATION_MAP:

                    if (!USBDevice.DeviceConnected(patriotVID, patriotPID))
                    {
                        _retry_counter = int.MaxValue;
                        break;
                    }

                    Log("GetStationMap Start");
                    if (!PDIDLLWrapper.Instance.GetStationMap(ref _dwMap))
                    {
                        _retry_counter++;
                        break;
                    }

                    Log("_dwMap" + _dwMap.ToString());
                    if (_dwMap > 0)
                    {
                        patriotConnectionStatus++;
                        _retry_counter = 0;
                        m_connection_timer.Interval = 20;
                        //m_connection_timer_Elapsed(sender, e);
                    }
                    else
                    {
                        m_connection_timer.Interval = 100;
                        TimeSpan elapsed = DateTime.Now - _last_connection;
                        if (elapsed.TotalMinutes >= 1)
                        {
                            _last_connection = DateTime.Now;
                            //G4Wrapper.Instance.ResetTracker();
                            _retry_counter = int.MaxValue;
                            break;
                        }
                    }

                    break;

                case PDICONNECTIONSTATUS.READ_SINGLE_FRAME:

                    //check if dongle is plugged in
                    if (!USBDevice.DeviceConnected(patriotVID, patriotPID))
                    {
                        _retry_counter = int.MaxValue;
                        break;
                    }

                    //try to read a frame to check if everything is ok..
                    Log("ReadSinglePno");
                    if (PDIDLLWrapper.Instance.ReadSinglePno(hWND))
                    {
                        if (patriotBuffer != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(patriotBuffer);
                            patriotBuffer = IntPtr.Zero;
                        }

                        patriotBuffer = Marshal.AllocHGlobal(G4BUFFERLEN);

                        //Set buffer for continuous acquisition
                        bool sres = PDIDLLWrapper.Instance.SetPnoBuffer(patriotBuffer, G4BUFFERLEN);
                        patriotConnectionStatus++;
                        _retry_counter = 0;
                        // m_connection_timer_Elapsed(sender, e);
                    }
                    else
                    {
                        TimeSpan elapsed = DateTime.Now - _last_connection;

                        if (elapsed.TotalSeconds > 2)
                        {
                            _last_connection = DateTime.Now;
                            _retry_counter++;

                            break;
                        }
                    }

                    break;

                case PDICONNECTIONSTATUS.SER_DATA_LIST:  //set measure unit

                    Log("SetDataList");
                    PDIDLLWrapper.Instance.SetMetric(true);
                    if (PDIDLLWrapper.Instance.SetDataList())
                    {
                        patriotConnectionStatus++;
                        _retry_counter = 0;
                        // m_connection_timer_Elapsed(sender, e);
                    }
                    else
                    {
                        _retry_counter++;
                        break;
                    }

                    break;

                case PDICONNECTIONSTATUS.START_CONT_PNO: //statr continuous acquisiton and PDIProc thread which parse data

                    Log("StartContPnoG4");
                    if (PDIDLLWrapper.Instance.StartContPno(hWND))
                    {
                        PDIThread = new Thread(new ThreadStart(PDIProc));
                        m_close = false;
                        PDIThread.IsBackground=true;
                        PDIThread.Start();
                        PDIThread.Priority = ThreadPriority.AboveNormal;
                        patriotConnectionStatus++;
                        _retry_counter = 0;
                        // m_connection_timer_Elapsed(sender, e);
                        m_connection_timer.Interval = 200;
                    }
                    else
                    {
                        _retry_counter++;
                        break;
                    }

                    break;

                case PDICONNECTIONSTATUS.ACQUIRING:
                    //check FRAME COUNTER. If no new data is acquired
                    //within 100 ms close the thread and go back to state 1

                    if (_last_frame_count == _old_frame_count)
                    {
                        TimeSpan elapsed = DateTime.Now - _last_connection;

                        if (elapsed.TotalSeconds > 2)
                        {
                            _real_frame_rate = 0;
                            if (!USBDevice.DeviceConnected(patriotVID, patriotPID))
                            {
                                _retry_counter = int.MaxValue;
                                break;
                            }
                            _last_connection = DateTime.Now;
                            _retry_counter++;
                        }

                        break;
                    }

                    _last_connection = DateTime.Now;
                    _retry_counter = 0;
                    _old_frame_count = _last_frame_count;

                    break;

                case PDICONNECTIONSTATUS.CLOSE_THREAD:

                    m_connection_timer.Interval = 50;
                    CloseThread();
                    patriotConnectionStatus++;
                    break;

                case PDICONNECTIONSTATUS.STOP_ACQUISITION:

                    try
                    {
                        if (PDIDLLWrapper.Instance != null)
                        {
                            PDIDLLWrapper.Instance.StopContPno();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }
                    patriotConnectionStatus++;

                    break;

                case PDICONNECTIONSTATUS.CLEAR_PNOBUFFER:

                    try
                    {
                        if (PDIDLLWrapper.Instance != null)
                        {
                            PDIDLLWrapper.Instance.ClearPnoBuffer();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }
                    patriotConnectionStatus++;

                    break;

                case PDICONNECTIONSTATUS.DISCONNECTING:
                    try
                    {
                        if (PDIDLLWrapper.Instance != null)
                        {
                            PDIDLLWrapper.Instance.Disconnect();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }

                    patriotConnectionStatus++;
                    break;

                case PDICONNECTIONSTATUS.END:
                    break;
            }

            if (_retry_counter > 20)
            {
                _retry_counter = 0;
                patriotConnectionStatus = PDICONNECTIONSTATUS.SEARCHING_DEVICE;
                m_connection_timer.Interval = 500;
                if (_recording_data)
                {
                    RaiseOnStopSave(STOP_SAVE_REASON.DEVICES_STATUS_CHANGE);
                }
                CloseThread();
            }

            if (m_connection_timer != null)
                m_connection_timer.Enabled = true;

            switch (patriotConnectionStatus)
            {
                case PDICONNECTIONSTATUS.NOT_INITIALIZED:
                    ConnectionStatus = CONNECTIONSTATUS.NOT_INITIALIZED;
                    break;

                case PDICONNECTIONSTATUS.SEARCHING_DEVICE:
                    ConnectionStatus = CONNECTIONSTATUS.SEARCHING_DEVICE;
                    break;

                case PDICONNECTIONSTATUS.GET_STATION_MAP:
                case PDICONNECTIONSTATUS.READ_SINGLE_FRAME:
                case PDICONNECTIONSTATUS.SER_DATA_LIST:
                case PDICONNECTIONSTATUS.START_CONT_PNO:
                    ConnectionStatus = CONNECTIONSTATUS.INITIALIZING;
                    break;

                case PDICONNECTIONSTATUS.ACQUIRING:
                    ConnectionStatus = CONNECTIONSTATUS.ACQUIRING;
                    break;

                case PDICONNECTIONSTATUS.CLOSE_THREAD:
                case PDICONNECTIONSTATUS.STOP_ACQUISITION:
                case PDICONNECTIONSTATUS.CLEAR_PNOBUFFER:
                case PDICONNECTIONSTATUS.DISCONNECTING:
                    ConnectionStatus = CONNECTIONSTATUS.DISCONNECTING;
                    break;

                case PDICONNECTIONSTATUS.END:
                    ConnectionStatus = CONNECTIONSTATUS.END;
                    break;

                default:
                    break;
            }
        }

        private void PDIProc()
        {
            try
            {
                if (_num_thread > 0)
                    return;

                _num_thread++;

                byte[] buf = null;

                int frame_counter = 0;
                int lost_frames = 0;
                int total_frames = 0;

                //intialize an high-performance timer

                Stopwatch timer_fr = new Stopwatch();
                timer_fr.Start();

                //start acquisition loop
                while (!m_close)
                {
                    int current_frame_counter = 0;
                    //get current frame number
                    if (PDIDLLWrapper.Instance.LastHostFrameCount(ref current_frame_counter))
                    {
                        //if current frame number is the same of previous do 1ms sleep in order to free resouces
                        if (current_frame_counter == frame_counter)
                        {
                            timeBeginPeriod(1);
                            System.Threading.Thread.Sleep(3);
                            timeEndPeriod(1);

                            continue;
                        }

                        //number frames to read
                        int number_frames_to_read = current_frame_counter - frame_counter;

                        if (number_frames_to_read > 5)
                            number_frames_to_read = 1;

                        total_frames += (current_frame_counter - frame_counter);

                        if (!PDIDLLWrapper.Instance.LastPnoPtr(ref buf, number_frames_to_read))
                        {
                            lost_frames += (current_frame_counter - frame_counter - 1);

                            if ((DateTime.Now - _last_frame_received).TotalMilliseconds > 500)
                            {
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            //offset    type    value
                            //02        BYTE    sensor
                            //06        SHORT   size
                            //08        UINT32  FC
                            //12        3*float POS
                            //24        4*float QUAT

                            //BYTE ucSensor = pBuf[i + 2];
                            //SHORT shSize = pBuf[i + 6];

                            //// skip rest of header
                            //i += 8;

                            BinaryReader sr = new BinaryReader(new MemoryStream(buf));

                            int numSensors = buf.Length / 40;
                            List<FRAME> framesToPass = new List<FRAME>();
                            for (int i = 0; i < numSensors; i++)
                            {
                                sr.BaseStream.Seek(2 + i * 40, SeekOrigin.Begin);
                                byte sensor = sr.ReadByte();
                                sr.BaseStream.Seek(6 + i * 40, SeekOrigin.Begin);
                                short size = sr.ReadInt16();
                                uint fc = sr.ReadUInt32();

                                Vector3 pos = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());

                                float w = sr.ReadSingle();
                                float x = sr.ReadSingle();
                                float y = sr.ReadSingle();
                                float z = sr.ReadSingle();

                                Quaternion quaterinon = new Quaternion(x, y, z, w);

                                FRAME f = new FRAME
                                {
                                    Sensor = sensor,
                                    FrameCount = fc,
                                    Pos = pos,
                                    Ori = quaterinon
                                };
                                framesToPass.Add(f);
                            }

                            SetRawData(framesToPass);
                        }

                        //PDWORD pFC = (PDWORD)(&pBuf[i]);
                        //PFLOAT pPno = (PFLOAT)(&pBuf[i + 4]);

                        //_sntprintf(szFrame, _countof(szFrame), _T("%2d   %d  %+011.6f %+011.6f %+011.6f   %+011.6f %+011.6f %+011.6ff\r"),
                        //         ucSensor, *pFC, pPno[0], pPno[1], pPno[2], pPno[3], pPno[4], pPno[5]);

                        PDIDLLWrapper.Instance.GetStationMap(ref _dwMap);

                        //update frame counter
                        frame_counter = current_frame_counter;

                        _last_frame_count = frame_counter;
                    }
                } //END THREAD

                try
                {
                    if (PDIDLLWrapper.Instance != null)
                        PDIDLLWrapper.Instance.StopContPno();
                }
                catch (Exception)
                {
                }

                _num_thread--;
                if (_num_thread != 0)
                {
                    Log("PDIProc multiple thread error:" + _num_thread.ToString());
                }
            }
            catch (Exception)
            { }
        }
    }
}