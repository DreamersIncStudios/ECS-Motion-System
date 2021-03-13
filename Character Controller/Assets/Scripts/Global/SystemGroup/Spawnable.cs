using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class Spawnable : MonoBehaviour,IConvertGameObjectToEntity
{
    [HideInInspector] public Entity reference;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        reference = entity;
    }

    public void OnDestroy()
    {
        EntityManager mgr = World.DefaultGameObjectInjectionWorld.EntityManager;
        mgr.AddComponent<DestroyTag>(reference);
    }

    public struct DestroyTag : IComponentData
    {
       
    }

}
