using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dreamers.InventorySystem.MissionSystem.Interfaces
{
    public interface IDefeat
    {
        GameObject DefeatWhat { get; }
        uint DefeatHowMany { get; }

    }
}