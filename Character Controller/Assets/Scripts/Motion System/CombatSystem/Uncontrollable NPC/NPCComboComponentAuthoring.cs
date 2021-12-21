using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace DreamersInc.ComboSystem.NPC
{
    [RequireComponent(typeof(Animator))]
    public class NPCComboComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public NPCCombos Combo;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new NPCComboComponent() { animator = GetComponent<Animator>(), combo = Instantiate(Combo) };
            dstManager.AddComponentData(entity, data);
        }

    }
    public class NPCComboComponent : IComponentData {
        public NPCCombos combo;
        public Animator animator;
    }
}