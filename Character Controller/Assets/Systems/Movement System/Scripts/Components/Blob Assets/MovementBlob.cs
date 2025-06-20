using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MotionSystem.Components.Blob
{
    public struct MovementData
    {
        public int ID;
        public float3 OGCapsuleCenter;
        public float OGCapsuleHeight;
        public float m_MovingTurnSpeed;
        public float m_StationaryTurnSpeed;
        public float m_JumpPower;
        public float m_GravityMultiplier;
        public float m_RunCycleLegOffset; //specific to the character in sample assets, will need to be modified to work with others
        public float m_MoveSpeedMultiplier;
        public float m_AnimSpeedMultiplier;
        public float m_OrigGroundCheckDistance;
    }

    public struct MovementBlobAsset
    {
        public BlobArray<MovementData> Array;

        public MovementData GetMovementData(int index)
        {
            foreach (var moveData in Array.ToArray())
            {
                if(index == moveData.ID)
                    return moveData;
            }
            return default;
        }
        public bool HasMovementData(int index)
        {
            return Array.ToArray().Any(moveData => index == moveData.ID);
        }
    }
}
