using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Unity.Entities;

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private static void ComboInputHandling([NotNull] PlayerComboComponent comboList, [NotNull] Command handler, ControllerInfo pc, [NotNull] Animator anim)
        {
            // ReSharper disable once Unity.BurstLoadingManagedType
            if (anim.IsInTransition(0) || comboList.Combo.ShowMovesPanel) return;
            foreach (ComboSingle comboTest in comboList.Combo.ComboLists)
            {
                foreach (var comboOption in comboTest.ComboList.Where(comboOption => handler.StateInfo.IsName(comboOption.CurrentStateName.ToString())))
                {
                    handler.currentStateExitTime = comboOption.AnimationEndTime;
                    if (!comboOption.InputAllowed(handler.StateInfo.normalizedTime)) continue;
                    var trigger = comboOption.Trigger;
                    if (comboTest.Unlocked && handler.QueueIsEmpty && !pc.Blockb && !pc.DodgeB)
                    {
                        switch (trigger.attackType)
                        {
                            case AttackType.LightAttack:
                                if (pc.LightAttackb)
                                {
                                    handler.InputQueue.Enqueue(trigger);
                                    pc.ChargedTime = 0.0f;
                                }
                                break;
                            case AttackType.HeavyAttack:
                                if (pc.HeavyAttackb)
                                {
                                    handler.InputQueue.Enqueue(trigger);
                                    pc.ChargedTime = 0.0f;
                                }
                                break;
                            //TODO Review
                            case AttackType.ChargedLightAttack:
                                if (pc.ChargedLightAttackb)
                                {
                                    handler.InputQueue.Enqueue(trigger);
                                    pc.ChargedTime = 0.0f;
                                }
                                break;
                            case AttackType.ChargedHeavyAttack:
                                if (pc.ChargedHeavyAttackb)
                                {
                                    handler.InputQueue.Enqueue(trigger);
                                    pc.ChargedTime = 0.0f;
                                }
                                break;
                            case AttackType.Projectile:
                                if (pc.Projectileb)
                                {
                                    handler.InputQueue.Enqueue(trigger);
                                    pc.ChargedTime = 0.0f;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else if (pc.DodgeB) {
                        if (Input.GetAxis("Vertical") > .1)
                        {
                            handler.InputQueue.Enqueue(new AnimationTrigger()
                            {
                                triggerAnimIndex = 0,
                                attackType = AttackType.Dodge,
                                TransitionDuration = .25f,
                            });
                        }
                        else
                        {
                            handler.InputQueue.Enqueue(new AnimationTrigger()
                            {
                                triggerAnimIndex = 1,
                                attackType = AttackType.Dodge,
                                TransitionDuration = 0.25f,

                            });
                        }

                    }
                    else if (pc.Blockb)
                    {
                                  
                        if (pc.DodgeB)
                        {
                            Debug.Log("Dodge");
                            if (Input.GetAxis("Horizontal") > .1)
                            {
                                handler.InputQueue.Enqueue(new AnimationTrigger()
                                {
                                    triggerAnimIndex = 0,
                                    attackType = AttackType.Dodge,
                                    TransitionDuration = .25f,
                                });
                            }
                            else
                            {
                                handler.InputQueue.Enqueue(new AnimationTrigger()
                                {
                                    triggerAnimIndex = 1,
                                    attackType = AttackType.Dodge,
                                    TransitionDuration = 0.25f,

                                });
                            }
                        }
                        handler.InputQueue.Enqueue(new AnimationTrigger()
                        {
                            attackType = AttackType.Defend,

                        });
                                   
                    }
                }
            }
        }

    }
}