using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamers.Global;

namespace Dreamers.InventorySystem.MissionSystem.SO
{

#if UNITY_EDITOR
    public static class CreateSO
    {
        [MenuItem("Assets/Create/Missions")]
        static public void CreateRecoveryItem()
        {
            ScriptableObjectUtility.CreateAsset<DefeatEnemyMissionSO>("Defeat Enemy Mission", out DefeatEnemyMissionSO Item);
            ItemDatabase.LoadDatabaseForce();
            Item.setItemID((uint)QuestDatabase.Missions.Count + 1);
            Debug.Log(Item.MissionID);
            // need to deal with duplicate itemID numbers 

        }
    }
#endif
}