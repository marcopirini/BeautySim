using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace Device.BeautySim
{
    public class BeautySim
    {
        public AMessageArrived AmessageArrivedEvent;
        public Enum_InquiringSimulator InquiringState = Enum_InquiringSimulator.NOT_LINKED;

        private const int baudRate = 115200;
        private const int bufferSize = 100;
        private byte[] buffer = null;
        private Thread CloseDown;
        private SerialPort comPort = null;

        private Func<byte[], byte[]> decrypt_function = null;
        private Func<byte[], byte[]> encryption_function = null;
        private bool imClosed = true;
        private DateTime lastReadTime = DateTime.MinValue;

        public event EventHandler SerialNumberAcquired;

        public BeautySim(string portName, Func<byte[], byte[]> encryption_function, Func<byte[], byte[]> decrypt_function)
        {
            try
            {
                CloseDown = new Thread(new ThreadStart(CloseSerialPortOnExit));
                CloseDown.IsBackground = true;
                this.encryption_function = encryption_function;
                this.decrypt_function = decrypt_function;

                comPort = new SerialPort(portName, baudRate);
                comPort.RtsEnable = true;
                comPort.ReadTimeout = 100;
                comPort.DataReceived += ComPort_DataReceived;
                imClosed = false;
                comPort.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
            InquiringState = Enum_InquiringSimulator.NOT_LINKED;

            if (!Init())
            {
                try
                {
                    CloseDown.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                }
            }
            else
            {
                SendMessage("ASKSERIAL");
                SendMessage("ASKFIRMWARE");
            }
        }

        ~BeautySim()
        {
        }

        public delegate void AMessageArrived(string serial, byte type, byte[] message);

        public static bool DevicePresent
        {
            get {

                return (USBDevice.DeviceConnected("VID_2341", "PID_8037") || USBDevice.DeviceConnected("VID_2341", "PID_8237"));



            }
        }

        public string COM
        {
            get
            {
                return comPort.PortName;
            }
        }

        public string FirmwareVersion { get; private set; }
        public float Flux { get; private set; } //ml/s
        public short AccX { get; private set; }
        public short AccY { get; private set; }
        public short AccZ { get; private set; }
        public short AbsAccX { get; private set; }
        public short AbsAccY { get; private set; }
        public short AbsAccZ { get; private set; }
        public bool IsSimulator { get; private set; }

        public byte Number { get; internal set; }

        public string SerialNumber { get; private set; }
        public bool TagPresence { get; private set; }
        public float CurrentVolume { get; private set; }
        public DateTime CurrentStartTime { get; set; }
        public string Orientation { get; set; }
        public int HardwareVersion { get; private set; } = 1;

        public void Dispose()
        {
            if (comPort != null)
            {
                try
                {
                    comPort.DataReceived -= ComPort_DataReceived;
                    imClosed = true;

                    //close port in new thread to avoid hang
                    CloseDown.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public void SendMessage(string typeMessage)
        {
            try
            {
                List<byte> message = new List<byte>();

                switch (typeMessage)
                {
                    case "ASKSERIAL":
                        AddCode(ref message, Enum_SimulatorMessageType.SERIAL);
                        break;

                    case "ASKFIRMWARE":
                        AddCode(ref message, Enum_SimulatorMessageType.FIRMWARE);
                        break;

                    case "ASKFLUXGYRO":
                        AddCode(ref message, Enum_SimulatorMessageType.FLUXGYRO);
                        break;

                    case "SET_DEBUGON":
                        AddCode(ref message, Enum_SimulatorMessageType.PA_DEBUGMODE_ON);
                        break;

                    case "SET_DEBUGOFF":
                        AddCode(ref message, Enum_SimulatorMessageType.PA_DEBUGMODE_OFF);
                        break;

                    default:
                        break;
                }
                AddTerminator(ref message);
                Write(message.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void AddCode(ref List<byte> message, Enum_SimulatorMessageType messageType)
        {
            try
            {
                message.Add((byte)messageType);
            }
            catch (Exception)
            {
            }
        }

        private void AddFloat(ref List<byte> message, float floatVal)
        {
            try
            {
                byte[] c = BitConverter.GetBytes(floatVal);
                for (int i = 0; i < 4; i++)
                {
                    message.Add(c[i]);
                }
            }
            catch (Exception)
            {
            }
        }

        private void AddInitiator(ref List<byte> message)
        {
            try
            {
                message.Add((byte)'*');
                message.Add((byte)'/');
            }
            catch (Exception)
            {
            }
        }

        private void AddInt32(ref List<byte> message, Int32 int32Val)
        {
            try
            {
                byte[] c = BitConverter.GetBytes(int32Val);
                for (int i = 0; i < 4; i++)
                {
                    message.Add(c[i]);
                }
            }
            catch (Exception)
            {
            }
        }

        private void AddString(ref List<byte> message, string toAdd)
        {
            char[] listchar = toAdd.ToCharArray();
            foreach (char c in listchar)
                try
                {
                    message.Add((byte)c);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                }
        }

        private void AddTerminator(ref List<byte> message)
        {
            try
            {
                message.Add((byte)'\n');
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void CloseSerialPortOnExit()
        {
            comPort.Close();
            comPort.Dispose();
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!imClosed)
                {
                    UpdateDataFromSensor();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        
        private bool Init()
        {
            try
            {
                DateTime start = DateTime.Now;
                while (!IsSimulator && (DateTime.Now - start).TotalMilliseconds < 4000)
                {
                    SendMessage("ASKFIRMWARE");
                    SendMessage("SERIAL");
                    System.Threading.Thread.Sleep(200);
                }

                //qui chiedo se è un grasp e il seriale...
                return IsSimulator;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private void ProcessReceivedMessage(byte type, byte[] message)
        {
            try
            {
                switch (type)
                {
                    case (byte)Enum_SimulatorMessageType.FIRMWARE:
                        FirmwareVersion = System.Text.Encoding.Default.GetString(message);
                        if (FirmwareVersion.StartsWith("BTY"))
                        {
                            IsSimulator = true;
                        }
                        break;

                    case (byte)Enum_SimulatorMessageType.SERIAL:
                        SerialNumber = System.Text.Encoding.Default.GetString(message);
                        if (Convert.ToInt64(SerialNumber)>= 20200501001)
                        {
                            //MessageBox.Show("HardwareVersion set to 2"); 
                            HardwareVersion = 2;
                        }
                        OnSerialNumberAcquired(new EventArgs());
                        EnableDebugMode();
                        break;

                    case (byte)Enum_SimulatorMessageType.FLUX:
                        Flux = CalculateFlux(BitConverter.ToInt16(message, 0));
                        UpdateVolume(Flux, DateTime.Now);
                        break;

                    case (byte)Enum_SimulatorMessageType.FLUXGYRO:
                        Flux = CalculateFlux(BitConverter.ToInt16(message, 0));
                        AccX = BitConverter.ToInt16(message, 2);
                        AccY = BitConverter.ToInt16(message, 4);
                        AccZ = BitConverter.ToInt16(message, 6);

                        AbsAccX = Math.Abs(AccX);
                        AbsAccY = Math.Abs(AccY);
                        AbsAccZ = Math.Abs(AccZ);

                        int max3 = Math.Max(AbsAccX, Math.Max(AbsAccY, AbsAccZ));
                        if (max3==AbsAccX)
                        {
                            Orientation = "X" + (AccX > 0 ? "POS" : "NEG");
                        }
                        else
                        {
                            if (max3 == AbsAccY)
                            {
                                Orientation = "Y" + (AccY > 0 ? "POS" : "NEG");
                            }
                            else
                            {
                                Orientation = "Z" + (AccZ > 0 ? "POS" : "NEG");
                            }
                        }

                        UpdateVolume(Flux, DateTime.Now);
                        break;


                }
                if (AmessageArrivedEvent != null)
                {
                    AmessageArrivedEvent(SerialNumber, type, message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        protected virtual void OnSerialNumberAcquired(EventArgs e)
        {
            SerialNumberAcquired?.Invoke(this, e);
        }

        public void UpdateVolume(float flux, DateTime readTime)
        {

            if (!(lastReadTime==DateTime.MinValue))
            {
                if (Math.Abs(flux) > 0.05)
                {

                    CurrentVolume = CurrentVolume + ((float)flux * (float)(readTime - lastReadTime).TotalMilliseconds / 1000f);
                }
            }
            lastReadTime = readTime;

        }
        
        public void ZeroVolume()
        {
            lastReadTime = DateTime.MinValue;
            CurrentVolume = 0;
        }

        private float CalculateFlux(short v)
        {
            return (float)((v - 102) * 0.1 / (512 - 102) * 1000 / 60);
        }

        private void UpdateDataFromSensor()
        {
            try
            {
                if (comPort == null)
                {
                    return;
                }
                byte[] data = new byte[bufferSize];
                int recv = comPort.Read(data, 0, bufferSize);

                if (recv > 0)
                {
                    if (buffer == null)
                    {
                        buffer = new byte[recv];
                        Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
                    }
                    else
                    {
                        byte[] tmp = new byte[buffer.Length + recv];
                        Buffer.BlockCopy(buffer, 0, tmp, 0, buffer.Length);
                        Buffer.BlockCopy(data, 0, tmp, buffer.Length, recv);
                        buffer = tmp;
                    }

                    //ricerco un nuovo messaggio
                    int start_ptr = 0;
                    int len_message = 0;
                    byte message_type = 0;
                    bool validMessage = true;
                    while (start_ptr < buffer.Length - 2)
                    {
                        if (buffer[start_ptr] == '*' && buffer[start_ptr + 1] == '/')
                        {
                            start_ptr += 2;
                            //trovato inizio...
                            message_type = buffer[start_ptr];

                            switch (message_type)
                            {
                                case (byte)Enum_SimulatorMessageType.FIRMWARE:
                                    len_message = 7;
                                    break;

                                case (byte)Enum_SimulatorMessageType.SERIAL:
                                    len_message = 11;
                                    break;

                                case (byte)Enum_SimulatorMessageType.FLUXGYRO:
                                    len_message = 8;
                                    break;

                                case (byte)Enum_SimulatorMessageType.FLUX:
                                    len_message = 2;
                                    break;
                            }

                            break;
                        }

                        start_ptr++;
                    }
                    start_ptr++;
                    // se il messaggio è arrivato tutto...
                    if (validMessage && start_ptr + len_message + 2 < buffer.Length)
                    {
                        int end_ptr = start_ptr + len_message;
                        //se c'è anche il terminatore...
                        if (buffer[end_ptr] == '/' && buffer[end_ptr + 1] == '*')
                        {
                            byte[] message = new byte[len_message];
                            Buffer.BlockCopy(buffer, start_ptr, message, 0, len_message);

                            if (end_ptr + 2 == buffer.Length)
                            {
                                buffer = null;
                            }
                            else
                            {
                                byte[] tmp = new byte[buffer.Length - (end_ptr + 2)];
                                Buffer.BlockCopy(buffer, end_ptr + 2, tmp, 0, tmp.Length);
                                buffer = tmp;
                            }

                            ProcessReceivedMessage(message_type, message);
                        }
                        else
                        {
                            //Messaggio errato; va eliminato tutto il messaggio errato OPPURE fino al successivo Iniziatore */ se presente.
                            int indexStart = end_ptr + 2;
                            for (int i = start_ptr; i < buffer.Length - 1; i++)
                            {
                                if (buffer[i] == '*' && buffer[i + 1] == '/')
                                {
                                    indexStart = i;
                                    break;
                                }
                            }
                            byte[] tmp = new byte[buffer.Length - indexStart];
                            Buffer.BlockCopy(buffer, indexStart, tmp, 0, tmp.Length);
                            buffer = tmp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void Write(byte[] buffer)
        {
            try
            {
                //cripta dati
                buffer = encryption_function(buffer);
                comPort.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        public void EnableDebugMode()
        {
            SendMessage("SET_DEBUGON");
        }
    }
}