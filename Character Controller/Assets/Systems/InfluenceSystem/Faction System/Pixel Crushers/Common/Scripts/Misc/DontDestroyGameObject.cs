// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Marks the GameObject as DontDestroyOnLoad.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class DontDestroyGameObject : MonoBehaviour
    {

        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

    }

}
