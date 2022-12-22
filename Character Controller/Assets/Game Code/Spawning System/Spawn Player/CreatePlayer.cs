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

public class CreatePlayer : MonoBehaviour
{
    public GameObject SpawnParent;
    public GameObject PlayerOption; //Todo make an array and pull info from GM on which character to spawn
    public CharacterClass Info;
    public bool Party;
    public bool IsPlayer;
    public bool CombatCapable;
    [Header("Weapon Specs")]
    public float EquipResetTimer = 5.0f;
    [Header("Animation Movement Specs")]
    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    [SerializeField] float m_JumpPower = 12f;
    [Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
    [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField] float m_MoveSpeedMultiplier = 1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    [SerializeField] float m_GroundCheckDistance = 0.1f;
    [SerializeField] float3 GroundProbeVector;

   public ComboSO combo;

    public InventoryBase inventory;
    public EquipmentSave equipment;

    class Baking : Baker<CreatePlayer>
    {
        public override void Bake(CreatePlayer authoring)
        {

            if (authoring.SpawnParent == null)
            {
                authoring.SpawnParent = GameObject.FindWithTag("Respawn");
            }
            var go = Instantiate(authoring.PlayerOption, authoring.SpawnParent.transform);
            CameraControl.Instance.Follow.LookAt = go.GetComponentInChildren<LookHereTarget>().transform;
            CameraControl.Instance.Follow.Follow = go.transform;
            var Capsule = go.GetComponent<CapsuleCollider>();
            var anim = go.GetComponent<Animator>();
            var RB = go.GetComponent<Rigidbody>();
            BaseCharacterComponent character = new();
            character.SetupDataEntity(authoring.Info,go);
            AddComponentObject(character);
            AddComponentObject(new AnimatorComponent()
            {
                anim = anim,
                RB = RB,
                transform = anim.transform,
            });
            AddComponent(new PlayerTag());
            AddComponent(new AttackTarget());
            AddComponentObject(new Command());
            CharControllerE controllerData = new CharControllerE()
            {
                CapsuleRadius = Capsule.radius,
                OGCapsuleHeight = Capsule.height,
                OGCapsuleCenter = Capsule.center,
                CapsuleCenter = Capsule.center,
                CapsuleHeight = Capsule.height,
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
            }

          ;

            AddComponent(controllerData);
            AddComponentObject(new TransformGO() { transform = go.transform });
            if (authoring.IsPlayer)
                AddComponent(new Player_Control() { });
            var inventoryData = new CharacterInventory();
            inventoryData.Setup(authoring.inventory, authoring.equipment,character);
            AddComponentObject(inventoryData);
            var comboInfo = Instantiate(authoring.combo);
            go.GetComponent<VFXControl>().Init(comboInfo);
            AddComponentObject(new PlayerComboComponent { Combo = comboInfo });
        }
    }
}
