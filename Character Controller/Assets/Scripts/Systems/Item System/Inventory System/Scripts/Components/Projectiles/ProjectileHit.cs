using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class ProjectileHit : MonoBehaviour,IConvertGameObjectToEntity
{
    EntityManager MGR;
    [HideInInspector] public Entity reference;
    public void Start()
    {
        MGR = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 9)
        {
            if (MGR.Exists(reference))
                MGR.AddComponent<DestroyTag>(reference);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        reference = entity;
    }
}
