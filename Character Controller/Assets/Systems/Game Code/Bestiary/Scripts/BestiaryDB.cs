using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using AISenses.Authoring;
using Global.Component;
using AISenses;
using IAUS.ECS;
//using IAUS.ECS.Component;
using DreamersInc.InflunceMapSystem;
using UnityEngine.AI;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.Physics;
using DreamerInc.CombatSystem;
using Assets.Systems.Global.Function_Timer;
using DG.Tweening;

namespace BestiaryLibrary
{
    public static partial class BestiaryDB
    {
        static List<GameObject> LoadModels(string path) {
            List<GameObject> modelFromResources = new List<GameObject>();
            GameObject[] goLoaded = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
            foreach (var go in goLoaded)
            {
                modelFromResources.Add(go);
            }
            return modelFromResources;
        }

        private static Entity createEntity(EntityManager manager, string entityName = "")
        {

            EntityArchetype baseEntityArch = manager.CreateArchetype(
              typeof(Translation),
              typeof(Rotation),
              typeof(LocalToWorld),
              typeof(CopyTransformFromGameObject)
              );
            Entity baseDataEntity = manager.CreateEntity(baseEntityArch);
            if (entityName != string.Empty)
                manager.SetName(baseDataEntity, entityName);
            else
                manager.SetName(baseDataEntity, "NPC Data");

            return baseDataEntity;
        }
        private static GameObject SpawnGO(EntityManager manager, Entity linkEntity, Vector3 Position, string modelPath = "")
        {
            var Models = LoadModels(modelPath);
            int cnt = Random.Range(0, Models.Count);
            GameObject spawnedGO = GameObject.Instantiate(Models[cnt], Position, Quaternion.identity);
            manager.SetComponentData(linkEntity, new Translation { Value = Position });
            manager.AddComponentObject(linkEntity, spawnedGO.transform);
            if (spawnedGO.GetComponent<Animator>())
                manager.AddComponentObject(linkEntity, spawnedGO.GetComponent<Animator>());

            return spawnedGO;
        }

        private static void AddPhysics(EntityManager manager, Entity entityLink, GameObject spawnedGO)
        {
            UnityEngine.CapsuleCollider col = spawnedGO.GetComponent<UnityEngine.CapsuleCollider>();
            BlobAssetReference<Unity.Physics.Collider> spCollider = Unity.Physics.CapsuleCollider.Create(new CapsuleGeometry()
            {
                Radius = col.radius,
                Vertex0 = col.center + new Vector3(0, col.height, 0),
                Vertex1 = new float3(0, 0, 0)

            }, new CollisionFilter()
            {
                BelongsTo = (1 >> 10),
                CollidesWith = (1 >> 11),
                GroupIndex = 0
            });

            manager.AddComponentData(entityLink, new PhysicsCollider()
            { Value = spCollider });
            manager.AddComponentObject(entityLink, spawnedGO.GetComponent<UnityEngine.CapsuleCollider>());
            manager.AddComponentObject(entityLink, spawnedGO.GetComponent<Rigidbody>());
            manager.AddSharedComponentData(entityLink, new PhysicsWorldIndex());

        }

        private static void AddTargetingAndInfluence(EntityManager manager, Entity entityLink) {
            manager.AddComponentData(entityLink, new ):
            manager.AddComponentData(entityLink, new ):
            manager.AddComponentData(entityLink, new ):
            
        }

        public static Entity SpawnTowerAndCreateEntityDataWithVFX(Vector3 Position, PhysicsInfo physicsInfo, string entityName = "") {
            VFXManager.Instance.PlayVFX(6,Position, 6);
            Entity temp = new Entity();
            FunctionTimer.Create(() => {
                temp = SpawnTowerAndCreateEntityData(Position+ Vector3.down*5, physicsInfo,  out GameObject spawnGO, entityName);
                spawnGO.transform.DOMoveY(Position.y+1.2f, 3);
            }, 2  , "Spawn Tower");

            return temp;
        }
      public static Entity SpawnTowerAndCreateEntityData(Vector3 Position, PhysicsInfo physicsInfo, out GameObject spawnedGO, string entityName = "") {
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype npcDataArch = manager.CreateArchetype(
               typeof(Translation),
               typeof(Rotation),
               typeof(LocalToWorld),
               typeof(EnemyStats),
               //typeof(IAUSBrain),
               typeof(AITarget),
               typeof(InfluenceComponent),
               //typeof(Wait),
               //typeof(StateBuffer),
               typeof(Perceptibility),
               typeof(Vision),
               //typeof(RenderComponent),
               typeof(ScanPositionBuffer),
               typeof(CopyTransformFromGameObject),
               typeof(PhysicsCollider),
               typeof(PhysicsWorldIndex),
                  typeof(PhysicsInfo)

               );

            Entity npcDataEntity = manager.CreateEntity(npcDataArch);
            if (entityName != string.Empty)
                manager.SetName(npcDataEntity, entityName);
            else
                manager.SetName(npcDataEntity, "NPC Data");
            var Models = LoadModels("NPCs/Combat/Tower");
            int cnt = Random.Range(0, Models.Count);
            #region GameObject Setup
            spawnedGO = GameObject.Instantiate(Models[cnt], Position, Quaternion.identity);
            manager.SetComponentData(npcDataEntity, new Translation { Value = Position });
            manager.AddComponentObject(npcDataEntity, spawnedGO.transform);
            if(spawnedGO.GetComponent<Animator>())
            manager.AddComponentObject(npcDataEntity, spawnedGO.GetComponent<Animator>());

            #endregion

            //Todo Change Later Box Collider????
            #region Physics
            UnityEngine.BoxCollider col = spawnedGO.GetComponent<UnityEngine.BoxCollider>();
            BlobAssetReference<Unity.Physics.Collider> spCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry()
            {
               Center = col.center,
               Size = col.size,
               Orientation = quaternion.identity,

            }, new CollisionFilter()
            {
                BelongsTo = physicsInfo.BelongsTo.Value,
                CollidesWith = physicsInfo.CollidesWith.Value,
                GroupIndex = 0
            });
            manager.SetComponentData(npcDataEntity, new PhysicsCollider()
            { Value = spCollider });
            manager.SetComponentData(npcDataEntity, new PhysicsInfo
            {
                BelongsTo = physicsInfo.BelongsTo,
                CollidesWith = physicsInfo.CollidesWith
            });


            manager.AddComponentObject(npcDataEntity, spawnedGO.GetComponent<UnityEngine.BoxCollider>());

            #endregion

            #region Stats
            EnemyCharacter stats = spawnedGO.GetComponent<EnemyCharacter>();
            stats.SetupDataEntity(npcDataEntity);
            StaticObjectControllerAuthoring controller = spawnedGO.GetComponent<StaticObjectControllerAuthoring>();
            controller.SetupControllerEntityData(npcDataEntity);
            #endregion
            #region detection 
            manager.SetComponentData(npcDataEntity, new Vision()
            {
                viewRadius = 55,
                EngageRadius = 40,
                ViewAngle = 360
            });


            #endregion

            return npcDataEntity;

        }
    }
}