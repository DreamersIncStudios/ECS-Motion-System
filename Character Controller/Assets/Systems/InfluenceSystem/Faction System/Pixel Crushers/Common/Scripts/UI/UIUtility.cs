// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    public static class UIUtility
    {

        /// <summary>
        /// Ensures that the scene has an EventSystem.
        /// </summary>
        public static void RequireEventSystem()
        {
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                    typeof(UnityEngine.EventSystems.StandaloneInputModule)
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				    ,typeof(UnityEngine.EventSystems.TouchInputModule)
#endif
                    );
                var standaloneInputModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#if !UNITY_2021_1_OR_NEWER
                if (standaloneInputModule != null) standaloneInputModule.forceModuleActive = true;
#endif
            }
        }

        public static int GetAnimatorNameHash(AnimatorStateInfo animatorStateInfo)
        {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			return animatorStateInfo.nameHash;
#else
            return animatorStateInfo.fullPathHash;
#endif
        }

    }

}
