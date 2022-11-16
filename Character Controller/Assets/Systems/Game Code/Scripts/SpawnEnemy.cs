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

        public List<GameObject> EnemyModels;  // TODO load this from text data?????
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags collideWith;
        // Start is called before the first frame update
        void Start()
        {
            BestiaryDB.SpawnBasicAndCreateEntityData( new Vector3(-8, 1, 20), new PhysicsInfo() { BelongsTo= belongsTo, CollidesWith = collideWith});
            BestiaryDB.SpawnBasicAndCreateEntityData( new Vector3(-6, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData( new Vector3(-4, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData( new Vector3(-2, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData( new Vector3(0, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(2, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(4, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(6, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(8, 1, 20), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(-8, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(-6, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(-4, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(-2, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(0, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(2, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(4, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(6, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            BestiaryDB.SpawnBasicAndCreateEntityData(new Vector3(8, 0, 25), new PhysicsInfo() { BelongsTo = belongsTo, CollidesWith = collideWith });
            int towercnt = new int();
            BestiaryDB.SpawnBaseBeastAndCreateEntityData(new Vector3(0,0,40),
                new PhysicsInfo { BelongsTo = this.belongsTo, CollidesWith = this.collideWith}, 0 , "Bear");
        }

        
    
    
    }
}