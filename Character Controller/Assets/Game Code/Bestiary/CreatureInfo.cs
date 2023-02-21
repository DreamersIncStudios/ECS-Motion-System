using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using IAUS.ECS;
using UnityEditor;
using Global.Component;
using DreamersInc.ComboSystem;
using Dreamers.InventorySystem.Base;
using MotionSystem.Components;

namespace DreamersInc.BestiarySystem
{
    public class CreatureInfo : ScriptableObject
    {
        [SerializeField] private uint creatureID;
        public uint ID { get { return creatureID; } }
        public string Name;
        public CharacterClass stats;
        public GameObject Prefab;
        public PhysicsInfo PhysicsInfo;
        public MovementData Move;
        [Header("influence ")]
        public int factionID;
        public int BaseThreat;
        public int BaseProtection;
        public ComboSO Combo;
        public EquipmentSave Equipment;
#if UNITY_EDITOR

        public void setItemID(uint ID)
        {

            this.creatureID = ID;
        }
#endif
    }

#if UNITY_EDITOR
    public static partial class Creator {
        [MenuItem("Assets/Create/Bestiary/Creature Info")]
        static public void CreateCreatureInfo() {
            Dreamers.Global.ScriptableObjectUtility.CreateAsset<CreatureInfo>("Creature", out CreatureInfo info);
            BestiaryDB.LoadDatabase(true);
            info.setItemID((uint)BestiaryDB.Creatures.Count + 1);
        }

    }
#endif
}