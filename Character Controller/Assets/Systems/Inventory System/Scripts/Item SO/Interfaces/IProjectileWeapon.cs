using System.Collections.Generic;

namespace Dreamers.InventorySystem
{

    public interface IProjectileWeapon
    {
        public List<ProjectileSpellSO> EquippedSpell { get; }
        public List<ProjectileSpellSO> BakedSpell { get; }
        public void EquipSpell(ProjectileSpellSO spell);
        public void FireChargedSpell();
        public void FireSpell();
    }
}