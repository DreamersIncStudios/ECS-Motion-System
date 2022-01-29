using DreamersInc.DamageSystem.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem.ProjectileSystem
{
    [Serializable]
    public struct ProjectileBase
    {
        public uint ProjectileID;
        public string Name;
        public RoundType roundType;
        public Trajectory Trajectory;
        public int BaseDamage;
        public float DamageRadius;
        public Element element;
        public GameObject GO;
        //TODO Particle Effect system Need to be added 
        //public string Description 
        //TODO add description
    }

    public static class ProjectileDatabase {
        static public bool isLoaded { get; private set; }
        static public List<ProjectileBase> Items;

        private static void ValidateDatabase() {

            if (Items == null || !isLoaded)
            {
                Items = new List<ProjectileBase>();
                isLoaded = false;
            }
            else { isLoaded = true; }
        }
        public static void LoadDatabase()
        {
            if (isLoaded)
                return;
            LoadDatabaseForce();
        }
        public static void LoadDatabaseForce() {
            ProjectileBase[] itemsToLoad = TextReadIn();
    
            foreach (var item in itemsToLoad)
            {
                if (!Items.Contains(item))
                    Items.Add(item);
            }

        }
        public static void ClearDataBase() {
            isLoaded = false;
            Items.Clear();

        }
        public static ProjectileBase GetProjectile(int ID) {
            ValidateDatabase();
            LoadDatabase();
            foreach (ProjectileBase item in Items)
            {
                if (item.ProjectileID == ID)
                    return item;
            }
            return default;
        }
       static ProjectileBase[] TextReadIn() { 
            TextAsset textFile = Resources.Load("ProjectileData") as TextAsset;
            var lines = textFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            ProjectileBase[] array = new ProjectileBase[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                array[i] = new ProjectileBase()
                {
                    ProjectileID = uint.Parse(parts[0]),
                    Name = parts[1],
                    roundType = (RoundType)Enum.Parse(typeof(RoundType), parts[2]),
                    Trajectory = (Trajectory)Enum.Parse(typeof(Trajectory), parts[3]),
                    BaseDamage = int.Parse(parts[4]),
                    DamageRadius = float.Parse(parts[5]),
                    element = (Element)Enum.Parse(typeof(Element), parts[6]),
                    GO = Resources.Load(parts[7]) as GameObject
                };
            }

            return array;
        }
    }

    public enum RoundType { 
        Bullet, Energy, Magic, Grenade, Mortar, Missle    }
    public enum Trajectory {
        Linear ,
        Curve,
        Guide,
        Tracking
    }
}