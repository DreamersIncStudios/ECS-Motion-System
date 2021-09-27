using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.MissionSystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem.SO;

namespace Dreamers.InventorySystem.MissionSystem
{
    public static class QuestDatabase
    {
        public static Dictionary<uint, MissionQuestSO> Missions;
        static public bool isLoaded { get; private set; }
        private static bool ValidateDatabase() {
            if (Missions == null || !isLoaded)
            {
                Missions = new Dictionary<uint, MissionQuestSO>();
                return false;
            }
            else {
                isLoaded = true;
                return true;
            }
        }
        public static void LoadDatabase() {
            if (!isLoaded) {
                LoadDatabaseForced();
            }
        }

        static void LoadDatabaseForced() {
            Missions = new Dictionary<uint, MissionQuestSO> ();
            isLoaded = true;
            MissionQuestSO[] missionsToLoad = Resources.LoadAll<MissionQuestSO>(@"Missions");
            foreach (var item in missionsToLoad)
            {
                if (!Missions.ContainsKey(item.MissionID))
                {
                    Missions.Add(item.MissionID, item);
                }
                else {
                    Debug.LogWarning("Duplicated Quest ID Number. Quest named " + item.Name + " has not been added to the database. This mission is not attainable in build.");

                }
            }
        }

        public static void ClearDatabase() {
            isLoaded = false;
            Missions.Clear();
        }

        public static bool GetQuest(uint MissionID, out MissionQuestSO Quest) {
            if (!ValidateDatabase())
                LoadDatabase();
            return Missions.TryGetValue(MissionID, out Quest);
        }

    }
}