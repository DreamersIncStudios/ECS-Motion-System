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
using Unity.Mathematics;
using Sirenix.OdinInspector;

namespace DreamersInc.BestiarySystem
{
    public class CreatureInfo : ScriptableObject
    {
        [SerializeField] private uint creatureID;
        public uint ID { get { return creatureID; } }
       
        public int FactionID { get; set; }

        public string Name;
        [EnumToggleButtons] public CreatureType creatureType;
        [EnumToggleButtons] public NPCType GetNPCType;

        public CharacterClass stats;
        public GameObject Prefab;
        public PhysicsInfo PhysicsInfo;
        public MovementData Move;
        public float3 CenterOffset;

        [Header("influence ")]
        public int factionID;
        public int BaseThreat;
        public int BaseProtection;
        public ComboSO Combo;
        public EquipmentSave Equipment;
        public InventorySave Inventory;
        public uint ExpGiven;
        public bool hasAttack;
#if UNITY_EDITOR

        public void setItemID(uint ID)
        {

            this.creatureID = ID;
        }
#endif
    }

    public enum CreatureType { biped, quadruped, mecha, spirit, stationary, flying }

    public enum NPCType { vendor, Noncombatant, Combatant, Feral, Trained }

#if UNITY_EDITOR
    public static partial class Creator
    {
        [MenuItem("Assets/Create/Bestiary/Creature Info")]
        static public void CreateCreatureInfo()
        {
            Dreamers.Global.ScriptableObjectUtility.CreateAsset<CreatureInfo>("Creature", out CreatureInfo info);
            BestiaryDB.LoadDatabase(true);
            info.setItemID((uint)BestiaryDB.Creatures.Count + 1);
        }

    }
#endif
}