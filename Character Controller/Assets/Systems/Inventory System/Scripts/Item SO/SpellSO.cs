using Dreamers.InventorySystem.Interfaces;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public abstract class SpellSO : ItemBaseSO,ISpells
    {
        
        [Header("Spell Info")] public uint Size => size;

        [SerializeField] private uint size;
        public int ManaCost => -manaCost;
        [SerializeField] private int manaCost;
        public int ComboID => comboID;
        [SerializeField] private int comboID;
       
        
        public abstract void Activate(WeaponSO weaponSO, BaseCharacterComponent player,Entity entity);
        
        public abstract void Deactivate(WeaponSO weaponSo, BaseCharacterComponent player, Entity entity);

    }
}