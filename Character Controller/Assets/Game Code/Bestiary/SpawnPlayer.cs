using AISenses;
using AISenses.VisionSystems.Combat;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Base;
using DreamersInc.CombatSystem;
using DreamersInc.ComboSystem;
using DreamersInc.InflunceMapSystem;
using DreamersStudio.CameraControlSystem;
using MotionSystem;
using MotionSystem.Components;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.BestiarySystem
{
    public sealed partial class BestiaryDB : MonoBehaviour
    {
        public static bool SpawnPlayer(uint ID, out GameObject go,out  Entity entity) {
            var info = GetPlayer(ID);
            if (info != null)
            {
                go = Instantiate(info.Prefab);
                go.tag = "Player";
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                entity = CreateEntity(manager, info.Name);
              //  AddPhysics(manager, entity, go, PhysicsShape.Capsule, info.PhysicsInfo);
                BaseCharacterComponent character = new();
                character.GOrepresentative = go;
                character.SetupDataEntity(info.stats);
                TransformGO transformLink = new()
                {
                    transform = go.transform
                };
                manager.AddComponentData(entity, transformLink);
                manager.AddComponentObject(entity, character);

                CharacterInventory inventory = new();
                inventory.Setup(info.Equipment, character);
                manager.AddComponentData(entity, inventory);
                var anim = go.GetComponent<Animator>();
                var RB = go.GetComponent<Rigidbody>();

                manager.AddComponentData(entity, new AnimatorComponent()
                {
                    anim = anim,
                    RB = RB,
                    transform = anim.transform,
                });
                manager.AddComponent<StoreWeapon>(entity);

            }
            else
            {
                go = null;
                entity = Entity.Null;
            }
            return info != null;
        }


        public static bool SpawnPlayer(uint ID, EquipmentSave equipment=null) {


            if (SpawnPlayer(ID, out GameObject go, out Entity entity))
            {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var info = GetPlayer(ID);
                manager.AddComponent<PlayerTag>(entity);
                manager.AddComponent<Player_Control>(entity);

                manager.AddComponent<AttackTarget>(entity);
                manager.AddComponentObject(entity,new Command());
                var controllerData = new CharControllerE();
                controllerData.Setup(info.Move,go.GetComponent<UnityEngine.CapsuleCollider>());
                manager.AddComponentData(entity, controllerData);
                var comboInfo = Object.Instantiate(info.Combo);
                manager.AddComponentObject(entity, new PlayerComboComponent { Combo = comboInfo });
                manager.AddComponentData(entity, new InfluenceComponent
                {
                    factionID = info.factionID,
                    Protection = info.BaseProtection,
                    Threat = info.BaseThreat
                });
                manager.AddComponentData(entity, new Perceptibility
                {
                    movement = MovementStates.Standing_Still,
                    noiseState = NoiseState.Normal,
                    visibilityStates = VisibilityStates.Visible
                });
                manager.AddBuffer<ScanPositionBuffer>(entity);

                go.GetComponent<VFXControl>().Init(info.Combo);

                CameraControl.Instance.Follow.LookAt = go.GetComponentInChildren<LookHereTarget>().transform;
                CameraControl.Instance.Follow.Follow = go.transform;
                CameraControl.Instance.Target.Follow = go.transform;

                CameraControl.Instance.TargetGroup.m_Targets[1].target = go.transform;

                return true;
            }
            else
                return false;
        }
    }
}
