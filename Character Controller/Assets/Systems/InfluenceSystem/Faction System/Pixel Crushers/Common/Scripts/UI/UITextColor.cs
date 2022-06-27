// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// This script provides methods to change a Text element's color.
    /// You can tie it to hover events on buttons if you want the button's
    /// text color to change when hovered.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UITextColor : MonoBehaviour
    {

        public Color color;

        public UnityEngine.UI.Text text;

        private Color m_originalColor;

        private void Awake()
        {
            if (text == null) text = GetComponentInChildren<UnityEngine.UI.Text>();
            if (text != null) m_originalColor = text.color;
        }

        public void ApplyColor()
        {
            if (text == null) return;
            m_originalColor = text.color;
            text.color = color;
        }

        public void UndoColor()
        {
            if (text == null) return;
            text.color = m_originalColor;
        }

    }

}
