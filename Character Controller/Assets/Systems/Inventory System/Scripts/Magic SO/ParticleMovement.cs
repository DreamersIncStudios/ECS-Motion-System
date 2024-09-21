using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public struct ParticleMovement : IComponentData
    {
        public float Speed;

        public ParticleMovement(float speed)
        {
         Speed = speed;
         float3 forward = float3.zero;
        }
    }

    public partial class ParticleMovementSystem  : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().ForEach((Transform transform, ref LocalTransform localTransform,ref ParticleMovement move )=>{
                transform.position = localTransform.Position = transform.position +move.Speed*transform.forward;
            }).Run();
        }
    }
}
