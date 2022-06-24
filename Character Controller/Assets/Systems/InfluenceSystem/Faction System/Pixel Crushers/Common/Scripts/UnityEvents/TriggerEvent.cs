// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes events on trigger enter and exit.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class TriggerEvent : TagMaskEvent
    {

        [SerializeField]
        private GameObjectUnityEvent m_onTriggerEnter = new GameObjectUnityEvent();

        [SerializeField]
        private GameObjectUnityEvent m_onTriggerExit = new GameObjectUnityEvent();

        public GameObjectUnityEvent onTriggerEnter
        {
            get { return m_onTriggerEnter; }
            set { m_onTriggerEnter = value; }
        }

        public GameObjectUnityEvent onTriggerExit
        {
            get { return m_onTriggerExit; }
            set { m_onTriggerExit = value; }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!IsInTagMask(other.tag)) return;
            onTriggerEnter.Invoke(other.gameObject);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!IsInTagMask(other.tag)) return;
            onTriggerExit.Invoke(other.gameObject);
        }

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsInTagMask(other.tag)) return;
            onTriggerEnter.Invoke(other.gameObject);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!IsInTagMask(other.tag)) return;
            onTriggerExit.Invoke(other.gameObject);
        }

#endif

    }

}
