// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

    /// <summary>
    /// This wrapper for PixelCrushers.DiskSavedGameDataStorer keeps references intact 
    /// if you switch between the compiled assembly and source code versions of the original
    /// class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Save System/Saved Game Data Storers/Disk Saved Game Data Storer")]
    public class DiskSavedGameDataStorer : PixelCrushers.DiskSavedGameDataStorer
    {
    }

}
