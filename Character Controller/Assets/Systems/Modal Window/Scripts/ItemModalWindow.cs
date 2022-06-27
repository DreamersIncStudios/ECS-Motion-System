using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.Global;
using Dreamers.ModalWindows;
using UnityEngine.Events;
using Dreamers.InventorySystem.SO;

namespace Dreamers.InventorySystem.UISystem
{
    public class ItemModalWindow : MonoBehaviour
{
        [Header("Header")]
        [SerializeField] Transform headerArea;

        [Header("Body")]
        [SerializeField] Transform contentArea;
        [SerializeField] GameObject ItemPrefab;
        [SerializeField] Transform itemsParent;

        List<Button> itemsOnDisplay;

        [Header("Footer")]
        [SerializeField] Transform footerArea;
        InventoryBase inventoryToDisplay;
        CharacterInventory charInventory;
        public void ShowAsCharacterInventory(CharacterInventory inventory) {

            inventoryToDisplay = inventory.Inventory;
            charInventory = inventory;
            itemsOnDisplay = new List<Button>();
            DisplayItems(ItemType.None);
           
        
        }

        public void ShowAsStoreInventory() { }

        /// <summary>
        /// Show Item in Inventory basedon the filter selected 
        /// </summary>
        /// <param name="filter"> Filter selection of none shows all items </param>

        ItemType curFilter;
       void DisplayItems(ItemType filter) {

            if (itemsOnDisplay.Count != 0) {
                ClearItemList();
            }
            curFilter = filter;
            for (int i = 0; i < inventoryToDisplay.ItemsInInventory.Count; i++)
            {
                ItemSlot Slot = inventoryToDisplay.ItemsInInventory[i];
                int IndexOf = i;
                if (Slot.Item.Type == filter || filter == ItemType.None)
                {
                    var item = Instantiate(ItemPrefab, itemsParent).GetComponent<Button>();
                    itemsOnDisplay.Add(item);
                    item.image.sprite = Slot.Item.Icon;
                    item.GetComponentInChildren<TextMeshProUGUI>().text = Slot.Item.ItemName;
                    UnityEvent useItem = new UnityEvent();
                    UnityEvent dropItem = new UnityEvent();
                    if (Slot.Item.Type != ItemType.Quest)
                    {
                     dropItem.AddListener(()=>{ inventoryToDisplay.RemoveFromInventory(Slot.Item); });
                    }
                    switch(Slot.Item.Type){
                        case ItemType.General:
                            useItem.AddListener(() => {
                                RecoveryItemSO temp = (RecoveryItemSO)Slot.Item;
                                temp.Use(charInventory);
                            });
                            break;
                        case ItemType.Armor:
                            useItem.AddListener(() => {
                                bool equipedItem = false;
                                ArmorSO Armor = (ArmorSO)Slot.Item;
                                equipedItem = Armor.EquipItem(charInventory);
                            });
                            break;
                        case ItemType.Weapon:
                            useItem.AddListener(() => {
                                bool equipedItem = false;

                                WeaponSO weapon = (WeaponSO)Slot.Item;
                                equipedItem = weapon.EquipItem(charInventory);
                            });
                            break;
                    }
                    useItem.AddListener(() => { 
                        DisplayItems(filter);
                       var statWinodw = transform.root.GetComponentInChildren<CharacterStatModal>();
                        if (statWinodw) {
                            var basechar = charInventory.GetComponent<BaseCharacter>();

                            //Todo rewrite so we can target other characters
                            statWinodw.UpdateEquipmentGrid(basechar, charInventory.Equipment, charInventory);
                            statWinodw.UpdatePlayerStatsText(charInventory.GetComponent<BaseCharacter>());
                        }
                    });
                    item.onClick.AddListener(() => {
                        ModalWindow pop = Instantiate(UIManager.instance.ModalWindowPrefab, item.transform.root).GetComponent<ModalWindow>();
                       // pop.transform.SetParent(transform.parent);
                        pop.ShowAsItemPrompt(Slot.Item,useItem,dropItem);

                    });
                }
            }
        }


        /// <summary>
        /// Show Item in Inventory basedon the filter selected 
        /// </summary>
        /// <param name="filter"> Filter selection of 0 shows all items </param>
        public void DisplayItems(int filter) {
            DisplayItems((ItemType)filter);
        
        }

        public void Refresh() {
            DisplayItems(curFilter);
        }
        public void ClearItemList() {

            foreach (Transform child in itemsParent)
            {
                Object.Destroy(child.gameObject);
            }
        }

    }
}
