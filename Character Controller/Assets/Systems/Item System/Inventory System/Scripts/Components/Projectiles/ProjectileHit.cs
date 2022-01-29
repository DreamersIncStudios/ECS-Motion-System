using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.Global;
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
        if (collision.collider.gameObject.layer == 9|| collision.collider.gameObject.layer == 10)
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
