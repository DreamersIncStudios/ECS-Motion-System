// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes a callback method when an animator has entered and then exited a
    /// specified trigger state.
    /// </summary>
    public class UIAnimatorMonitor
    {

        public static float MaxWaitDuration = 10;

        private MonoBehaviour m_target;

        private bool m_lookedForAnimator = false;

        private Animator m_animator = null;

        private Animation m_animation = null;

        private Coroutine m_coroutine = null;

        public string currentTrigger { get; private set; }

        public UIAnimatorMonitor(GameObject target)
        {
            m_target = (target != null) ? target.GetComponent<MonoBehaviour>() : null;
            currentTrigger = string.Empty;
        }

        public UIAnimatorMonitor(MonoBehaviour target)
        {
            m_target = target;
            currentTrigger = string.Empty;
        }

        public void SetTrigger(string triggerName, System.Action callback, bool wait = true)
        {
            if (m_target == null) return;
            m_target.gameObject.SetActive(true);
            CancelCurrentAnimation();
            if (!m_target.gameObject.activeInHierarchy) return; // May still be inactive if quitting application.
            m_coroutine = m_target.StartCoroutine(WaitForAnimation(triggerName, callback, wait));
        }

        private IEnumerator WaitForAnimation(string triggerName, System.Action callback, bool wait)
        {
            if (HasAnimator() && !string.IsNullOrEmpty(triggerName))
            {
                if (IsAnimatorValid())
                {
                    // Run Animator and wait:
                    CheckAnimatorModeAndTimescale(triggerName);
                    m_animator.SetTrigger(triggerName);
                    currentTrigger = triggerName;
                    float timeout = Time.realtimeSinceStartup + MaxWaitDuration;
                    var goalHashID = Animator.StringToHash(triggerName);
                    var oldHashId = UIUtility.GetAnimatorNameHash(m_animator.GetCurrentAnimatorStateInfo(0));
                    var currentHashID = oldHashId;
                    if (wait)
                    {
                        while ((currentHashID != goalHashID) && (currentHashID == oldHashId) && (Time.realtimeSinceStartup < timeout))
                        {
                            yield return null;
                            currentHashID = IsAnimatorValid() ? UIUtility.GetAnimatorNameHash(m_animator.GetCurrentAnimatorStateInfo(0)) : 0;
                        }
                        if (Time.realtimeSinceStartup < timeout && IsAnimatorValid())
                        {
                            var clipLength = m_animator.GetCurrentAnimatorStateInfo(0).length;
                            if (Mathf.Approximately(0, Time.timeScale))
                            {
                                timeout = Time.realtimeSinceStartup + clipLength;
                                while (Time.realtimeSinceStartup < timeout)
                                {
                                    yield return null;
                                }
                            }
                            else
                            {
                                yield return new WaitForSeconds(clipLength);
                            }
                        }
                    }
                }
                else if (m_animation != null && m_animation.enabled)
                {
                    m_animation.Play(triggerName);
                    if (wait)
                    {
                        var clip = m_animation.GetClip(triggerName);
                        if (clip != null)
                        {
                            yield return new WaitForSeconds(clip.length);
                        }
                    }
                }
            }
            currentTrigger = string.Empty;
            m_coroutine = null;
            if (callback != null) callback.Invoke();
        }

        private bool HasAnimator()
        {
            if (m_animator == null && m_animation == null && !m_lookedForAnimator)
            {
                m_lookedForAnimator = true;
                if (m_target != null)
                {
                    m_animator = m_target.GetComponent<Animator>();
                    if (m_animator == null)
                    {
                        m_animation = m_target.GetComponent<Animation>();
                        if (m_animation == null)
                        {
                            m_animator = m_target.GetComponentInChildren<Animator>();
                            if (m_animator == null)
                            {
                                m_animation = m_target.GetComponentInChildren<Animation>();
                            }
                        }
                    }
                }
            }
            return (m_animator != null || m_animation != null);
        }

        private bool IsAnimatorValid()
        {
            return m_animator != null && m_animator.enabled && m_animator.runtimeAnimatorController != null;
        }

        private void CheckAnimatorModeAndTimescale(string triggerName)
        {
            if (m_animator == null) return;
            if (Mathf.Approximately(0, Time.timeScale) && (m_animator.updateMode != AnimatorUpdateMode.UnscaledTime))
            {
                m_animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }

        public void CancelCurrentAnimation()
        {
            if (m_coroutine == null || m_target == null) return;
            currentTrigger = string.Empty;
            m_target.StopCoroutine(m_coroutine);
            m_coroutine = null;
        }

    }

}
