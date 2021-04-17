using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamers.Global;

namespace Dreamers.InventorySystem
{
    public static class ItemDatabase 
    {
        static public List<ItemBaseSO> Items;
        static public bool isLoaded { get; private set; }

        private static void ValidateDatabase() {
            if (Items == null||!isLoaded )
            {
                Items = new List<ItemBaseSO>();
                isLoaded = false;
            }
            else { isLoaded = true; }
        }

        public static void LoadDatabase()
        {
            if (isLoaded)
                return;
            LoadDatabaseForce();
        }

        public static void LoadDatabaseForce()
        {
            Items = new List<ItemBaseSO>();
            isLoaded = true;
            ItemBaseSO[] itemsToLoad = Resources.LoadAll<ItemBaseSO>(@"Items");
            foreach (var item in itemsToLoad)
            {
                if (!Items.Contains(item))
                    Items.Add(item);
            }
        }
        public static void ClearDatabase() {
            isLoaded = false;
            Items.Clear();

        }
        public static ItemBaseSO GetItem(int SpawnID) {
            ValidateDatabase();
            LoadDatabase();
            foreach (ItemBaseSO item in Items)
            {
                if (item.ItemID == SpawnID)
                    return ScriptableObject.Instantiate(item) as ItemBaseSO;
                // Consider add switch to return Item as it proper type ?????

            }
            return null;
        }

#if UNITY_EDITOR
        public static class Creator { 
        
            [MenuItem("Assets/Create/RPG/Recovery Item")]
            static public void CreateRecoveryItem()
            {
                RecoveryItemSO Item;
                ScriptableObjectUtility.CreateAsset<RecoveryItemSO>("Item", out Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.Items.Count + 1);
                Debug.Log( Item.ItemID );
                // need to deal with duplicate itemID numbers 

            }
            [MenuItem("Assets/Create/RPG/Armor Item")]
            static public void CreateArmorItem()
            {
                ArmorSO Item;
                ScriptableObjectUtility.CreateAsset<ArmorSO>("Item", out Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.Items.Count + 1);
                Debug.Log(Item.ItemID);
                // need to deal with duplicate itemID numbers 

            }
            [MenuItem("Assets/Create/RPG/Weapon Item")]
            static public void CreateWeaponItem()
            {
                WeaponSO Item;
                ScriptableObjectUtility.CreateAsset<WeaponSO>("Item", out Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.Items.Count + 1);
                Debug.Log(Item.ItemID);
                // need to deal with duplicate itemID numbers 

            }

            [MenuItem("Assets/Create/RPG/Projectile Item")]
                 static public void CreateBlasterItem()
            {
                BlasterSO Item;
                ScriptableObjectUtility.CreateAsset<BlasterSO>("Item", out Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.Items.Count + 1);
                Debug.Log(Item.ItemID);
                // need to deal with duplicate itemID numbers 

            }
        }
#endif
    }
}