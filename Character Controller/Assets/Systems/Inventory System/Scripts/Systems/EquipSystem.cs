using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Stats.Entities;
using Dreamers.InventorySystem.Base;

namespace Dreamers.InventorySystem
{
    public partial class EquipSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ECB = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

            Entities.WithoutBurst().ForEach((Entity entity, CharacterInventory inventory, BaseCharacterComponent player, AddEquipment adder, AnimatorComponent Anim ) => 
            {
                adder.equipItem.EquipItem(inventory, player);
                ECB.RemoveComponent<AddEquipment>(entity);

            }).Run();
        }
    }
}