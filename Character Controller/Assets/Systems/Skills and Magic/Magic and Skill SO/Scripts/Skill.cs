using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillMagicSystem.AbilityEffects;
using Unity.Mathematics;
using Stats;


namespace SkillMagicSystem
{
    [CreateAssetMenu(fileName = "Skill", menuName ="Magic and Skills/Skills")]
    public class Skill : BaseAbility
    {

        public int chance { get; private set; }
        public int Amount { get; private set; }


        public override void Activate(BaseCharacter User, BaseCharacter targetCharacter= null)
        {
            if (targetCharacter == null)
                targetCharacter = User;

            if (CanCast(User))
            {
                foreach (BaseEffect effect in Effects)
                {
                    effect.Activate(targetCharacter, Amount, chance);
                }
            }
        }
        public override void Deactivate(BaseCharacter User, BaseCharacter targetCharacter) { }
        public override void Equip(BaseCharacter baseCharacter) { }
        public override void Unequip(BaseCharacter baseCharacter) { }
        public void AddToGrid()
        {  //Todo Add Later

        }
        public void RemoveFromGrid() { }
        public void DisplayUI() { }
        public void WriteToTextFile()
        {
            throw new System.NotImplementedException();
        }

        public int2 GridSize;
        public Shape GridShape;
        public Color MapColor { get; private set; }
        public bool Rotatable { get; private set; }
        public Dir dir;
    }
}
