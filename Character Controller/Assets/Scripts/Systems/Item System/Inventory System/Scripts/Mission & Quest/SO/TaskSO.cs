using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.MissionSystem.Interfaces;// Remove later;
namespace Dreamers.InventorySystem.MissionSystem.Task
{
    public abstract class TaskSO : ScriptableObject, ITask
    {
        public string Name { get { return _name; } }
        [SerializeField] string _name;
        public TaskTypes TaskType { get { return tasktype; } }
       [SerializeField] private TaskTypes tasktype;
        public bool Complete { get { return complete; } }
        [SerializeField] bool complete;
        //TODO show IDNumber
        [SerializeField] public uint TaskID { get; private set; }
  
        public void QuestRequirementsMet() {
            //TODO Implement UI Change
            Debug.Log("Task Complete");

        }

#if UNITY_EDITOR


        public void setItemIDandType(uint ID, TaskTypes type)
        {
            if (!TaskDatabase.Tasks.TryGetValue(ID, out _))
            {
                this.TaskID = ID;
                tasktype = type;
            }
            else
            {
                setItemIDandType(ID + 1, type);
            }
        }
    
#endif
    }

    public interface ITask {
        string Name { get; }
        TaskTypes TaskType{get;}
        uint TaskID { get; }
        bool Complete { get; }
    
        void QuestRequirementsMet();
    }
}