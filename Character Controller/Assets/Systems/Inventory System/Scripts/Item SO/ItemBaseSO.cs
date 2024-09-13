using Newtonsoft.Json;
using Unity.Entities;
using UnityEngine;
using Stats;
using Stats.Entities;

namespace Dreamers.InventorySystem.Interfaces
{
    [System.Serializable]
    public abstract class ItemBaseSO : ScriptableObject, IItemBase, IPurchasable
    {
        [SerializeField] private uint _itemID;

        public uint ItemID
        {
            get { return _itemID; }
        } // To be implemented with Database system/CSV Editor creator 

        [SerializeField] private string _itemName;

        public string ItemName
        {
            get { return _itemName; }
        }

        [TextArea(3, 6)] [SerializeField] private string _desc;

        public string Description
        {
            get { return _desc; }
        }

        [SerializeField] private Sprite _icon;

        public Sprite Icon
        {
            get { return _icon; }
        }

        [SerializeField] private uint _value;

        public uint Value
        {
            get { return _value; }
        }

        [SerializeField] private ItemType _type;

        public ItemType Type
        {
            get { return _type; }
        }

        [SerializeField] private bool _stackable;

        public bool Stackable
        {
            get { return _stackable; }
        }

  
        public bool Disposable => !QuestItem;

        [SerializeField] bool _questItem;

        public bool QuestItem
        {
            get { return _questItem; }
        }

        [SerializeField] private uint maxStackCount;
        public uint MaxStackCount
        {
            get { return maxStackCount; }
        }
#if UNITY_EDITOR

        public void setItemID(uint ID)
        {

            _itemID = ID;
        }
#endif

        public virtual void Use(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            characterInventory.Inventory.RemoveFromInventory(this);
        }


        public virtual string Serialize()
        {
            var serializeData = new SerializedItemSO(itemID: ItemID, itemName: ItemName, description: Description,
                value: Value, type: Type, stackable: Stackable, questItem: QuestItem);
            string output = JsonConvert.SerializeObject(serializeData);


            return output;
        }

        public virtual void Deserialize()
        {
        }
    }

    public class SerializedItemSO
        {
            public uint ItemID;
            public string ItemName;
            public string Description;
            public uint Value;
            public ItemType Type;
            public bool Stackable;
            public bool QuestItem;

            public SerializedItemSO()
            {
            }

            public SerializedItemSO(uint itemID, string itemName, string description, uint value, ItemType type, bool stackable, bool questItem)
            {
                ItemID = itemID;
                ItemName = itemName;
                Description = description;
                Value = value;
                Type = type;
                Stackable = stackable;
                QuestItem = questItem;
            }
        }
    
}