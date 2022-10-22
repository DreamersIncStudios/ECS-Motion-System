using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Core.SaveSystems;
using DreamerInc.CombatSystem;

namespace DreamersInc.ComboSystem
{
    public class PlayerComboComponentAuthoring : MonoBehaviour, ISave
    {
        public ComboSO Combo;
        ComboSO comboInstance;
        Animator anim;
  
        public void SetupDataEntity(Entity entity)
        {
            comboInstance = Instantiate(Combo);
            EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dstManager.SetComponentData(entity, new PlayerComboComponent() {
                Combo = comboInstance
            });
        }
        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        bool vfxActive; 
        void Update() {

        }

        public void TriggerVFX() {
            var state = anim.GetCurrentAnimatorStateInfo(0);
            var vfx = comboInstance.GetVFX(state);
            vfx.SpawnVFX(this.transform);
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
        public ComboSO Combo;
        public bool WeaponEquipped { get; set; }
    }
}