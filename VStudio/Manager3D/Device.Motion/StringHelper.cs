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

using System;
using System.Text;

namespace Device.Motion
{
   internal class StringHelper
    {
        public static byte[] StringToBuffer(string value,int len)
        {
            if (value != null && value.Length > 0)
            {
                byte[] ret = ASCIIEncoding.ASCII.GetBytes(value);
                byte[] tmp = new byte[len];
                Buffer.BlockCopy(ret, 0, tmp, 0, Math.Min(10, ret.Length));
                return tmp;
            }
            else
                return new byte[len];


        }

        public static string BufferToString(byte[] value)
        {
            if (value == null)
                return string.Empty;

            int index = 0;
            for (index = 0; index < value.Length; index++)
            {
                if (value[index] == 0)
                    break;
            }



            return ASCIIEncoding.ASCII.GetString(value,0,index);
        }
    
    }

}
