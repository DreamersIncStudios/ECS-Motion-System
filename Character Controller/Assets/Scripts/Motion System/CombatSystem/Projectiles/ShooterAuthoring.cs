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
        ShootingData.LastTimeShot = 0.0f;
        Entity point = dstManager.CreateEntity();
        dstManager.AddComponentObject(point, ShootPoint.transform); // Add child transform manually
        dstManager.AddComponentData(point, new Translation()); // Have to add all this stuff manually too
        dstManager.AddComponentData(point, new Rotation());
        dstManager.AddComponentData(point, new LocalToWorld());
        dstManager.AddComponentData(point, new CopyTransformFromGameObject()); // Or CopyTransformToGameObject - Only if you need to sync transforms

        // - Only if you want the parent child relationship
        dstManager.AddComponentData(point, new Parent { Value = entity });

        ShootingData.ShootFromHere = point;

        dstManager.AddComponentData(entity, ShootingData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ProjectilePrefab);
    
    }

}

[Serializable]
public class ShooterComponent : IComponentData {
    public int RoundsPerMin;
    public int RoundsPerShot;
    [HideInInspector] public int RoundsLeftToSpawn;
    public bool IsShooting => RoundsLeftToSpawn > 0;
    [HideInInspector] public bool HasShotBeenCharge;
    public float NormalSpeed;
 
    public GameObject ProjectileEntity;

    public Entity ShootFromHere;
    public float3 Offset;
   [HideInInspector] public float LastTimeShot;
    public bool Wait => LastTimeShot > 0.0f;
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
