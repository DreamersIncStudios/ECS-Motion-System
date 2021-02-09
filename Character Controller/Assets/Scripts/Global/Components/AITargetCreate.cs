using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Global.Component
{
    public class AITargetCreate : MonoBehaviour, IConvertGameObjectToEntity
    {
        public AITarget aITarget;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            aITarget.GetInstanceID = gameObject.GetInstanceID();
            dstManager.AddComponentData(entity, aITarget);
        }

    }
}
