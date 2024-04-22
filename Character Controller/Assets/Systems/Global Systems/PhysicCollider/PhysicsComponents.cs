using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DreamerInc.PhysicsSpawnSystem
{
    internal class ColliderGameObjectData : IComponentData
    {
        public UnityObjectRef<GameObject> ColliderGO;

    }
    internal struct BoxColliderData : IComponentData
    {
        public readonly float3 Center;
        public readonly float3 Size;

        public BoxColliderData(BoxCollider col)
        {
            Center = col.center;
            Size = col.size;
        }
    }

    public struct SphereColliderData : IComponentData
    {
        public float3 Center;
        public float Radius;
    }

    public struct CylinderColliderData : IComponentData
    {
        public float3 Center;
        public float Radius;
        public float Height;
    }

    public struct CapsuleColliderData : IComponentData
    {
        public readonly float3 Center;
        public readonly float Radius;
        public readonly float Height;

        public CapsuleColliderData(CapsuleCollider col)
        {
            Center = col.center;
            Height = col.height;
            Radius = col.radius;
        }
    }

    class ColliderCleanup : ICleanupComponentData
    {
        public Collider DestroyThisCollider;
    }
    class GameObjectCleanup : ICleanupComponentData
    {
        public GameObject DestroyThisGameObject;
    }
}