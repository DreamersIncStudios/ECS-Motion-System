using Unity.Entities;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Base;

namespace Dreamers.InventorySystem
{

    public interface IItemBase
    {
        uint ItemID { get; }
        string ItemName { get; }
        string Description {get;}
        Sprite Icon { get; }
        int Value { get; }
        ItemType Type { get; }
        bool Stackable { get; }
        bool Disposible { get; }
        bool QuestItem { get; }

        void Use(CharacterInventory characterInventory, int IndexOf);

        void AddToInventory(CharacterInventory characterInventory);
        void RemoveFromInventory(CharacterInventory characterInventory, int IndexOf);

    }
    [System.Serializable]
    public abstract class ItemBaseSO : ScriptableObject, IItemBase
    {
        [SerializeField] private uint _itemID;
        public uint ItemID { get { return _itemID; }  } // To be implemented with Database system/CSV Editor creator 
        [SerializeField] private string _itemName;
        public string ItemName { get { return _itemName; } }
        [TextArea(3,6)]
        [SerializeField] private string _desc;
        public string Description { get { return _desc; } }
        [SerializeField] private Sprite _icon;
        public Sprite Icon { get { return _icon; } }

        [SerializeField] private int _value;
        public int Value { get { return _value; } }
        [SerializeField] private ItemType _type;
        public ItemType Type { get { return _type; } }
        [SerializeField]  private bool _stackable;
        public bool Stackable { get { return _stackable; } }
        //[SerializeField] bool _disposible;
        public bool Disposible { get { return !QuestItem; } }
        [SerializeField] bool _questItem;
        public bool QuestItem { get { return _questItem; } }

        public void setItemID(uint ID)
        {

#if UNITY_EDITOR
            _itemID = ID;
#endif
        }
        public  void Use(CharacterInventory characterInventory, int IndexOf)
        {
          

            RemoveFromInventory(characterInventory, IndexOf);

        }
        public abstract void Use(CharacterInventory characterInventory, int IndexOf, BaseCharacter player);

        //How we handle Equip Potions
        public abstract void EquipItem(CharacterInventory characterInventory,int IndexOf, BaseCharacter player );
        public abstract void Unequip(CharacterInventory characterInventory, BaseCharacter player);
        public abstract void Convert(Entity entity, EntityManager dstManager);


        public virtual void AddToInventory(CharacterInventory characterInventory)
        {
            InventoryBase inventory = characterInventory.Inventory;
            bool addNewSlot = true; 
              for (int i = 0; i < inventory.ItemsInInventory.Count; i++)
                {
                    ItemSlot itemInInventory = inventory.ItemsInInventory[i];
                    if (Stackable && itemInInventory.Item.ItemID == ItemID && itemInInventory.Count < 99)
                    {
                        itemInInventory.Count++;
                        addNewSlot = false;
                    }
                    inventory.ItemsInInventory[i] = itemInInventory;
                }
            
            if (inventory.OpenSlot && addNewSlot) 
                inventory.ItemsInInventory.Add(
                    new ItemSlot() {
                    Item = this,
                    Count=1});

        }

     
        

        public void RemoveFromInventory(CharacterInventory characterInventory, int IndexOf) // consider having inventory
        {
            InventoryBase inventory = characterInventory.Inventory;

            ItemSlot updateItem = inventory.ItemsInInventory[IndexOf];
            if (Stackable && updateItem.Count > 1)
            {
                updateItem.Count--;
                inventory.ItemsInInventory[IndexOf] = updateItem;
            }
            else { inventory.ItemsInInventory.RemoveAt(IndexOf); }
        }


    }
    public enum ItemType
    {
        None, General, Weapon, Armor,Crafting_Materials, Blueprint_Recipes,Quest
    }
}