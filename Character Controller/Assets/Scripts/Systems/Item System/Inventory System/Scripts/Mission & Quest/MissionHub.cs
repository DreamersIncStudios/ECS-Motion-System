using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem.MissionSystem.SO;

namespace Dreamers.InventorySystem.MissionSystem {
    public class MissionHub
    {
        public MissionQuestSO SelectedMission;
        List<MissionQuestSO> OpenStoryMissions;
        List<MissionQuestSO> SideQuests;
    }
}