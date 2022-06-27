using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.Generic;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem;
using Dreamers.InventorySystem.SO;

namespace Dreamers.InventorySystem
{
    [RequireComponent(typeof(BoxCollider))]

    public class CreateStore : MonoBehaviour
    {
        public StoreTypes StoreType;
        private GameObject player;
        private bool canOpen => (bool)player;

       public List<int> InitialItemsID;
        
        public Shop shop;

        private void Start()
        {
            shop = new Shop(StoreType.ToString(), StoreType, output());
        }
        public List<IPurchasable> output() {
            List<IPurchasable> temp = new List<IPurchasable>();
            switch (StoreType) {
                case StoreTypes.General:
                case StoreTypes.Armor:
                case StoreTypes.Weapon:
                case StoreTypes.Item:
                    foreach (var entry in InitialItemsID)
                    {
                        temp.Add(ItemDatabase.GetItem(entry));
                    }
                    break;
                case StoreTypes.Mission:
                    foreach (var entry in InitialItemsID)
                    {
                        temp.Add(QuestDatabase.GetQuest((uint)entry));
                    }
                    break;
            }
                return temp;
        }
        private void Update()
        {
            if (canOpen)
            {
                if (Input.GetKeyUp(KeyCode.V) && shop.Displayed) { shop.CloseStore(); }
                if (Input.GetKeyUp(KeyCode.V) && !shop.Displayed)
                {
                    shop.OpenStore(player.GetComponent<CharacterInventory>());
                }

            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                player = other.gameObject;

            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                player = null;
            }
        }
    }

 
    public enum StoreTypes { General, Item, Weapon, Armor, Mission}

}