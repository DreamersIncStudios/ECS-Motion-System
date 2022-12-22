using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace DreamersInc.CombatSystem
{
    [DisableAutoCreation]
    public partial class EquipSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, AnimatorComponent animc, CharacterInventory inventory, ref DrawWeapon tag) => {
                
               inventory.Equipment.EquippedWeapons[WeaponSlot.Primary].DrawWeapon(animc.anim);
                EntityManager.RemoveComponent<DrawWeapon>(entity);
            }).Run();
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, AnimatorComponent animc, CharacterInventory inventory, ref StoreWeapon tag) => { 
               inventory.Equipment.EquippedWeapons[WeaponSlot.Primary].StoreWeapon(animc.anim);
                EntityManager.RemoveComponent<StoreWeapon>(entity);

            }).Run();

        }

    }
    public struct DrawWeapon : IComponentData { }
    public struct StoreWeapon : IComponentData { }

}