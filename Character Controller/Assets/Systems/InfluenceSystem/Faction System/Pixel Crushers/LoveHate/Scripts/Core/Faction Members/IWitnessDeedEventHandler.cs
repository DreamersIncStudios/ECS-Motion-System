// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnWitnessDeed().
    /// </summary>
    public interface IWitnessDeedEventHandler : IEventSystemHandler
    {

        /// <summary>
        /// Raises the witness deed event.
        /// </summary>
        /// <param name="rumor">The faction member's subjective evaluation of the deed, 
        /// which may or may not be added to the faction member's memory based on its
        /// perceived importance to the faction member.</param>
        void OnWitnessDeed(Rumor rumor);

    }

}
