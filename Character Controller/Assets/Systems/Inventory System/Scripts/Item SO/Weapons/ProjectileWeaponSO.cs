using System.Collections.Generic;
using DreamersInc.DamageSystem.Interfaces;
using Sirenix.Utilities;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class ProjectileWeaponSO : WeaponSO, IProjectileWeapon
    {
        public List<ProjectileSpellSO> EquippedSpell => equippedSpell;
        private List<ProjectileSpellSO> equippedSpell;

        public List<ProjectileSpellSO> BakedSpell => bakedSpell; // TODO rename 
       [SerializeField] private List<ProjectileSpellSO> bakedSpell;
        private ProjectileSpellSO currentSpell = null;
        [SerializeField] uint maxNumOfSpells = 3;
        [SerializeField] private TypeOfDamage typeOfDamage => TypeOfDamage.Projectile;

        public override bool Equip(BaseCharacterComponent player)
        {
            if (!base.Equip(player)) return false;
            if (bakedSpell.IsNullOrEmpty())
            {
               Debug.LogError("NO Baked Spells");
            }
            currentSpell = BakedSpell[0];
            WeaponModel.GetComponent<IDamageDealer>().SetStatData(player, typeOfDamage);
            return true;
        }

        public void EquipSpell( ProjectileSpellSO spell )
        {
            if(equippedSpell.Count < maxNumOfSpells)
                equippedSpell.Add( spell );
        }

        public override void DrawWeapon(Animator anim)
        {
            base.DrawWeapon(anim);
            anim.SetBool("Projectile Drawn", true);
        }

        public override void StoreWeapon(Animator anim)
        {
            base.StoreWeapon(anim);
            anim.SetBool("Projectile Drawn", false);

        }

        public void FireSpell()
        {
            currentSpell.Activate(this, CharacterEquipped, Entity.Null );
        }
        
        public void FireChargedSpell()
        {
        }
    }
}