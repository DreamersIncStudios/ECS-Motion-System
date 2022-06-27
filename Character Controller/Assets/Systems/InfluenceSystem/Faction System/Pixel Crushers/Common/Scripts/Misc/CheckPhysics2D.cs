// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    /// <summary>
    /// Checks if the scripting symbol USE_PHYSICS2D is defined.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class CheckPhysics2D : MonoBehaviour
    {

        public UnityEvent usePhysics2DDefined = new UnityEvent();
        public UnityEvent usePhysics2DUndefined = new UnityEvent();

        private void Start()
        {
#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER
            usePhysics2DDefined.Invoke();
#else
            usePhysics2DUndefined.Invoke();
#endif
        }
    }

}
