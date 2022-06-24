// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    [AddComponentMenu("")] // Use wrapper.
    public class MessageSystemLogger : MonoBehaviour
    {

        [Tooltip("Log a message when this GameObject sends a message to the Message System.")]
        public bool logWhenSendingMessages;

        [Tooltip("Log a message when this GameObject receives a message from the Message System.")]
        public bool logWhenReceivingMessages;

        private void OnEnable()
        {
            if (logWhenSendingMessages)
            {
                MessageSystem.LogWhenSendingMessages(gameObject);
            }
            if (logWhenReceivingMessages)
            {
                if (GetComponent(typeof(IMessageHandler)) == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("MessageSystem: " + name + " doesn't have any IMessageHandler components. Can't log when receiving messages.", this);
                }
                else
                {
                    MessageSystem.LogWhenReceivingMessages(gameObject);
                }
            }
        }

        private void OnDisable()
        {
            if (logWhenSendingMessages)
            {
                MessageSystem.StopLoggingWhenSendingMessages(gameObject);
            }
            if (logWhenReceivingMessages)
            {
                MessageSystem.StopLoggingWhenReceivingMessages(gameObject);
            }
        }
    }
}