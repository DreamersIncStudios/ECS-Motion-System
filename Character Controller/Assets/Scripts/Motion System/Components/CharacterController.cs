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

        public float Forward;
        public float Turn;
        public float H;
        public float V;
        public bool Jump;
        public bool Crouch;

        public Vector3 Move;
        public bool Walk;
    }

    public struct AI_Control : IComponentData { }
    public struct Player_Control : IComponentData { }
}

