using System.Collections.Generic;
using Stats.Entities;
using Dreamers.InventorySystem.Interfaces;
using Sirenix.Serialization;
using System;

namespace Dreamers.InventorySystem.Base { 

public class EquipmentBase
    {
        public Dictionary<ArmorType, ArmorSO> EquippedArmor = new Dictionary<ArmorType, ArmorSO>();
        public Dictionary<WeaponSlot, WeaponSO> EquippedWeapons = new Dictionary<WeaponSlot, WeaponSO>();


        public int CurrentActivationPoints;
        public int MaxActivationPoints;
        public List<ItemSlot> QuickAccessItems;
        public int NumOfQuickAccessSlots;
        public bool OpenSlots { get { return QuickAccessItems.Count < NumOfQuickAccessSlots; } }

        public void Init() { 
            QuickAccessItems= new List<ItemSlot>();
            NumOfQuickAccessSlots= 2;
        }
        public void Init(EquipmentSave save, BaseCharacterComponent player, int size =2) { 

            QuickAccessItems= new List<ItemSlot>();
            NumOfQuickAccessSlots=  size;
           LoadEquipment(player,save);
        }

        void reloadEquipment(BaseCharacterComponent player) {
            foreach (ArmorSO so in EquippedArmor.Values) {
                so.Equip(player);
            }
            foreach (WeaponSO so in EquippedWeapons.Values)
            {
                so.Equip(player);
            }
        }

        void LoadEquipment(BaseCharacterComponent PC, EquipmentSave Save)
        {
            foreach (ArmorSO SO in Save.EquippedArmors)
            {
              if(  SO.Equip(PC))
                    EquippedArmor[SO.ArmorType] = SO;
            }
            foreach (WeaponSO SO in Save.EquippedWeapons)
            {
                if (SO)
                {
                    if(SO.Equip(PC))
                    EquippedWeapons[SO.Slot] = SO;
                }
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
