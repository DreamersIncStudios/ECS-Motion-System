using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.DamageSystem;
using DreamersInc.CombatSystem;
using System.Linq;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.DamageSystem.Interfaces;
using PrimeTween;
using Stats;
using Stats.Entities;
using UnityEngine.VFX;
namespace MotionSystem.Systems
{
    public class EquipWeaponControl : MonoBehaviour
    {
        Animator anim;
        WeaponDamage damage;
        private UnityEngine.VFX.VisualEffect graph;
        AnimatorStateInfo stateInfo;
        public WeaponSO CurEquipWeapon { get; private set; }
        private static readonly int WeaponInHand = Animator.StringToHash("Weapon In Hand");

        private void Start()
        {
            anim = GetComponent<Animator>();
            damage = GetComponentInChildren<WeaponDamage>();
            if (damage?.GetComponentInChildren<UnityEngine.VFX.VisualEffect>())
                graph = damage?.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
        }

        public void EquipWeaponAnim()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if(!damage)
                damage = GetComponentInChildren<WeaponDamage>();
            var entity = damage.transform.root.GetComponent<Damageable>().SelfEntityRef;

            if (!anim)
                anim = GetComponent<Animator>();
            
            if (!CurEquipWeapon)
            {
                var inventory = entityManager.GetComponentData<CharacterInventory>(entity);
                 if(!inventory.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary,out var temp)) 
                     return;
                 CurEquipWeapon = temp;
                 
            }

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim.GetBool(WeaponInHand))
                anim.SetBool(WeaponInHand, true);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);

            var stats = entityManager.GetComponentData<BaseCharacterComponent>(entity);
            CurEquipWeapon.ActiveSpell?.Activate(CurEquipWeapon,stats,entity);
        }

        public void UnequipWeaponAnim()
        {
            var entity =damage.transform.root.GetComponent<Damageable>().SelfEntityRef;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (!CurEquipWeapon)
            {
                var inventory = entityManager.GetComponentData<CharacterInventory>(entity);
                if(!inventory.Equipment.EquippedWeapons.TryGetValue(WeaponSlot.Primary,out var temp)) 
                    return;
                CurEquipWeapon = temp;
                 
            }
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (anim.GetBool(WeaponInHand) && stateInfo.IsTag("Unequip"))
                anim.SetBool(WeaponInHand, false);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);
            
            var stats = entityManager.GetComponentData<BaseCharacterComponent>(entity);
            CurEquipWeapon.ActiveSpell.Deactivate(CurEquipWeapon,stats,entity);

        }

        public void CalculateCriticalHit()
        {
            if (!damage)
            {
                damage = GetComponentInChildren<WeaponDamage>();
            }

            damage.CheckChance();
        }

        /// <summary>
        /// Turn on a Damage detection of weapons attached to character
        /// </summary>
        /// <param name="value">True or False by int .</param>
        public void DoDamage(int value)
        {
            if (!damage)
            {
                damage = GetComponentInChildren<WeaponDamage>();
            }

            damage.SetDamageBool(value >= 1 ? true : false);
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
    }
}