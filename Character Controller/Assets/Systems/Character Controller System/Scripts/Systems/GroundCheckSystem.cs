using DreamersInc;
using MotionSystem.Components;
using MotionSystem.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace MotionSystem
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(AnimatorUpdate))]
    public partial struct GroundCheckSystem : ISystem
    {
        private NativeParallelMultiHashMap<int, QuadrantData> quadrantMultiHashMap;
        private const int QuadrantYMultiplier = 1000;
        private const int QuadrantCellSize = 100;
        public EntityQuery Query;

        private struct QuadrantData
        {
            public Entity entity;
            public float3 position;

        }

        private static int GetPositionHashMapKey(float3 position)
        {
            return (int)(Mathf.Floor(position.x / QuadrantCellSize) + (QuadrantYMultiplier * Mathf.Floor(position.y / QuadrantCellSize)));
        }

        private int GetEntityCountInHashMap(NativeParallelMultiHashMap<int, QuadrantData> quadrantMap, int hashMapKey)
        {
            var count = 0;
            if (!quadrantMap.TryGetFirstValue(hashMapKey, out _,
                    out var iterator)) return count;
            do
            {
                count++;
            }
            while (quadrantMap.TryGetNextValue(out _, ref iterator));
            return count;
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            quadrantMultiHashMap = new NativeParallelMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
            Query = state.GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(CharControllerE)),
            ComponentType.ReadWrite(typeof(Animator))}
            });
        }


        public void OnDestroy(ref SystemState state)
        {
            quadrantMultiHashMap.Dispose();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonEntity<Player_Control>(out Entity entityPlayer)) return;

            if (Query.CalculateEntityCount() != quadrantMultiHashMap.Capacity)
            {
                quadrantMultiHashMap.Clear();
                quadrantMultiHashMap.Capacity = Query.CalculateEntityCount();

                new SetQuadrantDataHashMapJob { QuadrantMap = quadrantMultiHashMap.AsParallelWriter() }.ScheduleParallel(Query);
            }

            var playerPosition = SystemAPI.GetComponent<LocalToWorld>(entityPlayer).Position;
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var world = physicsWorldSingleton.CollisionWorld;
            state.Dependency = new GroundCheckJob
            {
                HashKey = GetPositionHashMapKey((int3)playerPosition),
                World = world
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct GroundCheckJob : IJobEntity {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public int HashKey;

            private void Execute(ref LocalTransform transform, ref CharControllerE control)
            {
                if (control.SkipGroundCheck)
                {
                    return;
                }
                control.IsGrounded = GroundCheck(transform, control, HashKey) ||
                                     GroundCheck(transform, control, HashKey + 1) ||
                                     GroundCheck(transform, control, HashKey - 1) ||
                                     GroundCheck(transform, control, HashKey + QuadrantYMultiplier) ||
                                     GroundCheck(transform, control, HashKey - QuadrantYMultiplier);
            }

            private bool GroundCheck(LocalTransform transform, CharControllerE control, int hashKeyIndex)
            {
                if (hashKeyIndex != GetPositionHashMapKey((int3)transform.Position)) return false;
                var groundRays = new NativeList<RaycastInput>(Allocator.Temp);
                var filter = new CollisionFilter
                {
                    BelongsTo = ((1 << 10)),
                    CollidesWith = ((1 << 6) | (1 << 9)),
                    GroupIndex = 0
                };
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(0, .2f, 0),
                    End = transform.Position + new float3(0, -control.GroundCheckDistance, 0),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(0, .2f, .25f),
                    End = transform.Position + new float3(0, -control.GroundCheckDistance, .25f),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(0, .1f, -.25f),
                    End = transform.Position + new float3(0, -control.GroundCheckDistance, -.25f),
                    Filter =filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(.25f, .1f, 0),
                    End = transform.Position + new float3(.25f, -control.GroundCheckDistance, 0),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(-.25f, .1f, 0),
                    End = transform.Position + new float3(-.25f, -control.GroundCheckDistance, 0),
                    Filter = filter
                });

                foreach (var ray in groundRays)
                {

                    var raycastArray = new NativeList<RaycastHit>(Allocator.Temp);

                    if (!World.CastRay(ray, ref raycastArray)) continue;
                    groundRays.Dispose();
                    return true;
                }
                return false;

            }
        }
               [BurstCompile]
               private partial struct SetQuadrantDataHashMapJob : IJobEntity
        {
            public NativeParallelMultiHashMap<int, QuadrantData>.ParallelWriter QuadrantMap;

            private void Execute(Entity entity, [ReadOnly]in LocalTransform transform)
            {
                var hashMapKey = GetPositionHashMapKey(transform.Position);
                QuadrantMap.Add(hashMapKey, new QuadrantData { 
                    entity = entity,
                    position = transform.Position
                });
            }
        }
    }
}
