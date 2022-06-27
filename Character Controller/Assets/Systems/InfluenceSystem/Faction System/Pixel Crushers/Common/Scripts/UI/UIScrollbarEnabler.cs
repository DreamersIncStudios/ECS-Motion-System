// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;

namespace PixelCrushers
{

    /// <summary>
    /// Enables a scrollbar only if the content is larger than the scroll rect. This component only
    /// only shows or hides the scrollbar when the component is enabled or when CheckScrollbar is invoked.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UIScrollbarEnabler : MonoBehaviour
    {

        [Tooltip("The scroll rect.")]
        [UnityEngine.Serialization.FormerlySerializedAs("container")]
        public UnityEngine.UI.ScrollRect scrollRect = null;

        [Tooltip("The content inside the scroll rect. The scrollbar will be enabled if the content is taller than the scroll rect.")]
        [UnityEngine.Serialization.FormerlySerializedAs("content")]
        public RectTransform scrollContent = null;

        [Tooltip("The scrollbar to enable or disable. If scroll rect doesn't have a scrollbar, just scrolls scroll rect.")]
        public UnityEngine.UI.Scrollbar scrollbar = null;

        [Tooltip("Scroll smoothly instead of jumping to reset value.")]
        public bool smoothScroll = false;

        public float smoothScrollSpeed = 5;

        protected bool m_started = false;
        protected bool m_checking = false;
        protected RectTransform m_scrollRectTransform = null;

        protected virtual void Start()
        {
            m_started = true;
            CheckScrollbar();
        }

        public virtual void OnEnable()
        {
            if (m_started) CheckScrollbar();
        }

        public virtual void OnDisable()
        {
            m_checking = false;
        }

        public virtual void CheckScrollbar()
        {
            if (m_checking || scrollRect == null || scrollContent == null || !gameObject.activeInHierarchy || !enabled) return;
            StopAllCoroutines();
            StartCoroutine(CheckScrollbarAfterUIUpdate(false, 0));
        }

        public virtual void CheckScrollbarWithResetValue(float value)
        {
            if (m_checking || scrollRect == null || scrollContent == null || !gameObject.activeInHierarchy || !enabled) return;
            StopAllCoroutines();
            StartCoroutine(CheckScrollbarAfterUIUpdate(true, value));
        }

        protected virtual IEnumerator CheckScrollbarAfterUIUpdate(bool useResetValue, float resetValue)
        {
            m_checking = true;
            yield return null;
            if (scrollbar != null)
            {
                scrollbar.gameObject.SetActive(scrollContent.rect.height > scrollRect.GetComponent<RectTransform>().rect.height);
            }
            m_checking = false;
            yield return null;
            if (useResetValue)
            {
                if (smoothScroll)
                {
                    var contentHeight = scrollContent.rect.height;
                    if (m_scrollRectTransform == null) m_scrollRectTransform = scrollRect.GetComponent<RectTransform>();
                    var scrollRectHeight = m_scrollRectTransform.rect.height;
                    var needToScroll = contentHeight > scrollRectHeight;
                    if (needToScroll)
                    {
                        var ratio = scrollRectHeight / contentHeight;
                        var timeout = Time.time + 10f; // Avoid infinite loops by maxing out at 10 seconds.
                        while (scrollRect.verticalNormalizedPosition > 0.01f && Time.time < timeout)
                        {
                            var newPos = scrollRect.verticalNormalizedPosition - smoothScrollSpeed * Time.deltaTime * ratio;
                            scrollRect.verticalNormalizedPosition = Mathf.Max(0, newPos);
                            yield return null;
                        }
                    }
                    scrollRect.verticalNormalizedPosition = 0;
                }
                else
                {
                    if (scrollbar != null) scrollbar.value = resetValue;
                    scrollRect.verticalNormalizedPosition = resetValue;
                }
            }
        }

    }
}
