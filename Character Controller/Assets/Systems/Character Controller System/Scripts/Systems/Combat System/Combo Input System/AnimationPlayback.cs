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
            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, AnimatorComponent animC, Rigidbody RB, Command handler, ref AttackTarget attackTarget) =>
            {

                if (animC.anim == null)
                    return;
                var anim = animC.anim;
                var transform = anim.transform;
                handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

                handler.InputQueue ??= new Queue<AnimationTrigger>();
                if (handler.TakeInput)
                {
                    AnimationTrigger temp = handler.InputQueue.Dequeue();
                    if (!anim.GetBool("Weapon In Hand") && !handler.AlwaysDrawnWeapon)
                    {
                        switch (temp.attackType)
                        {
                            case AttackType.LightAttack:
                                anim.CrossFade("Equip_Light", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.HeavyAttack:
                                anim.CrossFade("Equip_Heavy", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;
                            case AttackType.SpecialAttack:
                                anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                                EntityManager.AddComponent<DrawWeapon>(entity);
                                break;

                        }

                    }
                    else
                    {
                        if (temp.attackType != AttackType.Defend)
                        {
                            anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
                        }
                        else
                        {
                            if (!handler.StateInfo.IsTag("Defend"))
                            {
                                anim.CrossFade("Enter Defence", .15f);
                            }
                        }

                    }

                    if (!attackTarget.TargetInRange)
                    {
                        Vector3 dir = ((Vector3)attackTarget.AttackTargetLocation - anim.transform.position).normalized;
                        RB.velocity = new Vector3(dir.x * RB.velocity.x, dir.y * RB.velocity.y, dir.z * RB.velocity.z);
                    }
                    // this need to move to animation event
                }
                if (!anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne") && !handler.StateInfo.IsTag("Defend"))
                {
                    if (anim.GetBool("Weapon In Hand") && !handler.AlwaysDrawnWeapon)
                    {
                        if (!handler.BareHands)
                            anim.CrossFade("Locomation_Grounded_Weapon0", .25f, 0, .25f);
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
