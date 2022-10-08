using Stats;
using UnityEngine;

namespace SkillMagicSystem.AbilityEffects
{
    public abstract class BaseEffect : ScriptableObject, iEffect {
        public TriggerTypes GetTrigger { get { return trigger; } }
        [SerializeField] TriggerTypes trigger;
        public Targets GetTarget { get { return target; } }
        [SerializeField] Targets target;
        public float Duration { get { return duration; } }
        [SerializeField] float duration = 3;
        public int VFXID { get { return vfxID; } }
        public float VFXDelay { get { return vfxDelay; } }
        public float VFXDuration { get { return vfxDuration; } }
        [SerializeField] int vfxID;
        [SerializeField] float vfxDelay;
        [SerializeField] float vfxDuration;

        public virtual void Activate(BaseCharacter Target, int amount =0 , int chance=100) { }
        public virtual void Deactivate(BaseCharacter Target, int amount=0) { }
        public bool ActivateOnChance(int chance)
        {

            int rndNum = Random.Range(0, 100);
            return chance >= rndNum;

        }
    }
}
