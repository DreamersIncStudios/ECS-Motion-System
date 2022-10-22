using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.ComboSystem;
using Dreamers.InventorySystem;

namespace MotionSystem
{
    public class CheckEquipSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((CharacterInventory inventory, PlayerComboComponent combo) =>
            {
                combo.WeaponEquipped = inventory.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary, out _);


            });
        }
    }
}
