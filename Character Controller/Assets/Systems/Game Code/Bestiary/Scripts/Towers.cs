using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Global.Component;
using AISenses;
//using IAUS.ECS.Component;
using DreamersInc.InflunceMapSystem;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.Physics;


namespace BestiaryLibrary
{
    public static partial class BestiaryDB
    {
        public static Entity SpawnTowerAndCreateEntityData(Vector3 Position, string entityName = "")
        {
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity entityLink = createEntity(manager, entityName);
            GameObject spawnedGO = SpawnGO(manager, entityLink, Position);
           
            AddPhysics(manager, entityLink, spawnedGO);
            #region Stats
            EnemyCharacter stats = spawnedGO.GetComponent<EnemyCharacter>();
            stats.SetupDataEntity(entityLink);


            #endregion
            return entityLink;

        }
    }


}