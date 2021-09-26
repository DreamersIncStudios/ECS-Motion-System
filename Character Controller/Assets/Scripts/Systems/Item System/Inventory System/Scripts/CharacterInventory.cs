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
        public int Gold { get; private set; }
#if UNITY_EDITOR

        public EquipmentSave Save;
#endif
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            self = entity;
        }

        public void Start()
        {
            Menu = new DisplayMenu(PC);
#if UNITY_EDITOR
            Equipment.LoadEquipment(PC,Save);
#endif
            Gold = 2000; //TODO remove in final 

        }
        bool CloseMenu => Input.GetKeyUp(KeyCode.I) && Menu.Displayed;
        bool OpenMenu => Input.GetKeyUp(KeyCode.I) && !Menu.Displayed;
        private void Update()
        {
            if (CloseMenu) { Menu.CloseCharacterMenu(); }
            if (OpenMenu)
            { Menu.OpenCharacterMenu(Inventory); }
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
        public void AdjustGold(int modValue)
        {
            if (modValue <= Gold)
                Gold =(int)Mathf.Clamp(Gold+ modValue, 0,Mathf.Infinity);
        }
    }
}