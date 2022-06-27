// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Implementation of DataSerializer that uses JsonUtility.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class JsonDataSerializer : DataSerializer
    {

        [Tooltip("Use larger but more human-readable format.")]
        [SerializeField]
        private bool m_prettyPrint;

        public bool prettyPrint
        {
            get { return m_prettyPrint; }
            set { m_prettyPrint = value; }
        }

        public override string Serialize(object data)
        {
#if UNITY_5_3_6 || UNITY_5_3_7 || UNITY_5_4_OR_NEWER
            return JsonUtility.ToJson(data, m_prettyPrint);
#else
            Debug.LogWarning("Save System: JSON Serialization is not supported before Unity 5.3.6.");
            return string.Empty;
#endif
        }

        public override T Deserialize<T>(string s, T data = default(T))
        {
#if UNITY_5_3_6 || UNITY_5_3_7 || UNITY_5_4_OR_NEWER
            if (Equals(data, default(T)))
            {
                return JsonUtility.FromJson<T>(s);
            }
            else
            {
                JsonUtility.FromJsonOverwrite(s, data);
                return data;
            }
#else
            Debug.LogWarning("Save System: JSON Serialization is not supported before Unity 5.3.6.");
            return default(T);
#endif
        }

    }

}
