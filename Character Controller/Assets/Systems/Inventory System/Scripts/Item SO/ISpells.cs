using Dreamers.InventorySystem.Interfaces;
using Stats;
using Stats.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    
    public interface ISpells
    {
        public uint Size { get; }
        public uint ManaCost { get; }

        public void Activate(SpellBookSO spellBookSo, BaseCharacterComponent player);
    }

    public abstract class SpellSO : ItemBaseSO,ISpells
    {
        
        [Header("Spell Info")] public uint Size => size;

        [SerializeField] private uint size;
        public uint ManaCost => manaCost;
        [SerializeField] private uint manaCost;

        public override void Use(CharacterInventory characterInventory, BaseCharacter player)
        {
            throw new System.NotImplementedException();
        }
        public abstract void Activate(SpellBookSO spellBookSo, BaseCharacterComponent player);
    }
}

    
