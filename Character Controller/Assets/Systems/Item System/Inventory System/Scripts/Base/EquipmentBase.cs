using System.Collections.Generic;
using Stats;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.SO;
using UnityEngine;
namespace Dreamers.InventorySystem.Base
{

    [System.Serializable]
    public class EquipmentBase
    {


        public Dictionary<ArmorType, ArmorSO> EquippedArmor = new Dictionary<ArmorType, ArmorSO>();
        public Dictionary<WeaponSlot, WeaponSO> EquippedWeapons = new Dictionary<WeaponSlot, WeaponSO>();


        public int CurrentActivationPoints;
        public int MaxActivationPoints;
        public List<ItemSlot> QuickAccessItems;
        public int NumOfQuickAccessSlots;
        public bool OpenSlots { get { return QuickAccessItems.Count < NumOfQuickAccessSlots; } }


        public EquipmentSave GetEquipmentSave()
        {
            EquipmentSave Save = new EquipmentSave();
            foreach (var item in EquippedArmor)
            {
                if (!item.Value.Equipped)
                {
                    Save.EquippedArmors.Add(item.Value);
                }
            }

            foreach (var item in EquippedWeapons)
            {
                if (!item.Value.Equipped)
                {
                    Save.EquippedWeapons.Add(item.Value);
                }
            }
            return Save;
        }

        public void LoadEquipment(BaseCharacter PC, EquipmentSave Save)
        {
            foreach (ArmorSO SO in Save.EquippedArmors)
            {
                var copy = Object.Instantiate(SO);
                copy.Equip(PC);
                EquippedArmor[copy.ArmorType] = copy;
            }
            foreach (WeaponSO SO in Save.EquippedWeapons)
            {
                var copy = Object.Instantiate(SO);

                copy.Equip(PC);
                    EquippedWeapons[copy.Slot] = copy;
                
            }
        }

    }
    [System.Serializable]
    public class EquipmentSave
    {


        public List<WeaponSO> EquippedWeapons;
        public List<ArmorSO> EquippedArmors;

    }
}
