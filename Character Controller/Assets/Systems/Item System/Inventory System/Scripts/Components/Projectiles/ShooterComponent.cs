using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Mathematics;
using Dreamers.InventorySystem.ProjectileSystem;

namespace Dreamers.InventorySystem{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct ShooterComponent : IComponentData
    {
        public int RateOfFire;
        public int RoundsPerShot;
        [HideInInspector] public int RoundsLeftToSpawn;
        public bool IsShooting => RoundsLeftToSpawn > 0;
        [HideInInspector] public bool HasShotBeenCharge;
        public float NormalSpeed;
        public int ProjectileID;
        public ProjectileBase Projectile => ProjectileDatabase.GetProjectile(ProjectileID);
        public float Offset;
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



}