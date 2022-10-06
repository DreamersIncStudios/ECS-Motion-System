using Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillMagicSystem.AbilityEffects
{
    public class DrainEffect : BaseAbility, IOnCommand, IOnHitEffect, IOnCommandHit
    {
        public float Delay => throw new System.NotImplementedException();

        [SerializeField] int chance;

        public bool DoHitAction { get; private set; }
        public bool ActivateOnChance(int chance)
        {

            int rndNum = Random.Range(0, 100);
            return chance >= rndNum;

        }
        public void OnChanceCheck()
        {
            DoHitAction = ActivateOnChance(chance);
        }

        public void OnCommand(BaseCharacter Target, int Amount)
        {
            throw new System.NotImplementedException();
        }

        public void OnCommandHit(BaseCharacter Target, int Amount)
        {
            throw new System.NotImplementedException();
        }

        public void OnHit(BaseCharacter Target, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
