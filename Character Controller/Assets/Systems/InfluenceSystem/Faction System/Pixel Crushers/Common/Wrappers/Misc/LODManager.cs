// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

    /// <summary>
    /// This wrapper for PixelCrushers.LODManager keeps references intact if you
    /// switch between the compiled assembly and source code versions of the original
    /// class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Common/Misc/LOD Manager")]
    public class LODManager : PixelCrushers.LODManager
    {
    }

}
