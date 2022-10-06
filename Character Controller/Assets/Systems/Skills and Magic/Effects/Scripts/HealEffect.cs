using Assets.Systems.Global.Function_Timer;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace SkillMagicSystem.AbilityEffects
{
    [CreateAssetMenu(fileName = "Healing Effect", menuName = "Magic and Skills/Heal Effect")]

    public class HealEffect : BaseEffect, IOnHitEffect, IOnKillEffect, IOnCommand, IOnTimeEffect, IOnPlayerDeath
    {

        [SerializeField] float range;
        [SerializeField] float intervalTime;
        public float Delay { get { return delay; } }
        [SerializeField] float delay;
        [SerializeField] int chance;

        public bool DoHitAction { get; private set; }
        public override void Activate(BaseCharacter Targart,int amount = 0, int chance =100) {
            base.Activate(Targart,amount,chance);
            Debug.Log("Called at SO");
            switch (GetTrigger) {
                case TriggerTypes.OnCommand:
                    OnCommand(Targart,amount);
                    break;
                case TriggerTypes.OnHit:
                    OnHit(Targart, amount);
                    break;
                case TriggerTypes.OnKill:
                    OnKill(Targart,amount);
                    break;
                case TriggerTypes.OnTimer:
                    OnTimer(Targart, amount, intervalTime);
                    break;
                case TriggerTypes.OnPlayerDeath:
                    OnPlayerDeath(Targart,amount);
                    break;

            }
        
        }
        public void OnChanceCheck()
        {
            DoHitAction = ActivateOnChance(chance);
        }

        public void OnHit(BaseCharacter Target, int amount)
        {
            if (DoHitAction)
            {
               Heal(Target,amount);
                DoHitAction=false;
            }
        }

        public void OnKill(BaseCharacter Target, int amount)
        {
            if (DoHitAction)
            {
                Heal(Target, amount);
                DoHitAction = false;

            }
        }

        public void OnCommand(BaseCharacter Target, int Amount)
        {
            Heal(Target, Amount);

        }
        FunctionTimer intervalTimer;
        public void OnTimer(BaseCharacter Target, int amount, float interval)
        {
            intervalTimer = FunctionTimer.Create(() =>
            {
                Heal(Target, amount); 
                
            }
                , interval, "Heal",true);
        }
        public void CancelTimer() {
            FunctionTimer.StopTimer("Heal");
        }
        public void OnPlayerDeath(BaseCharacter Target, int amount)
        {
            if (DoHitAction)
            {
                Heal(Target, amount);
                DoHitAction = false;

            }
        }

        

        void Heal(BaseCharacter Target, int Amount) {
            switch (GetTarget)
            {
                case Targets.Self:
                case Targets.Enemy:
                case Targets.TeamMember:

                    Target.TakeDamage(Amount, TypeOfDamage.Recovery, Element.Holy);

                    if (EffectVFX != null)
                    {
                        //Todo add position offset for VFX
                        var spawned = Instantiate(EffectVFX, Target.transform.position, Quaternion.identity).GetComponent<VisualEffect>(); // figure out how to postion 
                        spawned.transform.SetParent(Target.transform, false);
                        spawned.Play();
                        Destroy(spawned.gameObject, Duration);
                    }
                 
                    break;
                case Targets.AOE:
                    var Cols = Physics.OverlapSphere(Target.gameObject.transform.position, range);
                    foreach (Collider coll in Cols)
                    {
                        if (coll.GetComponent<BaseCharacter>())
                        {
                            var character = coll.GetComponent<BaseCharacter>();
                            character.TakeDamage(Amount, TypeOfDamage.Recovery, Element.Holy);
                        }
                    }
                    break;
            }

        }


    }
}
