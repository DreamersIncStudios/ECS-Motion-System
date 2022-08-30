using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace LODRenderSystem
{
    public partial class RenderRangeSystem : SystemBase
    {
        EntityQuery renderCheckRemove;
        EntityQuery AnimateCheckRemove;

        EntityQuery renderCheckAdd;
        EntityQuery AnimateCheckAdd;


        EntityCommandBufferSystem entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            renderCheckRemove = GetEntityQuery( new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadOnly(typeof(LODTag)), ComponentType.ReadOnly(typeof(RenderTag)) }
            });
            renderCheckAdd = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadOnly(typeof(LODTag)) },
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(RenderTag)) }
               
            });
            AnimateCheckRemove = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadOnly(typeof(LODTag)), ComponentType.ReadOnly(typeof(AnimateTag)) }
            });
            AnimateCheckAdd = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadOnly(typeof(LODTag)) },
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(AnimateTag)) }

            });
            entityCommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        }
        protected override void OnUpdate()
        {
            float3 pos = Camera.main.transform.position;

            JobHandle systemDeps = Dependency;
            systemDeps = new RenderRemoveJob() {
                CameraPost = pos,
                EntityChunk = GetEntityTypeHandle(),
                LocalChunk = GetComponentTypeHandle<LocalToWorld>(true),
                ParallelWriter = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),


            }.ScheduleParallel(renderCheckRemove, systemDeps);

            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);

            systemDeps = new RenderAddJob()
            {
                CameraPost = pos,
                EntityChunk = GetEntityTypeHandle(),
                LocalChunk = GetComponentTypeHandle<LocalToWorld>(true),
                ParallelWriter = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),


            }.ScheduleParallel(renderCheckAdd, systemDeps);

            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);

            systemDeps = new AnimateAddJob()
            {
                CameraPost = pos,
                EntityChunk = GetEntityTypeHandle(),
                LocalChunk = GetComponentTypeHandle<LocalToWorld>(true),
                ParallelWriter = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),


            }.ScheduleParallel(AnimateCheckAdd, systemDeps);

            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);
            systemDeps = new AnimateRemoveJob()
            {
                CameraPost = pos,
                EntityChunk = GetEntityTypeHandle(),
                LocalChunk = GetComponentTypeHandle<LocalToWorld>(true),
                ParallelWriter = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),


            }.ScheduleParallel(AnimateCheckRemove, systemDeps);

            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);

            Dependency = systemDeps;

        }


        public struct RenderRemoveJob : IJobChunk
        {
            public ComponentTypeHandle<LocalToWorld> LocalChunk;
            [ReadOnly] public EntityTypeHandle EntityChunk;
            public float3 CameraPost;
            public EntityCommandBuffer.ParallelWriter ParallelWriter;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                float renderRange = 80;
                NativeArray<LocalToWorld> localToWorlds = chunk.GetNativeArray(LocalChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    float dist = Vector3.Distance(CameraPost, localToWorlds[i].Position);
                    if (dist > renderRange)
                        ParallelWriter.RemoveComponent<RenderTag>(chunkIndex, entities[i]);


                }
            }
        }

        public struct RenderAddJob : IJobChunk
        {
            public ComponentTypeHandle<LocalToWorld> LocalChunk;
            [ReadOnly] public EntityTypeHandle EntityChunk;
            public float3 CameraPost;
            public EntityCommandBuffer.ParallelWriter ParallelWriter;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                float renderRange = 80;
                NativeArray<LocalToWorld> localToWorlds = chunk.GetNativeArray(LocalChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    float dist = Vector3.Distance(CameraPost, localToWorlds[i].Position);
                    if (dist < renderRange)
                        ParallelWriter.AddComponent<RenderTag>(chunkIndex, entities[i]);


                }
            }
        }
        public struct AnimateRemoveJob : IJobChunk
        {
            public ComponentTypeHandle<LocalToWorld> LocalChunk;
            [ReadOnly] public EntityTypeHandle EntityChunk;
            public float3 CameraPost;
            public EntityCommandBuffer.ParallelWriter ParallelWriter;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                float AnimateRange = 60;
                NativeArray<LocalToWorld> localToWorlds = chunk.GetNativeArray(LocalChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    float dist = Vector3.Distance(CameraPost, localToWorlds[i].Position);
                    if (dist > AnimateRange)
                        ParallelWriter.RemoveComponent<AnimateTag>(chunkIndex, entities[i]);


                }
            }
        }
        public struct AnimateAddJob : IJobChunk
        {
            public ComponentTypeHandle<LocalToWorld> LocalChunk;
            [ReadOnly] public EntityTypeHandle EntityChunk;
            public float3 CameraPost;
            public EntityCommandBuffer.ParallelWriter ParallelWriter;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                float animinateRange = 60;
                NativeArray<LocalToWorld> localToWorlds = chunk.GetNativeArray(LocalChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    float dist = Vector3.Distance(CameraPost, localToWorlds[i].Position);
                    if (dist < animinateRange)
                        ParallelWriter.AddComponent<AnimateTag>(chunkIndex, entities[i]);


                }
            }
        }

    }
}
