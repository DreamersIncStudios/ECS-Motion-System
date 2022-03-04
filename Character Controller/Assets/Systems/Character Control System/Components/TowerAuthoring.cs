using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class TowerAuthoring : MonoBehaviour
{
    public void Start()
    { 
        using (var blobAssetStore = new BlobAssetStore())
        {
            var childEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
           
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                InjectOriginalComponents(childEntity, child, World.DefaultGameObjectInjectionWorld.EntityManager);
            }
        }
    }
    void InjectOriginalComponents(Entity entity, Transform transform, EntityManager entityManager)
    {
#if UNITY_EDITOR
        entityManager.SetName(entity, transform.name);
#endif
        foreach (var com in transform.GetComponents<Component>())
        {
            if (com is GameObjectEntity || com is ConvertToEntity || com is StopConvertToEntity)
                continue;
            entityManager.AddComponentObject(entity, com);
        }
    }

}
