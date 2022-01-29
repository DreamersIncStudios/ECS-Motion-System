using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem.MissionSystem.Interfaces
{
    public interface IGivenQuest
    {
        int CostToTake { get; }
        bool Repeatable { get; }

    }
}
