using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace MotionSystem.Components
{
    public struct CharControllerE: IComponentData
    {
        public float3 CapsuleCenter;
        public float CapsuleRadius;
        public float CapsuleHeight;
        public float3 OGCapsuleCenter;
        public float OGCapsuleHeight;
        public float Forward;
        public float Turn;
        public float H;
        public float V;
        public bool Jump;
        public bool Crouch;

        public Vector3 Move;
        public bool Walk;
        public Vector3 GroundNormal;
        public bool IsGrounded;
        public float GroundCheckDistance;

        public float m_MovingTurnSpeed;
        public float m_StationaryTurnSpeed;
        public float m_JumpPower;
        public float m_GravityMultiplier;
        public float m_RunCycleLegOffset; //specific to the character in sample assets, will need to be modified to work with others
        public float m_MoveSpeedMultiplier;
        public float m_AnimSpeedMultiplier;
        public float m_OrigGroundCheckDistance;
        public bool AI;
        public LayerMask GroundCheckLayerMask;

        public bool EquipWeapon;
        public float EquipResetTimer;
        public float TimerForEquipReset;
        public bool LightAtk;
        public bool HeavyAtk;
        public bool ProjectileAtk;

      
    }

    public struct AI_Control : IComponentData {
        public bool IsGrounded;
    }
    public struct Player_Control : IComponentData { }
}

