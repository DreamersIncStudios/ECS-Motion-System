using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.ComboSystem;
using DreamersInc.ComboSystem.NPC;

namespace IAUS.ECS.Component
{
    [GenerateAuthoringComponent]
    public struct NPCAttackBuffer : IBufferElementData
    {
        public AnimationTrigger Trigger;
    
    }


    public struct AttackActionTag : IComponentData
    {

    }

}