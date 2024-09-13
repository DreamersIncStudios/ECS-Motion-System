using System;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using System.Collections.Generic;
using System.Linq;
using Dreamers.InventorySystem;
using PrimeTween;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleDamage : MonoBehaviour, IDamageDealer
{
    public Action OnHitAction { get; set; }
    public Action ChanceCheck { get; set; }
    public Action CriticalEventCheck { get; set; }

    public Stat Magic_Offense { get; private set; }
    public Stat Range_Offense { get; private set; }
    public Stat Melee_Offense { get; private set; }
    public Attributes Skill { get; private set; }
    public Attributes Speed { get; private set; }
    public TypeOfDamage TypeOfDamage { get; private set; }
    int damage;
    [SerializeField] float moveDistance;
    [SerializeField] float growScale = 1;
    [SerializeField] float moveTime;
    [SerializeField] bool delay;
    [SerializeField] float delayTime;
    public ElementName ElementName { get; set; }
    public List<Effects> effects { get; set; }

    private Entity caster;

    private void Start()
    {
        startDamage = false;
        if (delay)
            Invoke(nameof(MoveDamageCollider), delayTime);
        else
            MoveDamageCollider();
    }

    private void MoveDamageCollider()
    {
        startDamage = true;
        transform.DOMove(transform.position + transform.root.forward * moveDistance, moveTime);
        transform.DOScaleX(growScale, moveTime);
    }
    private void CheckForEffectStatusChange()
    {
        foreach (var effect in effects.Where(effect => Random.Range(0, 1) < effect.Chance))
        {
            switch (effect.ElementName)
            {
                case ElementName.Fire:
                    break;
                case ElementName.Water:
                    break;
                case ElementName.Earth:
                    break;
                case ElementName.Wind:
                    break;
                case ElementName.Ice:
                    break;
                case ElementName.Holy:
                    break;
                case ElementName.Dark:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    bool startDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
        if (!startDamage || damageable.SelfEntityRef == caster) return;
        damageable.TakeDamage(DamageAmount(), TypeOfDamage.MagicAoE, ElementName, caster,level);
        Debug.Log($"hit {other.name}");
        CheckForEffectStatusChange();


        OnHitAction?.Invoke();
    }

    public int BaseDamage
    {
        get
        {
            // Todo Add mod value for Magic infused/ Enchanted weapon
            var output = TypeOfDamage switch
            {
                TypeOfDamage.MagicAoE => Magic_Offense.AdjustBaseValue,
                TypeOfDamage.Projectile => Range_Offense.AdjustBaseValue,
                TypeOfDamage.Melee => Melee_Offense.AdjustBaseValue,
                _ => Melee_Offense.AdjustBaseValue,
            };
            return output;
        }
    }

    public float CriticalHitMod => CriticalHit ? Random.Range(1.5f, 2.15f) : 1;
    
    public float MagicMod { get; private set; }
    public bool CriticalHit
    {
        get
        {
            var prob = Mathf.RoundToInt(Random.Range(0, 255));
            var threshold =  (Skill.AdjustBaseValue + Speed.AdjustBaseValue) / 2;
            return prob < threshold;
        }
    }

    public void SetElement(ElementName value)
    {
        ElementName = value;
        //TODO Balance 
        MagicMod =  ElementName != ElementName.None ? Magic_Offense.AdjustBaseValue / 10.0f : 1.0f;
    }

    public void SetDamageType()
    {
        throw new NotImplementedException();
    }
    private float RandomMod => Random.Range(.85f, 1.15f);
    public int DamageAmount()
    {
        return Mathf.RoundToInt(BaseDamage * RandomMod );
    }

    public bool DoDamage => true;

    public void SetDamageBool(bool value)
    {
    }

    private  uint level;
    public void SetStatData(BaseCharacterComponent stats, TypeOfDamage damageType)
    {
        Magic_Offense = stats.GetStat((int)StatName.MagicOffence);
        Range_Offense = stats.GetStat((int)StatName.RangedOffence);
        Melee_Offense = stats.GetStat((int)StatName.MeleeOffence);
        Speed = stats.GetPrimaryAttribute((int)AttributeName.Speed);
        Skill = stats.GetPrimaryAttribute((int)AttributeName.Skill);
        TypeOfDamage = damageType;
        level = (uint)stats.Level;
    }


}
