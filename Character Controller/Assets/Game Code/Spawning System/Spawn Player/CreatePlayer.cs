using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem;
using Stats.Entities;
using Dreamers.InventorySystem.Base;
using Unity.Mathematics;
using DreamersStudio.CameraControlSystem;
using MotionSystem.Components;
using MotionSystem;
using DreamersInc;
using DreamersInc.ComboSystem;
using AISenses.VisionSystems.Combat;
using DreamersInc.InflunceMapSystem;
using AISenses;
using GameCode.Spawn;
using Stats;

public class CreatePlayerAuthoring : MonoBehaviour
{
    //public GameObject SpawnParent;
    public GameObject PlayerOption;
    public CharacterClass Info;
    public bool Party;
    public bool IsPlayer;
    public bool CombatCapable;
    [Header("Weapon Specs")]
    public float EquipResetTimer = 5.0f;
    [Header("Animation Movement Specs")]
    [SerializeField]  float m_MovingTurnSpeed = 360;
    [SerializeField]  float m_StationaryTurnSpeed = 180;
    [SerializeField]  float m_JumpPower = 12f;
    [Range(1f, 4f)][SerializeField]  float m_GravityMultiplier = 2f;
    [SerializeField]  float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField]  float m_MoveSpeedMultiplier = 1f;
    [SerializeField]  float m_AnimSpeedMultiplier = 1f;
    [SerializeField] float m_GroundCheckDistance = 0.1f;
    [SerializeField]  float3 GroundProbeVector;

    public ComboSO combo;
    [Header("Inventory")]

    public InventoryBase inventory;
    public EquipmentSave equipment;
    public int factionID;

    [Header("influence ")]
    public int Threat;
    public int Protection;



    public class PlayerBaking : Baker<CreatePlayerAuthoring>
    {
        public override void Bake(CreatePlayerAuthoring authoring)
        {
            EquipmentSave testing = new()
            {
                EquippedWeapons = new(),
                EquippedArmors = new()
            };
            foreach (WeaponSO so in authoring.equipment.EquippedWeapons) { 
                testing.EquippedWeapons.Add(Instantiate(so));
            }
            foreach (ArmorSO so in authoring.equipment.EquippedArmors)
            {
                testing.EquippedArmors.Add(Instantiate(so));
            }
            AddComponentObject(new SpawnGO()
            {
                SpawnThis = authoring.PlayerOption,
                Info = authoring.Info,
                equipment = testing
            }); ;
           

              BaseCharacterComponent character = new();
              character.SetupDataEntity(authoring.Info);

              AddComponentObject(character);
              AddComponent(new PlayerTag());
              AddComponent(new AttackTarget());
              AddComponentObject(new Command());
            CharControllerE controllerData = new ()
            {
                CapsuleRadius = .4f,
                OGCapsuleHeight = 1.9f,
                OGCapsuleCenter = new float3(0,.95f,0),
                CapsuleCenter = new float3(0, .95f, 0),
                CapsuleHeight = 1.9f,
                IsGrounded = true,
                AI = false,
                CombatCapable = authoring.CombatCapable,
                EquipResetTimer = authoring.EquipResetTimer,
                m_AnimSpeedMultiplier = authoring.m_AnimSpeedMultiplier,
                m_GravityMultiplier = authoring.m_GravityMultiplier,
                m_JumpPower = authoring.m_JumpPower,
                m_MoveSpeedMultiplier = authoring.m_MoveSpeedMultiplier,
                m_MovingTurnSpeed = authoring.m_MovingTurnSpeed,
                m_RunCycleLegOffset = authoring.m_RunCycleLegOffset,
                m_StationaryTurnSpeed = authoring.m_StationaryTurnSpeed,
                m_OrigGroundCheckDistance = authoring.m_GroundCheckDistance,
                GroundCheckDistance = authoring.m_GroundCheckDistance
            };

              AddComponent(controllerData);
       
            AddComponent(new Player_Control() { });
           // var inventoryData = new CharacterInventory();
           // inventoryData.Setup(authoring.inventory, authoring.equipment, character);
           //AddComponentObject(inventoryData);
            var comboInfo = Object.Instantiate(authoring.combo);
            AddComponentObject(new PlayerComboComponent { Combo = comboInfo });
            AddComponent(new InfluenceComponent
            {
                factionID = authoring.factionID,
                Protection = authoring.Protection,
                Threat = authoring.Threat
            });
            AddComponent(new Perceptibility
            {
                movement = MovementStates.Standing_Still,
                noiseState = NoiseState.Normal,
                visibilityStates = VisibilityStates.Visible
            });
            AddBuffer<ScanPositionBuffer>();
        }
    }
}


