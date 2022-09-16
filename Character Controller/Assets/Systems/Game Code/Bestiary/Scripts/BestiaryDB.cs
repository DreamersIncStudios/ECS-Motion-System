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

namespace BestiaryLibrary
{
    public static class BestiaryDB 
    {
        static List<GameObject> LoadModels(string path) {
            List < GameObject> modelFromResources = new List<GameObject> ();
            GameObject[] goLoaded = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToArray();
            foreach (var go in goLoaded)
            {
                modelFromResources.Add(go);
            }
            return modelFromResources;
        }

      public static Entity SpawnTowerAndCreateEntityData(Vector3 Position, PhysicsInfo physicsInfo, string entityName = "") {
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
            GameObject spawnedGO = GameObject.Instantiate(Models[cnt], Position, Quaternion.identity);
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


            manager.AddComponentObject(npcDataEntity, spawnedGO.GetComponent<UnityEngine.CapsuleCollider>());
            manager.AddComponentObject(npcDataEntity, spawnedGO.GetComponent<Rigidbody>());

            #endregion

            #region Stats
            EnemyCharacter stats = spawnedGO.GetComponent<EnemyCharacter>();
            stats.SetupDataEntity(npcDataEntity);
            StaticObjectController controller = spawnedGO.GetComponent<StaticObjectController>();
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
