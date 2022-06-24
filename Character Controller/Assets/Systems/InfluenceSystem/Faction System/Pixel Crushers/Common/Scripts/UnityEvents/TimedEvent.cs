// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes an event after a specified duration. The timer can be configured to
    /// start when the script starts or manually.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class TimedEvent : MonoBehaviour
    {

        public enum TimingMode { Seconds, Frames }

        [Tooltip("Count duration in seconds or number of frames.")]
        [SerializeField]
        private TimingMode m_mode = TimingMode.Seconds;

        [Tooltip("After starting timer, wait this many seconds before firing event. Mode must be set to Seconds.")]
        [SerializeField]
        private float m_duration = 0;

        [Tooltip("After starting timer, wait this many frames before firing event. Mode must be set to Frames.")]
        [SerializeField]
        private int m_frames = 0;

        [Tooltip("Start timer when this component starts.")]
        [SerializeField]
        private bool m_activateOnStart = true;

        [SerializeField]
        private UnityEvent m_onTimeReached = new UnityEvent();

        /// <summary>
        /// Count duration in seconds or number of frames.
        /// </summary>
        public TimingMode mode
        {
            get { return m_mode; }            
            set { m_mode = value; }
        }

        /// <summary>
        /// After starting timer, if Mode is Seconds, wait this many seconds before firing event.
        /// </summary>
        public float duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        /// <summary>
        /// After starting timer, if Mode is Frames, wait this many seconds before firing event.
        /// </summary>
        public int frames
        {
            get { return m_frames; }
            set { m_frames = value; }
        }

        /// <summary>
        /// Start timer when this component starts.
        /// </summary>
        public bool activateOnStart
        {
            get { return m_activateOnStart; }
            set { m_activateOnStart = value; }
        }

        private UnityEvent onTimeReached
        {
            get { return m_onTimeReached; }
            set { m_onTimeReached = value; }
        }

        protected virtual void Start()
        {
            if (activateOnStart) StartTimer(duration);
        }

        protected virtual void OnDisable()
        {
            CancelTimer();
        }

        /// <summary>
        /// Starts the timer manually.
        /// </summary>
        public virtual void StartTimer()
        {
            StartTimer(duration);
        }

        /// <summary>
        /// Starts the timer manually with a specified duration.
        /// </summary>
        /// <param name="duration">Duration to wait before invoking the event.</param>
        public virtual void StartTimer(float duration)
        {
            if (mode == TimingMode.Frames)
            {
                StartCoroutine(CountFrames());
            }
            else
            {
                Invoke("TimeReached", duration);
            }
        }

        private IEnumerator CountFrames()
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
            TimeReached();
        }

        /// <summary>
        /// Cancels the timer's current countdown.
        /// </summary>
        protected virtual void CancelTimer()
        {
            CancelInvoke("TimeReached");
            StopAllCoroutines();
        }

        protected virtual void TimeReached()
        {
            onTimeReached.Invoke();
        }

        public void DestroyUnityObject(UnityEngine.Object o)
        {
            Destroy(o);
        }

    }

}