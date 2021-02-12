using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
[System.Serializable]
[InternalBufferCapacity(60)]
[GenerateAuthoringComponent]
public struct AnimationCombo : IBufferElementData
{
    public AnimCombo GetTest;

}
[System.Serializable]
public struct AnimCombo
{
    public ComboAnimNames CurremtStateName;
    public float2 NormalizedInputTime;
    public float AnimationEndTime;
    public bool InputAllowed(float time) => time > NormalizedInputTime.x && time < NormalizedInputTime.y;
    public bool Unlocked;
    public AnimationTriggers LightAttack;
    public AnimationTriggers HeavyAttack;
    public AnimationTriggers ChargedLightAttack;
    public AnimationTriggers ChargeHeavytAttack;
    public AnimationTriggers Projectile;


}
[System.Serializable]
public struct AnimationTriggers
{
    public ComboAnimNames TriggeredAnimName;
    public float StartOffset;
}

public enum ComboAnimNames { None, Grounded, Targeted_Locomation, Locomation_Grounded_Weapon,
    Equip_Light, Equip_Heavy, Equip_LightCharged, Equip_HeavyCharged, Equip_Projectile,
    Light_Attack1, Light_Attack2, Light_Attack3, Light_Attack4, Light_Attack5, Light_Attack6

        , Ground_attack02 }