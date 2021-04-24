
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class Vender : MonoBehaviour
    {
        public StoreBase Base;
        DisplayStore Store;
        public GameObject player;
        bool canOpen =false;

        private void Start()
        {
            Store = new DisplayStore(Base);
            Store.CloseStore();
        }
        private void Update()
        {
            if (canOpen) {
                if (Input.GetKeyUp(KeyCode.V) && Store.Displayed) { Store.CloseStore(); }
                if (Input.GetKeyUp(KeyCode.V) && !Store.Displayed)
                {
                    Store.OpenStore();
                }

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                canOpen = true;
                player = other.gameObject;
                Base.CharacterInventory = player.GetComponent<CharacterInventory>();

            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("Player")) {
                player = null;
                Base.CharacterInventory = null;
                canOpen = false;
            }
        }
    }
}