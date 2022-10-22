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

        private static void AddPhysics(EntityManager manager, Entity entityLink, GameObject spawnedGO, PhysicsShape shape, PhysicsInfo physicsInfo)
        {
            BlobAssetReference<Unity.Physics.Collider> spCollider = new BlobAssetReference<Unity.Physics.Collider>();
            switch (shape)
            {
                case PhysicsShape.Capsule:
                    UnityEngine.CapsuleCollider col = spawnedGO.GetComponent<UnityEngine.CapsuleCollider>();
                    spCollider = Unity.Physics.CapsuleCollider.Create(new CapsuleGeometry()
                    {
                        Radius = col.radius,
                        Vertex0 = col.center + new Vector3(0, col.height, 0),
                        Vertex1 = new float3(0, 0, 0)

                    }, new CollisionFilter()
                    {
                        BelongsTo = physicsInfo.BelongsTo.Value,
                        CollidesWith = physicsInfo.CollidesWith.Value,
                        GroupIndex = 0
                    });

                    manager.AddComponentObject(entityLink, spawnedGO.GetComponent<UnityEngine.CapsuleCollider>());
                    manager.AddComponentObject(entityLink, spawnedGO.GetComponent<Rigidbody>());

                    break;
                case PhysicsShape.Box:
                    UnityEngine.BoxCollider box = spawnedGO.GetComponent<UnityEngine.BoxCollider>();
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
                    manager.AddComponentData(entityLink, new PhysicsCollider()
                    { Value = spCollider });
                    manager.AddComponentObject(entityLink, spawnedGO.GetComponent<UnityEngine.BoxCollider>());

                    break;
            }
            manager.AddSharedComponentData(entityLink, new PhysicsWorldIndex());
            manager.AddComponentData(entityLink, new PhysicsCollider()
            { Value = spCollider });
            manager.AddComponentData(entityLink, new PhysicsInfo
            {
                BelongsTo = physicsInfo.BelongsTo,
                CollidesWith = physicsInfo.CollidesWith
            });
        }

        private static void AddTargetingAndInfluence(EntityManager manager, Entity entityLink, Vision visionData) {
            manager.AddComponentData(entityLink, new AITarget() { });
            manager.AddComponentData(entityLink, new InfluenceComponent() { });
            manager.AddComponentData(entityLink, visionData);
            manager.AddComponentData(entityLink, new Perceptibility() { });
            manager.AddBuffer<ScanPositionBuffer>(entityLink);


        }




        enum PhysicsShape { Box, Capsule, Sphere,  Cyclinder, Custom}
    }
}
