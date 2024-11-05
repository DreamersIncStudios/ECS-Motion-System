using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        
        public void AssignEvent(string eventName, UnityAction test)
        { 
            var matchingEvent = animationEvents.Find(se => se.EventName == eventName);
            matchingEvent?.OnAnimationEvent.AddListener(test);
            matchingEvent?.OnAnimationEvent.AddListener(()=> Debug.Log(eventName));
            if(matchingEvent != null)
                Debug.Log($"Event {eventName} was assigned to event {matchingEvent.EventName}");
        }
        public void RemoveEvent(string eventName, UnityAction test)
        { 
            var matchingEvent = animationEvents.Find(se => se.EventName == eventName);
            matchingEvent?.OnAnimationEvent.RemoveListener(test);
        }
    }
}