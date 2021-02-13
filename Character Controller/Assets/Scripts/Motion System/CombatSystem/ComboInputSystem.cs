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
    public bool HeavyAttack => Input.GetKeyUp(InputSet.HeavyAttack);
    public bool ChargedLightAttack => Input.GetKeyUp(InputSet.ChargedLightAttack); // change to time base later 
    public bool ChargedHeavyAttack => Input.GetKeyUp(InputSet.ChargedHeavyAttack); // change to time base later 
    public bool Projectile => Input.GetKeyUp(InputSet.Projectile);


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
    bool TransitionToLocomotion => !StateInfo.IsTag("Locomotion") && StateInfo.normalizedTime > .95f;
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Player_Control PC, ComboComponent ComboList) =>
        {
           
            StateInfo = ComboList.animator.GetCurrentAnimatorStateInfo(0);
            if (!ComboList.animator.IsInTransition(0))
            {
                foreach (AnimationCombo comboOption in ComboList.combo.ComboList)
                {
                    if (StateInfo.IsName(comboOption.CurremtStateName.ToString()))
                    {
                        currentStateExitTime = comboOption.AnimationEndTime;
                        //Light
                        if (!comboOption.LightAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (LightAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                        //Heavy
                        if (!comboOption.HeavyAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (HeavyAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                        //Charge Light
                        if (!comboOption.ChargedLightAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (ChargedLightAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                        //Charge Heavy
                        if (!comboOption.ChargeHeavytAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (ChargedHeavyAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                        //projectile
                        if (!comboOption.Projectile.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (Projectile && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                    }
                }

                if (TakeInput)
                {
                    AnimationTriggers temp = InputQueue.Dequeue();
                    ComboList.animator.CrossFade(temp.TriggeredAnimName.ToString(), temp.TransitionDuration, 0, temp.StartOffset);

                }

                if (TransitionToLocomotion)
                {
                    ComboList.animator.CrossFade("Locomation_Grounded_Weapon", .25f, 0, 0.25f);
                }
            }
        });

    }
}
