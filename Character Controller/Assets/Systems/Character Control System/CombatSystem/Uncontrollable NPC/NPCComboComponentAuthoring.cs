using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using System.Threading.Tasks;

namespace DreamersInc.ComboSystem.NPC
{
    [RequireComponent(typeof(Animator))]
    public class NPCComboComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public ComboSO Combo;
        Entity entity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            ComboSO temp = Instantiate(Combo);
            temp.UpdateTotalProbability();
            var data = new NPCComboComponent() {  combo = temp};
            dstManager.AddComponentData(entity, data);
            this.entity = entity;

        }

        public async void Setup()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            ComboSO temp = ScriptableObject.Instantiate(Combo);
            temp.UpdateTotalProbability();
            var data = new NPCComboComponent() { combo = Instantiate(Combo) };
            dstManager.AddComponentData(entity, data);
        }



    }
    public class NPCComboComponent : IComponentData {
        public ComboSO combo;
       // public Animator animator;
    }
}