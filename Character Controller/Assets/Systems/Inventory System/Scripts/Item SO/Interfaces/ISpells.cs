using Dreamers.InventorySystem;
using Stats;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem.Interfaces
{
    
    public interface ISpells
    {
        public uint Size { get; }
        public uint ManaCost { get; }

        public void Activate(WeaponSO weaponSo, BaseCharacterComponent player,Entity entity);
        public void Activate(SpellBookSO spellBookSo, BaseCharacterComponent player,Entity entity);
        public void Deactivate(WeaponSO weaponSo, BaseCharacterComponent player,Entity entity);
        public void Deactivate(SpellBookSO spellBookSo, BaseCharacterComponent player,Entity entity);

    }

    public abstract class SpellSO : ItemBaseSO,ISpells
    {
        
        [Header("Spell Info")] public uint Size => size;

        [SerializeField] private uint size;
        public uint ManaCost => manaCost;
        [SerializeField] private uint manaCost;
        public int ComboID => comboID;
        [SerializeField] private int comboID;
        public override void Use(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Activate(SpellBookSO spellBookSo, BaseCharacterComponent player,Entity entity)
        {
        }

        public virtual void Activate(WeaponSO weaponSO, BaseCharacterComponent player,Entity entity)
        {
        }

        public virtual void Deactivate(WeaponSO weaponSo, BaseCharacterComponent player, Entity entity)
        {
        }

        public virtual void Deactivate(SpellBookSO spellBookSo, BaseCharacterComponent player, Entity entity)
        {
        }
    }
}

    
