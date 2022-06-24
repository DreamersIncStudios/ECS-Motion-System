// Copyright (c) Pixel Crushers. All rights reserved.

#if UNITY_5_3_OR_NEWER
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace PixelCrushers
{

    /// <summary>
    /// This implementation of SceneTransitionManager plays optional outro and 
    /// intro animations, and optionally loads a loading scene.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class StandardSceneTransitionManager : SceneTransitionManager
    {

        [Tooltip("Pause time during the transition.")]
        public bool pauseDuringTransition = true;

        [Serializable]
        public class TransitionInfo
        {
            [Tooltip("Animator for this transition.")]
            public Animator animator;
            [Tooltip("Trigger parameter to set.")]
            public string trigger;
            [Tooltip("Duration to wait for the animation.")]
            public float animationDuration;
            [Tooltip("Total duration to wait for the transition.")]
            public float minTransitionDuration;
            public UnityEvent onTransitionStart = new UnityEvent();
            public UnityEvent onTransitionEnd = new UnityEvent();
            public void TriggerAnimation()
            {
                if (animator == null || string.IsNullOrEmpty(trigger)) return;
                animator.SetTrigger(trigger);
            }
        }

        [Tooltip("Transition to play before leaving the current scene.")]
        public TransitionInfo leaveSceneTransition = new TransitionInfo();

        [Tooltip("If set, show this loading scene while loading the real destination scene asynchronously.")]
        public string loadingSceneName;

        [Tooltip("Transition to play after entering the new scene.")]
        public TransitionInfo enterSceneTransition = new TransitionInfo();

        public override IEnumerator LeaveScene()
        {
            leaveSceneTransition.onTransitionStart.Invoke();
            var startTime = Time.realtimeSinceStartup;
            var minAnimationTime = startTime + leaveSceneTransition.animationDuration;
            var minEndTime = startTime + Mathf.Max(leaveSceneTransition.minTransitionDuration, leaveSceneTransition.animationDuration);
            if (pauseDuringTransition)
            {
                Time.timeScale = 0;
            }
            leaveSceneTransition.TriggerAnimation();
            while (Time.realtimeSinceStartup < minAnimationTime)
            {
                yield return null;
            }
            if (!string.IsNullOrEmpty(loadingSceneName))
            {
                yield return SceneManager.LoadSceneAsync(loadingSceneName);
            }
            while (Time.realtimeSinceStartup < minEndTime)
            {
                yield return null;
            }
            leaveSceneTransition.onTransitionEnd.Invoke();
        }

        public override IEnumerator EnterScene()
        {
            enterSceneTransition.onTransitionStart.Invoke();
            var startTime = Time.realtimeSinceStartup;
            var minAnimationTime = startTime + enterSceneTransition.animationDuration;
            var minEndTime = startTime + Mathf.Max(enterSceneTransition.minTransitionDuration, enterSceneTransition.animationDuration);
            enterSceneTransition.TriggerAnimation();
            while (Time.realtimeSinceStartup < minAnimationTime)
            {
                yield return null;
            }
            while (Time.realtimeSinceStartup < minEndTime)
            {
                yield return null;
            }
            if (pauseDuringTransition)
            {
                Time.timeScale = 1; //---Always reset to normal time.
            }
            enterSceneTransition.onTransitionEnd.Invoke();
        }

    }
}
#endif
