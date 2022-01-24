using DreamersInc.DamageSystem.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem.ProjectileSystem
{
    public class ProjectileBase
    {
        public uint ProjectileID;
        public RoundType roundType;
        public Trajectory Trajectory;
        public int BaseDamage;
        public float DamageRadius;
        public Element element;
        public GameObject GO;
        //TODO Particle Effect system Need to be added 
    }

    public class ProjectileDatabase {
        static public bool isLoaded { get; private set; }
        private TextAsset ProjectileData;
        private static void ValidateDatabase() { }
        public static void LoadDatabase()
        {
            if (isLoaded)
                return;
            LoadDatabaseForce();
        }
        public static void LoadDatabaseForce() {
            TextAsset textFile = Resources.Load("ProjectileData") as TextAsset;

        }
        public static void ClearDataBase() { }
        public static ProjectileBase GetProjectile() {

            return null;
        }

    }

    public enum RoundType { 
        Bullet, Energy, Magic, Grenade, Mortar, Missle    }
    public enum Trajectory { 
        Linear, Curve, Guide,Tracking
    }
}