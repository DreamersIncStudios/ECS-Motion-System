// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

    /// <summary>
    /// This class holds a text note in the Unity editor. In builds, it's
    /// an empty class that destroys itself on Awake.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Common/Misc/Editor Note")]
    public class EditorNote : MonoBehaviour
    {

#if UNITY_EDITOR
        [SerializeField]
        [TextArea(1,50)]
        private string m_note;

        public string note { get { return m_note; } set { m_note = value; } }
#else
        private void Awake()
        {
            Destroy(this);
        }
#endif

    }

}
