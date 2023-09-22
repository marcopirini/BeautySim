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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace BeautySim.Common
{
    [Serializable]
    public class XMLBase
    {
        [XmlIgnore]
        public string FileName { get; private set; }

        public virtual bool Save<T>(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (TextWriter txt = new StreamWriter(file))
                {
                    try
                    {
                        serializer.Serialize(txt, this);
                        txt.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                FileName = file;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static T Load<T>(string file) where T : XMLBase
        {
            if (File.Exists(file))
            {
                using (TextReader txt = new StreamReader(file))
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        T obj = (T)serializer.Deserialize(txt);

                        txt.Close();
                        obj.FileName = file;
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        txt.Close();
                        Debug.WriteLine(ex.Message);
                        return null;
                    }
                }
            }

            return null;
        }

        public static T Load<T>(Stream str, string fileName) where T : XMLBase
        {
            using (TextReader txt = new StreamReader(str))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    T obj = (T)serializer.Deserialize(txt);

                    txt.Close();
                    obj.FileName = fileName;
                    return obj;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            return null;
        }
    }
}