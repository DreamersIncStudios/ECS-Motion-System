using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;
using MotionSystem.Components;
using ECS.Utilities;
using Unity.Mathematics;
using Components.MovementSystem;

namespace MotionSystem.Archetypes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(UnityEngine.CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]

    public class NPCCharacterController : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool CombatCapable;

        CapsuleCollider Col => GetComponent<CapsuleCollider>();
        Rigidbody RB => GetComponent<Rigidbody>();

        [Header("Animation Movement Specs")]
        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;

        [SerializeField] float3 GroundProbeVector;
        public LayerMask GroundCheckLayer;
        [Header("Weapon Specs")]
        public float EquipResetTimer;

        public Entity ObjectEntity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            ObjectEntity = entity;

            var control = new CharControllerE()
            {
                CapsuleRadius = Col.radius,
                OGCapsuleHeight = Col.height,
                OGCapsuleCenter = Col.center,
                CapsuleCenter = Col.center,
                CapsuleHeight = Col.height,
                m_AnimSpeedMultiplier = m_AnimSpeedMultiplier,
                m_GravityMultiplier = m_GravityMultiplier,
                m_JumpPower = m_JumpPower,
                m_MoveSpeedMultiplier = m_MoveSpeedMultiplier,
                m_MovingTurnSpeed = m_MovingTurnSpeed,
                m_RunCycleLegOffset = m_RunCycleLegOffset,
                m_StationaryTurnSpeed = m_StationaryTurnSpeed,
                m_OrigGroundCheckDistance = m_GroundCheckDistance,
                GroundCheckDistance = m_GroundCheckDistance,

                IsGrounded = true,
                AI = true,
                CombatCapable = CombatCapable,
                EquipResetTimer = EquipResetTimer

            };
            dstManager.AddComponentData(entity, control);

            var move = new Movement() { CanMove = true };
            var AI = new AI_Control() { };
            dstManager.AddComponentData(entity, move);
            dstManager.AddComponentData(entity, AI);

            var transformtransitiion = new TransformComponent();
            dstManager.AddComponentData(entity, transformtransitiion);
            dstManager.AddComponent<Unity.Transforms.CopyTransformFromGameObject>(entity);
        }

        // Start is called before the first frame update
        void Start()
        {
            RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }


    }
}