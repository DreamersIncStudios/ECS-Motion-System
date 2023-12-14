using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using DreamersInc.DamageSystem;
using DreamersInc.CombatSystem;

namespace MotionSystem.Systems{
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

            if(!anim)
                anim = GetComponent<Animator>();

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (!anim.GetBool(Property))
                anim.SetBool(Property, true);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>().Update(World.DefaultGameObjectInjectionWorld.Unmanaged);
        }

        public void UnequipWeaponAnim()
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (anim.GetBool(Property) && stateInfo.IsTag("Unequip"))
            anim.SetBool(Property, false);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EquipSystem>().Update(World.DefaultGameObjectInjectionWorld.Unmanaged);
        }
        public void CalculateCriticalHit() {
            if (!damage)
            {
                damage = GetComponentInChildren<WeaponDamage>();
            }
            damage.CheckChance();
        }

        public void DoDamage(int value) {
            if (!damage)
            {
                damage = GetComponentInChildren<WeaponDamage>();
            }
            damage.SetDamageBool(value >= 1 ? true : false);
        }

    }
}