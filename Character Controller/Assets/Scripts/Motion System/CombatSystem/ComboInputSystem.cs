using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using System.Collections.Generic;




public class ComboInputSystem : ComponentSystem
{

    protected override void OnCreate()
    {
        base.OnCreate();
        movespanel = new GameObject();
    }


    GameObject movespanel;
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Player_Control PC, ComboComponentAuthoring ComboList, Animator anim, Command handler) =>
        {
            if(handler.InputQueue == null)
                handler.InputQueue = new Queue<AnimationTriggers>();

            handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (PC.DisplayCombos)
            {
                ComboList.Combo.ShowMovesPanel = !ComboList.Combo.ShowMovesPanel;
                if (ComboList.Combo.ShowMovesPanel)
                    movespanel = ComboList.Combo.DisplayCombo();
                else
                    Object.Destroy(movespanel);
            }

            if (!anim.IsInTransition(0) && !ComboList.Combo.ShowMovesPanel)
            {
                foreach (AnimationCombo comboOption in ComboList.Combo.ComboList)
                {
                    if (handler.StateInfo.IsName(comboOption.CurremtStateName.ToString()))
                    {
                        handler.currentStateExitTime = comboOption.AnimationEndTime;
                        //Light
                        if (comboOption.LightAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                if (PC.LightAttack && handler.QueueIsEmpty)
                                {
                                    handler.InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                        //Heavy
                        if (comboOption.HeavyAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                if (PC.HeavyAttack && handler.QueueIsEmpty)
                                {
                                    handler.InputQueue.Enqueue(comboOption.HeavyAttack);
                                }
                            }
                        }
                        //Charge Light
                        if (comboOption.ChargedLightAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                if (PC.ChargedLightAttack && handler.QueueIsEmpty)
                                {
                                    handler.InputQueue.Enqueue(comboOption.ChargedLightAttack);
                                }
                            }
                        }
                        //Charge Heavy
                        if (comboOption.ChargedHeavyAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                if (PC.ChargedHeavyAttack && handler.QueueIsEmpty)
                                {
                                    handler.InputQueue.Enqueue(comboOption.ChargedHeavyAttack);
                                }
                            }
                        }
                        //projectile
                        if (comboOption.Projectile.Unlocked)
                        {
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                if (PC.Projectile && handler.QueueIsEmpty)
                                {
                                    handler.InputQueue.Enqueue(comboOption.Projectile);
                                }
                            }
                        }
                    }
                }
            }
        });

        Entities.ForEach((Animator anim, Command handler) =>
        {

            if (handler.TakeInput)
                {
                    AnimationTriggers temp = handler.InputQueue.Dequeue();
                    anim.CrossFade(temp.TriggeredAnimName.ToString(), temp.TransitionDuration, 0, temp.StartOffset);

                }

            if (handler.TransitionToLocomotion)
            {
                anim.CrossFade("Locomation_Grounded_Weapon", .25f, 0, 0.25f);
            }
            
        });

    }
}
