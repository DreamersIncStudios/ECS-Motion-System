using System;
using System.Threading.Tasks;
using DreamersInc.EntityUtilities;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.InventorySystem
{
    public class DestroyAfterSecond : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        bool hastriggered = false;

        private void Start()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        public   async void DestroyAfterSeconds(int delay, Entity entity)
        {
            selfEntityRef = entity;
                await Task.Delay(delay*1000);
                if (!hastriggered)
                {
                    await DestroyEntity();
                }
        }

        private async Task DestroyEntity()
        {
            if (!Application.isPlaying) return;
            var ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            ecbSystem.CreateCommandBuffer().DestroyEntity(selfEntityRef);
            await Task.Delay(2000);
            if(World.DefaultGameObjectInjectionWorld.EntityManager.Exists(selfEntityRef))
                EntityExtensions.RemoveAllComponents(World.DefaultGameObjectInjectionWorld.EntityManager, selfEntityRef);
            Destroy(this.gameObject);
        }

        void Destroy()
        {
            if (!Application.isPlaying) return;
            var ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            ecbSystem.CreateCommandBuffer().DestroyEntity(selfEntityRef);
            if(World.DefaultGameObjectInjectionWorld.EntityManager.Exists(selfEntityRef))
                EntityExtensions.RemoveAllComponents(World.DefaultGameObjectInjectionWorld.EntityManager, selfEntityRef);
            Destroy(this.gameObject);
        }

        private Entity selfEntityRef;

        public void OnTriggerEnter(Collider other)
        {
            hastriggered = true;
           Destroy();
        }
    }
    
}