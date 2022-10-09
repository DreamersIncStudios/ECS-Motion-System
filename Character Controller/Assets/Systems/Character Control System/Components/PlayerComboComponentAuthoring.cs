using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Core.SaveSystems;

namespace DreamersInc.ComboSystem
{
    public class PlayerComboComponentAuthoring : MonoBehaviour, ISave
    {
        public ComboSO Combo;
        public void Load(string jsonData)
        {
            throw new System.NotImplementedException();
        }
        public void SetupDataEntity(Entity entity)
        {
            EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dstManager.SetComponentData(entity, new PlayerComboComponent() {
                Combo = Combo// Instantiate(Combo)
            });
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
        public bool WeaponEquipped;
    }
}