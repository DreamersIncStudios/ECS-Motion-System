using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.DamageSystem;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
namespace Dreamers.InventorySystem
{


    public class ModifierSpellSO : SpellSO
    {
        [SerializeField] private List<StatModifier> modifiers;
        [SerializeField] private Effects effects;
        private ParticleSystem vfxObject;
        

        public override void Activate(WeaponSO weapon, BaseCharacterComponent player, Entity entity)
        {
            player.ModCharacterStats(modifiers);
            weapon.SetEffect(effects, false);

            if(effects.VFX== null) return;
                vfxObject = Instantiate(effects.VFX, weapon.WeaponModel.transform ).GetComponent<ParticleSystem>();
            var weaponInHand = weapon.WeaponModel.transform.root.GetComponent<Animator>().GetBool("Weapon In Hand");
            if (!weaponInHand) return;
            vfxObject.Play();
            
            if (effects.StatusEffects == StatusEffects.None) return;
            
            World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<IncrementalVitalChange>(entity).Add(
                new IncrementalVitalChange(VitalName.Mana, -2, weapon.WeaponModel.gameObject.GetInstanceID()));

        }
        
        public override async void Deactivate(WeaponSO weapon, BaseCharacterComponent player, Entity entity)
        {
            player.ModCharacterStats(modifiers, false);
            var buffer = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<IncrementalVitalChange>(entity);
            for (var i = buffer.Length - 1; i >= 0; i--) 
            {
                if(buffer[i].ID == weapon.WeaponModel.gameObject.GetInstanceID())
                {
                    buffer.RemoveAt(i); 
                }
            }
            await Task.Delay(250);
            
            if(vfxObject== null)
                return;
            vfxObject.Stop();
            Destroy(vfxObject.gameObject);
        }
        
    }

    [System.Serializable]
    public struct Effects
    {
        public GameObject VFX;
        public ElementName ElementName;
        public StatusEffects StatusEffects;
        [Range(0,1)]public float Chance;
        public float Damage;
        public int Duration; //-1 continuous until cured using item or spell
        public int Repetition;
    }
}