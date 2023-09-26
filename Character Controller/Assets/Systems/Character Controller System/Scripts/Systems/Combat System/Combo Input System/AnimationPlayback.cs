using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Transforms;
using DG.Tweening;
using AISenses.VisionSystems.Combat;
using Stats.Entities;
using DreamersInc.CombatSystem;
using Dreamers.InventorySystem;
using MotionSystem.Components;

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private void AnimationTriggering()
        {
            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, Animator Anim, Rigidbody RB, Command handler, ref AttackTarget attackTarget) =>
            {
                var transform = Anim.transform;
                handler.StateInfo = Anim.GetCurrentAnimatorStateInfo(0);

                handler.InputQueue ??= new Queue<AnimationTrigger>();
                if (handler.TakeInput)
                {
                    AnimationTrigger temp = handler.InputQueue.Dequeue();
                    if (!Anim.GetBool("Weapon In Hand") && !handler.AlwaysDrawnWeapon)
                    {
                        switch (temp.attackType)
                        {
                            case AttackType.LightAttack:
                                Anim.CrossFade("Equip_Light", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.HeavyAttack:
                                Anim.CrossFade("Equip_Heavy", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.SpecialAttack:
                                Anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                        }

                    }
                    else
                    {
                        if (temp.attackType != AttackType.Defend && temp.attackType != AttackType.Dodge)
                        {
                            Anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);

                        }
                        else if (temp.attackType == AttackType.Dodge) {
                            if (temp.triggerAnimIndex == 0)
                            {
                                Anim.CrossFade("Dodge0", temp.TransitionDuration,0,0,0);
                            }
                            else {

                                Anim.CrossFade("Dodge1", temp.TransitionDuration,0,0,0);
                            }
                        }
                        else
                        {
                            if (!handler.StateInfo.IsTag("Defend") && !handler.StateInfo.IsTag("Dodge") && !handler.StateInfo.IsTag("Exit"))
                            {
                                Anim.CrossFade("Enter Defence", .15f);
                            } else if (handler.StateInfo.IsTag("Dodge") && handler.StateInfo.normalizedTime> .85f)
                            { 
                                Anim.CrossFade("Enter Defence", .15f);

                            }
                        }

                    }

                    if (!attackTarget.TargetInRange)
                    {
                        Vector3 dir = ((Vector3)attackTarget.AttackTargetLocation - Anim.transform.position).normalized;
                        RB.velocity = new Vector3(dir.x * RB.velocity.x, dir.y * RB.velocity.y, dir.z * RB.velocity.z);
                    }
                    // this need to move to animation event
                }
                if (!Anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne") && !handler.StateInfo.IsTag("Defend"))
                {
                    if (Anim.GetBool("Weapon In Hand") && !handler.AlwaysDrawnWeapon)
                    {
                        if (!handler.BareHands)
                            Anim.CrossFade("Locomation_Grounded_Weapon0", .25f, 0, .25f);
                        else
                            Anim.CrossFade("Grounded0", .25f, 0, .25f);
                    }
                    else
                        Anim.CrossFade("Grounded0", .25f, 0, .25f);

                }
                if (handler.StateInfo.IsName("Unequip"))
                {
                    EntityManager.AddComponent<StoreWeapon>(entity);
                }
            }).Run();
        }
    }
}
