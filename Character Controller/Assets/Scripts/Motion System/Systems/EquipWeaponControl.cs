using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Dreamers.InventorySystem;
using MotionSystem.Components;

namespace MotionSystem.System {
    public class EquipWeaponControl : MonoBehaviour
    {
        Entity characterEntity;
        EntityManager manager;
        Animator anim;
        public float Delay = 10f;
        private void Start()
        {
            characterEntity = GetComponent<CharacterInventory>().self;
            anim = GetComponent<Animator>();
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        }
        public void EquipWeaponAnim()
        {
            //   anim.SetBool("CanDoDamage", true);
            CharControllerE temp = manager.GetComponentData<CharControllerE>(characterEntity);
            temp.TimerForEquipReset = Delay;
            manager.SetComponentData<CharControllerE>(characterEntity, temp);
            anim.SetBool("Weapon In Hand", true);
        }

        public void UnequipWeaponAnim()
        {
            //   anim.SetBool("CanDoDamage", false);
            anim.SetBool("Weapon In Hand", false);

        }
    }
}