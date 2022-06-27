// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// Uses the MessageSystem to keep data synchronized between a source and a listener.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class DataSynchronizer : MonoBehaviour, IMessageHandler
    {

        /// <summary>
        /// Message that data source sends to inform listeners that the value changed.
        /// </summary>
        public const string DataSourceValueChangedMessage = "Data Source Value Changed";

        /// <summary>
        /// Message that listeners send to request this data synchronizer to change the data source's value.
        /// </summary>
        public const string RequestDataSourceChangeValueMessage = "Request Data Source Change Value";

        [Tooltip("A name to associate with the data source. Data change messages that reference this name will invoke the value update events.")]
        [SerializeField]
        private string m_dataSourceName;

        [SerializeField]
        private ObjectUnityEvent m_onRequestDataSourceChangeValue = new ObjectUnityEvent();

        /// <summary>
        /// A name to associate with the data source. Data change messages that reference this name will invoke the value update events.
        /// </summary>
        public string dataSourceName
        {
            get { return m_dataSourceName; }
            set { m_dataSourceName = value; }
        }

        /// <summary>
        /// Event to send to data source to request it to change value.
        /// </summary>
        public ObjectUnityEvent onRequestDataSourceChangeValue
        {
            get { return m_onRequestDataSourceChangeValue; }
            set { m_onRequestDataSourceChangeValue = value; }
        }

        protected virtual void OnEnable()
        {
            MessageSystem.AddListener(this, RequestDataSourceChangeValueMessage, dataSourceName);
        }

        protected virtual void OnDisable()
        {
            MessageSystem.RemoveListener(this, RequestDataSourceChangeValueMessage, dataSourceName);
        }

        /// <summary>
        /// Handles messages requesting to change the value of the data source. On receipt of the 
        /// RequestDataSourceChangeValueMessage, invokes the onRequestDataSourceChangeValue event,
        /// passing the first argument of the message as the new value.
        /// </summary>
        /// <param name="messageArgs">First argument is the new value.</param>
        public void OnMessage(MessageArgs messageArgs)
        {
            onRequestDataSourceChangeValue.Invoke(messageArgs.firstValue);
        }

        /// <summary>
        /// When the data source has changed its value, it should call this method to inform listeners.
        /// </summary>
        /// <param name="newValue">The data source's new value.</param>
        public void DataSourceValueChanged(object newValue)
        {
            MessageSystem.SendMessage(this, DataSourceValueChangedMessage, dataSourceName, newValue);
        }

    }
}
