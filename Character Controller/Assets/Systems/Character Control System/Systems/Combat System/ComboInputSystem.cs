using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.SO;
using Dreamers.InventorySystem.Base;
using Unity.Transforms;
using System.Collections;
using AISenses.VisionSystems.Combat;
using DG.Tweening;

namespace DreamersInc.ComboSystem
{
    public class ComboInputSystem : ComponentSystem
    {
        EntityCommandBuffer commandBuffer;
        protected override void OnCreate()
        {
            base.OnCreate();
            movespanel = new GameObject();
        }


        GameObject movespanel;
        //TODO Decouple this code split into small chu
        protected override void OnUpdate()
        {

            Entities.ForEach((ref Player_Control PC, PlayerComboComponent ComboList, Animator anim, Command handler) =>
            {
                if (handler.InputQueue == null)
                    handler.InputQueue = new Queue<AnimationTrigger>();
                EquipmentBase equipmentBase = anim.GetComponent<CharacterInventory>().Equipment;
                WeaponSO So;
                if (equipmentBase.EquippedWeapons.TryGetValue(WeaponSlot.Primary, out So) || equipmentBase.EquippedWeapons.TryGetValue(WeaponSlot.Secondary, out So))
                {
                    handler.WeaponIsEquipped = true;
                }
                else
                    handler.WeaponIsEquipped = false;

                if (PC.InSafeZone)
                {
                // add logic for play to store weapon

                return;
                }

                if (!anim.IsInTransition(0) && !ComboList.Combo.ShowMovesPanel)
                {
                    foreach (AnimationCombo comboOption in ComboList.Combo.ComboList)
                    {
                        if (handler.StateInfo.IsName(comboOption.CurrentStateName.ToString()))
                        {
                            handler.currentStateExitTime = comboOption.AnimationEndTime;
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                foreach (AnimationTrigger trigger in comboOption.Triggers)
                                {
                                    if (trigger.Unlocked && handler.QueueIsEmpty)
                                    {
                                        switch (trigger.Type)
                                        {
                                            case AttackType.LightAttack:
                                                if (PC.LightAttack)
                                                {
                                                    handler.InputQueue.Enqueue(trigger);
                                                    PC.ChargedTime = 0.0f;
                                                }
                                                break;
                                            case AttackType.HeavyAttack:
                                                if (PC.HeavyAttack)
                                                {
                                                    handler.InputQueue.Enqueue(trigger);
                                                    PC.ChargedTime = 0.0f;
                                                }
                                                break;
                                                //TODO Review
                                            case AttackType.ChargedLightAttack:
                                                if (PC.ChargedLightAttack)
                                                {
                                                    handler.InputQueue.Enqueue(trigger);
                                                    PC.ChargedTime = 0.0f;
                                                }
                                                break;
                                            case AttackType.ChargedHeavyAttack:
                                                if (PC.ChargedHeavyAttack)
                                                {
                                                    handler.InputQueue.Enqueue(trigger);
                                                    PC.ChargedTime = 0.0f;
                                                }
                                                break;
                                            case AttackType.Projectile:
                                                if (PC.Projectile)
                                                {
                                                    handler.InputQueue.Enqueue(trigger);
                                                    PC.ChargedTime = 0.0f;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }       
                        }
                    }
                }
            });


            Entities.WithNone<ShooterComponent>().ForEach((ref AttackTarget attackTarget, Animator anim, Command handler, Transform transform) =>
            {
                handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

                if (handler.InputQueue == null)
                {
                    handler.InputQueue = new Queue<AnimationTrigger>();
                }
                if (handler.TakeInput)
                {
                    AnimationTrigger temp = handler.InputQueue.Dequeue();
                    if (!anim.GetBool("Weapon In Hand")) {
                        switch (temp.Type) {
                            case AttackType.LightAttack:
                                anim.CrossFade("Equip_Light", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);

                                break;
                            case AttackType.HeavyAttack:
                        anim.CrossFade("Equip_Heavy", temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);

                                break;
                        }
                    }
                    else {

                        anim.CrossFade(temp.TriggerString, temp.TransitionDuration, 0, temp.TransitionOffset, temp.EndofCurrentAnim);
               
                    }
                    if (!attackTarget.AttackTargetLocation.Equals(new float3(1, 1, 1)))
                    {
                        transform.DOMove(attackTarget.MoveTo(transform.position), .5f, false);
                    }
                    // this need to move to animation event
                }
                if (!anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne"))
                {
                    if (anim.GetBool("Weapon In Hand"))
                    {
                        if (!handler.BareHands)
                            anim.CrossFade("Locomation_Grounded_Weapon0", .25f, 0, .25f);
                        else
                            anim.CrossFade("Grounded0", .25f, 0, .25f);
                    }
                    else
                        anim.CrossFade("Grounded0", .25f, 0, .25f);

                }


            });


        }

    }
}