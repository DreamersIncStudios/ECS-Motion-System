// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    public enum HelpBoxMessageType { None, Info, Warning, Error }

    /// <summary>
    /// Attribute to draw a help box.
    /// </summary>
    public class HelpBoxAttribute : PropertyAttribute
    {

        public string text;
        public HelpBoxMessageType messageType;

        public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None)
        {
            this.text = text;
            this.messageType = messageType;
        }
    }
}