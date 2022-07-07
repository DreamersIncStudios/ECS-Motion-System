using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace DreamerInc.CombatSystem
{
    [GenerateAuthoringComponent]
    public class UIPrefab_Entities : IComponentData
    {
        public GameObject uiPrefab;
    }

}