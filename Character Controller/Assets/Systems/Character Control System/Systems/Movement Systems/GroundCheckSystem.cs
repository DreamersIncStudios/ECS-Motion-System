using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;
using MotionSystem.Components;

namespace MotionSystem.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public sealed partial class GroundCheckSystem : SystemBase
    {
        EntityQuery groundCheckers;
        EntityCommandBufferSystem entityCommandBufferSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            groundCheckers = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(LocalToWorld)), ComponentType.ReadWrite(typeof(CharControllerE)) }

            });

            entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            this.RegisterPhysicsRuntimeSystemReadWrite();

        }
    
        protected override void OnUpdate()
        {
            CollisionWorld collisionWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
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
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(0, .2f, 0),
                        End = tranforms[i].Position - new Unity.Mathematics.float3(0, -control.GroundCheckDistance, 0),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ((1 << 10)),
                            CollidesWith = ((1 << 6)),
                            GroupIndex = -1
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(0, .2f, .25f),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(0, -control.GroundCheckDistance, .25f),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ((1 << 10)),
                            CollidesWith = ((1 << 6)),
                            GroupIndex = -1
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(0, .1f, -.25f),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(0, -control.GroundCheckDistance, -.25f),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ((1 << 10)),
                            CollidesWith = ((1 << 6)),
                            GroupIndex = 0
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(.25f, .1f, 0),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(.25f, -control.GroundCheckDistance, 0),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ((1 << 10)),
                            CollidesWith = ((1 << 6)),
                            GroupIndex = 0,
                        }
                    });
                    groundRays.Add(new RaycastInput()
                    {
                        Start = tranforms[i].Position + new Unity.Mathematics.float3(-.25f, .1f, 0),
                        End = tranforms[i].Position + new Unity.Mathematics.float3(-.25f, -control.GroundCheckDistance, 0),
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ((1 << 10)),
                            CollidesWith = ((1 << 6)),
                            GroupIndex = 0
                        }
                    });
                    control.ApplyRootMotion = false;

                    foreach (var ray in groundRays)
                    {

                        NativeList<Unity.Physics.RaycastHit> raycastArray = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);

                        if (control.IsGrounded = world.CastRay(ray, ref raycastArray))
                        {
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