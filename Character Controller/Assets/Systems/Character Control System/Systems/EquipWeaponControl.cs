using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem;
using MotionSystem.Components;
using DreamersInc.DamageSystem;

namespace MotionSystem.System {
    public class EquipWeaponControl : MonoBehaviour
    {

        Animator anim;
        WeaponDamage damage;
        
        private void Start()
        {
            anim = GetComponent<Animator>();
            damage = GetComponentInChildren<WeaponDamage>();

        }
        public void EquipWeaponAnim()
        {
 
            anim.SetBool("Weapon In Hand", true);
        }

        public void UnequipWeaponAnim()
        {
            anim.SetBool("Weapon In Hand", false);

        }
        public void DoDamage(int value) {
            if (!damage) { 
            damage = GetComponentInChildren<WeaponDamage>();
            }
            damage.SetDamageBool(value == 1 ? true : false);
        }

    }
}