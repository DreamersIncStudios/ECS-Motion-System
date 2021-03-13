using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using System.Collections.Generic;

using Unity.Transforms;

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
            if(handler.InputQueue == null)
                handler.InputQueue = new Queue<AnimationTriggers>();
            if (PC.InSafeZone)
                return;
                       
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
                     //   projectile
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


        Entities.ForEach(( ShooterComponent shoot, Animator anim, Command handler) =>
        {
            handler.StateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (handler.TakeInput)
            {
                AnimationTriggers temp = handler.InputQueue.Dequeue();

                anim.CrossFade(temp.TriggeredAnimName.ToString(), temp.TransitionDuration, 0, temp.StartOffset);
                // this need to move to animation event
                if (temp.TriggeredAnimName == ComboAnimNames.projectile)
                {
                    LocalToWorld localToWorld = GetComponentDataFromEntity<LocalToWorld>()[shoot.ShootPointEntity];
                   GameObject bullet = MonoBehaviour.Instantiate(shoot.ProjectileEntity,localToWorld.Position,localToWorld.Rotation);
                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * shoot.NormalSpeed;
                    Object.Destroy(bullet, 20);
                    
                }

            }
            if (!anim.IsInTransition(0) && handler.TransitionToLocomotion)
            {
               
                anim.CrossFade("Locomation_Grounded_Weapon", .25f, 0, .25f);
            }
        
        });



    }
}
