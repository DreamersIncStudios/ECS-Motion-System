using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Core.SaveSystems;

namespace DreamersInc.ComboSystem
{
    public class PlayerComboComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity, ISave
    {
        public ComboSO Combo;
        public Command command;
        Entity entity;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            var data = new PlayerComboComponent() { Combo = Instantiate(Combo) };
            dstManager.AddComponentData(entity, data);
            dstManager.AddComponentData(entity, command);
            this.entity = entity;
   
           
        }

        public void Load(string jsonData)
        {
            throw new System.NotImplementedException();
        }
        public void Setup()
        {
            EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var data = new PlayerComboComponent() { Combo = Instantiate(Combo) };
            dstManager.AddComponentData(entity, command);
            dstManager.AddComponentData(entity, data);
        }

        public void Save()
        {
            SaveSystem.Instance.gameData.GetCharacterSaveData.PlayerCombos = GetSaveData();

        }
        public SaveData GetSaveData()
        {
            return Combo.GetSaveData();
        }
    }
    public class PlayerComboComponent : IComponentData
    {
        public ComboSO Combo;
    }
}