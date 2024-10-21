using System.Collections.Generic;
using UnityEngine;

namespace DreamersInc.AnimationEventSystem
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] private List<AnimationEvent> animationEvents = new();

        public void OnAnimationEventTriggered(string eventName)
        {
            var matchingEvent = animationEvents.Find(se => se.EventName == eventName);
            matchingEvent?.OnAnimationEvent?.Invoke();
        }
    }
}