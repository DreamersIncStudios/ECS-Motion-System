using Unity.Entities;
using UnityEngine;
namespace Global.Component
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct AITarget : IComponentData
    {
        public TargetType Type;
        public Race GetRace;
        public int NumOfEntityTargetingMe;
        public bool CanBeTargeted => NumOfEntityTargetingMe < 2;
        [HideInInspector] public int MaxNumberOfTarget; // base off of Threat Level
        public bool CanBeTargetByPlayer;
        public bool IsFriend(Race race) {
            bool test = new bool();
            switch (race) {
                case Race.Angel:
                    switch (GetRace) {
                        case Race.Angel:
                        case Race.Human:
                            test = true;
                            break;
                        case Race.Daemon:
                            test = false;
                            break;
                    }
                    break;
            }
            
            return test; }


    }
    [System.Serializable]
    public enum TargetType
    {
        None, Character, Location, Vehicle
    }

    public enum Race
    {
        Angel, Daemon, Human // More Types of be added 

    }
}