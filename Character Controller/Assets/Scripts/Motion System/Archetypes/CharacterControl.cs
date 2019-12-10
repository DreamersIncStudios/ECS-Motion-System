using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;
using MotionSystem.Components;
using IAUS.ECS.Component;
using ECS.Utilities;

namespace MotionSystem.Archetypes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof (NavMeshAgent))]


    public class CharacterControl : MonoBehaviour,IConvertGameObjectToEntity
    {
        [Header("Party")]
        public bool AI_Control;
        public bool Party;
        NavMeshAgent Agent;
        CapsuleCollider Col;
        Rigidbody RB;
        [Header("Animation Movement Specs")]
        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;
        public LayerMask GroundCheckLayer;

        [Header("Weapon Specs")]
        public float EquipResetTimer;

        Entity ObjectEntity;
         EntityManager Manager;

        GameMasterSystem GMS;

        public void Start()
        {
            GMS = GameMasterSystem.GMS;

            RB = this.GetComponent<Rigidbody>();
            RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            if(Party && GMS.Party.Count<= GMS.MaxParty )
            GMS.Party.Add(ObjectEntity);

        }
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            GMS = GameMasterSystem.GMS;
            ObjectEntity = entity;
            Manager = dstManager;

            Agent = this.GetComponent<NavMeshAgent>();
            var data = new ECS.Utilities.TransformComponenet { };
            dstManager.AddComponentData(entity, data);

            if (Party)
            {
                var playerparty = new PlayerParty() { };
                dstManager.AddComponentData(entity, playerparty);
            }
         

            Col = this.GetComponent<CapsuleCollider>();
            var control = new CharControllerE() { CapsuleRadius = Col.radius, OGCapsuleHeight = Col.height,
                OGCapsuleCenter = Col.center, CapsuleCenter = Col.center, CapsuleHeight = Col.height,
                m_AnimSpeedMultiplier = m_AnimSpeedMultiplier, m_GravityMultiplier = m_GravityMultiplier, m_JumpPower = m_JumpPower,
                m_MoveSpeedMultiplier = m_MoveSpeedMultiplier,
                m_MovingTurnSpeed = m_MovingTurnSpeed, m_RunCycleLegOffset = m_RunCycleLegOffset, m_StationaryTurnSpeed = m_StationaryTurnSpeed,
                m_OrigGroundCheckDistance = m_GroundCheckDistance, GroundCheckLayerMask = GroundCheckLayer, GroundCheckDistance = m_GroundCheckDistance
               , IsGrounded = true, AI = AI_Control,
                EquipResetTimer = EquipResetTimer

            };
            dstManager.AddComponentData(entity, control);
            if (AI_Control)
            {
                var move = new Movement() { CanMove = true};
                var AI = new AI_Control() { };
                dstManager.AddComponentData(entity, move);
                dstManager.AddComponentData(entity, AI);

            }
            else {
                if (Party )
                {
                    Agent.enabled = false;
                    var player = new Player_Control() { };
                    dstManager.AddComponentData(entity, player);
                }
            }
            var transformtransitiion = new TransformComponenet();
            dstManager.AddComponentData(entity, transformtransitiion);
            
        }
// Need To Determine a New method for disabling gameobject. See performance different for destorying and recreating. 
    }
}