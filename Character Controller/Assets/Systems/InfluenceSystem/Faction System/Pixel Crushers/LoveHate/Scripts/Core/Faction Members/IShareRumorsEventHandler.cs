// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnShareRumors(FactionMember).
    /// </summary>
    public interface IShareRumorsEventHandler : IEventSystemHandler
    {

        /// <summary>
        /// Raises the share rumors event.
        /// </summary>
        /// <param name="other">The other faction member that this faction
        /// member is sharing deeds with.</param>
        void OnShareRumors(FactionMember other);

    }

}
