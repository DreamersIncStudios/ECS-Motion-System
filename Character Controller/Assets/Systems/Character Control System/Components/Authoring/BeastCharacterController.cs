using MotionSystem.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace MotionSystem
{

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(UnityEngine.CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]

    public class BeastCharacterController : MonoBehaviour
    {
        CapsuleCollider Col => GetComponent<CapsuleCollider>();
        Rigidbody RB => GetComponent<Rigidbody>();

        [Header("Animation Movement Specs")]
        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float m_MoveSpeedMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;

        [SerializeField] float3 GroundProbeVector;
        // Start is called before the first frame update
        void Start()
        {
            RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        }

        public void SetupDataEntity(EntityManager em, Entity entity)
        {

            em.AddComponentData(entity, new BeastControllerComponent()
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
                m_StationaryTurnSpeed = m_StationaryTurnSpeed,
                m_OrigGroundCheckDistance = m_GroundCheckDistance,
                GroundCheckDistance = m_GroundCheckDistance,
                IsGrounded = true,
                AI = true,
                AnimationSpeed = 1.0f

            });



            em.AddComponent<Unity.Transforms.CopyTransformFromGameObject>(entity);
        }
    }
}
