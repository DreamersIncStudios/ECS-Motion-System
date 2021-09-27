using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.MissionSystem.Interfaces;

namespace Dreamers.InventorySystem.MissionSystem.SO
{
    public class DefeatEnemyMissionSO : MissionQuestSO, IDefeat
    {
        public GameObject DefeatWhat => throw new System.NotImplementedException();

        public uint DefeatHowMany => throw new System.NotImplementedException();
    }
}