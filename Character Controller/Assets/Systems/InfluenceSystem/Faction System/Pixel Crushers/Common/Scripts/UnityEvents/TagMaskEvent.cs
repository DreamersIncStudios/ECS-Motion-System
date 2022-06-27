// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    public abstract class TagMaskEvent : MonoBehaviour
    {

        [Tooltip("Only fire event if triggered by GameObject with one of these tags.")]
        public TagMask tagMask = new TagMask();

        public bool IsInTagMask(string tag)
        {
            return tagMask.IsInTagMask(tag);
        }

    }

}
