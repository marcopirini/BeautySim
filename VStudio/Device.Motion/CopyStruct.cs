/******************************************
 * Class name:CopyStruct
 * Author:  Denis
 * Creation: 
 * Last modify:
 * Version:
 * 
 * DESCRIPTION
 * Copy structure utility
 * 
 * *****************************************/

using System;
using System.Runtime.InteropServices;

namespace Device.Motion
{
    public class CopyStruct   <T>
    {
        public static byte[] StructToByte(T source)
        {
            byte[] data = new byte[Marshal.SizeOf(source)];
            IntPtr pnt_v = Marshal.AllocHGlobal(Marshal.SizeOf(source));
            Marshal.StructureToPtr(source, pnt_v, false);
            Marshal.Copy(pnt_v, data, 0, Marshal.SizeOf(source));
            Marshal.FreeHGlobal(pnt_v);
            return data;
        }

        public static void ByteToStruct(ref T dest, byte[] data, int offset)
        {

            int len = Marshal.SizeOf(dest);
            IntPtr pnt_dest = Marshal.AllocHGlobal(Marshal.SizeOf(dest));
            //Marshal.StructureToPtr(dest, pnt_dest, false);
            Marshal.Copy(data, offset, pnt_dest, Marshal.SizeOf(dest));
            dest = (T)Marshal.PtrToStructure(pnt_dest, typeof(T));
            Marshal.FreeHGlobal(pnt_dest);

        }
    }
}
