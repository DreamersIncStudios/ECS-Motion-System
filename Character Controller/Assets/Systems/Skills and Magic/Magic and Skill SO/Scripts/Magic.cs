using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillMagicSystem.AbilityEffects;
using Unity.Mathematics;
using Stats;
using Unity.Entities;
using DreamersInc.DamageSystem.Interfaces;

namespace SkillMagicSystem
{

    [CreateAssetMenu(fileName = "Magic", menuName = "Magic and Skills/Magic")]
    public class Magic : BaseAbility
    {

        public int chance { get; private set; }
        public int Amount { get; private set; }

        public void AddModStat() { }
        public void RemoveModStat() { }

        public int2 GridSize;
        public Shape GridShape;
        public Color MapColor { get; private set; }
        public bool Rotatable { get; private set; }
        public Dir dir;
        public AnimTrigger test;


        public override void Activate(BaseCharacter User, BaseCharacter targetCharacter = null)
        {
            if (targetCharacter == null)
                targetCharacter = User;

            if (CanCast(User))
            {
                EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
                em.AddComponentData(User.SelfEntityRef, new AdjustMana() { Value = -ManaRqd });
                //User.AdjustMana(-ManaRqd);// Todo Need to create Mana 
                foreach (BaseEffect effect in Effects)
                {
                    effect.Activate(targetCharacter, Amount, chance);
                }
            }
        }

        public override void Deactivate(BaseCharacter User, BaseCharacter targetCharacter = null)
        {
            if (targetCharacter = null)
                targetCharacter = User;

            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            em.AddComponentData(User.SelfEntityRef, new AdjustMana() { Value = ManaRqd });
            foreach (BaseEffect effect in Effects)
            {
                effect.Deactivate(targetCharacter);
            }
        }
        public override void Equip(BaseCharacter targetCharacter)
        {
            if (Trigger != TriggerTypes.OnEquip)
                return;
            foreach (BaseEffect effect in Effects)
            {
                if (effect.GetTrigger == TriggerTypes.OnEquip)
                    effect.Activate(targetCharacter, Amount, chance);
            }
        }
        public override void Unequip(BaseCharacter targetCharacter)
        {
            if (Trigger != TriggerTypes.OnEquip)
                return;

            foreach (BaseEffect effect in Effects)
            {
                if (effect.GetTrigger == TriggerTypes.OnEquip)
                    effect.Activate(targetCharacter, Amount, chance);
            }
        }

        public void AddToGrid()
        {  //Todo Add Later

        }
        public void RemoveFromGrid() { }
        public void DisplayUI() { }
        public void WriteToTextFile()
        {
            throw new System.NotImplementedException();
        }
    }
}
