// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// A StringAsset is a ScriptableObject that encapsulates a string. It's useful
    /// to share references to a string, where the value of that string can change.
    /// </summary>
    public class StringAsset : ScriptableObject
    {

        [TextArea]
        [SerializeField]
        private string m_text;

        public string text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public override string ToString()
        {
            return text;
        }

    }

}
