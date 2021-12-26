using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using IAUS.ECS.Component;

namespace DreamersInc.ComboSystem.NPC
{
    public class AttackSetup : ComponentSystem
    {
        protected override void OnUpdate() {
            Entities.ForEach((ref AttackActionTag tag, NPCComboComponent Combo) => { });

        }
    }
}