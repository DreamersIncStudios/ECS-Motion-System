// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnModifyPad().
    /// </summary>
    public interface IModifyPadDeedEventHandler : IEventSystemHandler
    {

        /// <summary>
        /// Raises the modify pad event. The faction member's PAD values will have
        /// already been modified when this event is raised.
        /// </summary>
        /// <param name="happinessChange">Happiness change.</param>
        /// <param name="pleasureChange">Pleasure change.</param>
        /// <param name="arousalChange">Arousal change.</param>
        /// <param name="dominanceChange">Dominance change.</param>
        void OnModifyPad(float happinessChange, float pleasureChange, float arousalChange, float dominanceChange);

    }

}
