using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem;
using MotionSystem.Components;
using DreamersInc.DamageSystem;

namespace MotionSystem.Systems{
    public class EquipWeaponControl : MonoBehaviour
    {

        Animator anim;
        WeaponDamage damage;
        AnimatorStateInfo stateInfo;
        private void Start()
        {
            anim = GetComponent<Animator>();
            damage = GetComponentInChildren<WeaponDamage>();
        }
        public void EquipWeaponAnim()
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim.GetBool("Weapon In Hand"))
                anim.SetBool("Weapon In Hand", true);
        }

        public void UnequipWeaponAnim()
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (anim.GetBool("Weapon In Hand") && stateInfo.IsTag("Unequip"))
            anim.SetBool("Weapon In Hand", false);

        }
        public void CalculateCriticalHit() {
            if (!damage)
            {
                damage = GetComponentInChildren<WeaponDamage>();
            }
            damage.CheckChance();
        }

        public void DoDamage(int value) {
            if (!damage) { 
            damage = GetComponentInChildren<WeaponDamage>();
            }
            damage.SetDamageBool(value >= 1 ? true : false);
        }

    }
}