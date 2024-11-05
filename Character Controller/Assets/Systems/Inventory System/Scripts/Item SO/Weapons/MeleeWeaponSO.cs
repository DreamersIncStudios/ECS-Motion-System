using DreamersInc.DamageSystem.Interfaces;
using Sirenix.OdinInspector;
using Stats.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class MeleeWeaponSO : WeaponSO, IMeleeWeapon
    {
        
        [SerializeField] private bool breakable;
        public bool Breakable => breakable;
        [SerializeField] private TypeOfDamage typeOfDamage => TypeOfDamage.Melee;

        [ShowIf("Breakable")]
        [SerializeField] private float maxDurable;
        public float MaxDurability => maxDurable;
        public float CurrentDurability { get; set; }
        public override bool Equip(BaseCharacterComponent player)
        {
            if (!base.Equip(player)) return false;
            WeaponModel.GetComponent<IDamageDealer>().SetStatData(player, typeOfDamage);
            return true;

        }
    }
}