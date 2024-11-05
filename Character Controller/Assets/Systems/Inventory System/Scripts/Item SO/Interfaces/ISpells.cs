using Dreamers.InventorySystem;
using Stats;
using Stats.Entities;
using Unity.Entities;

namespace Dreamers.InventorySystem.Interfaces
{
    
    public interface ISpells
    {
        public uint Size { get; }
        public int ManaCost { get; }

        public void Activate(WeaponSO weaponSo, BaseCharacterComponent player,Entity entity);
        public void Deactivate(WeaponSO weaponSo, BaseCharacterComponent player,Entity entity);
     
    }
}

    
