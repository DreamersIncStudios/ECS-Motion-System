using AISenses;
using AISenses.VisionSystems.Combat;
using Components.MovementSystem;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Base;
using DreamersInc.CombatSystem;
using DreamersInc.ComboSystem;
using DreamersInc.InflunceMapSystem;
using Global.Component;
using MotionSystem;
using MotionSystem.Components;
using Stats;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace DreamersInc.BestiarySystem
{
    public sealed partial class BestiaryDB : MonoBehaviour
    {
        public static bool SpawnNPC(uint ID, out GameObject go, out Entity entity)
        {
            var info = GetCreature(ID);
            if (info != null)
            {
                go = Instantiate(info.Prefab);
                go.tag = "Enemy NPC";
                go.layer = 9;
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                entity = CreateEntity(manager, info.Name + " NPC");
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
                manager.AddComponentData(entity, transformLink);
                manager.AddComponentObject(entity, character);

                CharacterInventory inventory = new();
                inventory.Setup(entity, info.Equipment, character);
                manager.AddComponentData(entity, inventory);
                if (go.TryGetComponent<Rigidbody>(out var RB))
                    manager.AddComponentObject(entity, RB);
                manager.AddComponentData(entity, new AnimatorComponent()
                {
                    anim = go.TryGetComponent<Animator>(out var anim) ? anim : null,
                    transform = go.GetComponent<Transform>()
                });

                //  manager.AddComponent<StoreWeapon>(entity);
                manager.AddComponentData(entity, new InfluenceComponent
                {
                    factionID = info.factionID,
                    Protection = info.BaseProtection,
                    Threat = info.BaseThreat
                });
                manager.AddComponentData(entity, new AITarget()
                {
                    FactionID = info.factionID,
                    NumOfEntityTargetingMe = 3,
                    CanBeTargetByPlayer = true,
                    Type = TargetType.Character,
                    CenterOffset = info.CenterOffset
                });
                if (go.TryGetComponent<OnDestoryEvents>(out var death))
                    death.SetExp(info.ExpGiven);
            }
            else
            {
                go = null;
                entity = Entity.Null;
            }
            return info != null;

        }


        public static bool SpawnNPC(uint ID, EquipmentSave equipment = null)
        {
            if (SpawnNPC(ID, out GameObject go, out Entity entity))
            {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var info = GetCreature(ID);
                //
                if (info.GetNPCType == NPCType.Combatant)
                {
                    var comboInfo = Object.Instantiate(info.Combo);
                    manager.AddComponentObject(entity, new PlayerComboComponent { Combo = comboInfo });
                }

                switch (info.creatureType)
                {
                    case CreatureType.biped:
                        var controllerData = new CharControllerE();
                        controllerData.Setup(info.Move, go.GetComponent<UnityEngine.CapsuleCollider>());
                        controllerData.AI = true;
                        controllerData.Walk = true;
                        manager.AddComponentData(entity, controllerData);

                        manager.AddComponent<AttackTarget>(entity);
                        manager.AddComponentObject(entity, new Command());
                        break;
                    case CreatureType.quadruped:
                        var beastControllerData = new BeastControllerComponent();
                        beastControllerData.Setup(info.Move, go.GetComponent<UnityEngine.CapsuleCollider>());
                        break;
                }
                if (info.creatureType != CreatureType.stationary)
                {
                    var agent = go.GetComponent<NavMeshAgent>();
                    manager.AddComponentObject(entity, agent);
                    var move = new Movement()
                    {
                        Acceleration = agent.acceleration,
                        MovementSpeed = agent.speed,
                        StoppingDistance = agent.stoppingDistance,
                        Offset = agent.baseOffset,
                    };
                    manager.AddComponentData(entity, move);
                }

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

                //   go.GetComponent<VFXControl>().Init(info.Combo);


                return true;
            }
            else
                return false;
        }

        public static bool SpawnNPC(uint ID, out GameObject go, EquipmentSave equipment = null)
        {
            if (SpawnNPC(ID, out go, out Entity entity))
            {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var info = GetCreature(ID);
                switch (info.creatureType)
                {
                    case CreatureType.biped:
                        var controllerData = new CharControllerE();
                        controllerData.Setup(info.Move, go.GetComponent<UnityEngine.CapsuleCollider>());
                        controllerData.AI = true;
                        controllerData.Walk = true;
                        manager.AddComponentData(entity, controllerData);

                        manager.AddComponent<AttackTarget>(entity);
                        manager.AddComponentObject(entity, new Command());
                        break;
                    case CreatureType.quadruped:
                        var beastControllerData = new BeastControllerComponent();
                        beastControllerData.Setup(info.Move, go.GetComponent<UnityEngine.CapsuleCollider>());
                        beastControllerData.AI = true;
                        beastControllerData.Walk = true;

                        manager.AddComponentData(entity, beastControllerData);
                        manager.AddComponent<AttackTarget>(entity);
                        manager.AddComponentObject(entity, new Command());

                        break;
                }
                if (info.creatureType != CreatureType.stationary) {
                    if (go.TryGetComponent<NavMeshAgent>(out var agent))
                    {
                        manager.AddComponentObject(entity, agent);
                        var move = new Movement()
                        {
                            Acceleration = agent.acceleration,
                            MovementSpeed = agent.speed,
                            StoppingDistance = agent.stoppingDistance,
                            Offset = agent.baseOffset,
                        };
                        manager.AddComponentData(entity, move);
                    }
                }


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

                //  go.GetComponent<VFXControl>().Init(info.Combo);

                return true;
            }
            else
                return false;
        }


        public static bool SpawnNPC(uint ID, Vector3 Position, EquipmentSave equipment = null)
        {
            if (SpawnNPC(ID, out GameObject go, equipment))
            {
                go.transform.position = Position;
                return true;
            }
            else { return false; }
        }

    }
}