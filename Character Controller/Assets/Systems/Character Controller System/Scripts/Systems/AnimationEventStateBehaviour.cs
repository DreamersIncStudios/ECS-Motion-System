using System;
using UnityEngine;

namespace DreamersInc.AnimationEventSystem
{
    public class AnimationEventStateBehaviour : StateMachineBehaviour
    {
        public string EventName;
        [Range(0f, 1f)] public float TriggerTime;
        private bool hasTriggered;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hasTriggered = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var currentTime = stateInfo.normalizedTime % 1f;
            if (hasTriggered || !(currentTime >= TriggerTime)) return;
            NotifyReceiver(animator);
            hasTriggered = true;
        }

        private void NotifyReceiver(Animator animator)
        {
             var receiver = animator.GetComponent<AnimationEventReceiver>();
             if (receiver == null) return;
             receiver.OnAnimationEventTriggered(EventName);
        }
    }
}