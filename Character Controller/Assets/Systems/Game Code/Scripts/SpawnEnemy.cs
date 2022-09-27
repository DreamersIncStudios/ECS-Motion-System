using AISenses;
using AISenses.VisionSystems.Combat;
using Components.MovementSystem;
using DreamersInc.ComboSystem;
using DreamersInc.ComboSystem.NPC;
using Global.Component;
using MotionSystem.Archetypes;
using MotionSystem.Components;
using Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Mathematics;
using BestiaryLibrary;

namespace GameCoreCode
{
    public class SpawnEnemy : MonoBehaviour
    {

        public List<GameObject> EnemyModels; // TODO load this from text data?????
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags collideWith;
        // Start is called before the first frame update
        void Start()
        {
            SpawnEnemyAndCreateEntityData(0, new Vector3(-8, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-6, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-4, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-2, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(0, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(2, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(4, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(6, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(8, 1, 20));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-8, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-6, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-4, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(-2, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(0, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(2, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(4, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(6, 0, 25));
            SpawnEnemyAndCreateEntityData(0, new Vector3(8, 0, 25));
            int towercnt = new int();
            BestiaryDB.SpawnTowerAndCreateEntityDataWithVFX(new Vector3(0,0,40),
                new PhysicsInfo { BelongsTo = this.belongsTo, CollidesWith = this.collideWith},
                $"Tower {towercnt}");
        }
        public  Entity SpawnEnemyAndCreateEntityData(int choice, Vector3 SpawnPosition)
        {
            if (choice > EnemyModels.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(choice), $"Input outside of bounds of expection input value: {choice}");
            }
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype playerDataArch = em.CreateArchetype(
                  typeof(Translation),
                  typeof(Rotation),
                  typeof(LocalToWorld),
                  typeof(CharControllerE),
                  typeof(EnemyStats),
                  typeof(AI_Control),
                  typeof(Command),
                  typeof(AITarget),
                  typeof(Vision),
                  typeof(NPCComboComponent),
                  typeof(CopyTransformFromGameObject),
                  typeof(AttackTarget),
                  typeof(ScanPositionBuffer),
                  typeof(Movement),
                  typeof(PhysicsCollider),
                  typeof(PhysicsWorldIndex),
                  typeof(PhysicsInfo)

                  );

            Entity EnemyDataEntity = em.CreateEntity(playerDataArch);
            GameObject spawnedGO = GameObject.Instantiate(EnemyModels[choice], SpawnPosition, Quaternion.identity);


            em.SetName(EnemyDataEntity, "Enemy Data");

            em.SetComponentData(EnemyDataEntity, new Translation { Value = spawnedGO.transform.position });
            em.SetComponentData(EnemyDataEntity, new Rotation { Value = spawnedGO.transform.rotation });

            em.SetComponentData(EnemyDataEntity, new AITarget
            {
                Type = TargetType.Character,
                GetRace = Race.Human,
                MaxNumberOfTarget = 5,
                CanBeTargetByPlayer = true,
                CenterOffset = new float3(0, 1, 0)
            });
            em.SetComponentData(EnemyDataEntity, new Command()
            {
                InputQueue = new Queue<AnimationTrigger>(),
                BareHands = false,
                WeaponIsEquipped = true
            });
            em.SetComponentData(EnemyDataEntity, new Vision()
            {
                viewRadius = 55,
                EngageRadius = 40,
                ViewAngle = 165
            });
            em.SetComponentData(EnemyDataEntity, new Movement() { CanMove = false });

            BlobAssetReference<Unity.Physics.Collider> spCollider = Unity.Physics.CapsuleCollider.Create(new CapsuleGeometry()
            {
                Radius = .4f,
                Vertex0 = new float3(0, 1.8f, 0),
                Vertex1 = new float3(0, 0, 0)
                
            },new CollisionFilter()
            {
                BelongsTo = belongsTo.Value,
                CollidesWith = collideWith.Value,
                GroupIndex = 0
            }
            );
            
            em.SetComponentData(EnemyDataEntity, new PhysicsCollider()
            { Value = spCollider });
            em.SetComponentData(EnemyDataEntity, new PhysicsInfo
            {
                BelongsTo = belongsTo,
                CollidesWith = collideWith,
            });

            em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<Animator>());
            em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<UnityEngine.CapsuleCollider>());

            em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<Rigidbody>());
            em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<NavMeshAgent>());

            em.AddComponentObject(EnemyDataEntity, spawnedGO.transform);
            em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponentInChildren<Renderer>());
            spawnedGO.GetComponent<EnemyCharacter>().SetupDataEntity(EnemyDataEntity);
            spawnedGO.tag = "Enemy NPC";
            spawnedGO.GetComponent<NPCCharacterController>().SetupDataEntity(EnemyDataEntity);
            spawnedGO.GetComponent<NPCComboComponentAuthoring>().SetupDataEntity(EnemyDataEntity);

            return EnemyDataEntity;
        }
        
    
    
    }
}