using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem.Base {
    [System.Serializable]
    public class InventoryBase
    {
        public List<ItemSlot> ItemsInInventory;
        public uint MaxInventorySize;
        public InventoryBase(uint size) 
        {
            ItemsInInventory = new List<ItemSlot>();
            MaxInventorySize = size;
        }
        // Need to Update for Stackable items;
        public bool OpenSlots(ItemSlot Slot) {
            if (Slot.Item.Stackable) 
            {
                for (int i = 0; i < ItemsInInventory.Count; i++)
                {
                    ItemSlot itemInInventory = ItemsInInventory[i];
                    if (itemInInventory.Item.ItemID == Slot.Item.ItemID && itemInInventory.Count < 99)
                    {
                        return true;

                    }
                    if (itemInInventory.Item.ItemID == Slot.Item.ItemID && itemInInventory.Count == 99)
                    {
                        return ItemsInInventory.Count < MaxInventorySize;
                    }
                }
                return false;
            }
            else
            return ItemsInInventory.Count < MaxInventorySize;  
        }
        public ItemBaseSO FirstIndexOfItem(int ItemID) {
            foreach (ItemSlot itemSlot in ItemsInInventory) {
                if (itemSlot.Item.ItemID == ItemID)
                    return itemSlot.Item;
            }

            return null;
        }

        public ItemSlot FindItemSlot(int ItemID)
        {
            foreach (ItemSlot itemSlot in ItemsInInventory)
            {
                if (itemSlot.Item.ItemID == ItemID)
                    return itemSlot;
            }

            return new ItemSlot();
        }

        public ItemSlot FindItemSlot(int ItemID, out int indexOf)
        {
            indexOf = -1;
            foreach (ItemSlot itemSlot in ItemsInInventory)
            {
                if (itemSlot.Item.ItemID == ItemID)
                    indexOf = ItemsInInventory.IndexOf(itemSlot);
                    return itemSlot;
            }

            return new ItemSlot();
        }

        public List<ItemSlot> GetItemsByType(ItemType Type) {
            List<ItemSlot> ItemByType = new List<ItemSlot>();
            foreach(ItemSlot Slot in ItemsInInventory)
            {
                if(Type == ItemType.None)
                    ItemByType.Add(Slot);
                else if (Slot.Item.Type == Type)
                {
                    ItemByType.Add(Slot);
                }
            }
                return ItemByType;
        }

        public InventorySave GetInventorySave() {
            InventorySave Save = new InventorySave();
            Save.MaxInventorySize = MaxInventorySize;
            Save.ItemsInInventory = ItemsInInventory;
            return Save;
        }

        public void LoadInventory(InventorySave inventorySave) {
            MaxInventorySize = inventorySave.MaxInventorySize;
            ItemsInInventory = inventorySave.ItemsInInventory;
        }


        public bool OpenSlot { get { return ItemsInInventory.Count < MaxInventorySize; } }
   
    }

    [System.Serializable]
    public struct ItemSlot{
        public ItemBaseSO Item;
        public int Count;
        
    }
    [System.Serializable]
    public class InventorySave {
        public List<ItemSlot> ItemsInInventory;
        public uint MaxInventorySize;
    }
}