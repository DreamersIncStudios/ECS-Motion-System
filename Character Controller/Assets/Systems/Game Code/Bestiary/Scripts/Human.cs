using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Global.Component;
using AISenses;
//using IAUS.ECS.Component;
using DreamersInc.InflunceMapSystem;
using Unity.Entities;
using DreamerInc.CombatSystem;
using Assets.Systems.Global.Function_Timer;
using DG.Tweening;
using Unity.Transforms;
using Unity.Mathematics;
using MotionSystem.Archetypes;
using DreamersInc.ComboSystem.NPC;

namespace BestiaryLibrary
{
    public static partial class BestiaryDB
    {
        public static Entity SpawnBasicAndCreateEntityData(Vector3 Position, PhysicsInfo physicsInfo, string entityName = "")
        {
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity entityLink = createEntity(manager, entityName);
            GameObject spawnedGO = SpawnGO(manager, entityLink, Position, "NPCs/Combat/Human",0);

            AddPhysics(manager, entityLink, spawnedGO, PhysicsShape.Capsule, physicsInfo);
            manager.SetComponentData(entityLink, new Translation { Value = spawnedGO.transform.position });
            manager.SetComponentData(entityLink, new Rotation { Value = spawnedGO.transform.rotation });
            AddTargetingAndInfluence(manager, entityLink,
                new AITarget()
                {
                    Type = TargetType.Character,
                    GetRace = Race.Human,
                    MaxNumberOfTarget = 5,
                    CanBeTargetByPlayer = true,
                    CenterOffset = new float3(0, 1, 0)
                },
                new Vision()
            {
                viewRadius = 55,
                EngageRadius = 40,
                ViewAngle = 360
            }
   );
            AddMovementSystems(manager, entityLink, spawnedGO);

            spawnedGO.GetComponent<EnemyCharacter>().SetupDataEntity(manager, entityLink);
            spawnedGO.tag = "Enemy NPC";
            spawnedGO.GetComponent<NPCCharacterController>().SetupDataEntity(entityLink);
            spawnedGO.GetComponent<NPCComboComponentAuthoring>().SetupDataEntity(entityLink);

            return entityLink;

        }

    }
}
