using System;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using AISenses.VisionSystems.Combat;
using DreamersInc.CombatSystem;
using DreamersInc.InputSystems;
using UnityEngine.InputSystem;

// ReSharper disable Unity.BurstLoadingManagedType

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private void AnimationTriggering()
        {
            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, Animator anim, Rigidbody rb, Command handler, ref AttackTarget attackTarget) =>
            {
                var transform = anim.transform;
                handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

                handler.InputQueue ??= new Queue<AnimationTrigger>();
                if (handler.TakeInput)
                {
                    AnimationTrigger temp = handler.InputQueue.Dequeue();
                    if (!anim.GetBool(WeaponHand) && !handler.AlwaysDrawnWeapon)
                    {
                        switch (temp.AttackType)
                        {
                            case AttackType.LightAttack:
                                anim.CrossFade("Equip_Light", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndOfCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.HeavyAttack:
                                anim.CrossFade("Equip_Heavy", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndOfCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.SpecialAttack:
                                anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndOfCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.Dodge:
                                anim.CrossFade(temp.triggerAnimIndex == 0 ? "Dodge0" : "Dodge1",
                                    temp.TransitionDuration, 0, 0, 0);

                                break;
                            case AttackType.none:
                                break;
                            case AttackType.ChargedLightAttack:
                                break;
                            case AttackType.ChargedHeavyAttack:
                                break;
                            case AttackType.Projectile:
                                break;
                            case AttackType.ChargedProjectile:
                                break;
                            case AttackType.Grounded:
                                break;
                            case AttackType.Targeted_Locomation:
                                break;
                            case AttackType.Locomation_Grounded_Weapon:
                                break;
                            case AttackType.Defend:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    }
                    else
                    {
                        if (temp.AttackType != AttackType.Defend && temp.AttackType != AttackType.Dodge)
                        {
                            anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndOfCurrentAnim);

                        }
                        else if (temp.AttackType == AttackType.Dodge)
                        {
                            anim.CrossFade(temp.triggerAnimIndex == 0 ? "Dodge0" : "Dodge1", temp.TransitionDuration, 0,
                                0, 0);
                        }
                        else
                        {
                            if (!anim.IsInTransition(0)&&!handler.StateInfo.IsTag("Defend") && !handler.StateInfo.IsTag("Dodge") && !handler.StateInfo.IsTag("Exit"))
                            {
                                anim.CrossFade("Enter Defence", .15f);
                                anim.SetBool(Block,true);
                            } else if (handler.StateInfo.IsTag("Dodge") && handler.StateInfo.normalizedTime> .85f)
                            { 
                                anim.CrossFade("Enter Defence", .15f);
                                anim.SetBool(Block,true);

                            }
                        }

                    }

                    if (!attackTarget.TargetInRange)
                    {
                        Vector3 dir = ((Vector3)attackTarget.AttackTargetLocation - anim.transform.position).normalized;
                        var linearVelocity = rb.linearVelocity;
                        linearVelocity = new Vector3(dir.x * linearVelocity.x, dir.y * linearVelocity.y, dir.z * linearVelocity.z);
                        rb.linearVelocity = linearVelocity;
                    }
                    // this need to move to animation event
                }
                if (!anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne") && !handler.StateInfo.IsTag("Defend"))
                {
                    if (anim.GetBool(WeaponHand) && !handler.AlwaysDrawnWeapon)
                    {
                        if (!handler.BareHands)
                            anim.CrossFade("Locomotion_Grounded_Weapon0", .25f, 0, .25f);
                        else
                            anim.CrossFade("Grounded0", .25f, 0, .25f);
                    }
                    else
                        anim.CrossFade("Grounded0", .25f, 0, .25f);

                }
                if (handler.StateInfo.IsName("Unequip"))
                {
                    EntityManager.AddComponent<StoreWeapon>(entity);
                }
            }).Run();
        }
    }
}
