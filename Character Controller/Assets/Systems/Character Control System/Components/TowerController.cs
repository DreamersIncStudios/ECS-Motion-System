using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace MotionSystem.Components.TowerSystem
{
    public struct TowerController : IComponentData
    {
        public int RateOfFire;
        public int RoundsPerShot;
        [HideInInspector] public int RoundsLeftToSpawn;
        public int ProjectileID;
        [Range(0.0f,1.0f)]
        public float Accuracy;

    }

    public enum RangeWeaponType { 
        None, Assault_Rifle,Rifle, Chemical, Artillery
    }
}