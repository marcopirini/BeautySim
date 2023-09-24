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

namespace Device.Motion
{
    public static class ClassExtensions
    {
        public static double ToDegree(this double value)
        {
            return value * 180.0 / Math.PI;
        }

    }
}
