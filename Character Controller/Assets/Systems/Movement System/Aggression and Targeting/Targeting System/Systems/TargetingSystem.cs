using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityStandardAssets.CrossPlatformInput;
using DreamersStudio.CameraControlSystem;
using Global.Component;
using Unity.Collections;
using AISenses.VisionSystems.Combat;
using System.ComponentModel;

namespace AISenses.VisionSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
  // [UpdateAfter(typeof(VisionSystemJobs))]
    public partial class TargetingSystem : SystemBase
    {
        EntityQuery Player;
        //        EntityQuery PlayersParty;

        protected override void OnCreate()
        {
            base.OnCreate();
            Player = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Vision)), ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(ScanPositionBuffer)), ComponentType.ReadOnly(typeof(Player_Control)) },
            });
            //           PlayersParty = GetEntityQuery(new EntityQueryDesc()
            //            {
            //                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Vision)), ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(ScanPositionBuffer)), ComponentType.ReadOnly(typeof(PlayerParty))},
            //                None = new ComponentType[] { ComponentType.ReadOnly(typeof(Player_Control)) }

            //            });

        }
        int index = 0;

        float ChangeDelay;
        bool looking = false;
        bool IsTargeting = false;
        bool PausingBetweenChange => ChangeDelay > 0.0f;
        bool ChangeTargetNeg => CrossPlatformInputManager.GetAxis("Change Target") < -.65f;
        bool ChangeTargetPos => CrossPlatformInputManager.GetAxis("Change Target") > .65f;

        protected override void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.JoystickButton9))
            {
                IsTargeting = !IsTargeting;
            }



            ComponentDataFromEntity<AITarget> Target = GetComponentDataFromEntity<AITarget>(); ;
            Entities.WithoutBurst().ForEach((ref Player_Control PC, ref DynamicBuffer<ScanPositionBuffer> buffer, ref AttackTarget attackTarget) =>
            {
                if (CameraControl.Instance.OnTargetingChanged != null)
                {
                    CameraControl.Instance.OnTargetingChanged(this, new CameraControl.OnTargetingChangedEventArgs { isTargeting = this.IsTargeting });
                }

                if (buffer.Length == 0)
                {
                    CameraControl.Instance.TargetGroup.m_Targets[0].target = null;

              
                    return;
                }
                var bufferArray = buffer.ToNativeArray(Allocator.Temp);
                bufferArray.Sort(new HitDistanceComparer());
                if (PausingBetweenChange)
                {
                    ChangeDelay -= Time.DeltaTime;
                    return;
                }

             
                if (IsTargeting)
                {
                    GameObject temp = null;
                    if (!looking)
                    {
                      
                        temp = EntityManager.GetComponentObject<Transform>(buffer[index].target.entity).gameObject;
                        if (CameraControl.Instance.OnTargetChanged != null)
                        {
                            CameraControl.Instance.OnTargetChanged(this, new CameraControl.OnTargetChangedEventArgs
                            {
                                Target = EntityManager.GetComponentObject<Transform>(buffer[index].target.entity).gameObject
                    });
                        }
                        looking = true;
                    }

                    if (ChangeTargetNeg)
                    {
                        index--;
                        if (index < 0)
                            index = buffer.Length - 1;
                        ChangeDelay = .15f;
                        if (CameraControl.Instance.OnTargetChanged != null)
                        {
                            CameraControl.Instance.OnTargetChanged(this, new CameraControl.OnTargetChangedEventArgs
                            {
                                Target = EntityManager.GetComponentObject<Transform>(buffer[index].target.entity).gameObject
                    });
                        }
                           
                    }

                    if (ChangeTargetPos)
                    {
                        index++;
                        if (index > buffer.Length - 1)
                            index = 0;
                        ChangeDelay = .35f;
                        if (CameraControl.Instance.OnTargetChanged != null)
                        {
                            CameraControl.Instance.OnTargetChanged(this, new CameraControl.OnTargetChangedEventArgs
                            {
                                Target = EntityManager.GetComponentObject<Transform>(buffer[index].target.entity).gameObject
                    });
                        }
                    }
                }
                else
                {
                    if (looking)
                    {
                        index = 0;
                        looking = false;
                    }
                }
                attackTarget.AttackTargetIndex = index;
                attackTarget.isTargeting = looking;


            }).Run();
        }

        public static Object FindObjectFromInstanceID(int iid)
        {
            return (Object)typeof(Object)
                    .GetMethod("FindObjectFromInstanceID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .Invoke(null, new object[] { iid });

        }
    }
}