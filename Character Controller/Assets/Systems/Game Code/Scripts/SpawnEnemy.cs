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

public class SpawnEnemy : MonoBehaviour
{

    public List<GameObject> EnemyModels; // TODO load this from text data?????

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SpawnEnemyAndCreateEntityData(int choice, Vector3 SpawnPosition)
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
              typeof(Movement)
              );

        Entity EnemyDataEntity = em.CreateEntity(playerDataArch);
        GameObject spawnedGO = GameObject.Instantiate(EnemyModels[choice], SpawnPosition, Quaternion.identity);


        em.SetName(EnemyDataEntity, "Player Data");

        em.SetComponentData(EnemyDataEntity, new Translation { Value = spawnedGO.transform.position });

        em.SetComponentData(EnemyDataEntity, new AITarget
        {
            Type = TargetType.Character,
            GetRace = Race.Human,
            MaxNumberOfTarget = 5,
            CanBeTargetByPlayer = true,
            CenterOffset = Vector3.zero
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
        em.SetComponentData(EnemyDataEntity, new Movement() { CanMove = true });

        em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<Animator>());
        em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<CapsuleCollider>());
        em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponent<Rigidbody>());
        em.AddComponentObject(EnemyDataEntity, spawnedGO.transform);
        em.AddComponentObject(EnemyDataEntity, spawnedGO.GetComponentInChildren<Renderer>());
        spawnedGO.GetComponent<EnemyCharacter>().SetupDataEntity(EnemyDataEntity);
        spawnedGO.tag = "Player";
        spawnedGO.GetComponent<NPCCharacterController>().SetupDataEntity(EnemyDataEntity);
        spawnedGO.GetComponent<PlayerComboComponentAuthoring>().SetupDataEntity(EnemyDataEntity);
    }
}
