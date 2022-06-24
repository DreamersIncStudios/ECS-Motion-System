// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes an event when the component is disabled or destroyed.
    /// If you don't want to invoke the event when the component is disabled
    /// and destroyed by a scene change, call SceneNotifier.NotifyWillUnloadScene().
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class DisappearEvent : MonoBehaviour
    {

        public enum EventTrigger { OnDisable, OnDestroy }

        [Tooltip("Run the event when this happens.")]
        [SerializeField]
        private EventTrigger m_eventTrigger = EventTrigger.OnDestroy;

        [SerializeField]
        private UnityEvent m_onDisappeared = new UnityEvent();

        /// <summary>
        /// Run the event when this happens.
        /// </summary>
        public EventTrigger eventTrigger
        {
            get { return m_eventTrigger; }
            set { m_eventTrigger = value; }
        }

        public UnityEvent onDisappeared
        {
            get { return m_onDisappeared; }
            set { m_onDisappeared = value; }
        }

        private bool m_ignore = false;

        private void OnEnable()
        {
            SceneNotifier.willUnloadScene -= OnWillUnload;
            SceneNotifier.willUnloadScene += OnWillUnload;
        }

        private void OnDisable()
        {
            SceneNotifier.willUnloadScene -= OnWillUnload;
            if (!m_ignore && eventTrigger == EventTrigger.OnDisable)
            {
                RunEvent();
            }
        }

        private void OnDestroy()
        {
            if (!m_ignore && eventTrigger == EventTrigger.OnDestroy)
            {
                RunEvent();
            }
        }

        public void RunEvent()
        {
            onDisappeared.Invoke();
        }

        public void OnWillUnload(int sceneIndex)
        {
            m_ignore = true;
        }

        private void OnApplicationQuit()
        {
            m_ignore = true;
        }

    }

}