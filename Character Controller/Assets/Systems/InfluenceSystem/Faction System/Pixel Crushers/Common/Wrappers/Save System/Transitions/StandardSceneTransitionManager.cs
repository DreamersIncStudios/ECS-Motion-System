// Copyright (c) Pixel Crushers. All rights reserved.

#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace PixelCrushers.Wrappers
{

    /// <summary>
    /// This wrapper for PixelCrushers.StandardSceneTransitionManager keeps references intact if you switch 
    /// between the compiled assembly and source code versions of the original class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Save System/Scene Transition Managers/Standard Scene Transition Manager")]
    public class StandardSceneTransitionManager : PixelCrushers.StandardSceneTransitionManager
    {
    }

}
#endif