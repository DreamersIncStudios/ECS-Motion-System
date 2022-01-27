using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;

namespace DreamersInc.ComboSystem.Mounted
{
    [GenerateAuthoringComponent]
    public struct TrackTarget : IComponentData
    {
        public bool HasRotation { get; set; }
        public float3 positionToTarget { get; set; }
        public quaternion DirectionToTarget { get; set; }
        public uint Speed;

    }

}