using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Dreamers.InventorySystem;

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

            systemDeps = new FireRoundJob()
            {
                EntityChunk = GetEntityTypeHandle(),
                TargetChunk = GetComponentTypeHandle<TrackTarget>(true),
                ShooterInfo = GetComponentDataFromEntity<ShooterComponent>(false),
                ChildChunk = GetBufferTypeHandle<Child>(true),
                ECB = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter()
            }
            .ScheduleParallel(mountedWeapon, systemDeps);
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
                    float angleX = new float();
                    if (dir.x >= 0)
                    {
                        if( dir.z > 0)
                            angleY = Vector3.Angle(track.positionToTarget - home, Vector3.back)+180 ;
                        else
                            angleY = -Vector3.Angle(track.positionToTarget - home, Vector3.forward);

                    }
                
                    else {
                        if (dir.z > 0)
                            angleY = Vector3.Angle(track.positionToTarget - home, Vector3.forward);
                        else
                            angleY = Vector3.Angle(track.positionToTarget - home, Vector3.forward);
                    }
                    home = localToWorld.Position; ;
                    
                    angleX = Mathf.Clamp((Vector3.Angle( track.positionToTarget - localToWorld.Position, localToWorld.Up)-90), -20, 20);
                    Quaternion test = Quaternion.Euler(angleX, angleY,0);

                    track.DirectionToTarget = test;
                                track.HasRotation = true;
                            track.Speed = 30;

                        trackTargets[i] = track;
                }
            }
        }

        struct FireRoundJob : IJobChunk
        {
            [ReadOnly] public ComponentTypeHandle<TrackTarget> TargetChunk;
            [ReadOnly] public EntityTypeHandle EntityChunk;
            [ReadOnly] public BufferTypeHandle<Child> ChildChunk;
            public EntityCommandBuffer.ParallelWriter ECB;
            [NativeDisableParallelForRestriction]public ComponentDataFromEntity<ShooterComponent> ShooterInfo;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<TrackTarget> trackTargets = chunk.GetNativeArray(TargetChunk);
               BufferAccessor<Child> childs =  chunk.GetBufferAccessor(ChildChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityChunk);

                for (int i = 0; i < chunk.Count; i++)
                {
                    DynamicBuffer<Child> child = childs[i];
                    if (!trackTargets[i].InRange)
                        continue;
                    else {
                        ShooterComponent info = ShooterInfo[child[0].Value];
                        info.RoundsLeftToSpawn += info.RoundsPerShot;
                        ShooterInfo[child[0].Value] = info;
                        ECB.RemoveComponent<TrackTarget>(chunkIndex,entities[i]);
                    }
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

              //  Debug.DrawLine(transform.position, Vector3.zero);
                if (track.HasRotation) {
                   transform.rotation = Quaternion.RotateTowards(transform.rotation,track.DirectionToTarget, track.Speed*Time.DeltaTime);
                }
                track.InRange = transform.rotation == track.DirectionToTarget;
            });
        }
    }
}