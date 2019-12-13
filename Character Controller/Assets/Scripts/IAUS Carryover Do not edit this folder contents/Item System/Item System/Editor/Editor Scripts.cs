using UnityEngine;
using UnityEditor;
using ScriptableObjectEditor;


namespace ItemSystem
{

    static public class ItemUtility
    {
        [MenuItem("Assets/Create/RPG/Item")]

        static public void CreateItem()
        {
            BaseItem test;
            ScriptableObjectUtility.CreateAsset<BaseItem>(out test);
            ItemDatabase.LoadDatabaseForce();
            test.ItemID = ItemDatabase._items.Count+1;
         
        }
        [MenuItem("Assets/Create/RPG/Armor")]
        static public void CreateArmor()
        {
           Armor test;
            ScriptableObjectUtility.CreateAsset<Armor>( out test);
            ItemDatabase.LoadDatabaseForce();
            test.ItemID = ItemDatabase._items.Count + 1;
        }
        [MenuItem("Assets/Create/RPG/Weapon")]
        static public void CreateWeapon()
        {
            Weapon test;
            ScriptableObjectUtility.CreateAsset<Weapon>(out test);
            ItemDatabase.LoadDatabaseForce();
            test.ItemID = ItemDatabase._items.Count + 1;
        }

    }



}
