// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes events on collision enter and exit.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class CollisionEvent : TagMaskEvent
    {

        [SerializeField]
        private GameObjectUnityEvent m_onCollisionEnter = new GameObjectUnityEvent();

        [SerializeField]
        private GameObjectUnityEvent m_onCollisionExit = new GameObjectUnityEvent();

        public GameObjectUnityEvent onCollisionEnter
        {
            get { return m_onCollisionEnter; }
            set { m_onCollisionEnter = value; }
        }

        public GameObjectUnityEvent onCollisionExit
        {
            get { return m_onCollisionExit; }
            set { m_onCollisionExit = value; }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!IsInTagMask(collision.gameObject.tag)) return;
            onCollisionEnter.Invoke(collision.gameObject);
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (!IsInTagMask(collision.gameObject.tag)) return;
            onCollisionExit.Invoke(collision.gameObject);
        }

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsInTagMask(collision.gameObject.tag)) return;
            onCollisionEnter.Invoke(collision.gameObject);
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!IsInTagMask(collision.gameObject.tag)) return;
            onCollisionExit.Invoke(collision.gameObject);
        }

#endif

    }

}
