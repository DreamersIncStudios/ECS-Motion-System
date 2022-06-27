// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Attribute to draw a StringField's text value as a multi-line text area.
    /// </summary>
    public class StringFieldTextAreaAttribute : PropertyAttribute
    {

        public bool expandHeight;

        public StringFieldTextAreaAttribute(bool expandHeight = true)
        {
            this.expandHeight = expandHeight;
        }
    }
}