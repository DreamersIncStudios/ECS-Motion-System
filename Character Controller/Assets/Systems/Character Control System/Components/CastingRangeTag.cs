using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Stats;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityStandardAssets.CrossPlatformInput;
using Utilities.ReactiveSystem;
using MotionSystem.CAD;
using MotionSystem.Components;

[assembly: RegisterGenericComponentType(typeof(AIReactiveSystemBase<InCastingRangeTag, CharControllerE, MotionSystem.CAD.InRangeReactor>.StateComponent))]
[assembly: RegisterGenericJobType(typeof(AIReactiveSystemBase<InCastingRangeTag, CharControllerE, MotionSystem.CAD.InRangeReactor>.ManageComponentAdditionJob))]
[assembly: RegisterGenericJobType(typeof(AIReactiveSystemBase<InCastingRangeTag, CharControllerE, MotionSystem.CAD.InRangeReactor>.ManageComponentRemovalJob))]


namespace MotionSystem.CAD
{
    public struct InCastingRangeTag : IComponentData
    {
        bool test;
    }
    public struct CasterTag : IComponentData { }

    public partial class CastingTimeSystem : SystemBase
    {
        EntityQuery Caster;
        EntityQuery MovingObjects;
        EntityQuery InCastingRange;
        EntityCommandBufferSystem entityCommandBufferSystem;
        public static CastingTimeSystem instance;

        protected override void OnCreate()
        {
            instance = this;
            base.OnCreate();
            Caster = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(CasterTag)), ComponentType.ReadOnly(typeof(LocalToWorld)) }
            });
            MovingObjects = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)) },
                Any = new ComponentType[] { ComponentType.ReadOnly(typeof(NPCStats)), ComponentType.ReadOnly(typeof(EnemyStats)), ComponentType.ReadOnly(typeof(PlayerStatComponent)) },
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(InCastingRangeTag)) },
            });
            InCastingRange = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(InCastingRangeTag)), ComponentType.ReadOnly(typeof(LocalToWorld)) }
            });
            entityCommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
            Caster.SetChangedVersionFilter(
                new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)) }
                );
            MovingObjects.SetChangedVersionFilter(
            new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)) }
            );
            InCastingRange.SetChangedVersionFilter(
            new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)) }
            );
        }
        bool casting => CrossPlatformInputManager.GetAxis("Target Trigger") > .3f;
       public  float resetTimer;
        bool reset => resetTimer > 0.0f;
        public bool Release;
        protected override void OnUpdate()
        {
            if (reset)
                resetTimer -= Time.DeltaTime;
            if (!casting && Release)
                Release = false;
            JobHandle systemDeps = Dependency;
            if (casting && !reset && !Release) 
            {
                float3 pos = Caster.ToComponentDataArray<LocalToWorld>(Allocator.TempJob)[0].Position;
                systemDeps = new AddInRangeTagJob() {
                    CasterPostion =pos,
                    ECB = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                    EntitiesChunk = GetEntityTypeHandle(),
                    LocalToWorldChunk = GetComponentTypeHandle<LocalToWorld>(true)
                }.ScheduleParallel(MovingObjects, systemDeps);
                entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);

                systemDeps = new ExitCastingRange() {
                    CasterPostion =pos,
                    ECB = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                    EntitiesChunk = GetEntityTypeHandle(),
                    LocalToWorldChunk = GetComponentTypeHandle<LocalToWorld>(true)
                }.ScheduleParallel(InCastingRange, systemDeps);
                entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);

            }
            else
            {
                systemDeps = new RemoveInRangeTagJob() {
                    ECB = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                    EntitiesChunk = GetEntityTypeHandle()
                }.ScheduleParallel(InCastingRange, systemDeps);

            }
            Dependency = systemDeps;
        }
        [BurstCompile]
        struct AddInRangeTagJob : IJobChunk
        {
            public float3 CasterPostion;
            [ReadOnly] public ComponentTypeHandle<LocalToWorld> LocalToWorldChunk;
            [ReadOnly] public EntityTypeHandle EntitiesChunk;
            public EntityCommandBuffer.ParallelWriter ECB;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<LocalToWorld> toWorlds = chunk.GetNativeArray(LocalToWorldChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntitiesChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    float dist = Vector3.Distance(CasterPostion, toWorlds[i].Position);
                    if (dist < 50) // move to Caster Class
                    { 
                        ECB.AddComponent<InCastingRangeTag>(chunkIndex, entities[i]);
                    }
                }

            }
        }
        [BurstCompile]

        struct ExitCastingRange : IJobChunk
        {
            public float3 CasterPostion;
            [ReadOnly] public ComponentTypeHandle<LocalToWorld> LocalToWorldChunk;
            [ReadOnly] public EntityTypeHandle EntitiesChunk;
            public EntityCommandBuffer.ParallelWriter ECB;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<LocalToWorld> toWorlds = chunk.GetNativeArray(LocalToWorldChunk);
                NativeArray<Entity> entities = chunk.GetNativeArray(EntitiesChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    float dist = Vector3.Distance(CasterPostion, toWorlds[i].Position);
                    if (dist > 50) // move to Caster Class
                    {
                        ECB.RemoveComponent<InCastingRangeTag>(chunkIndex, entities[i]);
                    }
                }

            }
        }
        [BurstCompile]

        struct RemoveInRangeTagJob : IJobChunk
        {
            [ReadOnly] public EntityTypeHandle EntitiesChunk;
            public EntityCommandBuffer.ParallelWriter ECB;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entities = chunk.GetNativeArray(EntitiesChunk);
                for (int i = 0; i < chunk.Count; i++)
                {

                    ECB.RemoveComponent<InCastingRangeTag>(chunkIndex, entities[i]);
                }

            }
        }

    }
    public struct InRangeReactor : IComponentReactorTagsForAIStates<InCastingRangeTag, CharControllerE>
    {
        public void ComponentAdded(Entity entity, ref InCastingRangeTag newComponent, ref CharControllerE Controller)
        {
            Controller.AnimationSpeed = .25f;
        }

        public void ComponentRemoved(Entity entity, ref CharControllerE Controller, in InCastingRangeTag oldComponent)
        {
            Controller.AnimationSpeed = 1.0f;

        }

        public void ComponentValueChanged(Entity entity, ref InCastingRangeTag newComponent, ref CharControllerE AIStateCompoment, in InCastingRangeTag oldComponent)
        {
        }

        public class CastingReactiveSystem : AIReactiveSystemBase<InCastingRangeTag, CharControllerE, InRangeReactor>
        {
            protected override InRangeReactor CreateComponentReactor()
            {
                return new InRangeReactor();
            }
        }
    }

}
