#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0

using UnityEngine;
using UnityEditor;
using System.IO;
using PixelCrushers;

namespace PixelCrushers.Wrappers
{

    public static class StringAssetMenu
    {

        [MenuItem("Assets/Create/Pixel Crushers/Common/Text/String Asset", false, 0)]
        public static void CreateStringAsset()
        {
            AssetUtility.CreateAsset<StringAsset>("String Asset");
        }

    }

}

#endif