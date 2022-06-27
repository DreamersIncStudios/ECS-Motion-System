// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnRememberDeed().
    /// </summary>
    public interface IRememberDeedEventHandler : IEventSystemHandler
    {

        /// <summary>
        /// Raises the remember deed event.
        /// </summary>
        /// <param name="rumor">The faction member's subjective evaluation of the deed, 
        /// which was just added to memory.</param>
        void OnRememberDeed(Rumor rumor);

    }

}
