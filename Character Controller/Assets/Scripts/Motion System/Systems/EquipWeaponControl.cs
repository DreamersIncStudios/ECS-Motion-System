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
        Entity characterEntity;
        EntityManager manager;
        Animator anim;
        public float Delay = 10f;
        WeaponDamage damage;
        
        private void Start()
        {
            characterEntity = GetComponent<CharacterInventory>().self;
            anim = GetComponent<Animator>();
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            damage = GetComponentInChildren<WeaponDamage>();

        }
        public void EquipWeaponAnim()
        {
            CharControllerE temp = manager.GetComponentData<CharControllerE>(characterEntity);
            temp.TimerForEquipReset = Delay;
            manager.SetComponentData<CharControllerE>(characterEntity, temp);
            anim.SetBool("Weapon In Hand", true);
        }

        public void UnequipWeaponAnim()
        {
            anim.SetBool("Weapon In Hand", false);

        }
        public void DoDamage(int value) {
            damage.SetDamageBool(value == 1 ? true : false);
        }

    }
}