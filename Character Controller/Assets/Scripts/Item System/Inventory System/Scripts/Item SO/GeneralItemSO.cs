using Dreamers.InventorySystem.Base;
using Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Dreamers.InventorySystem
{
    [System.Serializable]
    public class GeneralItemSO : ItemBaseSO, IGeneral
    {
        [SerializeField] private TypeOfGeneralItem _GeneralType;
        public TypeOfGeneralItem GeneralItemType { get { return _GeneralType; } }

        public override void EquipItem( CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
           
        }

        public override void Unequip(CharacterInventory characterInventory, BaseCharacter player)
        {
         
        }

        public override void Use(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
          
        }
        public override void Convert(Entity entity, EntityManager dstManager)
        { }
    }
}
