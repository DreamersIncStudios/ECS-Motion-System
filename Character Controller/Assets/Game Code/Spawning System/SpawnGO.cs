using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Base;
using DreamersInc.CombatSystem;
using DreamersInc.ComboSystem;
using DreamersInc.InflunceMapSystem;
using DreamersStudio.CameraControlSystem;
using MotionSystem;
using Stats;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GameCode.Spawn
{
    public class SpawnGO : IComponentData
    {
        public GameObject SpawnThis;
        public CharacterClass Info;
        public EquipmentSave equipment;
    }


    public partial class SpawnGOSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            foreach (var (pgo, character, combo, entity) in SystemAPI.Query<SpawnGO, BaseCharacterComponent, PlayerComboComponent>().WithEntityAccess())
            {
                GameObject go = GameObject.Instantiate(pgo.SpawnThis);
                CameraControl.Instance.Follow.LookAt = go.GetComponentInChildren<LookHereTarget>().transform;
                CameraControl.Instance.Follow.Follow = go.transform;
                CameraControl.Instance.Target.Follow = go.transform;

               // var Capsule = go.GetComponent<CapsuleCollider>();
                var anim = go.GetComponent<Animator>();
                var RB = go.GetComponent<Rigidbody>();
                go.tag = "Player";
                character.GOrepresentative = go;
                character.SetupDataEntity(pgo.Info);
                CharacterInventory inventory = new ();
                inventory.Setup(entity, pgo.equipment, character);
                ecb.AddComponent(entity, inventory);
          
                ecb.AddComponent(entity, new TransformGO() { transform = go.transform });

           
                go.GetComponent<VFXControl>().Init(combo.Combo);
                ecb.SetComponent(entity, new InfluenceComponent
                {
                    factionID = 0,
                    Protection = 10,
                    Threat = 10
                }); 

                ecb.RemoveComponent<SpawnGO>(entity);
                ecb.AddComponent<StoreWeapon>(entity);

                combo.WeaponEquipped= true;
            }

        }

      
    }
}