using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using MotionSystem.Components;
using RaycastHit = Unity.Physics.RaycastHit;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
namespace MotionSystem.System
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

    [UpdateAfter(typeof(AnimatorUpdate))]

    public class GroundCheckSytem : SystemBase
    {
        EntityQuery groundCheckers;
        EntityCommandBufferSystem entityCommandBufferSystem;
        EndFramePhysicsSystem m_EndFramePhysicsSystem;
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();

            groundCheckers = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(CharControllerE)) }

            });

            entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_EndFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();

        }
        protected override void OnUpdate()
        {
            Dependency = JobHandle.CombineDependencies(Dependency, m_BuildPhysicsWorldSystem.GetOutputDependency());

            CollisionWorld collisionWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
            Dependency = JobHandle.CombineDependencies(Dependency, m_EndFramePhysicsSystem.GetOutputDependency());
            JobHandle systemDeps = Dependency;

            systemDeps = new GroundCheckJob()
            {
                ControllChunk = GetComponentTypeHandle<CharControllerE>(false),
                PositionChunk = GetComponentTypeHandle<LocalToWorld>(true),
                world = collisionWorld,
                physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld


            }.ScheduleParallel(groundCheckers, systemDeps);

            systemDeps.Complete();

            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().AddInputDependency(systemDeps);
            Dependency = systemDeps;

        }
        [BurstCompile]
        public struct GroundCheckJob : IJobChunk
        {
            [ReadOnly] public ComponentTypeHandle<LocalToWorld> PositionChunk;
            public ComponentTypeHandle<CharControllerE> ControllChunk;
            [ReadOnly] public PhysicsWorld physicsWorld;
            [ReadOnly] public CollisionWorld world;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<LocalToWorld> tranforms = chunk.GetNativeArray(PositionChunk);
                NativeArray<CharControllerE> Controls = chunk.GetNativeArray(ControllChunk);
                for (int i = 0; i < chunk.Count; i++)
                {
                    CharControllerE control = Controls[i];
                    if (control.SkipGroundCheck)
                        continue;
                    NativeList<RaycastInput> groundRays = new NativeList<RaycastInput>(Allocator.Temp);
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(0, .5f, 0),
                        End = tranforms[i].Position - new Unity.Mathematics.float3(0, control.GroundCheckDistance, 0),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = ((1 << 10)),
                            GroupIndex = 0
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(0, .1f, .25f),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(0, -control.GroundCheckDistance, .25f),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = ((1 << 10)),
                            GroupIndex = 0
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(0, .1f, -.25f),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(0, -control.GroundCheckDistance, -.25f),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = ((1 << 10)),
                            GroupIndex = 0
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(.25f, .1f, 0),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(.25f, -control.GroundCheckDistance, 0),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = ((1 << 10)),
                            GroupIndex = 0
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(-.25f, .1f, 0),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(-.25f, -control.GroundCheckDistance, 0),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = ((1 << 10)),
                            GroupIndex = 0
                        }
                    });
                    control.ApplyRootMotion = false;
                    foreach (var ray in groundRays)
                    {
                        if (control.IsGrounded = world.CastRay(ray, out RaycastHit raycastHit))
                        {
                            //Debug.Log(raycastHit.Entity);
                            control.ApplyRootMotion = true;
                            Controls[i] = control;
                            groundRays.Dispose();
                            break;
                        }
                    }


                    Controls[i] = control;

                }

            }
        }
    }
}

