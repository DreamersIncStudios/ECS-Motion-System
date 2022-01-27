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
                    if (!track.HasRotation)
                    {
                        track.DirectionToTarget = Quaternion.LookRotation((track.positionToTarget - localToWorld.Position));
                        track.HasRotation = true;
                        track.Speed = 30;
                    }

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


                if (track.HasRotation) {
                   transform.rotation = Quaternion.RotateTowards(transform.rotation, track.DirectionToTarget, track.Speed*Time.DeltaTime);
                
                }


            });
        }
    }
}