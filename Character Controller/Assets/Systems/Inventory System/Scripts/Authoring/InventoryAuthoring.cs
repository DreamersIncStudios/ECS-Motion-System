using Dreamers.InventorySystem.Base;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class InventoryAuthoring : MonoBehaviour
    {
        public InventoryBase inventory;
        public EquipmentBase equipment;

        class Baking : Baker<InventoryAuthoring>
        {
            public override void Bake(InventoryAuthoring authoring)
            {
                var data = new CharacterInventory();
                data.Setup();
                AddComponentObject(data);
                AddComponentObject(new AnimatorComponent());
            }
        }

    }
}