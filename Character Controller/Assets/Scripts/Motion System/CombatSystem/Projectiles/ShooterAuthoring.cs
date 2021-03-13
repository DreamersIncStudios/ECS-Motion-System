using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Mathematics;
using Unity.Transforms;
public class ShooterAuthoring : MonoBehaviour,IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    public GameObject ProjectilePrefab; //Move to SO later 
    public GameObject ShootPoint;
    public ShooterComponent ShootingData;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        ShootingData.ProjectileEntity = ProjectilePrefab.gameObject;
        ShootingData.LastTimeShot = -1000f;
        Entity point = dstManager.CreateEntity();
        dstManager.AddComponentObject(point, ShootPoint.transform); // Add child transform manually
        dstManager.AddComponentData(point, new Translation()); // Have to add all this stuff manually too
        dstManager.AddComponentData(point, new Rotation());
        dstManager.AddComponentData(point, new LocalToWorld());
        dstManager.AddComponentData(point, new CopyTransformFromGameObject()); // Or CopyTransformToGameObject - Only if you need to sync transforms

        // - Only if you want the parent child relationship
        dstManager.AddComponentData(point, new Parent { Value = entity });

        ShootingData.ShootPointEntity = point;

        dstManager.AddComponentData(entity, ShootingData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ProjectilePrefab);
    
    }

}

[Serializable]
public class ShooterComponent : IComponentData {
    public float FiringRate;
    public float BulletAmountPerShot;
    public float NormalSpeed;
    public float ChargedSpeed;
 
    public GameObject ProjectileEntity;

    public Entity ShootPointEntity;
    [NonSerialized]
    public float LastTimeShot;
}

[Serializable]
public struct DamageProjectile : IComponentData
{
    public float Speed;
    [NonSerialized]
    public float3 PreviousPosition;

    public float Damage;
    public float Radius;
}
