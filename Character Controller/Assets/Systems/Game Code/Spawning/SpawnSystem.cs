using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


namespace DreamersInc.SpawnSystems
{
    [BurstCompile]
    public partial struct SpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            //foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
            //{
            //    if (!spawner.ValueRO.spawned)
            //    {
            //        var spawn = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            //        state.EntityManager.SetComponentData(spawn, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
            //        Debug.Log("ran");
            //        spawner.ValueRW.spawned = true;
            //    }
           // }
        }
       
    }
}