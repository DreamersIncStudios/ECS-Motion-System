using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Dreamers.InventorySystem.MissionSystem.SO;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem.Task;
namespace Dreamers.InventorySystem.MissionSystem
{

    [System.Serializable]
    public class MissionHub
    {
        public MissionQuestSO SelectedMission;
        List<MissionQuestSO> OpenStoryMissions; //TODO Change to dictonary 
        Dictionary<uint, MissionQuestSO> SideQuests;
        UnityEvent<NPC> onKillEvent;
        UnityEvent<ItemBaseSO> onCollectEvent;
        UnityEvent OnDeathEvent;
        public MissionHub(MissionQuestSO CurrentStoryMission = default, List<MissionQuestSO> OpenStoryMissions = default, List<MissionQuestSO> OpenSideQuest = default)
        {
            onKillEvent = new UnityEvent<NPC>();
            onCollectEvent = new UnityEvent<ItemBaseSO>();
            OnDeathEvent = new UnityEvent();

            SelectedMission = CurrentStoryMission;
            this.OpenStoryMissions = new List<MissionQuestSO>();
            this.OpenStoryMissions = OpenStoryMissions;
            SideQuests = new Dictionary<uint, MissionQuestSO>();
            if (OpenSideQuest.Count > 0)
            {
                foreach (var item in OpenSideQuest)
                {
                    SideQuests.Add(item.MissionID, item);
                    item.AcceptQuest(this);
                }
            }
        }
        public bool AddMissionSide(MissionQuestSO SO)
        {
            if (!SideQuests.ContainsKey(SO.MissionID))
            {
                SideQuests.Add(SO.MissionID, SO);
                Register(SO);
                return true;
            }
            else
                return false;
        }
        public void UnlockStoryMisstion(MissionQuestSO SO)
        {
            //TODO Check completed Story mission list if not completed already add 

            //            OpenStoryMissions.Add(SO);
        }

        public void Register(MissionQuestSO SO)
        {
            foreach (var item in SO.workingTasks)
            {
                switch (item.TaskType)
                {
                    case Interfaces.TaskTypes.Defeat:
                        DefeatEnemyTaskSO defeatEnemy = (DefeatEnemyTaskSO)item;
                        defeatEnemy.ResetCount();
                        onKillEvent.AddListener(defeatEnemy.IncrementCounter);
                        if (defeatEnemy.ResetOnDeath)
                            OnDeathEvent.AddListener(defeatEnemy.ResetCount);

                        break;

                }
            }
        }
        public void Deregister(TaskSO tasks)
        {
            switch (tasks.TaskType)
            {
                case Interfaces.TaskTypes.Defeat:
                    DefeatEnemyTaskSO defeatEnemy = (DefeatEnemyTaskSO)tasks;
                    onKillEvent.RemoveListener(defeatEnemy.IncrementCounter);
                    if (defeatEnemy.ResetOnDeath)
                        OnDeathEvent.RemoveListener(defeatEnemy.ResetCount);

                    break;

            }

        }
        public void OnCollect(ItemBaseSO item)
        {
            onCollectEvent.Invoke(item);
        }


        public void OnKill(NPC npc)
        {
            Debug.Log("kill called");
            onKillEvent.Invoke(npc);
        }
        public void OnDeath() { }
    }
    //Todo change to SO Type of something later;
    public enum NPC { test1, test2, test3, test4, test5 }

}