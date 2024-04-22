using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.DamageSystem;
using DreamersInc.CombatSystem;
using System.Linq;
using DG.Tweening;
using DreamersInc.DamageSystem.Interfaces;

namespace MotionSystem.Systems
{
    public class EquipWeaponControl : MonoBehaviour
    {
        Animator anim;
        WeaponDamage damage;
        AnimatorStateInfo stateInfo;
        private static readonly int Property = Animator.StringToHash("Weapon In Hand");

        private void Start()
        {
            anim = GetComponent<Animator>();
            damage = GetComponentInChildren<WeaponDamage>();
        }

        public void EquipWeaponAnim()
        {

            if (!anim)
                anim = GetComponent<Animator>();

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim.GetBool(Property))
                anim.SetBool(Property, true);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);
        }

        public void UnequipWeaponAnim()
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (anim.GetBool(Property) && stateInfo.IsTag("Unequip"))
                anim.SetBool(Property, false);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>()
                .Update(World.DefaultGameObjectInjectionWorld.Unmanaged);
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


        /// <summary>
        /// Warps the player to the location of the nearest visible enemy within the specified warp range.
        /// </summary>
        /// <remarks>
        /// This method finds the nearest visible enemies within the specified warp range and warps the player to the position of the nearest enemy.
        /// </remarks>
        /// <param name="warpRange">The maximum distance at which an enemy can be considered visible for warping.</param>
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