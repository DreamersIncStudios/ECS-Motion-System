using System;
using UnityEngine.Events;

namespace DreamersInc.AnimationEventSystem
{
    [Serializable]
    public class AnimationEvent
    {
        public string EventName;
        public UnityEvent OnAnimationEvent;
    }
}
