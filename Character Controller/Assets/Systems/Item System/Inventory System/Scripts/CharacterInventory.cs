using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.UISystem;
using Dreamers.InventorySystem.MissionSystem;
using Stats;

namespace Dreamers.InventorySystem
{
    public class CharacterInventory : MonoBehaviour,IConvertGameObjectToEntity
    {
        private BaseCharacter PC => this.GetComponent<BaseCharacter>();
        private Animator anim => this.GetComponent<Animator>();
        public InventoryBase Inventory;
        public EquipmentBase Equipment;
        public MissionHub QuestLog;
        public Entity self { get; private set; }
        public int Gold { get; private set; }
//#if UNITY_EDITOR

        public EquipmentSave Save;
//#endif
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            self = entity;
        }

        public void Start()
        {
            QuestLog = new MissionHub(null, null, new List<MissionSystem.SO.MissionQuestSO>());

            Instantiate( QuestDatabase.GetQuest((uint)1)).AcceptQuest(QuestLog);
            //TODO Make Starter system for new Game
            Equipment.LoadEquipment(PC,Save);

            Gold = 2000; //TODO remove in final 

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