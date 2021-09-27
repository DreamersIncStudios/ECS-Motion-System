using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.MissionSystem.Interfaces;

namespace Dreamers.InventorySystem.MissionSystem.SO
{
    public abstract class MissionQuestSO : ScriptableObject, IBase, IPurchasable
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

        public uint Value { get { return 0; } }
        public uint MaxStackCount { get { return 0; } }
        public bool Stackable { get { return false; } }

        #endregion

#if UNITY_EDITOR

        public void setItemID(uint ID)
        {

            this.ID = ID;
        }
#endif

        // determine if virtual or abstract
        public virtual void AcceptQuest() { 
      
        }
        public virtual void CompleteQuest() { }
        public virtual void QuestRequirementsMet() { }

    }

}
