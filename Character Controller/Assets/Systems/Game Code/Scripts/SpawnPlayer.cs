using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using MotionSystem.Components;
using Stats;
using DreamersInc.ComboSystem;
using Global.Component;
using AISenses;
using MotionSystem.Archetypes;
using AISenses.VisionSystems.Combat;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Authoring;

namespace GameCoreCode
{
    public class SpawnPlayer : MonoBehaviour
    {
        public List<GameObject> PlayerModels; // TODO load this from text data?????
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags collideWith;
        // Start is called before the first frame update
        void Start()
        {
            SpawnPlayerAndCreateEntityData(0, Vector3.zero);

        }

        public void SpawnPlayerAndCreateEntityData(int choice, Vector3 SpawnPosition)
        {
            if (choice > PlayerModels.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(choice), $"Input outside of bounds of expection input value: {choice}");
            }
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype playerDataArch = em.CreateArchetype(
                  typeof(Translation),
                  typeof(Rotation),
                  typeof(LocalToWorld),
                  typeof(CharControllerE),
                  typeof(PlayerStatComponent),
                  typeof(Player_Control),
                  typeof(Command),
                  typeof(AITarget),
                  typeof(Vision),
                  typeof(PlayerComboComponent),
                  typeof(CopyTransformFromGameObject),
                  typeof(AttackTarget),
                  typeof(ScanPositionBuffer),
                  typeof(PhysicsCollider),
                  typeof(PhysicsWorldIndex),
                  typeof(PhysicsInfo)

                  );

            Entity playerDataEntity = em.CreateEntity(playerDataArch);
            GameObject spawnedGO = GameObject.Instantiate(PlayerModels[choice], SpawnPosition, Quaternion.identity);


            em.SetName(playerDataEntity, "Player Data");

            em.SetComponentData(playerDataEntity, new Translation { Value = spawnedGO.transform.position });

            em.SetComponentData(playerDataEntity, new AITarget
            {
                Type = TargetType.Character,
                GetRace = Race.Human,
                MaxNumberOfTarget = 5,
                CanBeTargetByPlayer = true,
                CenterOffset = Vector3.zero
            });
            em.SetComponentData(playerDataEntity, new Command()
            {
                InputQueue = new Queue<AnimationTrigger>(),
                BareHands = false,
                WeaponIsEquipped = true
            });
            em.SetComponentData(playerDataEntity, new Vision()
            {
                viewRadius = 55,
                EngageRadius = 40,
                ViewAngle = 165
            });
            BlobAssetReference<Unity.Physics.Collider> spCollider = Unity.Physics.CapsuleCollider.Create(new CapsuleGeometry()
            {
                Radius = .4f,
                Vertex0 = new float3(0, 1.8f, 0),
                Vertex1 = new float3(0, 0, 0)

            }, new CollisionFilter()
            {
                BelongsTo = belongsTo.Value,
                CollidesWith = collideWith.Value,
                GroupIndex = 0
            });
            em.SetComponentData(playerDataEntity, new PhysicsCollider()
            { Value = spCollider });
            em.SetComponentData(playerDataEntity, new PhysicsInfo
            {
                BelongsTo = belongsTo,
                CollidesWith = collideWith,
            });

            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponent<Animator>());
            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponent<UnityEngine.CapsuleCollider>());
            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponent<Rigidbody>());
            em.AddComponentObject(playerDataEntity, spawnedGO.transform);
            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponentInChildren<Renderer>());
            spawnedGO.GetComponent<PlayerCharacter>().SetupDataEntity(playerDataEntity);
            spawnedGO.tag = "Player";
            spawnedGO.GetComponent<CharacterControl>().SetupDataEntity(playerDataEntity);
            spawnedGO.GetComponent<PlayerComboComponentAuthoring>().SetupDataEntity(playerDataEntity);
            SetupCamera(spawnedGO);
        }



        public CinemachineFreeLook freeLook;
        public CinemachineFreeLook Target;
        public CinemachineTargetGroup group;

        void SetupCamera(GameObject Player)
        {
            freeLook.Follow = Player.transform;
            freeLook.LookAt = Player.GetComponentInChildren<LookHereTarget>().transform;
            Target.Follow = Player.transform;
            group.m_Targets[1].target = Player.transform;
        }

    }
}