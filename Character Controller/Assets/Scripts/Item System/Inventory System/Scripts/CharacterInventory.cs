using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.UISystem;

using Stats;
namespace Dreamers.InventorySystem
{
    public class CharacterInventory : MonoBehaviour, IConvertGameObjectToEntity
    {
        private BaseCharacter PC => this.GetComponent<BaseCharacter>();
        public InventoryBase Inventory;
        public EquipmentBase Equipment;
        DisplayMenu Menu;
        public Entity self { get; private set; }
        public int Gold;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            self = entity;
        }

        void Awake()
        {
            Inventory = new InventoryBase();
            Equipment = new EquipmentBase();
        }

        public void Start()
        {
            Menu = new DisplayMenu(PC, this);
        }

        private void Update()
        {
            if (Menu == null)
                Menu = new DisplayMenu(PC, this);

            if (Input.GetKeyUp(KeyCode.I) && Menu.Displayed) { Menu.CloseInventory(); }

            if (Input.GetKeyUp(KeyCode.I) && !Menu.Displayed) { Menu.OpenInventory(Inventory); }
        }

    }
}