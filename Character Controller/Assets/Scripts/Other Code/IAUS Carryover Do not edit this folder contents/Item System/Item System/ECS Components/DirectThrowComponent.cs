using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
namespace ItemSystem.projectiles
{
    public class DirectThrowComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            throw new System.NotImplementedException();
        }
    }

    public class DirectThrow : IComponentData {
        public float force;
        public float3 Direction;

    }

    //public class ThrowSystem : ComponentSystem
    //{
    //    protected override void OnUpdate()
    //    {
    //        throw new System.NotImplementedException();
    //    }


    //}



}