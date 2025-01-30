using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Streaming.SceneManagement.SectionMetadata
{
    public struct Circle : IComponentData
    {
        public float Radius; // Proximity radius within which to consider loading a section
        public float3 Center;
    }
}
