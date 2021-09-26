using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.Generic;
using Dreamers.InventorySystem.Interfaces;

namespace Dreamers.InventorySystem
{
    [RequireComponent(typeof(BoxCollider))]

    public class CreateStore : MonoBehaviour
    {
        public StoreTypes StoreType;
        private GameObject player;
        private bool canOpen => (bool)player;
     [SerializeReference]  public List<ItemBaseSO> InitialItems;
        public Shop shop;

        private void Awake()
        {
            shop = new Shop(StoreType.ToString(),InitialItems);
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