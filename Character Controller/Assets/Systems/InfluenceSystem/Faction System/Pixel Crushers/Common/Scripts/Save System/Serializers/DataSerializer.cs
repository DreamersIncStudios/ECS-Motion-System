// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Abstract base class for serializers that serialize objects to strings and
    /// deserialize strings back into objects.
    /// </summary>
    public abstract class DataSerializer : MonoBehaviour
    {

        public abstract string Serialize(object data);
        public abstract T Deserialize<T>(string s, T data = default(T));

    }

}
