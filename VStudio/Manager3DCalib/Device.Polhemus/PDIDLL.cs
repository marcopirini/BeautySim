using System;
using System.Runtime.InteropServices;

namespace Device.Polhemus
{
    internal class PDIDLL
    {
                
        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "Create")]
        private static extern IntPtr Create_32();
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "Create")]
        private static extern IntPtr Create_64();
        public static IntPtr Create()
        {
            return IntPtr.Size == 8 /* 64bit */ ? Create_64() : Create_32();
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "Connect")]
        private static extern bool Connect_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "Connect")]
        private static extern bool Connect_64(IntPtr value);
        public static bool Connect(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? Connect_64(value) : Connect_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "Disconnect")]
        static extern bool Disconnect_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "Disconnect")]
        static extern bool Disconnect_64(IntPtr value);
        public static bool Disconnect(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? Disconnect_64(value) : Disconnect_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "Delete")]
        static extern void Delete_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "Delete")]
        static extern void Delete_64(IntPtr value);
        public static void Delete(IntPtr value)
        {
            if (IntPtr.Size == 8) /* 64bit */
                Delete_64(value);
            else
                Delete_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "DiscoverCnx")]
        static extern PDIePiCommType DiscoverCnx_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "DiscoverCnx")]
        static extern PDIePiCommType DiscoverCnx_64(IntPtr value);
        public static PDIePiCommType DiscoverCnx(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? DiscoverCnx_64(value) : DiscoverCnx_32(value);
        }


        
        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "GetStationMap")]
        static extern bool GetStationMap_32(IntPtr value,ref int dwMap);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "GetStationMap")]
        static extern bool GetStationMap_64(IntPtr value, ref int dwMap);
        public static bool GetStationMap(IntPtr value, ref int dwMap)
        {
            return IntPtr.Size == 8 /* 64bit */ ? GetStationMap_64(value,ref dwMap) : GetStationMap_32(value,ref dwMap);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "ResetTracker")]
        static extern bool ResetTracker_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "ResetTracker")]
        static extern bool ResetTracker_64(IntPtr value);
        public static bool ResetTracker(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? ResetTracker_64(value) : ResetTracker_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "CnxReady")]
        static extern bool CnxReady_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "CnxReady")]
        static extern bool CnxReady_64(IntPtr value);
        public static bool CnxReady(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? CnxReady_64(value) : CnxReady_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "StopContPno")]
        static extern bool StopContPno_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "StopContPno")]
        static extern bool StopContPno_64(IntPtr value);
        public static bool StopContPno(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? StopContPno_64(value) : StopContPno_32(value);
        }

       

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "GetFrameRate")]
        static extern bool GetFrameRate_32(IntPtr value,ref PDIePiFrameRate FR);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "GetFrameRate")]
        static extern bool GetFrameRate_64(IntPtr value, ref PDIePiFrameRate FR);
        public static bool GetFrameRate(IntPtr value, ref PDIePiFrameRate FR)
        {
            return IntPtr.Size == 8 /* 64bit */ ? GetFrameRate_64(value, ref FR) : GetFrameRate_32(value, ref FR);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "GetLastResult")]
        static extern PDIePiErrCode GetLastResult_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "GetLastResult")]
        static extern PDIePiErrCode GetLastResult_64(IntPtr value);
        public static PDIePiErrCode GetLastResult(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? GetLastResult_64(value) : GetLastResult_32(value);
        }


        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "ReadSinglePnoBuf")]
        static extern bool ReadSinglePnoBuf_32(IntPtr value,ref byte[] buf,ref int size);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "ReadSinglePnoBuf")]
        static extern bool ReadSinglePnoBuf_64(IntPtr value, ref byte[] buf, ref int size);
        public static bool ReadSinglePnoBuf(IntPtr value, ref byte[] buf, ref int size)
        {
            return IntPtr.Size == 8 /* 64bit */ ? ReadSinglePnoBuf_64(value,ref buf,ref size) : ReadSinglePnoBuf_32(value, ref buf, ref size);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "ReadSinglePno")]
        static extern bool ReadSinglePno_32(IntPtr value, IntPtr hwnd);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "ReadSinglePno")]
        static extern bool ReadSinglePno_64(IntPtr value, IntPtr hwnd);
        public static bool ReadSinglePno(IntPtr value, IntPtr hwnd)
        {
            return IntPtr.Size == 8 /* 64bit */ ? ReadSinglePno_64(value,hwnd) : ReadSinglePno_32(value, hwnd);
        }


        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "LastHostFrameCount")]
        static extern bool LastHostFrameCount_32(IntPtr value,ref int FC);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "LastHostFrameCount")]
        static extern bool LastHostFrameCount_64(IntPtr value, ref int FC);
        public static bool LastHostFrameCount(IntPtr value, ref int FC)
        {
            return IntPtr.Size == 8 /* 64bit */ ? LastHostFrameCount_64(value, ref FC) : LastHostFrameCount_32(value, ref FC);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "LastPnoPtr")]
        static extern bool LastPnoPtr_32(IntPtr value,IntPtr buf,ref int size);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "LastPnoPtr")]
        static extern bool LastPnoPtr_64(IntPtr value, IntPtr buf, ref int size);
        public static bool LastPnoPtr(IntPtr value, IntPtr buf, ref int size)
        {
            return IntPtr.Size == 8 /* 64bit */ ? LastPnoPtr_64(value, buf, ref size) : LastPnoPtr_32(value, buf, ref size);
        }


        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "SetPnoBuffer")]
        static extern bool SetPnoBuffer_32(IntPtr value,IntPtr buf,int size);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "SetPnoBuffer")]
        static extern bool SetPnoBuffer_64(IntPtr value, IntPtr buf, int size);
        public static bool SetPnoBuffer(IntPtr value, IntPtr buf, int size)
        {
            return IntPtr.Size == 8 /* 64bit */ ? SetPnoBuffer_64(value, buf, size) : SetPnoBuffer_32(value, buf, size);
        }


        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "ResetPnoPtr")]
        static extern bool ResetPnoPtr_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "ResetPnoPtr")]
        static extern bool ResetPnoPtr_64(IntPtr value);
        public static bool ResetPnoPtr(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? ResetPnoPtr_64(value) : ResetPnoPtr_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "ClearPnoBuffer")]
        static extern bool ClearPnoBuffer_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "ClearPnoBuffer")]
        static extern bool ClearPnoBuffer_64(IntPtr value);
        public static bool ClearPnoBuffer(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? ClearPnoBuffer_64(value) : ClearPnoBuffer_32(value);
        }

        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "SetDataList")]
        static extern bool SetDataList_32(IntPtr value);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "SetDataList")]
        static extern bool SetDataList_64(IntPtr value);
        public static bool SetDataList(IntPtr value)
        {
            return IntPtr.Size == 8 /* 64bit */ ? SetDataList_64(value) : SetDataList_32(value);
        }


        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "StartContPno")]
        static extern bool StartContPno_32(IntPtr value, IntPtr hwnd);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "StartContPno")]
        static extern bool StartContPno_64(IntPtr value, IntPtr hwnd);
        public static bool StartContPno(IntPtr value,IntPtr hwnd)
        {
            return IntPtr.Size == 8 /* 64bit */ ? StartContPno_64(value,hwnd) : StartContPno_32(value, hwnd);
        }


        [DllImport("PDIDLLWrapperX86.dll", EntryPoint = "SetMetric")]
        static extern bool SetMetric_32(IntPtr value, bool metric);
        [DllImport("PDIDLLWrapperX64.dll", EntryPoint = "SetMetric")]
        static extern bool SetMetric_64(IntPtr value, bool metric);
        public static bool SetMetric(IntPtr value, bool metric)
        {
            return IntPtr.Size == 8 /* 64bit */ ? SetMetric_64(value, metric) : SetMetric_32(value, metric);
        }


    }
}
