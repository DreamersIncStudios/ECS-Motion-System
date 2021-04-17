using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.Interfaces;
namespace Dreamers.InventorySystem.Base { 

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
 
    }   

}
