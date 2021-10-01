using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem.Task;

namespace Dreamers.InventorySystem.MissionSystem.SO
{
    public class MissionQuestSO : ScriptableObject, IBase, IPurchasable
    {

        #region SO Variables
        public string Name { get { return missionName; } }
        [SerializeField] string missionName;
        public uint MissionID { get { return ID; } }
        [SerializeField] uint ID;
        public string Objective { get { return objective; } }
        [SerializeField] string objective;
        public int ReqdLevel { get { return reqdLevel; } }
        [SerializeField] int reqdLevel;
        public int GoldReward { get { return goldReward; } }
        [SerializeField] int goldReward;
        public IPurchasable ItemReward { get { return itemReward; } }
        [SerializeField] IPurchasable itemReward;
        public TaskTypes questType { get; private set; }
        public bool IsSideQuest { get { return isSideQuest; } }
        [SerializeField] bool isSideQuest;
        public uint Value { get { return 150; } }
        public uint MaxStackCount { get { return 0; } }
        public bool Stackable { get { return false; } }
        public List<TaskSO> Tasks { get { return tasks; } }
        [SerializeField] List<TaskSO> tasks;
        public List<TaskSO> workingTasks { get; private set; }
        #endregion
        public bool Sequential;
        public int CurrentTask;
  

#if UNITY_EDITOR

        public void setItemID(uint ID)
        {
            if (!QuestDatabase.Missions.TryGetValue(ID, out _))
                this.ID = ID;
            else
            {
                setItemID(ID + 1);
            }
        }
        public void SetSideQuest(bool set)
        {
            isSideQuest = set;
        }


        public void CreateQuest(string name, string Objective, int level, int GoldReward, IPurchasable items)
        {
            this.missionName = name;
            objective = Objective;
            reqdLevel = level;
            this.goldReward = GoldReward;
            itemReward = items;
        }
#endif
        // determine if virtual or abstract
        public void AcceptQuest(MissionHub hub)
        {
            workingTasks = new List<TaskSO>();
            foreach (var task in Tasks)
            {
                workingTasks.Add(Instantiate(task));
            }

            if (IsSideQuest)
            {
                hub.AddMissionSide(this);
            }
            else
            {
                hub.UnlockStoryMisstion(this);
            }
            numberOfCompleteTask = new int();
            missionHub = hub;
        }
        public void CompleteQuest()
        {
        }

        int numberOfCompleteTask;
        bool QuestComplete => numberOfCompleteTask == Tasks.Count;
        MissionHub missionHub;

        public void QuestRequirementsMet()
        {
            numberOfCompleteTask++;
            foreach (var item in Tasks)
            {
                if (item.Complete)
                {
                    missionHub.Deregister(item);
                }
            }
            //TODO Implement UI 

            if (QuestComplete)
            {
                Debug.Log(Name + " has been Completed. Please see ______ to turn in quest");
            }

        }



    }



}
