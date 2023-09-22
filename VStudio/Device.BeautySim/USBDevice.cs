using System;
using System.Collections.Generic;

namespace Device.BeautySim
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
                List<USBClass.DeviceProperties> ListOfDP = new List<USBClass.DeviceProperties>();

                return USBClass.GetUSBDevice(vid, pid, ref ListOfDP, false);

                //return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
