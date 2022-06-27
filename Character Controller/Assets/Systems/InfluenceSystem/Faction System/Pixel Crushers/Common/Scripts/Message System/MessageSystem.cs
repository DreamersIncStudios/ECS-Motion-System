// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// General purpose message system.
    /// </summary>
    public static class MessageSystem
    {

        public class ListenerInfo
        {
            public IMessageHandler listener;
            public string message;
            public string parameter;
            public int frameAdded;
            public bool removed;

            public ListenerInfo() { }

            public ListenerInfo(IMessageHandler listener, string message, string parameter)
            {
                this.listener = listener;
                this.message = message;
                this.parameter = parameter;
                this.frameAdded = Time.frameCount;
                this.removed = false;
            }

            public void Assign(IMessageHandler listener, string message, string parameter)
            {
                this.listener = listener;
                this.message = message;
                this.parameter = parameter;
                this.frameAdded = Time.frameCount;
                this.removed = false;
            }

            public void Clear()
            {
                this.listener = null;
                this.message = null;
                this.parameter = null;
                this.removed = false;
            }
        }

        private static List<ListenerInfo> s_listenerInfo = new List<ListenerInfo>();

        private static Pool<ListenerInfo> s_listenerInfoPool = new Pool<ListenerInfo>();

        private static HashSet<GameObject> s_sendersToLog = new HashSet<GameObject>();
        private static HashSet<GameObject> s_listenersToLog = new HashSet<GameObject>();

        private static bool s_sendInEditMode = false;

        private static bool s_debug = false;

        private static int s_sendMessageDepth = 0;

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            s_listenerInfo = new List<ListenerInfo>();
            s_listenerInfoPool = new Pool<ListenerInfo>();
            s_sendersToLog = new HashSet<GameObject>();
            s_listenersToLog = new HashSet<GameObject>();
            s_sendInEditMode = false;
            s_debug = false;
            s_sendMessageDepth = 0;
        }
#endif

        /// <summary>
        /// Send messages even when not playing.
        /// </summary>
        public static bool sendInEditMode
        {
            get { return s_sendInEditMode; }
            set { s_sendInEditMode = value; }
        }

        /// <summary>
        /// Log message system activity.
        /// </summary>
        public static bool debug
        {
            get { return s_debug && Debug.isDebugBuild; }
            set { s_debug = value; }
        }

        private static List<ListenerInfo> listenerInfo { get { return s_listenerInfo; } }

        private static Pool<ListenerInfo> listenerInfoPool { get { return s_listenerInfoPool; } }

        /// <summary>
        /// When we're in SendMessage(), don't remove items from listenerInfo because SendMessage() is
        /// currently looping through listenerInfo. Instead, mark them for removal afterward.
        /// </summary>
        private static int sendMessageDepth { get { return s_sendMessageDepth; } set { s_sendMessageDepth = value; } }

        /// <summary>
        /// Checks if the specified listener, message, and parameter is registered with the message system.
        /// </summary>
        /// <param name="listener">Listener to check.</param>
        /// <param name="message">Message to check.</param>
        /// <param name="parameter">Parameter to check, or blank for any parameter.</param>
        /// <returns></returns>
        public static bool IsListenerRegistered(IMessageHandler listener, string message, string parameter)
        {
            for (int i = 0; i < listenerInfo.Count; i++)
            {
                var x = listenerInfo[i];
                if (!x.removed && x.listener == listener && string.Equals(x.message, message) && (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(x.parameter)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, string message, string parameter)
        {
            if (debug) Debug.Log("MessageSystem.AddListener(listener=" + listener + ": " + message + "," + parameter + ")");

            // Check if listener is already registered:
            for (int i = 0; i < listenerInfo.Count; i++)
            {
                var x = listenerInfo[i];
                if (x.listener == listener && string.Equals(x.message, message) && (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(x.parameter)))
                {
                    x.removed = false;
                    return;
                }
            }

            // Otherwise add:
            var info = listenerInfoPool.Get();
            info.Assign(listener, message, parameter);
            listenerInfo.Add(info);
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, StringField message, StringField parameter)
        {
            AddListener(listener, StringField.GetStringValue(message), StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, StringField message, string parameter)
        {
            AddListener(listener, StringField.GetStringValue(message), parameter);
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, string message, StringField parameter)
        {
            AddListener(listener, message, StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for, or blank for all messages.</param>
        /// <param name="parameter">Message parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, string message, string parameter)
        {
            if (debug) Debug.Log("MessageSystem.RemoveListener(listener=" + listener + ": " + message + "," + parameter + ")");
            if (listenerInfo.Count <= 0) return;
            for (int i = listenerInfo.Count - 1; i >= 0; i--)
            {
                var x = listenerInfo[i];
                if (x.listener == listener &&
                    (string.Equals(x.message, message) || string.IsNullOrEmpty(message)) &&
                    (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(parameter)))
                {
                    x.removed = true;
                    if (sendMessageDepth == 0)
                    {
                        listenerInfo.RemoveAt(i);
                        x.Clear();
                        listenerInfoPool.Release(x);
                    }
                }
            }
        }

        private static void RemoveMarkedListenerInfo()
        {
            var listenersToRemove = listenerInfo.FindAll(x => x.removed);
            listenerInfo.RemoveAll(x => x.removed);
            for (int i = 0; i < listenersToRemove.Count; i++)
            {
                var listenerToRemove = listenersToRemove[i];
                listenerToRemove.Clear();
                listenerInfoPool.Release(listenerToRemove);
            }
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, StringField message, StringField parameter)
        {
            RemoveListener(listener, StringField.GetStringValue(message), StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, StringField message, string parameter)
        {
            RemoveListener(listener, StringField.GetStringValue(message), parameter);
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, string message, StringField parameter)
        {
            RemoveListener(listener, message, StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Removes a listener from listening to all messages.
        /// </summary>
        public static void RemoveListener(IMessageHandler listener)
        {
            RemoveListener(listener, string.Empty, string.Empty);
        }

        /// <summary>
        /// Log a debug message when this object sends a message.
        /// </summary>
        public static void LogWhenSendingMessages(GameObject sender)
        {
            if (sender == null) return;
            s_sendersToLog.Add(sender);
        }

        /// <summary>
        /// Stop logging debug messages when this object sends a message.
        /// </summary>
        public static void StopLoggingWhenSendingMessages(GameObject sender)
        {
            if (sender == null) return;
            s_sendersToLog.Remove(sender);
        }

        /// <summary>
        /// Log a debug message when this listener receives a message.
        /// </summary>
        public static void LogWhenReceivingMessages(GameObject listener)
        {
            if (listener == null) return;
            s_listenersToLog.Add(listener);
        }

        /// <summary>
        /// Stop logging debug messages when this listener receives a message.
        /// </summary>
        public static void StopLoggingWhenReceivingMessages(GameObject listener)
        {
            if (listener == null) return;
            s_listenersToLog.Add(listener);
        }

        private static bool ShouldLogSender(object sender)
        {
            if (sender is UnityEngine.Object && (sender as UnityEngine.Object) == null) return false;
            return (sender is GameObject && s_sendersToLog.Contains(sender as GameObject)) ||
                (sender is Component && s_sendersToLog.Contains((sender as Component).gameObject));
        }

        private static bool ShouldLogReceiver(IMessageHandler receiver)
        {
            return (receiver is Component && (receiver as Component) != null && s_listenersToLog.Contains((receiver as Component).gameObject));
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, string message, string parameter, params object[] values)
        {
            if (!(Application.isPlaying || sendInEditMode)) return;
            if (debug || ShouldLogSender(sender)) Debug.Log("MessageSystem.SendMessage(sender=" + sender +
                ((target == null) ? string.Empty : (" target=" + target)) +
                ": " + message + "," + parameter + ")");
            var messageArgs = new MessageArgs(sender, target, message, parameter, values); // struct passed on stack; no heap allocated.
            try
            {
                sendMessageDepth++;
                for (int i = 0; i < listenerInfo.Count; i++)
                {
                    var x = listenerInfo[i];
                    if (x == null || x.removed) continue;
                    if (x.listener == null)
                    {
                        x.removed = true;
                        continue;
                    }
                    if (x.frameAdded == Time.frameCount) continue;
                    if (string.Equals(x.message, message) && (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(x.parameter)))
                    {
                        try
                        {
                            if (ShouldLogReceiver(x.listener))
                            {
                                Debug.Log("MessageSystem.SendMessage(sender=" + sender +
                                    ((target == null) ? string.Empty : (" target=" + target)) +
                                    ": " + message + "," + parameter + ")");
                            }
                            x.listener.OnMessage(messageArgs);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("Message System exception sending '" + message + "'/'" + parameter + "' to " + x.listener + ": " + e.Message);
                        }
                    }
                }
            }
            finally
            {
                sendMessageDepth--;
                if (sendMessageDepth == 0) RemoveMarkedListenerInfo();
            }
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, StringField message, string parameter, params object[] values)
        {
            SendMessageWithTarget(sender, target, StringField.GetStringValue(message), parameter, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, StringField message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, target, StringField.GetStringValue(message), StringField.GetStringValue(parameter), values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, string message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, target, message, StringField.GetStringValue(parameter), values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, string message, string parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message, parameter, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, StringField message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message.value, parameter.value, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, StringField message, string parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message.value, parameter, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, string message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message, parameter.value, values);
        }

        /// <summary>
        /// Sends a message. If the message contains a colon (:), the part after the 
        /// colon is sent as the parameter. If it contains a second colon, the part 
        /// after the second colon is sent as a value.
        /// </summary>
        public static void SendCompositeMessage(object sender, string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var parameter = string.Empty;
            object value = null;
            if (message.Contains(":")) // Parameter?
            {
                var colonPos = message.IndexOf(':');
                parameter = message.Substring(colonPos + 1);
                message = message.Substring(0, colonPos);

                if (parameter.Contains(":")) // Value?
                {
                    colonPos = parameter.IndexOf(':');
                    var valueString = parameter.Substring(colonPos + 1);
                    parameter = parameter.Substring(0, colonPos);
                    int valueInt;
                    bool isNumeric = int.TryParse(valueString, out valueInt);
                    if (isNumeric) value = valueInt; else value = valueString;
                }
            }
            if (value == null)
            {
                SendMessage(sender, message, parameter);
            }
            else
            {
                SendMessage(sender, message, parameter, value);
            }
        }


    }
}
