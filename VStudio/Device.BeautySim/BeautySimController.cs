using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Device.BeautySim
{
    public class BeautySimController
    {
        public AMessageArrivedDelegate AMessageArrivedEvent;

        public string ComPort = "";
        public float CurrentVolume = 0;

        public DateTime LastFluxReadingTime = DateTime.Now;
        public float LastReadFlux = 0;

        public ScannedSimulatorsDelegate ScannedSimulatorsEvent;
        public BeautySim Simulator;
        public USBClass Usbclass;

        private const float ACQUISITION_THREAD_FR = 30;
        private static BeautySimController instance;
        private Thread acquisitionThread = null;
        private bool exitAcquisitionThread = false;

        private bool firstReadingFlux = false;

        private uint frameCount = 0;

        private Stopwatch highResTimer;

        private HwndSource hwnd;

        private bool verbose = false;

        public event EventHandler SerialNumberAcquired;

        private BeautySimController()
        {
        }

        public delegate void AMessageArrivedDelegate(string serial, byte type, byte[] message);

        public delegate void ErrorMessageDelegate(string v);

        public delegate void ScannedSimulatorsDelegate();

        public static BeautySimController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BeautySimController();
                }
                return instance;
            }
        }

        public bool AbleToConnect { get; private set; }

        public string ErrorMessage { get; private set; }

        public void Dispose()
        {
            try
            {
                StopAcquisitionThread();

                DisposeDevices();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
            instance = null;
        }

        public void Go()
        {
            try
            {
                Usbclass = new USBClass();

                DiscoverBeautySimulators();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        public void StartHookDevices(HwndSource hwnd)
        {
            try
            {
                this.hwnd = hwnd;
                hwnd.AddHook(WndProc);
                //USB Connection
                Usbclass.RegisterForDeviceChange(true, hwnd.Handle);
                Usbclass.USBDeviceAttached += Usbclass_USBDeviceAttached;
                Usbclass.USBDeviceRemoved += Usbclass_USBDeviceRemoved;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        internal byte[] decrypt_function(byte[] buffer)
        {
            return buffer;
        }

        internal byte[] encrypt_function(byte[] buffer)
        {
            return buffer;
        }

        /// <summary>
        /// The acquisition thread func
        /// </summary>
        private void AcquisitionThreaed()
        {
            try
            {
                highResTimer.Start();
                while (!exitAcquisitionThread)
                {
                    if (highResTimer.ElapsedMilliseconds >= (1.0f / ACQUISITION_THREAD_FR) * 1000)
                    {
                        frameCount++;
                        UpdateData(Simulator, frameCount);
                        highResTimer.Restart();
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void AMessageArrivedListener(string serial, byte type, byte[] message)
        {
            try
            {
                if (AMessageArrivedEvent != null)
                {
                    AMessageArrivedEvent(serial, type, message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void Connect()
        {
            try
            {
                StartAcquisitionThread();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void DiscoverBeautySimulators()
        {
            try
            {
                //enumero usb device di tipo COM
                List<string> listSimCom = new List<string>();
                List<Win32DeviceMgmt.DeviceInfo> lst = Win32DeviceMgmt.GetAllCOMPorts();

                string VID = "VID_2341";
                List<string> PIDs = new List<string>() { "PID_8037", "PID_8237" };

                foreach (Win32DeviceMgmt.DeviceInfo item in lst)
                {
                    if (item.hardwareID.Contains(VID))
                    {
                        foreach (string pid in PIDs)
                        {
                            if (item.hardwareID.Contains(pid))
                            {
                                listSimCom.Add(item.name);

                                break;
                            }
                        }

                    }
                }

                bool BeautySimulatorIsInvolved = false;
                if (Simulator != null)
                {
                    if (!listSimCom.Contains(Simulator.COM))
                    {
                        //We have a disconnection of the BeautySimulator
                        BeautySimulatorIsInvolved = true;
                        Simulator.AmessageArrivedEvent -= new BeautySim.AMessageArrived(AMessageArrivedListener);
                        Simulator.Dispose();
                        Simulator = null;
                    }
                }
                else
                {
                    if (listSimCom.Count > 0)
                    {
                        //We have a connection of the BeautySimulator
                        BeautySimulatorIsInvolved = true;
                        BeautySim new_grasp = new BeautySim(listSimCom[0], encrypt_function, decrypt_function);
                        new_grasp.AmessageArrivedEvent += new BeautySim.AMessageArrived(AMessageArrivedListener);
                        new_grasp.SerialNumberAcquired += BeautySim_SerialNumberAcquired;
                        Simulator = new_grasp;
                    }
                }

                if (BeautySimulatorIsInvolved)
                {
                    if (AbleToConnect)
                    {
                        Connect();
                    }
                    if (ScannedSimulatorsEvent != null)
                    {
                        ScannedSimulatorsEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void BeautySim_SerialNumberAcquired(object sender, EventArgs e)
        {
            OnSerialNumberAcquired(e);
        }

        /// <summary>
        /// Stop manager thread
        /// </summary>
        private void DisposeDevices()
        {
            try
            {
                if (hwnd != null)
                {
                    hwnd.RemoveHook(WndProc);
                }
                Simulator.AmessageArrivedEvent -= new BeautySim.AMessageArrived(AMessageArrivedListener);
                Simulator.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// initizliaze manager
        /// </summary>
        private void SetupComponentsAtCreation(string simConfig)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void StartAcquisitionThread()
        {
            try
            {
                StopAcquisitionThread();
                highResTimer = new Stopwatch();
                exitAcquisitionThread = false;

                acquisitionThread = new Thread(new ThreadStart(AcquisitionThreaed));
                acquisitionThread.Priority = ThreadPriority.Normal;
             
                acquisitionThread.IsBackground = true;
                acquisitionThread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void StopAcquisitionThread()
        {
            try
            {
                if (acquisitionThread != null)
                {
                    exitAcquisitionThread = true;
                    try
                    {
                        acquisitionThread.Join(1000);
                    }
                    catch (Exception)
                    {
                    }

                    acquisitionThread = null;
                    highResTimer = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void UpdateData(BeautySim sensor, uint framecount)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        private void UpdateReadVolume()
        {
            try
            {
                if (firstReadingFlux)
                {
                    CurrentVolume = CurrentVolume + (LastReadFlux) * (float)(DateTime.Now - LastFluxReadingTime).TotalMilliseconds;
                }
                else
                {
                    firstReadingFlux = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Usbclass_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
        {
            try
            {
                DiscoverBeautySimulators();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Handle remove usb device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Usbclass_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
        {
            try
            {
                DiscoverBeautySimulators();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Handle usb messages
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                Usbclass.ProcessWindowsMessage(msg, wParam, lParam, ref handled);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (verbose)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
            return IntPtr.Zero;
        }

        private void ZeroVolume()
        {
            firstReadingFlux = false;
            CurrentVolume = 0;
        }

        protected virtual void OnSerialNumberAcquired(EventArgs e)
        {
            SerialNumberAcquired?.Invoke(this, e);
        }
    }
}