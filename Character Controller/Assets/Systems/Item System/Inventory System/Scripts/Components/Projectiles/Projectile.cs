using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Jobs;
using Stats;
using Unity.Mathematics;
using Unity.Burst;

[GenerateAuthoringComponent]
public struct Projectile : IComponentData
{
    public CollisionFilter Filter;
    public float RayLength;


}
public class ProjectileSystem : SystemBase
{
    EntityQuery ProjectilesEntityQ;
    EntityCommandBufferSystem entityCommandBufferSystem;
    EndFramePhysicsSystem m_EndFramePhysicsSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        ProjectilesEntityQ = GetEntityQuery(new EntityQueryDesc() { 
            All= new ComponentType[] { ComponentType.ReadWrite(typeof(Projectile)), ComponentType.ReadOnly(typeof(LocalToWorld))}
        });
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_EndFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
    }
    protected override void OnUpdate()
    {

        Dependency = JobHandle.CombineDependencies(Dependency, m_EndFramePhysicsSystem.GetOutputDependency());
        JobHandle systemDeps = Dependency;
        systemDeps = new ProjectileHitRayCast() {
            ProjectileChunk = GetComponentTypeHandle<Projectile>(false),
            ToWorldChunk = GetComponentTypeHandle<LocalToWorld>(true),
            physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld,
            EntityChunk = GetEntityTypeHandle(),
            buffer = entityCommandBufferSystem.CreateCommandBuffer(),
            Stats = GetComponentDataFromEntity<EnemyStats>(false)
        }.ScheduleSingle(ProjectilesEntityQ,systemDeps);
        entityCommandBufferSystem.AddJobHandleForProducer(systemDeps);
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().AddInputDependency(systemDeps);
        systemDeps.Complete();
    }

    // [BurstCompile]
    struct ProjectileHitRayCast : IJobChunk
    {
        public ComponentTypeHandle<Projectile> ProjectileChunk;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> ToWorldChunk;
        public PhysicsWorld physicsWorld;
       [ReadOnly] public EntityTypeHandle EntityChunk;
        public EntityCommandBuffer buffer;
        public ComponentDataFromEntity<EnemyStats> Stats;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<LocalToWorld> transforms = chunk.GetNativeArray(ToWorldChunk);
            NativeArray<Projectile> projectiles = chunk.GetNativeArray(ProjectileChunk);
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityChunk);
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
            for (int i = 0; i < chunk.Count; i++)
            {
                LocalToWorld transform = transforms[i];
                Projectile projectile = projectiles[i];
                RaycastInput Forward = new RaycastInput()
                {
                    Start = transform.Position,
                    End = transform.Position + transform.Forward * projectile.RayLength,
                    Filter = new CollisionFilter()
                    {
                        BelongsTo = ~0u,
                        CollidesWith = ((1<<9)|(1<<12)| (1 << 10) | (1 << 11)), // all 1s, so all layers, collide with everything
                        GroupIndex = 0
                    }
                };
                RaycastInput Back = new RaycastInput()
                {
                    Start = transform.Position,
                    End = transform.Position - transform.Forward * projectile.RayLength,
                    Filter = new CollisionFilter()
                    {
                        BelongsTo = ~0u,
                        CollidesWith = ((1 << 9) | (1 << 12) | (1 << 10) | (1 << 11)), // all 1s, so all layers, collide with everything
                        GroupIndex = 0
                    }
                };

                if (collisionWorld.CastRay(Forward, out Unity.Physics.RaycastHit Fhit))
                {
                    // see hit.Position
                    // see hit.SurfaceNormal
                    Entity e = physicsWorld.Bodies[Fhit.RigidBodyIndex].Entity;
                    if (Stats.HasComponent(e))
                    {
                        EnemyStats EC = Stats[e];
                            EC.AdjustHealth(-20);
                        Stats[e] = EC;
                        buffer.DestroyEntity(entities[i]);
                       
                    }
                }
                if (collisionWorld.CastRay(Back, out Unity.Physics.RaycastHit Bhit))
                {
                    // see hit.Position
                    // see hit.SurfaceNormal
                    Entity e = physicsWorld.Bodies[Bhit.RigidBodyIndex].Entity;
                    buffer.DestroyEntity(entities[i]);
                }
            }

        }
    }

}