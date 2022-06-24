using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class Closer : MonoBehaviour,IConvertGameObjectToEntity
{
    Entity self;
   
    private void OnDestroy()
    {
        CleanUpEntity();
    }

    private void OnApplicationQuit()
    {
        CleanUpEntity();

    }

    void CleanUpEntity() {
#if !UNITY_EDITOR
        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(self);
#endif
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        self = entity;
    }
}
