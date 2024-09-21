using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.DamageSystem;
using DreamersInc.CombatSystem;
using System.Linq;
using System.Xml.Serialization;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.DamageSystem.Interfaces;
using PrimeTween;
using Stats;
using Stats.Entities;
using UnityEngine.VFX;
namespace MotionSystem.Systems
{
    /// <summary>
    /// Controls the equipping and unequipping of weapons for a character, along with functionality for damaging and visual effects.
    /// </summary>
    public class EquipWeaponControl : MonoBehaviour
    {
        Animator anim;
        WeaponDamage meleeDamage;
        WeaponDamage rangeDamage;
        private UnityEngine.VFX.VisualEffect graph;
        AnimatorStateInfo stateInfo;
        public WeaponSO CurEquipWeapon { get; private set; }
        public ProjectileWeaponSO CurProjectileWeapon { get; private set; }
        private static readonly int WeaponInHand = Animator.StringToHash("Weapon In Hand");
        EntityManager entityManager;
        private Entity selfEntity;
        private void Start()
        {
             entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            anim = GetComponent<Animator>();
            GetWeapons();

            if (meleeDamage?.GetComponentInChildren<UnityEngine.VFX.VisualEffect>())
                graph = meleeDamage?.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
        }

        private void GetWeapons()
        {
            var weaponDamages = GetComponentsInChildren<WeaponDamage>();
            foreach (var weapon in weaponDamages)
            {
                switch (weapon.Type)
                {
                    case WeaponType.Sword:
                    case WeaponType.H2BoardSword:
                    case WeaponType.Katana:
                    case WeaponType.Bo_Staff:
                    case WeaponType.Mage_Staff:
                    case WeaponType.Club:
                    case WeaponType.Axe:
                    case WeaponType.Gloves:
                    case WeaponType.SpellBook:
                    case WeaponType.SpellBlade:
                    case WeaponType.Claws:
                        meleeDamage = weapon;
                        break;
                    case WeaponType.Pistol:
                    case WeaponType.Bow:
                        rangeDamage = weapon;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        /// Animates the equipping of a Melee weapon by setting appropriate animation states and
        /// managing necessary components such as WeaponDamage and CharacterInventory.
        /// </summary>
        public void EquipWeaponAnim()
        {
            if (!meleeDamage)
            {
                GetWeapons();
            }
            if(selfEntity == Entity.Null)
                selfEntity  = meleeDamage.transform.root.GetComponent<Damageable>().SelfEntityRef;

            if (!anim)
                anim = GetComponent<Animator>();

            if (!CurEquipWeapon && selfEntity != Entity.Null) 
            {
                if(!entityManager.HasComponent<CharacterInventory>(selfEntity))
                    return;
                var inventory = entityManager.GetComponentData<CharacterInventory>(selfEntity);
                 if(!inventory.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary,out var temp)) 
                     return;
                 CurEquipWeapon = temp;
                 
            }

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim.GetBool(WeaponInHand))
                anim.SetBool(WeaponInHand, true);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);

            var stats = entityManager.GetComponentData<BaseCharacterComponent>(selfEntity);
            CurEquipWeapon.ActiveSpell?.Activate(CurEquipWeapon,stats,selfEntity);
        }

        public void EquipProjectileWeaponAnim()
        {
            if (!rangeDamage)
            {
                GetWeapons();
            }
            if(selfEntity == Entity.Null)
                selfEntity  = meleeDamage.transform.root.GetComponent<Damageable>().SelfEntityRef;

            if (!anim)
                anim = GetComponent<Animator>();

            if (selfEntity == Entity.Null) return;
            if(!entityManager.HasComponent<CharacterInventory>(selfEntity))
                return;
            var inventory = entityManager.GetComponentData<CharacterInventory>(selfEntity);
            if(!inventory.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Projectile,out var temp)) 
                return;
            CurProjectileWeapon = (ProjectileWeaponSO)temp;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim.GetBool(WeaponInHand))
                anim.SetBool(WeaponInHand, true);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);

            var stats = entityManager.GetComponentData<BaseCharacterComponent>(selfEntity);
            CurProjectileWeapon.ActiveSpell?.Activate(CurProjectileWeapon,stats,selfEntity);
        }

        /// <summary>
        /// Handles the animation and logic for unequipping a melee weapon,
        /// updating necessary components such as the WeaponDamage, CharacterInventory, and animation states,
        /// while also deactivating any active spells associated with the current weapon.
        /// </summary>
        public void UnequipWeaponAnim()
        {
            if (!CurEquipWeapon)
            {
                var inventory = entityManager.GetComponentData<CharacterInventory>(selfEntity);
                if(!inventory.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary,out var temp)) 
                    return;
                CurEquipWeapon = temp;
                 
            }
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (anim.GetBool(WeaponInHand) && stateInfo.IsTag("Unequip"))
                anim.SetBool(WeaponInHand, false);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);
            
            var stats = entityManager.GetComponentData<BaseCharacterComponent>(selfEntity);
            CurEquipWeapon.ActiveSpell.Deactivate(CurEquipWeapon,stats,selfEntity);

        }

        public void CalculateCriticalHit()
        {
            if (!meleeDamage)
            {
                meleeDamage = GetComponentInChildren<WeaponDamage>();
            }

            meleeDamage.CheckChance();
        }

        /// <summary>
        /// Turn on a Damage detection of weapons attached to character
        /// </summary>
        /// <param name="value">True or False by int .</param>
        public void DoDamage(int value)
        {
            if (!meleeDamage)
            {
                meleeDamage = GetComponentInChildren<WeaponDamage>();
            }

            meleeDamage.SetDamageBool(value >= 1 ? true : false);
        }

        public void PlayVFX()
        {
            graph?.Play();
        }

        /// <summary>
        /// Warps the player to the location of the nearest visible enemy within the specified warp range.
        /// </summary>
        /// <remarks>
        /// This method finds the nearest visible enemies within the specified warp range and warps the player to the position of the nearest enemy.
        /// </remarks>
        public void Warp()
        {
            var visibleEnemies = GetVisibleEnemies();

            visibleEnemies = visibleEnemies.Where(enemy => !enemy.GetCollider.tag.Equals("Player")).ToList();

            // Tween to the location.
            if (visibleEnemies.Count == 0) return;
            transform.DOMove(visibleEnemies[0].GetCollider.transform.position, .75f);
            // Shader Magic 
        }

        [SerializeField] private int warpRange = 20;

        /// <summary>
        /// Returns a list of visible enemies within a specified warp range.
        /// </summary>
        /// <returns>A list of IDamageable objects representing the visible enemies.</returns>
        private List<IDamageable> GetVisibleEnemies()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDamageable>()
                .Where(enemy => IsVisibleByMainCamera(enemy) && IsInFrontOfPlayer(enemy))
                .Where(enemy => Vector3.Distance(this.transform.position, enemy.GetCollider.transform.position)<=warpRange)
                .OrderBy(enemy => Vector3.Distance(this.transform.position, enemy.GetCollider.transform.position))
                .ToList();
        }

        /// <summary>
        /// Check if the given IDamageable object is visible by the main camera.
        /// </summary>
        /// <param name="enemy">The IDamageable object to check.</param>
        /// <returns>True if the enemy is visible by the main camera, false otherwise.</returns>
        private bool IsVisibleByMainCamera(IDamageable enemy)
        {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main),
                enemy.GetCollider.bounds);
        }

        /// <summary>
        /// Determines if the specified enemy is in front of the player.
        /// </summary>
        /// <param name="enemy">The enemy to check.</param>
        /// <returns>True if the enemy is in front of the player, otherwise false.</returns>
        private bool IsInFrontOfPlayer(IDamageable enemy)
        {
            return Vector3.Dot((enemy.GetCollider.transform.position - transform.position).normalized,
                transform.forward) > 0;
        }
        
        public void FireSpell()
        {
            CurProjectileWeapon.FireSpell();
        }
    }
}