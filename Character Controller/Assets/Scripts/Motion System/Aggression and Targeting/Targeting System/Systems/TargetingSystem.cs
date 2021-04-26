using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityStandardAssets.CrossPlatformInput;
using DreamersStudio.CameraControlSystem;
using Global.Component;
namespace AISenses.VisionSystems
{
    [UpdateAfter(typeof(VisionSystem))]
    public class TargetingSystem : SystemBase
    {
        EntityQuery Player;
        EntityQuery PlayersParty;

        protected override void OnCreate()
        {
            base.OnCreate();
            Player = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Vision)), ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(ScanPositionBuffer)),ComponentType.ReadOnly(typeof(Player_Control)) },

            });
           PlayersParty = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(Vision)), ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(ScanPositionBuffer)), ComponentType.ReadOnly(typeof(PlayerParty))},
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(Player_Control)) }

            });

        }
        int index = 0;

        float ChangeDelay;
        bool looking = false;
        bool IsTargeting => CrossPlatformInputManager.GetAxis("Target Trigger") > .3f;
        bool PausingBetweenChange => ChangeDelay > 0.0f;
        bool ChangeTargetNeg => CrossPlatformInputManager.GetAxis("Change Target") < -.65f;
        bool ChangeTargetPos => CrossPlatformInputManager.GetAxis("Change Target") > .65f;

        protected override void OnUpdate()
        {
            ComponentDataFromEntity<AITarget> Target = GetComponentDataFromEntity<AITarget>(); ;
            Entities.WithoutBurst().ForEach((ref Player_Control PC, ref DynamicBuffer<ScanPositionBuffer> buffer) =>
            {
                if (buffer.Length == 0)
                    return;
                if (PausingBetweenChange)
                {
                    ChangeDelay -= Time.DeltaTime;
                    return;
                }

               
                if (CameraControl.Instance.isTargeting = IsTargeting)
                {
                    GameObject temp = null;
                    if (!looking)
                    {
                        temp = (GameObject)FindObjectFromInstanceID(Target[buffer[index].target.entity].GetInstanceID);
                        if (temp != null)
                            CameraControl.Instance.TargetGroup.m_Targets[0].target = temp.transform;
                        looking = true;
                    }

                        if (ChangeTargetNeg)
                    {
                        index--;
                        if (index< 0)
                            index = buffer.Length - 1;
                        ChangeDelay = .35f;
                      
                       temp = (GameObject)FindObjectFromInstanceID(Target[buffer[index].target.entity].GetInstanceID);
                        if (temp != null)
                            CameraControl.Instance.TargetGroup.m_Targets[0].target = temp.transform;
                    }

                    if (ChangeTargetPos)
                    {
                        index++;
                        if (index > buffer.Length - 1)
                            index = 0;
                        ChangeDelay = .35f;
                       temp = (GameObject)FindObjectFromInstanceID(Target[buffer[index].target.entity].GetInstanceID);
                        if (temp != null)
                            CameraControl.Instance.TargetGroup.m_Targets[0].target = temp.transform;
                    }

                    



                }
                else {
                    if (looking)
                    {
                        index = 0;
                        looking = false;
                    }
                } 
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