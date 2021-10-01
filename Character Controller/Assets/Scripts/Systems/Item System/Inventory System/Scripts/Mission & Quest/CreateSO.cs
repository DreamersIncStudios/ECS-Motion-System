using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamers.Global;
using Dreamers.InventorySystem.MissionSystem.Task;

#if UNITY_EDITOR

namespace Dreamers.InventorySystem.MissionSystem.SO
{

    public static class CreateSO
    {
        [MenuItem("Assets/Create/Missions")]
        static public void CreateQuest()
        {
            ScriptableObjectUtility.CreateAsset<MissionQuestSO>("Quest", out MissionQuestSO Item);
            QuestDatabase.LoadDatabaseForced();
            Item.setItemID((uint)QuestDatabase.Missions.Count);
        }

        [MenuItem("Assets/Create/TaskDefeat")]
        static public void CreateTask() {
            ScriptableObjectUtility.CreateAsset<DefeatEnemyTaskSO>("Quest", out DefeatEnemyTaskSO item);
            TaskDatabase.LoadDatabaseForced();
            item.setItemIDandType((uint)TaskDatabase.Tasks.Count, Interfaces.TaskTypes.Defeat);
        }

    }
}

#endif
