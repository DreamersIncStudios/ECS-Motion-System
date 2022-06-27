using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stats;
using Unity.Entities;
using Dreamers.InventorySystem.Interfaces;

namespace Dreamers.InventorySystem.SO
{
    public class MountedTurret : ScriptableObject, IItemBase, IProjectile
    {
        #region Item Variables
        [SerializeField] private uint _itemID;
        public uint ItemID { get { return _itemID; } } // To be implemented with Database system/CSV Editor creator 
        [SerializeField] private string _itemName;
        public string ItemName { get { return _itemName; } }
        [TextArea(3, 6)]
        [SerializeField] private string _desc;
        public string Description { get { return _desc; } }

        [SerializeField] private Sprite _icon;
        public Sprite Icon { get { return _icon; } }

        [SerializeField] private ItemType _type;
        public ItemType Type { get { return _type; } }
        [SerializeField] private bool _stackable;
        public bool Stackable { get { return _stackable; } }
        //[SerializeField] bool _disposible;
        public bool Disposible { get { return !QuestItem; } }
        [SerializeField] bool _questItem;
        public bool QuestItem { get { return _questItem; } }
        #endregion
        #region Projectile Variable

        [SerializeField] int roundsPerMin;

        public int RoundsPerMin { get { return roundsPerMin; } }
        [SerializeField] int roundsPerShot;

        public int RoundsPerShot { get { return roundsPerShot; } }
        [SerializeField] float normalSpeed;

        public float NormalSpeed { get { return normalSpeed; } }

        [SerializeField] float offset;
        public float ShootLocationOffset { get { return offset; } }

        #endregion
#if UNITY_EDITOR

        public void setItemID(uint ID)
        {

            _itemID = ID;
        }
#endif
        public void Use(CharacterInventory characterInventory)
        {
        
        }
    }
}