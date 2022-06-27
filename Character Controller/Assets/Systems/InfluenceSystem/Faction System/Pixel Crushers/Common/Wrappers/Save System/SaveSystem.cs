// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

    /// <summary>
    /// This wrapper for PixelCrushers.SaveSystem keeps references intact if you switch 
    /// between the compiled assembly and source code versions of the original class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Save System/Save System", -1)]
    public class SaveSystem : PixelCrushers.SaveSystem
    {
    }

}
