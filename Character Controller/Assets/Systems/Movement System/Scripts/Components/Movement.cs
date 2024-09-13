
//using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.MovementSystem
{
    public struct Movement : IComponentData
    {
        public float3 TargetLocation;
        public bool TargetLocationCrowded;
        public int MaxInfluenceAtPoint;
        public bool CanMove;
        public bool SetTargetLocation { get; set; }
    }



}