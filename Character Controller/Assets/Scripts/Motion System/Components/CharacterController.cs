using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace MotionSystem.Components
{
    public struct CharController: IComponentData
    {
        public float3 CapsuleCenter;
        public float Mass;
        public float CapsuleRadius;
        public float CapsuleHeight;
        public bool AI_Control;

        public float Forward;
        public float Turn;
        public bool Jump;
        public bool Crouch;
    }
}