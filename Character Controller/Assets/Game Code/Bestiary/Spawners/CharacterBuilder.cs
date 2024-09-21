using System;
using AISenses;
using AISenses.VisionSystems.Combat;
using Components.MovementSystem;
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
using ProjectDawn.Navigation;
using Stats;
using Stats.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using CapsuleCollider = UnityEngine.CapsuleCollider;
using MeshCollider = UnityEngine.MeshCollider;
using Object = UnityEngine.Object;
using SphereCollider = UnityEngine.SphereCollider;
namespace DreamersInc.BestiarySystem
{
    public class CharacterBuilder
    {
        private GameObject model;
        private readonly Entity entity;
        private BaseCharacterComponent character;
        private int factionID;
        private uint classLevel;
        private string tag;
        private ComboSO combo;
        private EntityManager manager;


        public CharacterBuilder(string entityName)
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var baseEntityArch = manager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld)
            );
            var baseDataEntity = manager.CreateEntity(baseEntityArch);
            manager.SetName(baseDataEntity, entityName != string.Empty ? entityName : "NPC Data");
            manager.SetComponentData(baseDataEntity, new LocalTransform() { Scale = 1 });
            entity = baseDataEntity;

        }

        public CharacterBuilder WithModel(GameObject go, Vector3 position, string tagging, out GameObject spawned)
        {
            spawned = model = Object.Instantiate(go);
            model.transform.position = position;
            tag = model.tag = tagging;
            if (entity == Entity.Null) return this;
            manager.SetComponentData(entity, new LocalTransform()
            {
                Position = position,
                Rotation = model.transform.rotation,
                Scale = 1
            });
            return this;
        }

        public CharacterBuilder WithModel(GameObject go, Vector3 position, string tagging)
        {
            return WithModel(go, position, tagging, out _);
        }
        public CharacterBuilder WithAnimation()
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            var anim = model.GetComponent<Animator>();
            manager.AddComponentObject(entity, anim);

            manager.AddComponentObject(entity, model.transform);
            if(!model.TryGetComponent<AnimationSpeed>(out var add)){
                add = model.AddComponent<AnimationSpeed>();
            }
            var link = new AnimationSpeedLink()
            {
                Link = add
            };
            TransformGO transformLink = new()
            {
                transform = model.transform
            };
            manager.AddComponentData(entity, transformLink);
            manager.AddComponentObject(entity, link);
            return this;
        }

        public CharacterBuilder WithAIControl()
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            manager.AddComponentData(entity, Agent.Default);
            manager.AddComponentData(entity, AgentBody.Default);
            manager.AddComponentData(entity, AgentLocomotion.Default);
            manager.AddComponentData(entity, new AgentShape()
            {
                Radius = 2,
                Height = 4,
                Type = ShapeType.Cylinder
            });
            manager.AddComponentData(entity, AgentCollider.Default);
            manager.AddComponentData(entity, AgentSonarAvoid.Default);
            manager.AddComponentData(entity, AgentSeparation.Default);
            manager.AddComponentData(entity, new GiveUpStopTimer());

            manager.GetComponentData<AgentBody>(entity).SetDestination(float3.zero);
            var move = new Movement()
            {

            };
            manager.AddComponentData(entity, move);
            manager.AddComponentData(entity, new AI_Control());
            return this;
        }

        public CharacterBuilder WithEntityPhysics(PhysicsInfo physicsInfo)
        {
            if (entity == Entity.Null)
                return this;

            if (model == null)
                return this;
            if (!model.TryGetComponent<UnityEngine.Collider>(out var col))
                return this;
            var spCollider = new BlobAssetReference<Unity.Physics.Collider>();
            switch (col)
            {
                case CapsuleCollider capsule:
                    spCollider = Unity.Physics.CapsuleCollider.Create(new CapsuleGeometry()
                    {
                        Radius = capsule.radius,
                        Vertex0 = capsule.center + new Vector3(0, capsule.height, 0),
                        Vertex1 = new float3(0, 0, 0)

                    }, new CollisionFilter()
                    {
                        BelongsTo = physicsInfo.BelongsTo.Value,
                        CollidesWith = physicsInfo.CollidesWith.Value,
                        GroupIndex = 0
                    });
                    break;
                case BoxCollider box:
                    if (box != null)
                        spCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry()
                        {
                            Center = box.center,
                            Size = box.size,
                            Orientation = quaternion.identity,
                        }, new CollisionFilter()
                        {
                            BelongsTo = physicsInfo.BelongsTo.Value,
                            CollidesWith = physicsInfo.CollidesWith.Value,
                            GroupIndex = 0
                        });
                    manager.AddComponentData(entity, new PhysicsCollider()
                        { Value = spCollider });
                    break;
                case SphereCollider sphere:
                    break;
                case MeshCollider:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            manager.AddSharedComponent(entity, new PhysicsWorldIndex());
            manager.AddComponentData(entity, new PhysicsCollider()
                { Value = spCollider });
            manager.AddComponentData(entity, new PhysicsInfo
            {
                BelongsTo = physicsInfo.BelongsTo,
                CollidesWith = physicsInfo.CollidesWith
            });
            if (model.TryGetComponent<Rigidbody>(out var rb))
                manager.AddComponentObject(entity, rb);

            return this;
        }




        public CharacterBuilder WithCombat(ComboSO combo)
        {
            this.combo = combo;
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            manager.AddComponent<StorePrimaryWeapon>(entity);

            var comboInfo = Object.Instantiate(combo);
            manager.AddComponentObject(entity, new PlayerComboComponent { Combo = comboInfo });
            return this;
        }

        public CharacterBuilder WithStats(CharacterClass stats, uint spawnid, uint exp = 0, bool limitHp = false,
            uint limit = 0)
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            BaseCharacterComponent data = new()
            {
                GORepresentative = model,
            };
            data.SetupDataEntity(stats, exp, spawnid);
            manager.AddComponentObject(entity, data);
            manager.AddBuffer<IncrementalVitalChange>(entity);
            model.GetComponent<Damageable>().SetData(entity, data);
            this.character = data;
            return this;
        }

        public CharacterBuilder WithCharacterDetection()
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            var vision = new Vision();
            vision.InitializeSense(character);
            manager.AddBuffer<ScanPositionBuffer>(entity);
            manager.AddComponentData(entity, vision);
           // manager.AddComponent<MapVision>(entity);

            return this;
        }

        public CharacterBuilder WithInventorySystem(InventorySave Inventory, EquipmentSave Equipment)
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;

            CharacterInventory inventory = new();
            inventory.AddGold(100000);
            manager.AddComponentData(entity, inventory);
            manager.GetComponentData<CharacterInventory>(entity).Setup(entity, Inventory, Equipment, character);

            return this;
        }
    
        public CharacterBuilder WithPlayerControl()
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            manager.AddComponent<Player_Control>(entity);
            manager.AddComponent<AttackTarget>(entity);
            manager.AddComponentData(entity, new SurroundCharacter());
            manager.AddComponentData(entity, new createTag());
            manager.AddComponentObject(entity, new Command()
            {
                EquippedAbilities = new Dreamers.InventorySystem.AbilitySystem.AbilityList(),
                InputTimeReset = 5.0f,
                InputTimer = 5.0f
            });
            CameraControl.Instance.Follow.LookAt = model.GetComponentInChildren<LookHereTarget>().transform;
            CameraControl.Instance.Follow.Follow = model.transform;
            CameraControl.Instance.Target.Follow = model.transform;

            return this;
        }
        public CharacterBuilder WithFactionInfluence(int factionID, int baseProtection, int baseThreat, uint classLevel,
            bool isPlayer = false)
        {
            this.factionID = factionID;
            this.classLevel = classLevel;
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            manager.AddComponentData(entity, new InfluenceComponent
            {
                factionID = factionID,
                Protection = baseProtection,
                Threat = baseThreat
            });
            manager.AddComponentData(entity, new AITarget()
            {
                FactionID = factionID,
                NumOfEntityTargetingMe = 3,
                CanBeTargetByPlayer = isPlayer,
                Type = TargetType.Character,
                //level = classLevel,
                CenterOffset = new float3(0, 1, 0) //todo add value to SO
            });

            manager.AddComponentData(entity, new Perceptibility
            {
                movement = MovementStates.Standing_Still,
                noiseState = NoiseState.Normal,
                visibilityStates = VisibilityStates.Visible
            });

            return this;
        }

        public CharacterBuilder WithMovement(MovementData Move, CreatureType creatureType, bool AI = false)
        {
            if (entity == Entity.Null) return this;
            if (model == null) return this;
            switch (creatureType)
            {
                case CreatureType.biped:
                    var controllerData = new CharControllerE();
                    controllerData.Setup(Move, model.GetComponent<UnityEngine.CapsuleCollider>(), AI);
                    manager.AddComponentData(entity, controllerData);
                    break;
                case CreatureType.quadruped:
                    var beastData = new BeastControllerComponent();
                    beastData.Setup(Move, model.GetComponent<UnityEngine.CapsuleCollider>(), AI);
                    manager.AddComponentData(entity, beastData);
                    break;
                case CreatureType.mecha:
                    break;
                case CreatureType.spirit:
                    break;
                case CreatureType.stationary:
                    break;
                case CreatureType.flying:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(creatureType), creatureType, null);
            }

            return this;
        }

        public Entity Build()
        {
        
            return entity;
        }

        public CharacterBuilder WithVFX()
        {
            model.GetComponent<VFXControl>().Init(combo);
            return this;
        }
    }
}