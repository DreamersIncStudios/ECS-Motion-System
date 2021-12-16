using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Base;
using Unity.Transforms;
using System.Collections;

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
        protected override void OnUpdate()
        {
            commandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

            Entities.ForEach((ref Player_Control PC, ComboComponentAuthoring ComboList, Animator anim, Command handler) =>
            {
                if (handler.InputQueue == null)
                    handler.InputQueue = new Queue<AnimationTriggers>();
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
                                        PC.ChargedTime = 0.0f;
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
                                        PC.ChargedTime = 0.0f;

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
                                        PC.ChargedTime = 0.0f;

                                    }
                                }
                            }

                        //   projectile
                        if (comboOption.Projectile.Unlocked)
                            {
                                if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                                {
                                    if (PC.Projectile && handler.QueueIsEmpty && !PC.Charged)
                                    {
                                        handler.InputQueue.Enqueue(comboOption.Projectile);
                                        PC.ChargedTime = 0.0f;
                                    }
                                    if (PC.Projectile && handler.QueueIsEmpty && PC.Charged)
                                    {
                                        handler.InputQueue.Enqueue(comboOption.ChargedProjectile);
                                        PC.ChargedTime = 0.0f;
                                    }
                                }
                            }
                        }
                    }
                }
            });


            Entities.ForEach((ShooterComponent shoot, Animator anim, Command handler) =>
            {
                handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

                if (handler.TakeInput)
                {
                    AnimationTriggers temp = handler.InputQueue.Dequeue();

                    anim.CrossFade(temp.TriggeredAnimName.ToString(), temp.TransitionDuration, 0, temp.StartOffset);
                // this need to move to animation event


                if (temp.TriggeredAnimName == ComboAnimNames.Projectile)
                    {
                        LocalToWorld localToWorld = GetComponentDataFromEntity<LocalToWorld>()[shoot.ShootFromHere];
                        if (!shoot.IsShooting)
                            shoot.RoundsLeftToSpawn += shoot.RoundsPerShot;

                    }
                    if (temp.TriggeredAnimName == ComboAnimNames.ChargedProjectile)
                    {
                        if (!shoot.IsShooting)
                        {
                            shoot.RoundsLeftToSpawn += shoot.RoundsPerShot;
                            shoot.HasShotBeenCharge = true;
                        }
                    }


                }
                if (!anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne"))
                {

                    anim.CrossFade("Locomation_Grounded_Weapon", .25f, 0, .25f);
                }

            });



            Entities.WithNone<ShooterComponent>().ForEach((Animator anim, Command handler) =>
            {
                handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

                if (handler.TakeInput)
                {
                    AnimationTriggers temp = handler.InputQueue.Dequeue();

                    anim.CrossFade(temp.TriggeredAnimName.ToString(), temp.TransitionDuration, 0, temp.StartOffset);
                // this need to move to animation event


            }
                if (!anim.IsInTransition(0) && handler.TransitionToLocomotion && !handler.StateInfo.IsTag("Airborne"))
                {

                    anim.CrossFade("Locomation_Grounded_Weapon", .25f, 0, .25f);
                }

            });


        }

    }
}