using UnityEngine;

namespace DreamersInc.AnimationEventSystem
{
    public class AnimationToggleEventStateBehaviour : StateMachineBehaviour
    {
        public string EventName;
        [Range(0f, 1f)] public float OnTriggerTime;
        [Range(0f, 1f)] public float OffTriggerTime;
        private bool hasToggleOn;
        private bool hasToggleOff;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hasToggleOn= false;
            hasToggleOff = true;
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var currentTime = stateInfo.normalizedTime % 1f;
            if (!hasToggleOn && (currentTime >= OnTriggerTime))
            {
                NotifyReceiver(animator, "On");
                hasToggleOn = true;
                hasToggleOff = false;

            }

            if (hasToggleOff || (!(currentTime >= OffTriggerTime))) return;
            NotifyReceiver(animator, "Off");
            hasToggleOff = true;
            hasToggleOn = false;
        }
        private void NotifyReceiver(Animator animator, string adder)
        {
            var receiver = animator.GetComponent<AnimationEventReceiver>();
            if (receiver == null) return;
            receiver.OnAnimationEventTriggered(EventName+adder);
        }
    }
}