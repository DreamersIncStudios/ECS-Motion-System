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

        public   async void DestroyAfterSeconds(int delay)
        {
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
            ecbSystem.CreateCommandBuffer().DestroyEntity(SelfEntityRef);
            await Task.Delay(2000);
            if(World.DefaultGameObjectInjectionWorld.EntityManager.Exists(SelfEntityRef))
                EntityExtensions.RemoveAllComponents(World.DefaultGameObjectInjectionWorld.EntityManager, SelfEntityRef);
            Destroy(this.gameObject);
        }

        public Entity SelfEntityRef { get; set; }

        public async void OnTriggerEnter(Collider other)
        {
            hastriggered = true;
            await DestroyEntity();
        }
    }
    
}