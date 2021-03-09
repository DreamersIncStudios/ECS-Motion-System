using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]

public class Command : IComponentData
{
    public Queue<AnimationTriggers> InputQueue;
    public bool QueueIsEmpty => InputQueue.Count == 0;
    public AnimatorStateInfo StateInfo;
    public float currentStateExitTime;

    public bool TakeInput => !QueueIsEmpty && StateInfo.normalizedTime > currentStateExitTime;
    public bool TransitionToLocomotion => !StateInfo.IsTag("Locomotion") && StateInfo.normalizedTime > .95f;
}
