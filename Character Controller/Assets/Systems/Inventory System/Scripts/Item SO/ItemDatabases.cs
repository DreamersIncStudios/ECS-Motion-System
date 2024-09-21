using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamers.InventorySystem.Interfaces;

#if UNITY_EDITOR
using Dreamers.Global;
#endif

namespace Dreamers.InventorySystem
{
    public static class ItemDatabase 
    {
        private static List<ItemBaseSO> items;
        private static bool isLoaded { get; set; }

        private static void ValidateDatabase() {
            if (items == null||!isLoaded )
            {
                items = new List<ItemBaseSO>();
                isLoaded = false;
            }
            else { isLoaded = true; }
        }

        private static void LoadDatabase()
        {
            if (isLoaded)
                return;
            LoadDatabaseForce();
        }

        public static void LoadDatabaseForce()
        {
            items = new List<ItemBaseSO>();
            isLoaded = true;
            ItemBaseSO[] itemsToLoad = Resources.LoadAll<ItemBaseSO>(@"Items");
            foreach (var item in itemsToLoad)
            {
                if (!items.Contains(item))
                    items.Add(item);
            }
        }
        public static void ClearDatabase() {
            isLoaded = false;
            items.Clear();

        }
        public static ItemBaseSO GetItem(int SpawnID) {
            ValidateDatabase();
            LoadDatabase();
            foreach (ItemBaseSO item in items)
            {
                if (item.ItemID == SpawnID)
                    return ScriptableObject.Instantiate(item) as ItemBaseSO;
                // Consider add switch to return Item as it derived type ?????

            }
            return null;
        }

#if UNITY_EDITOR
        public static class Creator {

            //[MenuItem("Assets/Create/RPG/Recovery Item")]
            //static public void CreateRecoveryItem()
            //{
            //    ScriptableObjectUtility.CreateAsset<RecoveryItemSO>("Item", out RecoveryItemSO Item);
            //    ItemDatabase.LoadDatabaseForce();
            //    Item.setItemID((uint)ItemDatabase.Items.Count + 1);
            //    Debug.Log( Item.ItemID );
            //AssetDatabase.SetLabels(Item, new [] {"Item","Curative"});
            //    // need to deal with duplicate itemID numbers 

            //}
            [MenuItem("Assets/Create/RPG/Armor Item")]
            public static void CreateArmorItem()
            {
                ScriptableObjectUtility.CreateAsset<ArmorSO>("Item", out ArmorSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equipable","Armor"});
                // need to deal with duplicate itemID numbers 

            }
            [MenuItem("Assets/Create/RPG/Weapon Item")]
            public static void CreateWeaponItem()
            {
                ScriptableObjectUtility.CreateAsset<WeaponSO>("Item", out WeaponSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equipable","Weapon"});
                // need to deal with duplicate itemID numbers 

            }
            [MenuItem("Assets/Create/RPG/SpellBook Item")]
            public static void CreateSpellBookItem()
            {
                ScriptableObjectUtility.CreateAsset<SpellBookSO>("Item", out SpellBookSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equipable","Weapon"});
                // need to deal with duplicate itemID numbers 

            }
            [MenuItem("Assets/Create/RPG/Weapon Spell Item")]
            public static void CreateWeaponSpellItem()
            {
                ScriptableObjectUtility.CreateAsset<SpawnedWeaponSpellSO>("Item", out SpawnedWeaponSpellSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equipable","Weapon","Spawn Spell"});
                // need to deal with duplicate itemID numbers 

            }
            [MenuItem("Assets/Create/RPG/Project Weapon")]
            public static void CreateProjectileWeapon()
            {
                ScriptableObjectUtility.CreateAsset<ProjectileWeaponSO>("Item", out ProjectileWeaponSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equipable","Weapon"});
                // need to deal with duplicate itemID numbers 

            }
            
            [MenuItem("Assets/Create/RPG/Modifier Spell Item")]
            public static void CreateModSpellItem()
            {
                ScriptableObjectUtility.CreateAsset<ModifierSpellSO>("Item", out ModifierSpellSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equipable","Weapon","Mod Spell", });
                // need to deal with duplicate itemID numbers 

            }  
            [MenuItem("Assets/Create/RPG/Spells/ Projectile Spells")]
            public static void CreateProjectileItem()
            {
                ScriptableObjectUtility.CreateAsset<ProjectileSpellSO>("Item", out ProjectileSpellSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID((uint)ItemDatabase.items.Count + 1);
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Equipable","Spell","Projectile", });
                // need to deal with duplicate itemID numbers 

            }
            
        }
#endif
    }
}