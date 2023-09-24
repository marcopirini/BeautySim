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
using System.Collections.Generic;
using System.Linq;
using VectorMath;

namespace Device.Motion
{
    public class MOTIONFRAMEList : List<MOTIONFRAME>
    {
        /// <summary>
        /// Get sensor struct instance
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public MOTIONFRAME GetSensor(int number)
        {
            var item = (from db in this where db.Sensor == number select db).SingleOrDefault();          
            return item;
        }

        /// <summary>
        /// Return the list of frames of specific sensor
        /// </summary>
        /// <param name="senosor"></param>
        /// <returns></returns>
        public List<MOTIONFRAME> GetFrames(int sensor)
        {
            try
            {
                var item = from db in this where db.Sensor == sensor select db;
                return item.ToList();
            }
            catch (Exception)
            {

                return new List<MOTIONFRAME>();
            }
          
        }

        /// <summary>
        /// Get position value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public Vector3 Pos(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return frame.Pos;
        }

        /// <summary>
        /// Get position value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public Matrix Mat(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return frame.Mat;
        }

        /// <summary>
        /// Get pitch value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public float Pitch(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return (float)frame.Pitch;
        }

        /// <summary>
        /// Get yaw value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        /// 
        public float Yaw(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return (float)frame.Yaw;
        }

        /// <summary>
        /// Get rool value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        /// 
        public float Roll(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return (float)frame.Roll;
        }

        /// <summary>
        /// Get dirx value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public Vector3 X(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return frame.X;
        }

        /// <summary>
        /// Get dirx value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public Vector3 Y(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return frame.Y;
        }
        /// <summary>
        /// Get dirx value
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        /// 
        public Vector3 Z(int sensor)
        {
            MOTIONFRAME frame = GetSensor(sensor);

            return frame.Z;
        }

       

   
        /// <summary>
        /// get sensor prensence
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool SensorPresent(int number)
        {
            var item = (from db in this where db.Sensor == number select db);
            return item.Count() > 0;
        }

    }
}
