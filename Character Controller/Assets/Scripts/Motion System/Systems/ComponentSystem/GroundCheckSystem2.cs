using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using MotionSystem.Components;
using Unity.Burst;
using Unity.Jobs;

namespace MotionSystem.System
{
    public class GroundCheckSystem2 : SystemBase
    {
        EntityCommandBufferSystem entityCommandBufferSystem;
        EndFramePhysicsSystem m_EndFramePhysicsSystem;
        EntityQuery Characters;

        protected override void OnCreate()
        {
            base.OnCreate();
            Characters = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(CharControllerE)), ComponentType.ReadOnly(typeof(LocalToWorld)) }
            });
            entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_EndFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
        }

        protected override void OnUpdate()
        {
            CollisionWorld collisionWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld;
            Dependency = JobHandle.CombineDependencies(Dependency, m_EndFramePhysicsSystem.GetOutputDependency());

            JobHandle systemDeps = Dependency;
            systemDeps = new GroundCheckJob()
            {
                ControllerChunk = GetComponentTypeHandle<CharControllerE>(false),
                TransformChunk = GetComponentTypeHandle<LocalToWorld>(true),
                world = collisionWorld,

            }.ScheduleSingle(Characters, systemDeps);

            entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().AddInputDependency(systemDeps);
            systemDeps.Complete();
        }
    }
    // [BurstCompile]
    struct GroundCheckJob : IJobChunk
    {
        public ComponentTypeHandle<CharControllerE> ControllerChunk;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> TransformChunk;
        [ReadOnly] public CollisionWorld world;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<CharControllerE> Controllers = chunk.GetNativeArray(ControllerChunk);
            NativeArray<LocalToWorld> Transforms = chunk.GetNativeArray(TransformChunk);
            for (int i = 0; i < chunk.Count; i++)
            {
                LocalToWorld transform = Transforms[i];
                CharControllerE controller = Controllers[i];
                GroundCheckRays GroundRays = new GroundCheckRays();


               var center = new RaycastInput()
                {
                    Start = transform.Position + new float3(0, .5f, 0),
                    End = transform.Position - new float3(0, .5f, 0),
                    Filter = controller.mask
                };
                Debug.Log(world.CastRay(center, out Unity.Physics.RaycastHit test1));

                var forward = new RaycastInput()
                {
                    Start = transform.Position + (float3)(transform.Up * 1.5f),
                    End = transform.Position + (float3)(transform.Up * 1.5f + transform.Forward* 1.25f),
                    Filter = controller.mask
                };
                GroundRays.left = new RaycastInput()
                {
                    End = transform.Position + (float3)(Vector3.up * .5f + Vector3.left * .1f),
                    Start = transform.Position - (float3)(Vector3.up * .25f + Vector3.left * .1f)
                };
                GroundRays.right = new RaycastInput()
                {
                    Start = transform.Position + (float3)(Vector3.up * .1f - Vector3.left * .1f),
                    End = transform.Position - (float3)(Vector3.up * .25f - Vector3.left * .1f)
                };
                GroundRays.front = new RaycastInput()
                {
                    Start = transform.Position + (float3)(Vector3.up * .1f + Vector3.forward * .1f),
                    End = transform.Position - (float3)(Vector3.up * .25f + Vector3.up * .1f)
                };
                GroundRays.back = new RaycastInput()
                {
                    Start = transform.Position + (float3)(Vector3.up * .1f - Vector3.forward * .1f),
                    End = transform.Position - (float3)(Vector3.up * .25f - Vector3.forward * .1f)
                };
                Debug.DrawLine(center.Start,center.End,Color.red);

                bool temp = false;

                if (world.CastRay(center, out Unity.Physics.RaycastHit test))
                {
                    temp = true;
                    controller.GroundNormal = test.SurfaceNormal;
                }
                if (world.CastRay(GroundRays.left))
                {
                    temp = true;

                }
                if (world.CastRay(GroundRays.right))
                {
                    temp = true;
                }
                if (world.CastRay(GroundRays.front))
                {
                    temp = true;
                }
                if (world.CastRay(GroundRays.back))
                {
                    temp = true;
                }
              //  controller.IsGrounded = temp;

                Controllers[i] = controller;
            }
        }
    }
    public struct GroundCheckRays
    {
        public RaycastInput front;
        public RaycastInput center;
        public RaycastInput back;
        public RaycastInput left;
        public RaycastInput right;

    }
}