using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using DreamersInc.DamageSystem;
using DreamersInc.CombatSystem;
using System.Linq;
using DG.Tweening;
using DreamersInc.DamageSystem.Interfaces;
using Sirenix.Utilities;

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

        public void DoDamage(int value)
        {
            if (!damage)
            {
                damage = GetComponentInChildren<WeaponDamage>();
            }

            damage.SetDamageBool(value >= 1 ? true : false);
        }


        public void Warp()
        {
            Debug.Log("WarpStrike");

            var visibleEnemies = GetVisibleEnemies();

            visibleEnemies = visibleEnemies.Where(enemy => !enemy.GetCollider.tag.Equals("Player")).ToList();

            Debug.Log(visibleEnemies.Count);
            // Tween to the location.
            if (visibleEnemies.Count == 0) return;
            transform.DOMove(visibleEnemies[0].GetCollider.transform.position, .75f);
            // Shader Magic 
        }

        [SerializeField] private int warpRange = 20;
        private List<IDamageable> GetVisibleEnemies()
        {
            return FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>()
                .Where(enemy => IsVisibleByMainCamera(enemy) && IsInFrontOfPlayer(enemy))
                .Where(enemy => Vector3.Distance(this.transform.position, enemy.GetCollider.transform.position)<=warpRange)
                .OrderBy(enemy => Vector3.Distance(this.transform.position, enemy.GetCollider.transform.position))
                .ToList();
        }

        private bool IsVisibleByMainCamera(IDamageable enemy)
        {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main),
                enemy.GetCollider.bounds);
        }

        private bool IsInFrontOfPlayer(IDamageable enemy)
        {
            return Vector3.Dot((enemy.GetCollider.transform.position - transform.position).normalized,
                transform.forward) > 0;
        }
    }
}