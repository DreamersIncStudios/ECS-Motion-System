using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;

namespace Dreamers.InventorySystem
{
    [System.Serializable]
    public class Kits
    {
        public string KitName;
        public uint Cost;
        public List<WeaponSO> Weapons;
        public List<ArmorSO> Armors;

        public void NewGameEquip(BaseCharacter character) {
            foreach (WeaponSO weapon in Weapons) {
                weapon.Equip(character);
            }
            foreach (ArmorSO armor in Armors)
            {
                armor.Equip(character);
            }
        }

        public void AddToInventory(CharacterInventory inventory) {
            foreach (WeaponSO weapon in Weapons)
            {
                inventory.Inventory.AddToInventory(weapon);

            }
            foreach (ArmorSO armor in Armors)
            {
                inventory.Inventory.AddToInventory(armor);

            }
        }
    }
}