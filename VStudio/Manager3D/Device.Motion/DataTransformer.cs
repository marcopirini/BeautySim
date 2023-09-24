/******************************************
 * Class name:
 * Author:
 * Creation:
 * Last modify:
 * Version:
 *
 * DESCRIPTION
 * transform data acquired
 *
 * *****************************************/

using VectorMath;
using System;
using System.Collections.Generic;

namespace Device.Motion
{
    public class DataTransformer
    {
        public const int MAX_SENSORS = 16;

        private GENERALSETTINGS _general_settings;
        private SENSORSETTINGS[] _sensor_settings;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataTransformer()
        {
            _sensor_settings = new SENSORSETTINGS[MAX_SENSORS];

            _general_settings = new GENERALSETTINGS();
            _general_settings.AxisTransform = Matrix.Identity;
            _general_settings.OriginTranslation = Vector3.Zero;
            _general_settings.ReferenceMode = REFERENCE_MODE.SENSOR;
            _general_settings.EulerCalcMode = EULER_CALC_MODE.XYZ;

            for (int i = 0; i < MAX_SENSORS; i++)
            {
                _sensor_settings[i] = new SENSORSETTINGS();
                _sensor_settings[i].sensor_number = i;
                _sensor_settings[i].position_offset = Vector3.Zero;
                _sensor_settings[i].rotation_offset = Quaternion.Identity;

                _sensor_settings[i].compensation_enabled = false;
                _sensor_settings[i].compensation_sensor = -1;

                _sensor_settings[i].scaler = Vector3.Zero;
            }
        }

        /// <summary>
        /// Transform frames from raw data according to General and Sensors settings
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public MOTIONFRAMEList TransformFrames(List<FRAME> frames)
        {
            return TransformFunction(frames, this);
        }

        public Func<List<FRAME>,DataTransformer, MOTIONFRAMEList> TransformFunction { get; set; }

        private Quaternion RelativeRotation(FRAME f)
        {
            Quaternion inv_offset = _sensor_settings[f.Sensor].rotation_offset;
            inv_offset.Invert();
            return f.Ori * inv_offset;
        }

        /// <summary>
        /// Get general settings
        /// </summary>
        public GENERALSETTINGS GeneralSetting { get { return _general_settings; } }

        /// <summary>
        /// Get sensor settings
        /// </summary>
        /// <param name="i">sensor index</param>
        /// <returns></returns>
        public SENSORSETTINGS SensorSettings(int i)
        {
            if (i < MAX_SENSORS)
            {
                return _sensor_settings[i];
            }
            return new SENSORSETTINGS();
        }

        /// <summary>
        /// Reset compenation status
        /// </summary>
        public void ResetCompensation()
        {
            for (int i = 0; i < MAX_SENSORS; i++)
            {
                _sensor_settings[i].compensation_enabled = false;
                _sensor_settings[i].compensation_sensor = -1;
            }
        }

        public void SetCompensation(int sensor, bool state, int compensation_sensor)
        {
            if (sensor < MAX_SENSORS && sensor >= 0)
            {
                _sensor_settings[sensor].compensation_sensor = compensation_sensor;
                _sensor_settings[sensor].compensation_enabled = state;
            }
        }

        /// <summary>
        /// set general settings
        /// </summary>
        /// <param name="settings"></param>
        public void SetGeneralSetting(GENERALSETTINGS settings)
        {
            _general_settings = settings;
        }

        /// <summary>
        /// set sensor settings
        /// </summary>
        /// <param name="index"></param>
        /// <param name="settings"></param>
        public void SetSensorSettings(int index, SENSORSETTINGS settings)
        {
            if (index < MAX_SENSORS)
            {
                _sensor_settings[index] = settings;
            }
        }
    }
}