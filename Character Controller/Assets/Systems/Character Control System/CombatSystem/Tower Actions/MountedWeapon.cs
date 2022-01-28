using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;

namespace DreamersInc.ComboSystem.Mounted
{
    [GenerateAuthoringComponent]
    public struct MountedWeapon : IComponentData
    { 
        //TODO Add fine Tuning with YZ rot offsets 
        //TODO Add inverse Z rot for Mortar rounds

        public float3 DirectionToTarget;

        public int2 RotationLimitAngle;
        public float3 centerline { get; set; }
       [SerializeField] public float2 BoundsX => new float2() { x = centerline.y - RotationLimitAngle.x, y = centerline.y + RotationLimitAngle.x };
        [SerializeField] public float2 BoundsY => new float2() { x = centerline.x - RotationLimitAngle.y, y = centerline.x + RotationLimitAngle.y };

        public bool PointAtTarget;

    }

    public class MountedWeaponSystem : SystemBase
    {
        EntityQuery mountedWeapon;
        EntityCommandBufferSystem entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            mountedWeapon = GetEntityQuery(new EntityQueryDesc() {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(TrackTarget)),
                    ComponentType.ReadWrite(typeof(MountedWeapon))
                }
            });

            Entities.WithoutBurst().ForEach((Entity entity,ref MountedWeapon Mounted, ref LocalToWorld transform) => {
                Mounted.centerline = transform.Position;
                EntityManager.AddComponent<TrackTarget>(entity);
            }).Run();

        }
        protected override void OnUpdate()
        {
            JobHandle systemDeps = Dependency;
            systemDeps = new TargetEntity()
            {
                TargetChunk = GetComponentTypeHandle<TrackTarget>(false),
                TransformsChunk = GetComponentTypeHandle<LocalToWorld>(true)
            }.ScheduleParallel(mountedWeapon,systemDeps);
            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);
            Dependency = systemDeps;

        }
        [BurstCompile]
        struct TargetEntity : IJobChunk
        {
           [ReadOnly] public ComponentTypeHandle<LocalToWorld> TransformsChunk;
            public ComponentTypeHandle<TrackTarget> TargetChunk;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<TrackTarget> trackTargets = chunk.GetNativeArray(TargetChunk);
                NativeArray<LocalToWorld> transforms = chunk.GetNativeArray(TransformsChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    TrackTarget track = trackTargets[i];
                    LocalToWorld localToWorld = transforms[i];
                    float3 home = localToWorld.Position;
                    home.y = 0; Vector3 dir = (localToWorld.Position - track.positionToTarget);
                    dir = dir.normalized;
                    float angleY = new float();
                    float angleZ = new float();
                    if (dir.x >= 0)
                    {
                        if( dir.z > 0)
                            angleY = Vector3.Angle(track.positionToTarget - home, Vector3.back) - 90;
                        else
                            angleY = -Vector3.Angle(track.positionToTarget - home, Vector3.forward) + 90;

                    }
                
                    else {
                        if (dir.z > 0)
                            angleY = -Vector3.Angle(track.positionToTarget - home, Vector3.back)-90;
                        else
                            angleY = Vector3.Angle(track.positionToTarget - home, Vector3.forward)+90 ;
                    }
                    home = localToWorld.Position; ;
                    
                    angleZ = Mathf.Clamp((Vector3.Angle( track.positionToTarget - localToWorld.Position, localToWorld.Up)-90)/2.0f, -20, 20);
                    Quaternion test = Quaternion.Euler(0, angleY,angleZ);

                    track.DirectionToTarget = test;
                                track.HasRotation = true;
                            track.Speed = 30;

                        trackTargets[i] = track;
                }
            }
        }
    }

    [UpdateAfter(typeof(MountedWeaponSystem))]
    public class RotationMounted : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref TrackTarget track,  Transform transform) => {

                Debug.DrawLine(transform.position, track.positionToTarget,Color.red);

                if (track.HasRotation) {
                   transform.rotation = Quaternion.RotateTowards(transform.rotation,track.DirectionToTarget, track.Speed*Time.DeltaTime);
                }

            });
        }
    }
}