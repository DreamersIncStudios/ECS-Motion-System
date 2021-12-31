using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Core.SaveSystems;

namespace DreamersInc.ComboSystem
{
    [RequireComponent(typeof(Animator))]
    public class PlayerComboComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity, ISave
    {
        public ComboSO Combo;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new PlayerComboComponent() { animator = GetComponent<Animator>(), combo = Instantiate(Combo) };
            dstManager.AddComponentData(entity, data);

        }

        public void Load(string jsonData)
        {
            throw new System.NotImplementedException();
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
        public ComboSO combo;
        public Animator animator;
    }
}