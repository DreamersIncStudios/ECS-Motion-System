using Unity.Mathematics;
using Unity.Collections;

[System.Serializable]
public struct AnimationCombo
{
    public ComboAnimNames CurremtStateName;
    public float2 NormalizedInputTime;
    public float AnimationEndTime;
    public bool InputAllowed(float time) => time > NormalizedInputTime.x && time < NormalizedInputTime.y;
    // consider adding late inputs ??????
    public AnimationTriggers LightAttack;
    public AnimationTriggers HeavyAttack;
    public AnimationTriggers ChargedLightAttack;
    public AnimationTriggers ChargedHeavyAttack;
    public AnimationTriggers Projectile;
    public AnimationTriggers ChargedProjectile;



}
[System.Serializable]
public struct AnimationTriggers
{
    public ComboNames Name; // Change To String ???????????
    public ComboAnimNames TriggeredAnimName; // Change to String ???????????
    public bool Unlocked;
    public float TransitionDuration;
    public float StartOffset;
}

public enum ComboAnimNames { None, Grounded, Targeted_Locomation, Locomation_Grounded_Weapon,
    Equip_Light, Equip_Heavy, Equip_LightCharged, Equip_HeavyCharged, Equip_Projectile,
    Light_Attack1, Light_Attack2, Light_Attack3, Light_Attack4, Light_Attack5, Light_Attack6,
    Heavy_Attack1, Heavy_Attack2, Heavy_Attack3, Heavy_Attack4, Heavy_Attack5, Heavy_Attack6
        , Ground_attack02, Light_Attack1_Alt, Projectile, ChargedProjectile
}

public enum ComboNames { 
    Combo_1, Combo_2, Combo_3, Combo_4,Combo_5, Combo_6, Combo_7, Combo_8, Combo_9, Combo_10
}