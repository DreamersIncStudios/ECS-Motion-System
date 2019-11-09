using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;
using MotionSystem.Components;
using IAUS.ECS.Component;


namespace MotionSystem.Archetypes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]


    public class CharacterControl : MonoBehaviour,IConvertGameObjectToEntity
    {
        CapsuleCollider Col;
        Rigidbody RB;
        public bool AI_Control;

        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Col = this.GetComponent<CapsuleCollider>();
            RB = this.GetComponent<Rigidbody>();
            var control = new CharController() { CapsuleRadius = Col.radius, CapsuleCenter = Col.center, CapsuleHeight = Col.height, Mass = RB.mass,
               m_AnimSpeedMultiplier=m_AnimSpeedMultiplier, m_GravityMultiplier=m_GravityMultiplier,m_JumpPower=m_JumpPower, m_MoveSpeedMultiplier=m_MoveSpeedMultiplier,
               m_MovingTurnSpeed=m_MovingTurnSpeed,m_RunCycleLegOffset=m_RunCycleLegOffset, m_StationaryTurnSpeed=m_StationaryTurnSpeed,
               m_OrigGroundCheckDistance=m_GroundCheckDistance
               

            };
            dstManager.AddComponentData(entity, control);
            if (AI_Control)
            {
                var move = new Movement() { };
                var AI = new AI_Control() { };
                this.gameObject.AddComponent<NavMeshAgent>();
                dstManager.AddComponentData(entity, move);
                dstManager.AddComponentData(entity, AI);

            }
            else {
                var player = new Player_Control() { };
                dstManager.AddComponentData(entity, player);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}