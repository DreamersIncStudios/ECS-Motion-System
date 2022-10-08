using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillMagicSystem
{
    [System.Serializable]
    public struct AnimTrigger 
    {
        public uint TriggerAnimIndex { get { return triggerAnimIndex; } set { triggerAnimIndex = value; } }
        public AttackType attackType;
        [SerializeField] uint triggerAnimIndex;
        public string TriggerString { get { return attackType.ToString() + TriggerAnimIndex; } }
        public float TransitionDuration;
        public float TransitionOffset;
        public float EndofCurrentAnim;
     

    }
    public enum AttackType
    {
        none, LightAttack, HeavyAttack, ChargedLightAttack, ChargedHeavyAttack, Projectile, ChargedProjectile, Grounded, Targeted_Locomation, Locomation_Grounded_Weapon,
        SpecialAttack

    }
}
