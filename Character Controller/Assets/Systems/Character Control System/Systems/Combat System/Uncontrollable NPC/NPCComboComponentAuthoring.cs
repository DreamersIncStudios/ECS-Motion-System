using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading.Tasks;
using System;

namespace DreamersInc.ComboSystem.NPC
{

    public class NPCComboComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public ComboSO Combo;
        public Command command;

        Entity entity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            this.entity = entity;
            dstManager.AddComponentData(entity, command);


        }

        public async void Setup()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            ComboSO temp = Instantiate(Combo);
            temp.UpdateTotalProbability();
            var data = new NPCComboComponent() { combo = temp };
            dstManager.AddComponentData(entity, data);
            dstManager.AddComponentData(entity, command);

        }



    }
    public class NPCComboComponent : IComponentData
    {
        public ComboSO combo;
    }
}