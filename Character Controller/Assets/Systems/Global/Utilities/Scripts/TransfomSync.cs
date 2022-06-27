using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Transforms;

namespace ECS.Utilities
{
    public class TransfomSync : MonoBehaviour, IConvertGameObjectToEntity
    {

        public void Awake()
        {

        }
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new TransformComponent { };
            dstManager.AddComponentData(entity, data);
        }

    }

    public struct TransformComponent : IComponentData { }

}

    