using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DreamersInc.ComboSystem
{
    [System.Serializable]
    public struct AnimationCombo
    {
        public AttackType Type;
        [SerializeField] uint triggerAnimIndex;
        public uint TriggerAnimIndex { get { return triggerAnimIndex; } set { triggerAnimIndex = value; } }
        public string CurrentStateName { get { return Type.ToString() + TriggerAnimIndex; } }
        public float2 NormalizedInputTime;
        public float AnimationEndTime;
        public bool InputAllowed(float time) => time > NormalizedInputTime.x && time < NormalizedInputTime.y;
        public float MaxProb { get; set; }
        public List<AnimationTrigger> Triggers;
        // TODO consider adding late inputs ??????

    }

    public interface ITrigger {
        public ComboNames Name { get; set; } // Change To String ???????????
        public string TriggerString { get; } 
    }
    [System.Serializable]
    public struct AnimationTrigger:ITrigger
    {
        [SerializeField] ComboNames name;
        public ComboNames Name { get { return name; }set { name = value; } } // Change To String ???????????
        public uint TriggerAnimIndex { get { return triggerAnimIndex; } set { triggerAnimIndex = value; } }
        public AttackType Type;
        [SerializeField] uint triggerAnimIndex;
        public string TriggerString { get { return Type.ToString() + TriggerAnimIndex; } }
        public bool Unlocked;
        public float TransitionDuration;
        public float TransitionOffset;
        [Tooltip(" testing Value")]
        public float EndofCurrentAnim;
        public float Chance;
        [Range(-1, 100)]
        [Tooltip("Value Must be between 0 and 100 \n " +
            "-1 is used for never repeat")]

        public int ChanceForNextAttack;
        public int LevelUnlocked;
        public float probabilityTotalWeight { get; set; }

        float probabilityPercent => Chance / probabilityTotalWeight * 100;
        public float probabilityRangeFrom { get; set; }
        float probabilityRangeTo => probabilityRangeFrom + Chance;
        public void SetRangeFrom(float StartPoint)
        {
            probabilityRangeFrom = StartPoint;
        }
        public bool Picked(float picked)
        {
            return picked > probabilityRangeFrom && picked < probabilityRangeTo;
        }

        //TODO add stat modifer increase or decrese likely hood of sequenctial attack based on stats
        public bool AttackAgain(float selected) {
            return selected <ChanceForNextAttack &&  ChanceForNextAttack !=-1;
        }
        public float delay;
        public bool trigger=> delay <= 0.0f;
        public void AdjustTime(float time) {
            delay -= time;
        }



    }
    [System.Serializable]
    public struct SetTrigger  {
        [SerializeField] ComboNames name;
        [SerializeField] ComboAnimNames triggerAnimName;

        public ComboNames Name { get { return name; } } // Change To String ???????????
        public ComboAnimNames TriggeredAnimName { get { return triggerAnimName; } } // Change to String ???????????

    }
    [System.Serializable]
    public struct ComboPattern
    {
        public ComboNames name;
       public  List<SetTrigger> Attacks;
    }
    public enum ComboAnimNames
    {
        None, Grounded, Targeted_Locomation, Locomation_Grounded_Weapon,
        Equip_Light, Equip_Heavy, Equip_LightCharged, Equip_HeavyCharged, Equip_Projectile,
        Light_Attack1, Light_Attack2, Light_Attack3, Light_Attack4, Light_Attack5, Light_Attack6,
        Heavy_Attack1, Heavy_Attack2, Heavy_Attack3, Heavy_Attack4, Heavy_Attack5, Heavy_Attack6
            , Ground_attack02, Light_Attack1_Alt, Projectile, ChargedProjectile
    }
    public enum AttackType { 
        none, LightAttack, HeavyAttack,ChargedLightAttack, ChargedHeavyAttack, Projectile, ChargedProjectile,Grounded, Targeted_Locomation, Locomation_Grounded_Weapon
    }
    public enum ComboNames
    {
        None, Combo_1, Combo_2, Combo_3, Combo_4, Combo_5, Combo_6, Combo_7, Combo_8, Combo_9, Combo_10,
        Projectile1
    }
}