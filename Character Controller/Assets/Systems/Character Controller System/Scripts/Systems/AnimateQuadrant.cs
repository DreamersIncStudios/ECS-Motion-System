using DreamersInc.QuadrantSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using MotionSystem.Components;
using Stats.Entities;
using Unity.Mathematics;
using Unity.Collections;
using DreamersInc;


namespace MotionSystem.Systems
{
    public partial class AnimateQuadrant : GenericQuadrantSystem
    {
        EntityQuery withTag;
        EntityQuery withoutTag;
        protected override void OnCreate()
        {
            base.OnCreate();
            query = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(CharControllerE)),
            ComponentType.ReadWrite(typeof(AnimatorComponent))}
            });
            withTag = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(CharControllerE)),
            ComponentType.ReadWrite(typeof(AnimatorComponent)), ComponentType.ReadOnly(typeof(animateTag))},
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(Player_Control)) }

            }); 
            
            withoutTag = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(CharControllerE)),
            ComponentType.ReadWrite(typeof(AnimatorComponent))},
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(animateTag)) }
            
            });

        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            DOStuff();
        }
        public void DOStuff()
        {
            var test = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.DefaultGameObjectInjectionWorld.Unmanaged);
            if (SystemAPI.TryGetSingletonEntity<Player_Control>(out Entity entityPlayer))
            {
                var playerPosition = EntityManager.GetComponentData<LocalToWorld>(entityPlayer).Position;
                var systemDeps = Dependency;
                systemDeps = new AnimatorAddJob()
                {
                    hashKey = GetPositionHashMapKey((int3)playerPosition),
                    writer = test.AsParallelWriter()
                }.ScheduleParallel(withoutTag, systemDeps);
                systemDeps.Complete();
                systemDeps = new AnimatorRemoveJob()
                {
                    hashKey = GetPositionHashMapKey((int3)playerPosition),
                    writer = test.AsParallelWriter()
                }.ScheduleParallel(withTag, systemDeps);

                systemDeps.Complete();
                Dependency = systemDeps;
            }
        }

        partial struct AnimatorAddJob : IJobEntity
        {

            public int hashKey;
            public EntityCommandBuffer.ParallelWriter writer;
            public void Execute(Entity entity, [EntityIndexInChunk] int index, [ReadOnly] in LocalTransform transform)
            {
                if (hashKey == GetPositionHashMapKey((int3)transform.Position))
                {
                    writer.AddComponent(index, entity, new animateTag());
                }
            }
        }
        partial struct AnimatorRemoveJob : IJobEntity
        {

            public int hashKey;
            public EntityCommandBuffer.ParallelWriter writer;
            public void Execute(Entity entity, [EntityIndexInChunk] int index, [ReadOnly] in LocalTransform transform)
            {

                if (hashKey != GetPositionHashMapKey((int3)transform.Position))
                {
                    writer.RemoveComponent<animateTag>(index, entity);
                }
            }
        }
    }
    public struct animateTag : IComponentData { }

}