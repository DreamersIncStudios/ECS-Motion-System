#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0

using UnityEngine;
using UnityEditor;
using System.IO;
using PixelCrushers;

namespace PixelCrushers.Wrappers
{

    public static class TextTableAssetMenu
    {

        [MenuItem("Assets/Create/Pixel Crushers/Common/Text/Text Table", false, 0)]
        public static void CreateTextTable()
        {
            AssetUtility.CreateAsset<TextTable>("Text Table");
        }

    }

}

#endif