// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers
{

    /// <summary>
    /// Interface for MessageSystem message handlers.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles a message that the message handler is listening for.
        /// </summary>
        /// <param name="messageArgs">The message that was sent to the MessageSystem.</param>
        void OnMessage(MessageArgs messageArgs);
    }

}