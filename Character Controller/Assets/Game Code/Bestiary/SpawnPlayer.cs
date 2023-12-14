using AISenses;
using AISenses.VisionSystems.Combat;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Base;
using DreamersInc.CombatSystem;
using DreamersInc.ComboSystem;
using DreamersInc.InflunceMapSystem;
using DreamersStudio.CameraControlSystem;
using Global.Component;
using MotionSystem;
using MotionSystem.Components;
using MotionSystem.Systems;
using Stats;
using Stats.Entities;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DreamersInc.BestiarySystem
{
    public sealed partial class BestiaryDB : MonoBehaviour
    {
        public static bool SpawnPlayer(uint ID, out GameObject go, out Entity entity, bool IsPlayer = false)
        {
            var info = GetPlayer(ID);
            if (info != null)
            {
                go = Instantiate(info.Prefab);
                if (IsPlayer)
                    go.tag = "Player";
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                entity = CreateEntity(manager, info.Name);
                AddPhysics(manager, entity, go, info.PhysicsInfo);
                BaseCharacterComponent character = new();
                character.GOrepresentative = go;
                character.SetupDataEntity(info.stats);
                TransformGO transformLink = new()
                {
                    transform = go.transform
                };
                go.GetComponent<Damageable>().SetData(entity, character);
                var vision = new Vision();
                vision.InitializeSense(character);
                manager.AddComponentData(entity, vision);
                manager.AddComponentData(entity, new SurroundCharacter());
                manager.AddComponentData(entity, new createTag());

                manager.AddComponentData(entity, transformLink);
                manager.AddComponentObject(entity, character);

                CharacterInventory inventory = new();

                manager.AddComponentData(entity, inventory);
                manager.GetComponentData<CharacterInventory>(entity).Setup(entity, info.Equipment, character);
                var anim = go.GetComponent<Animator>();
                var RB = go.GetComponent<Rigidbody>();
                manager.AddComponentObject(entity, RB);

                manager.AddComponentData(entity, new AITarget()
                {
                    FactionID = info.factionID,
                    NumOfEntityTargetingMe = 3,
                    CanBeTargetByPlayer = !IsPlayer,
                    Type = TargetType.Character,
                    CenterOffset = new float3(0, 1, 0) //todo add value to SO

                });
                manager.AddComponentObject(entity, anim);
                manager.AddComponentObject(entity, go.transform);

             
                manager.AddComponent<StoreWeapon>(entity);
                AnimationSpeedLink link = new AnimationSpeedLink()
                {
                    link = go.GetComponent<AnimationSpeed>()
                };
                manager.AddComponentObject(entity, link);
            }
            else
            {
                go = null;
                entity = Entity.Null;
            }
            return info != null;
        }


        public static bool SpawnPlayer(uint ID, EquipmentSave equipment = null, bool IsPlayer = false)
        {


            if (SpawnPlayer(ID, out GameObject go, out Entity entity, IsPlayer))
            {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var info = GetPlayer(ID);
                if (IsPlayer)
                {
                    manager.AddComponent<Player_Control>(entity);
                }
                else
                {
                    manager.AddComponent<AI_Control>(entity);
                }
                manager.AddComponent<AttackTarget>(entity);
                manager.AddComponentObject(entity, new Command()
                {
                    EquippedAbilities = new Dreamers.InventorySystem.AbilitySystem.AbilityList(),
                    InputTimeReset = 500.0f,
                    InputTimer = 500.0f
                });
                var controllerData = new CharControllerE();
                controllerData.Setup(info.Move, go.GetComponent<UnityEngine.CapsuleCollider>());
                manager.AddComponentData(entity, controllerData);
                var comboInfo = Object.Instantiate(info.Combo);
                manager.AddComponentObject(entity, go.GetComponentInChildren<SkinnedMeshRenderer>());
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

        public static bool SpawnPlayer(uint ID, out GameObject go, EquipmentSave equipment = null, bool IsPlayer = false)
        {


            if (SpawnPlayer(ID, out go, out Entity entity, IsPlayer))
            {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var info = GetPlayer(ID);
                manager.AddComponent<Player_Control>(entity);

                manager.AddComponent<AttackTarget>(entity);
                manager.AddComponentObject(entity, new Command()
                {
                    EquippedAbilities = new Dreamers.InventorySystem.AbilitySystem.AbilityList(),
                    InputTimeReset = 500.0f,
                    InputTimer = 500.0f
                });
                var controllerData = new CharControllerE();
                controllerData.Setup(info.Move, go.GetComponent<UnityEngine.CapsuleCollider>());
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


        public static bool SpawnPlayer(uint ID, Vector3 Position, EquipmentSave equipment = null, bool IsPlayer = false)
        {
            if (SpawnPlayer(ID, out GameObject go, equipment, IsPlayer))
            {
                go.transform.position = Position;
                return true;
            }
            else { return false; }
        }

        public static bool SpawnPlayer(uint ID, bool IsPlayer)
        {
            if (SpawnPlayer(ID, out GameObject go, null, IsPlayer))
            {
                return true;
            }
            else { return false; }
        }

        public static bool SpawnPlayer(uint ID, Vector3 Position, bool IsPlayer)
        {
            if (SpawnPlayer(ID, out GameObject go, null, IsPlayer))
            {
                go.transform.position = Position;
                return true;
            }
            else { return false; }
        }

    }


}