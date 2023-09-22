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
using System.IO;
using System.Runtime.InteropServices;

namespace Device.Motion
{
    //FILEHEADER
    //GENERALSETTINGS
    //SENSORSETTINGS
    //[DATA]

    public class MOTIONFILE
    {
        public MOTIONFILEHEADER Header;
  
        public List<FRAME> Frames = new List<FRAME>();

        public DataTransformer _transformer;
        /// <summary>
        /// Default constructor
        /// </summary>
        public MOTIONFILE()
        {
            _transformer = new DataTransformer();
           
        }

        public byte[] DataBuffer { get; private set; }

        /// <summary>
        /// Constructor from filename
        /// </summary>
        /// <param name="filename"></param>
        public MOTIONFILE(string filename)
        {

            _transformer = new DataTransformer();

            //Check if file exist
            if (!File.Exists(filename))
            {
                throw new SystemException(BeautySim.Globalization.Language.str_no_file);
            }

            BinaryReader br = null;
            try
            {
                br = new BinaryReader(new FileStream(filename, FileMode.Open));
                //read header to byte buffer
                byte[] header_buffer = br.ReadBytes(Marshal.SizeOf(Header));
                //copy to struct
                CopyStruct<MOTIONFILEHEADER>.ByteToStruct(ref Header, header_buffer, 0);

                //Do same with general settings
                GENERALSETTINGS GeneralSettings = new GENERALSETTINGS();
                byte[] gen_settings_buffer = br.ReadBytes(Marshal.SizeOf(GeneralSettings));
                CopyStruct<GENERALSETTINGS>.ByteToStruct(ref GeneralSettings, gen_settings_buffer, 0);
                _transformer.SetGeneralSetting(GeneralSettings);

                //Do same with SENSORSETTINGS                
                for (int i = 0; i < DataTransformer.MAX_SENSORS; i++)
                {
                    SENSORSETTINGS sensor_settings = new SENSORSETTINGS();
                    byte[] sensor_settings_buffer = br.ReadBytes(Marshal.SizeOf(sensor_settings));
                    CopyStruct<SENSORSETTINGS>.ByteToStruct(ref sensor_settings, sensor_settings_buffer, 0);
                    _transformer.SetSensorSettings(i, sensor_settings);
                }


                DataBuffer = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (br != null)
                    br.Close();
            }
        }

        /// <summary>
        /// Get online data transformer
        /// </summary>
        public DataTransformer Transformer
        {
            get { return _transformer; }
        }
    }
}
