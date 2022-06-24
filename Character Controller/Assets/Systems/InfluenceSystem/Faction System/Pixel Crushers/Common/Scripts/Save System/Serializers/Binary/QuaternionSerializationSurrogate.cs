// Based on code by takatok: https://forum.unity.com/threads/vector3-is-not-marked-serializable.435303/#post-2814558
using UnityEngine;
using System.Runtime.Serialization;

namespace PixelCrushers
{

    /// <summary>
    /// Adds .NET serialization support for Quaternion.
    /// </summary>
    public class QuaternionSerializationSurrogate : ISerializationSurrogate
    {

        // Method called to serialize a Quaternion object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {

            Quaternion quarternion = (Quaternion)obj;
            info.AddValue("w", quarternion.w);
            info.AddValue("x", quarternion.x);
            info.AddValue("y", quarternion.y);
            info.AddValue("z", quarternion.z);
        }

        // Method called to deserialize a Quaternion object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info,
                                           StreamingContext context, ISurrogateSelector selector)
        {

            Quaternion quaternion = (Quaternion)obj;
            quaternion.w = (float)info.GetValue("w", typeof(float));
            quaternion.x = (float)info.GetValue("x", typeof(float));
            quaternion.y = (float)info.GetValue("y", typeof(float));
            quaternion.z = (float)info.GetValue("z", typeof(float));
            obj = quaternion;
            return obj;
        }
    }
}