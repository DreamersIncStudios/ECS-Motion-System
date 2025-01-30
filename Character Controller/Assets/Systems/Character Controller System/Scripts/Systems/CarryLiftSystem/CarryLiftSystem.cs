using MotionSystem.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace  Dreamers.MotionSystem
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))] 
    public partial struct CarryLiftSystem : ISystem
    {
    

        public void OnCreate(ref SystemState state)
        { 
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();  
            var carriables = SystemAPI.QueryBuilder().WithAll<LocalToWorld, Interactable>().WithNone<CarriedTag>().Build();
            new PickUpJob()
            {
                Entities = carriables.ToEntityArray(Allocator.TempJob),
                Positions = carriables.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule();
        }

        partial struct PickUpJob : IJobEntity
        {
            public NativeArray<LocalToWorld> Positions;
            public NativeArray<Entity> Entities;
            public EntityCommandBuffer ECB;
            
          void Execute(Entity entity, [ChunkIndexInQuery]int index, ref LocalToWorld position, in CharControllerE controllerE)
            {
                for (int i = 0; i < Entities.Length; i++)
                {
                    var dist = Vector3.Distance(position.Position, Positions[i].Position);
                    if (!(dist <= .75f)) continue;
                   // Debug.Log("Pick up Possible");
                
                       // ECB.AddComponent(Entities[i], new CarriedTag(entity));
                      
                }
            }
        }
    }
   
}
