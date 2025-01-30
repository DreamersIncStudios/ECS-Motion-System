using System;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using AISenses.VisionSystems.Combat;
using DreamersInc.CombatSystem;
using DreamersInc.InputSystems;
using PrimeTween;
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
                        EntityManager.AddComponent<DrawPrimaryWeapon>(entity);
                        switch (temp.AttackType)
                        {
                            case AttackType.LightAttack:
                                anim.CrossFade("Equip_Light", temp.TransitionDuration, 0, temp.TransitionOffset,
                                    temp.EndOfCurrentAnim);
                                break;
                            case AttackType.HeavyAttack:
                                anim.CrossFade("Equip_Heavy", temp.TransitionDuration, 0, temp.TransitionOffset,
                                    temp.EndOfCurrentAnim);
                                break;
                            case AttackType.SpecialAttack:
                                anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset,
                                    temp.EndOfCurrentAnim);
                                break;
                            case AttackType.Dodge:
                                anim.CrossFade(temp.triggerAnimIndex == 0 ? "Dodge0" : "Dodge1",
                                    temp.TransitionDuration, 0, 0, 0);
                                break;
                            case AttackType.Defend:   
                                if (!anim.IsInTransition(0)&&!handler.StateInfo.IsTag("Defend") && !handler.StateInfo.IsTag("Dodge") && !handler.StateInfo.IsTag("Exit"))
                                {
                                    anim.CrossFade("Enter Defence", .15f);
                                    anim.SetBool(Block,true);
                                } 
                                else if (handler.StateInfo.IsTag("Dodge") && handler.StateInfo.normalizedTime> .85f)
                                { 
                                    anim.CrossFade("Enter Defence", .15f);
                                    anim.SetBool(Block,true);

                                }
                                break;
                            case AttackType.ChargedLightAttack:
                                break;
                            case AttackType.ChargedHeavyAttack:
                                break;
                            case AttackType.ChargedProjectile:
                            case AttackType.Projectile:
                                anim.CrossFade("Projectile0", temp.TransitionDuration, 0, temp.TransitionOffset,
                                    temp.EndOfCurrentAnim);
                                break;
                            case AttackType.Grounded:
                                break;
                            case AttackType.Targeted_Locomotion:
                                break;
                            case AttackType.Locomotion_Grounded_Weapon:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                    }
                    else
                    {
                        switch (temp.AttackType)
                        {
                            case AttackType.Projectile:
                                EntityManager.AddComponent<DrawSecondaryWeapon>(entity);
                                anim.CrossFade(
                                    !anim.GetBool("Projectile Drawn") ? "Projectile0" : "Projectile1",
                                    temp.TransitionDuration, 0, temp.TransitionOffset,
                                    temp.EndOfCurrentAnim);
                                break;
                            case not AttackType.Defend when temp.AttackType != AttackType.Dodge:
                                anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndOfCurrentAnim);
                                break;
                            case AttackType.Dodge:
                                anim.CrossFade(temp.triggerAnimIndex == 0 ? "Dodge0" : "Dodge1", temp.TransitionDuration, 0,
                                    0, 0);
                                break;
                            default:
                            {
                                if (!anim.IsInTransition(0)&&!handler.StateInfo.IsTag("Defend") && !handler.StateInfo.IsTag("Dodge") && !handler.StateInfo.IsTag("Exit"))
                                {
                                    anim.CrossFade("Enter Defence", .15f);
                                    anim.SetBool(Block,true);
                                } 
                                else if (handler.StateInfo.IsTag("Dodge") && handler.StateInfo.normalizedTime> .85f)
                                { 
                                    anim.CrossFade("Enter Defence", .15f);
                                    anim.SetBool(Block,true);

                                }

                                break;
                            }
                        }
                    }

         
                    // this need to move to animation event
                }
                if (!anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne") && !handler.StateInfo.IsTag("Defend") && !handler.StateInfo.IsTag("Equip"))
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

                if (!handler.StateInfo.IsName("Unequip")) return;
                EntityManager.AddComponent<StorePrimaryWeapon>(entity);
             
            }).Run();
            
            Entities.WithoutBurst().ForEach((Animator anim, Rigidbody rb, Command handler, in AttackTarget attackTarget) =>
                {
                    if (!attackTarget.TargetInRange || handler.StateInfo.IsName("Locomotion_Grounded_Weapon0") ||
                        handler.StateInfo.IsTag("Dodge") || handler.StateInfo.IsName("Grounded0")) return;
                    if (!attackTarget.TargetInRange) return;
                    
                    var dist = Vector3.Distance(anim.transform.position, attackTarget.AttackTargetLocation);
                    if (dist <= 1.5f)
                    {
                        rb.linearVelocity = Vector3.zero;
                    }
                    else
                    {
                        var speedFactor = Mathf.InverseLerp(1.5f, 0, dist);
                        var temp = anim.transform.position;
                        temp.y = attackTarget.AttackTargetLocation.y;
                        var dir = ((Vector3)attackTarget.AttackTargetLocation -  temp)
                            .normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(dir);
                        Tween.Rotation(anim.transform, targetRotation, .5f);
                    }

                }
                ).Run();
            
        }
    }
}
