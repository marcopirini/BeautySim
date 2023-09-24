using System;
using System.Collections.Generic;


namespace Device.Motion
{
    public class USBDevice
    {
        public static bool DeviceConnected(string VID, string PID)
        {
            try
            {
                //"VID_0F44", "PID_CF01"
                VID = VID.Replace("VID_", "");
                PID = PID.Replace("PID_", "");
                uint vid = Convert.ToUInt32(VID, 16);
                uint pid = Convert.ToUInt32(PID, 16);
                List<USBClassLibrary.USBClass.DeviceProperties> ListOfDP = new List<USBClassLibrary.USBClass.DeviceProperties>();
                bool res= USBClassLibrary.USBClass.GetUSBDevice(vid, pid, ref ListOfDP, true);
                return res;

                //return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
