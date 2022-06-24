// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
    [CreateAssetMenu(menuName = "Pixel Crushers/Common/Text/Text Table")]
#endif
    /// <summary>
    /// This wrapper for PixelCrushers.TextTable keeps references intact if you
    /// switch between the compiled assembly and source code versions of the original
    /// class.
    /// </summary>
    public class TextTable : PixelCrushers.TextTable
    {
    }

}
