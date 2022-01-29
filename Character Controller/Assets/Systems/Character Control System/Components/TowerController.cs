using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem.ProjectileSystem;
using Unity.Burst;
namespace MotionSystem.Components.TowerSystem
{
    [GenerateAuthoringComponent]
    public struct TowerController : IComponentData
    {
        public int RateOfFire;
        public int RoundsPerShot;
        [HideInInspector] public int RoundsLeftToSpawn;
        public int ProjectileID;
        [Range(0.0f,1.0f)]
        public float Accuracy; //Should This be part of the round or weapon???
        [BurstDiscard]
        [SerializeField]public ProjectileBase projectile => ProjectileDatabase.GetProjectile(ProjectileID);
    }

    public enum RangeWeaponType { 
        None, Assault_Rifle,Rifle, Chemical, Artillery
    }
}