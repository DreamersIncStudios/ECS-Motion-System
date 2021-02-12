using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using System.Collections.Generic;
using GameMaster;

public class ComboInputSystem : ComponentSystem
{
    public ControllerScheme InputSet => GameMasterSystem.GMS.InputSettings.UserScheme;
    public bool LightAttack => Input.GetKeyUp(InputSet.LightAttack);

    Queue<AnimationTriggers> InputQueue;

    protected override void OnCreate()
    {
        base.OnCreate();
        InputQueue = new Queue<AnimationTriggers>();

    }

    public bool QueueIsEmpty => InputQueue.Count == 0;

    float currentStateExitTime;
    DynamicBuffer<AnimationCombo> Combos;
    AnimatorStateInfo StateInfo;
    bool TakeInput =>!QueueIsEmpty && StateInfo.normalizedTime > currentStateExitTime;
    bool TransitionToLocomotion => !StateInfo.IsTag("Locomotion") && StateInfo.normalizedTime > .75f;
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, Animator Anim, ref Player_Control PC) =>
        {
           Combos = GetBufferFromEntity<AnimationCombo>()[entity];
            StateInfo = Anim.GetCurrentAnimatorStateInfo(0);
            if (!Anim.IsInTransition(0))
            {
                foreach (AnimationCombo comboOption in Combos)
                {
                    if (StateInfo.IsName(comboOption.GetTest.CurremtStateName.ToString()))
                    {
                        currentStateExitTime = comboOption.GetTest.AnimationEndTime;
                        if (!comboOption.GetTest.Unlocked)
                            return;
                        if (comboOption.GetTest.InputAllowed(StateInfo.normalizedTime))
                        {
                            if (LightAttack && QueueIsEmpty)
                            {
                                InputQueue.Enqueue(comboOption.GetTest.LightAttack);
                            }
                        }
                    }
                }

                if (TakeInput)
                {
                    AnimationTriggers temp = InputQueue.Dequeue();
                    Anim.CrossFade(temp.TriggeredAnimName.ToString(), .25f, 0, temp.StartOffset);

                }

                if (TransitionToLocomotion)
                {
                    Anim.CrossFade("Locomation_Grounded_Weapon", .25f, 0, 0.25f);
                }
            }
        });

    }
}
