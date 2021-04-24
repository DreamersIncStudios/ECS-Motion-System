using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.UISystem;

using Stats;
namespace Dreamers.InventorySystem
{
    public class CharacterInventory : MonoBehaviour,IConvertGameObjectToEntity
    {
        private BaseCharacter PC => this.GetComponent<BaseCharacter>();
        private Animator anim => this.GetComponent<Animator>();
        public InventoryBase Inventory;
        public EquipmentBase Equipment;
        DisplayMenu Menu;
        public Entity self { get; private set; }
        public int Gold;
#if UNITY_EDITOR

        public EquipmentSave Save;
#endif
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            self = entity;
        }

        public void Start()
        {
            Menu = new DisplayMenu(PC, this);
#if UNITY_EDITOR
            Equipment.LoadEquipment(PC,Save);
#endif
        }

        private void Update()
        {
            if (Menu == null)
                Menu = new DisplayMenu(PC, this);

            if (Input.GetKeyUp(KeyCode.I) && Menu.Displayed) { Menu.CloseInventory(); }
            if (Input.GetKeyUp(KeyCode.I) && !Menu.Displayed) { Menu.OpenInventory(Inventory); }
        }
        public void EquipWeaponAnim()
        {
            //   anim.SetBool("CanDoDamage", true);
            Equipment.EquippedWeapons[WeaponSlot.Primary].DrawWeapon(anim);

        }

        public void UnequipWeaponAnim()
        {
            //   anim.SetBool("CanDoDamage", false);
            Equipment.EquippedWeapons[WeaponSlot.Primary].StoreWeapon(anim);

        }

        void SaveInventory()
        {
            EquipmentSave SaveCurrentEquipment = Equipment.GetEquipmentSave();
            InventorySave SaveInventory = Inventory.GetInventorySave();

        }

        public void LoadInventory(EquipmentSave equipmentSave, InventorySave inventorySave) {
            Inventory.LoadInventory(inventorySave);
            Equipment.LoadEquipment(PC,equipmentSave);
        
        }

    }
}