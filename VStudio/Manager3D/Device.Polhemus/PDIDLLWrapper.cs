using System;
using System.Runtime.InteropServices;

namespace Device.Polhemus
{
    public class PDIDLLWrapper
    {
        private volatile static PDIDLLWrapper _instance;
        private IntPtr pdi_dll;

        public static PDIDLLWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PDIDLLWrapper();
                }
                return _instance;
            }
        }

        private PDIDLLWrapper()
        {
            try
            {
                pdi_dll = PDIDLL.Create();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            
        }

        public string LastError { get; set; }

        public bool Connect()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.Connect(pdi_dll);					
		}

        public bool Disconnect()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.Disconnect(pdi_dll);				
		}

        public PDIePiCommType DiscoverCnx()
		{
            if (pdi_dll == IntPtr.Zero)
                return PDIePiCommType.PI_CNX_NONE;

            return PDIDLL.DiscoverCnx(pdi_dll);				
		}



        public bool SetDataList()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.SetDataList(pdi_dll);

		}

        public bool SetMetric(bool value)
        {
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.SetMetric(pdi_dll, value);

        }


        public bool GetStationMap(ref int dwMap)
		{
            if (pdi_dll == IntPtr.Zero)
                return false;
            
            return PDIDLL.GetStationMap(pdi_dll, ref dwMap);
		}

        public bool ResetTracker()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.ResetTracker(pdi_dll);
		}
        public bool CnxReady()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.CnxReady(pdi_dll);
		}

        public bool StopContPno()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.StopContPno(pdi_dll);
			
		}

        public bool StartContPno(IntPtr hwnd)
        {
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.StartContPno(pdi_dll, hwnd);

        }


        public bool GetFrameRate(ref PDIePiFrameRate FR)
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.GetFrameRate(pdi_dll, ref FR);
		}

        public PDIePiErrCode GetLastResult()
		{
            if (pdi_dll == IntPtr.Zero)
                return PDIePiErrCode.PI_API_ERROR;

            return PDIDLL.GetLastResult(pdi_dll);
		}

        public bool ReadSinglePnoBuf(byte[] buf)
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            byte[] buffer = new byte[10000];
            int size = 0;

            bool res = PDIDLL.ReadSinglePnoBuf(pdi_dll, ref buffer, ref size);

            if (res)
            {
                buf = new byte[size];
                Buffer.BlockCopy(buffer, 0, buf, 0, size);
            }

            return res;

		}

        public bool ReadSinglePno(IntPtr hwnd)
		{
            if (pdi_dll == IntPtr.Zero)
                return false;
            bool res = PDIDLL.ReadSinglePno(pdi_dll, hwnd);
            return res;
			

		}

        public bool LastHostFrameCount(ref int FC)
		{
            if (pdi_dll == IntPtr.Zero)
                return false;

            return PDIDLL.LastHostFrameCount(pdi_dll,ref FC);
		}

        public bool LastPnoPtr(ref byte[] buf)
		{
			
            if (pdi_dll == IntPtr.Zero)
                return false;

           
            int size = 0;
            IntPtr ptr = Marshal.AllocHGlobal(500);

            bool res = PDIDLL.LastPnoPtr(pdi_dll,  ptr, ref size);

            if (res)
            {
                 buf = new byte[size];
                 Marshal.Copy(ptr, buf,0, size);               
                //Buffer.BlockCopy(buffer, 0, buf, 0, size);
            }

            Marshal.FreeHGlobal(ptr);

            return res;
		}

        public bool LastPnoPtr(ref byte[] buf, int num_frames)
		{
			
            
            if (pdi_dll == IntPtr.Zero)
                return false;
            

            return LastPnoPtr(ref buf);

		}


        public bool SetPnoBuffer(IntPtr buf,int length)
		{

            if (pdi_dll == IntPtr.Zero)
                return false;
            return PDIDLL.SetPnoBuffer(pdi_dll, buf, length);


		}

        public bool ResetPnoPtr()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;
            return PDIDLL.ResetPnoPtr(pdi_dll);
		}

        public bool ClearPnoBuffer()
		{
            if (pdi_dll == IntPtr.Zero)
                return false;
            return PDIDLL.ClearPnoBuffer(pdi_dll);
		}
		
		

        public void Dispose()
        {
            PDIDLL.Delete(pdi_dll);
            pdi_dll = IntPtr.Zero;
        }
						

    }
}
