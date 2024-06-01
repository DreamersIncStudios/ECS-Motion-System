using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dreamers.InventorySystem.Interfaces;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class SpellBookSO : WeaponSO
    {
        [SerializeField] List<SpellSO> spells;
        public List<SpellSO> Spells { get; private set; }
        public uint Storage => storage;
        [SerializeField] private  uint storage;
        private uint storageUsed;
        [SerializeField]public bool SpellBookFilled => storageUsed >= storage;
        public int CurIndex { get; private set; }

        public Vector3 CurSheathedPos { get; set; }
        public Vector3 CurHeldPos { get; set; }
        public Vector3 CurSheathedRot { get; set; }
        public Vector3 CurHeldRot { get; set; }

        public int CurComboID => curComboID;
        [SerializeField] private int curComboID;
        [SerializeField] private int defaultComboID;
        private bool delayInputCheck;

        public override bool Equip(BaseCharacterComponent player)
        {
            if (!base.Equip(player)) return false;
            Spells = new List<SpellSO> { BaseModelState };
            curComboID = BaseModelState.ComboID;
            
            activeSpell = Spells[0];
            
            foreach (var spell in spells.Where(spell => !SpellBookFilled && spell.Size < storage - storageUsed))
            {
                //Todo Add level Check 
                Spells.Add(spell);
                storageUsed += spell.Size;
            }

            return true;
        }

        public bool AddSpell(SpellSO spell)
        {
            if (!SpellBookFilled && spell.Size < storage - storageUsed)
            {
                //Todo Add level Check 
                Spells.Add(spell);
                storageUsed += spell.Size;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveSpell(SpellSO spell)
        {
            if (!Spells.Contains(spell)) return;
            storageUsed -= spell.Size;
            Spells.Remove(spell);
        }

        public void RemoveSpell(int index)
        {
            var spell = Spells[index];
            RemoveSpell(spell);
        }

        
        public void SwapSpell(int Index, Entity entity)
        {
            if(delayInputCheck) return;
          
            Debug.Log("swap spell");
            if (Index > spells.Count)
            {
                Index = 0;
                Revert();
            }
 
           
           
            var stats =
                World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<BaseCharacterComponent>(entity);

            if (Spells[Index].ManaCost > stats.CurMana) return;
            
            activeSpell.Deactivate(this, stats, entity);

            stats.AdjustMana(-(int)Spells[Index].ManaCost);
          //  Destroy(WeaponModel);
          Spells[Index].Activate(this, stats, entity);
            activeSpell = Spells[Index];

            curComboID = activeSpell.ComboID;
            CurIndex = Index;
           DelayInput();
        }

        async void DelayInput()
        {
            delayInputCheck = true;
            await Task.Delay(2500);
            delayInputCheck = false;
        }

        private void Revert()
        {
            CurIndex = 0;
            CurSheathedPos = SheathedPos;
            CurHeldPos = HeldPos;
            CurSheathedRot = SheathedRot;
            CurHeldRot = HeldRot;
        }

        public void OnValidate()
        {
            storageUsed = 0;
            foreach (var spell in Spells)
            {
                storageUsed += spell.Size;
            }

        }
    }

    
 
    
}