using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.ComboSystem;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace DreamersInc.CombatSystem
{
    [UpdateAfter(typeof(ComboInputSystem))]
    public partial class EquipSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithoutBurst().ForEach((CharacterInventory test, Command input) =>
            {
                if (!input.StateInfo.IsName("Grounded0")) return;
                if (test.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary, out _) && !input.WeaponIsEquipped)
                    input.WeaponIsEquipped = true;
                if (!test.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary, out _) && input.WeaponIsEquipped)
                    input.WeaponIsEquipped = false;
            }).Run();
            
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, Animator Anim, CharacterInventory character, ref DrawPrimaryWeapon tag) => {

                character.Equipment.EquippedWeapons[WeaponSlot.Primary].DrawWeapon(Anim);
                EntityManager.RemoveComponent<DrawPrimaryWeapon>(entity);
            }).Run();
            
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, Animator Anim, CharacterInventory character, ref StorePrimaryWeapon tag) =>
            {
                if (character.Equipment.EquippedWeapons.Count <= 0) return;
                if (character.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary, out WeaponSO primaryEquipped) &&
                    !primaryEquipped.AlwaysDrawn)
                    character.Equipment.EquippedWeapons[WeaponSlot.Primary].StoreWeapon(Anim);
                EntityManager.RemoveComponent<StorePrimaryWeapon>(entity);
            }).Run();

            
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, Animator Anim, CharacterInventory character, ref DrawSecondaryWeapon tag) => {

                character.Equipment.EquippedWeapons[WeaponSlot.Projectile].DrawWeapon(Anim);
                EntityManager.RemoveComponent<DrawSecondaryWeapon>(entity);
            }).Run();
            
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, Animator Anim, CharacterInventory character, ref StoreSecondaryWeapon tag) =>
            {
                if (character.Equipment.EquippedWeapons.Count <= 0) return;
                if (character.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Projectile, out var primaryEquipped) &&
                    !primaryEquipped.AlwaysDrawn)
                    character.Equipment.EquippedWeapons[WeaponSlot.Projectile].StoreWeapon(Anim);
                EntityManager.RemoveComponent<StoreSecondaryWeapon>(entity);
            }).Run();
            
        }

    }
    public struct DrawPrimaryWeapon : IComponentData { }
    public struct StorePrimaryWeapon : IComponentData { }

    public struct DrawSecondaryWeapon : IComponentData { }
    public struct StoreSecondaryWeapon : IComponentData { }
}