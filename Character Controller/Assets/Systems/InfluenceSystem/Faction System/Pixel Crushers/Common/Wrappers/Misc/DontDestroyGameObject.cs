// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

    /// <summary>
    /// This wrapper for PixelCrushers.DontDestroyGameObject keeps references intact if you
    /// switch between the compiled assembly and source code versions of the original
    /// class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Common/Misc/Dont Destroy GameObject")]
    public class DontDestroyGameObject : PixelCrushers.DontDestroyGameObject
    {
    }

}
