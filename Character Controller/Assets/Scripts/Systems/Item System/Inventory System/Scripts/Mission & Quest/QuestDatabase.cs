using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.MissionSystem.Task;
using Dreamers.InventorySystem.MissionSystem.SO;

namespace Dreamers.InventorySystem.MissionSystem
{
    public static class QuestDatabase
    {
        public static Dictionary<uint, MissionQuestSO> Missions;
        static public bool isLoaded { get; private set; }
        private static bool ValidateDatabase()
        {
            if (Missions == null || !isLoaded)
            {
                Missions = new Dictionary<uint, MissionQuestSO>();
                return false;
            }
            else
            {
                isLoaded = true;
                return true;
            }
        }
        public static void LoadDatabase()
        {
            if (!isLoaded)
            {
                LoadDatabaseForced();
            }
        }

        public static void LoadDatabaseForced()
        {
            Missions = new Dictionary<uint, MissionQuestSO>();
            isLoaded = true;
            MissionQuestSO[] missionsToLoad = Resources.LoadAll<MissionQuestSO>(@"Missions");
            foreach (var item in missionsToLoad)
            {
                if (!Missions.ContainsKey(item.MissionID))
                {
                    Missions.Add(item.MissionID, item);
                }
                else
                {
                    Debug.LogWarning("Duplicated Quest ID Number. Quest named " + item.Name + " has not been added to the database. This mission is not attainable in build.");

                }
            }
        }

        public static void ClearDatabase()
        {
            isLoaded = false;
            Missions.Clear();
        }

        public static bool GetQuest(uint MissionID, out MissionQuestSO Quest)
        {
            if (!ValidateDatabase())
                LoadDatabase();
            return Missions.TryGetValue(MissionID, out Quest);
        }
        public static MissionQuestSO GetQuest(uint missionID) {
            LoadDatabaseForced();
            Missions.TryGetValue(missionID, out MissionQuestSO Quest);
            return Quest;
        }
    }

    public static class TaskDatabase
    {
        public static Dictionary<uint, TaskSO> Tasks;
        static public bool isLoaded { get; private set; }
        private static bool ValidateDatabase()
        {
            if (Tasks == null || !isLoaded)
            {
                Tasks = new Dictionary<uint, TaskSO>();
                return false;
            }
            else
            {
                isLoaded = true;
                return true;
            }
        }
        public static void LoadDatabase()
        {
            if (!isLoaded)
            {
                LoadDatabaseForced();
            }
        }

        public static void LoadDatabaseForced()
        {
            Tasks = new Dictionary<uint, TaskSO>();
            isLoaded = true;
            TaskSO[] missionsToLoad = Resources.LoadAll<TaskSO>(@"Tasks");
            foreach (var item in missionsToLoad)
            {
                if (!Tasks.ContainsKey(item.TaskID))
                {
                    Tasks.Add(item.TaskID, item);
                }
                else
                {
                    Debug.LogWarning("Duplicated Quest ID Number. Quest named " + item.Name + " has not been added to the database. This mission is not attainable in build.");

                }
            }
        }

        public static void ClearDatabase()
        {
            isLoaded = false;
            Tasks.Clear();
        }

        public static bool GetQuest(uint TaskID, out TaskSO task)
        {
            if (!ValidateDatabase())
                LoadDatabase();
            return Tasks.TryGetValue(TaskID, out task);
        }
        public static TaskSO GetQuest(uint taskID)
        {
            LoadDatabaseForced();
            Tasks.TryGetValue(taskID, out TaskSO Task);
            return Task;
        }


    }
    
}