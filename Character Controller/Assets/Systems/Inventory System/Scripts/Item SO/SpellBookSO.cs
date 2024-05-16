using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamers.InventorySystem.Interfaces;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class SpellBookSO : WeaponSO
    {
        public List<SpellSO> Spells;
        public uint Storage => storage;
        [SerializeField] private  uint storage;
        private uint storageUsed;
        [SerializeField]public bool SpellBookFilled => storageUsed >= storage;
        public int CurIndex { get; private set; }

        public Vector3 CurSheathedPos { get; set; }
        public Vector3 CurHeldPos { get; set; }
        public Vector3 CurSheathedRot { get; set; }
        public Vector3 CurHeldRot { get; set; }

        public int CurComboID { get;  set; }
        [SerializeField] private int defaultComboID;
        private bool delayInputCheck;
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
            if (Index > CurIndex)
            {
                Index = 0;
                Revert();
                
            }
 
           
            var spell = Spells[Index];
            var stats =
                World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<BaseCharacterComponent>(entity);

            if (spell.ManaCost > stats.CurMana) return;
           
            stats.AdjustMana(-(int)Spells[Index].ManaCost);
            Destroy(WeaponModel);
            spell.Activate(this, stats);
            
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
            CurComboID = defaultComboID;
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