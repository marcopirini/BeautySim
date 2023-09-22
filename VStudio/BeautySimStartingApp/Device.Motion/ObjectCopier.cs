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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Device.Motion
{
    public static class ObjectCopier
    {
        /// <summary> 
        /// Perform a deep Copy of the object. 
        /// </summary> 
        /// <typeparam name="T">The type of object being copied.</typeparam> 
        /// <param name="source">The object instance to copy.</param> 
        /// <returns>The copied object.</returns> 
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException(BlockSim.Globalization.Language.str_type_serializ, "source");
            }

            // Don't serialize a null object, simply return the default for that object 
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // THE ERROR IS HERE
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    } 
}
