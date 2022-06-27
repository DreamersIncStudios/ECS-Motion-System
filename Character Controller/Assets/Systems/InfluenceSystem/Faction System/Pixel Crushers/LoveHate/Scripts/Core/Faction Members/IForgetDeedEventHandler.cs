// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnForgetDeed().
    /// </summary>
    public interface IForgetDeedEventHandler : IEventSystemHandler
    {

        /// <summary>
        /// Raises the forget deed event.
        /// </summary>
        /// <param name="rumor">The rumor that the faction member just forgot.</param>
        void OnForgetDeed(Rumor rumor);

    }

}
