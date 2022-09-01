using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows;
using Unity.VisualScripting;
using UnityEngine.AI;
using Unity.Entities;
using Unity.Transforms;
using MotionSystem.Components;
using Stats;
using DreamersInc.ComboSystem;
using Global.Component;
using AISenses;

namespace GameCoreCode
{
    public class SpawnPlayer : MonoBehaviour
    {
        public List<GameObject> PlayerModels; // TODO load this from text data?????

        // Start is called before the first frame update
        void Start()
        {
            SpawnPlayerAndCreateEntityData(0, Vector3.zero);

        }

        public void SpawnPlayerAndCreateEntityData(int choice, Vector3 SpawnPosition) {
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
                  typeof(AITarget),
                  typeof(Vision),
                  typeof(PlayerComboComponent),
                  typeof(CopyTransformFromGameObject)
                  );

            Entity playerDataEntity = em.CreateEntity(playerDataArch);
            GameObject spawnedGO = GameObject.Instantiate(PlayerModels[choice], SpawnPosition, Quaternion.identity);

            em.SetName(playerDataEntity, "Player Data");
            em.SetComponentData(playerDataEntity, new Translation { Value = spawnedGO.transform.position });
            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponent<Animator>());
            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponent<CapsuleCollider>());
            em.AddComponentObject(playerDataEntity, spawnedGO.transform);
            em.AddComponentObject(playerDataEntity, spawnedGO.GetComponentInChildren<Renderer>());
            spawnedGO.GetComponent<PlayerCharacter>().SetupDataEntity(playerDataEntity);
        }
    }
}