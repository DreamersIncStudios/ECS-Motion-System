using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dreamers.InventorySystem.MissionSystem.Interfaces
{
    public interface IDefeat
    {
        NPC DefeatWhat { get; }
        uint DefeatHowMany { get; }
        uint HowMany { get; }
        bool ResetOnDeath { get; }

    }
    //TODO move
    public enum TaskTypes { Empty, Collect, Defeat, Destroy, Defend}
}