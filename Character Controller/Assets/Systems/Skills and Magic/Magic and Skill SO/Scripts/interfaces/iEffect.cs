using Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillMagicSystem.AbilityEffects
{
    public interface iEffect 
    {
        public TriggerTypes GetTrigger { get; }
        public Targets GetTarget { get; }

        public GameObject EffectVFX { get; }
        public float Duration { get; }
        public void Activate(BaseCharacter baseCharacter, int amount, int chance);
        public void Deactivate(BaseCharacter Target, int amount);
        public bool ActivateOnChance(int chance);
      
    }
    public interface IOnHitEffect {

        public bool DoHitAction { get;  }
        public void OnHit(BaseCharacter Target, int amount);
        public void OnChanceCheck();
 
    }
    public interface IOnKillEffect
    {
        public bool DoHitAction { get; }

        public void OnKill(BaseCharacter Target, int amount);
        public void OnChanceCheck();


    }
    public interface IOnCommand { 
        public float Delay { get; }
        public void OnCommand(BaseCharacter Target, int Amount);
    }
    public interface IOnEequip { 
        public void OnEquip(BaseCharacter Target);
        public void OnUnequip(BaseCharacter Target);
    }
    public interface IOnPlayerDeath { 
        public void OnPlayerDeath(BaseCharacter Target,int amount);
        public void OnChanceCheck();


    }

    public interface IOnTimeEffect {
        public void OnTimer(BaseCharacter Target , int amount, float interval);
        public void CancelTimer();
     
    }
    public interface IOnGetHit {
        public bool DoHitAction { get; }

        public void OnGetHit(BaseCharacter Target);
        public void OnChanceCheck();


    }
    public interface IOnCommandHit
    {
        public float Delay { get; }
        public bool DoHitAction { get; }

        public void OnCommandHit(BaseCharacter Target, int Amount);
        public void OnChanceCheck();


    }
    public interface IOnCommandTimer
    {
        public float Duration { get; }
        public void OnCommandAddTimer(BaseCharacter Target, int Amount);
        public void CancelTimerCommand();
   
    }

}

