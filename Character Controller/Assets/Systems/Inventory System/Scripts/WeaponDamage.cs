using UnityEngine;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Dreamers.InventorySystem;
using Stats.Entities;
using Unity.Entities;

namespace DreamersInc.DamageSystem
{
    // [RequireComponent(typeof(MeshCollider))]
    public sealed class WeaponDamage : MonoBehaviour, IDamageDealer
    {
        public Action OnHitAction { get; set; }
        public Action ChanceCheck { get; set; }
        public Action CriticalEventCheck { get; set; }
        public Stat Magic_Offense { get; private set; }
        public Stat Range_Offense { get; private set; }
        public Stat Melee_Offense { get; private set; }
        public Attributes Skill { get; private set; }
        public Attributes Speed { get; private set; }

        private List<Effects> Effects { get; set; }

        private Entity ParentEntity => self.SelfEntityRef;

        public int BaseDamage
        {
            get
            {
              // Todo Add mod value for Magic infused/ Enchanted weapon
                var output = TypeOfDamage switch
                {
                    TypeOfDamage.MagicAoE => Magic_Offense.AdjustBaseValue,
                    TypeOfDamage.Projectile =>  Range_Offense.AdjustBaseValue,
                    TypeOfDamage.Melee => Melee_Offense.AdjustBaseValue,
                    _ => Melee_Offense.AdjustBaseValue,
                };
                return output;
            }
        }
        public float CriticalHitMod => CriticalHit ? Random.Range(1.5f, 2.15f) : 1;
        private float RandomMod => Random.Range(.85f, 1.15f);
        
        public bool CriticalHit
        {
            get
            {
                var prob = Mathf.RoundToInt(Random.Range(0, 255));
                var threshold =  (Skill.AdjustBaseValue + Speed.AdjustBaseValue) / 2;
                return prob < threshold;
            }
        }
        public float MagicMod { get; private set; }
        public ElementName ElementName { get; private set; }

        public TypeOfDamage TypeOfDamage { get; private set; }

        public bool DoDamage { get; private set; }

        public int DamageAmount()
        {
            return Mathf.RoundToInt(BaseDamage * RandomMod );
        }

        public void SetDamageBool(bool value)
        {
            DoDamage = value;
        }

        public void SetDamageType()
        {
            throw new NotImplementedException();
        }

        public void SetElement(ElementName value)
        {
            ElementName = value;
            //TODO Balance 
            MagicMod =  ElementName != ElementName.None ? Magic_Offense.AdjustBaseValue / 10.0f : 1.0f;
        }

        public void UpdateEffect(Effects effect, bool remove = false)
        {
            if (!remove)
            {
                Effects.Add(effect);
            }
            else
                Effects.Remove(effect);
        }

        private IDamageable self;

        // Use this for initialization
        private void Start()
        {
            Effects = new List<Effects>();
            registered = true;
            
            if (GetComponent<Collider>())
            {
                TypeOfDamage = TypeOfDamage.Melee;
                GetComponent<Collider>().isTrigger = true;
                self = GetComponentInParent<IDamageable>();
            }
            else
            {
                throw new ArgumentNullException(nameof(gameObject), $"Collider has not been setup on equipped weapon. Please set up Collider in Editor; {gameObject.transform.parent.name}");
            }
        }

        private float criticalHitMod;
        public void CheckChance()
        {
            criticalHitMod = CriticalHitMod;
            if (!criticalHitMod.Equals(1.0f)) { 
                CriticalEventCheck();
            }
            ChanceCheck.Invoke();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IDamageable>(out var hit)) return;
            //Todo add Friend filter.
            if (!DoDamage || hit == self) return;
            hit.TakeDamage(DamageAmount(), TypeOfDamage, ElementName, ParentEntity,level);
            CheckForEffectStatusChange();
            var root = transform.root;
            hit.ReactToHit(.5f, root.position, root.forward);
            OnHitAction?.Invoke();
        }

        private void CheckForEffectStatusChange()
        {
            foreach (var effect in Effects.Where(effect => Random.Range(0, 1) < effect.Chance))
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

        private uint level;

        private bool registered;
        
        /// <summary>
        /// Sets the stat data for the weapon.
        /// </summary>
        /// <param name="stats">The BaseCharacterComponent containing the stats.</param>
        /// <param name="damageType">The type of damage.</param>
     
        public void SetStatData(BaseCharacterComponent stats, TypeOfDamage damageType)
        {
            Magic_Offense = stats.GetStat((int)StatName.MagicOffence);
            Range_Offense = stats.GetStat((int)StatName.RangedOffence);
            Melee_Offense = stats.GetStat((int)StatName.MeleeOffence);
            Speed = stats.GetPrimaryAttribute((int)AttributeName.Speed);
            Skill = stats.GetPrimaryAttribute((int)AttributeName.Skill);
            TypeOfDamage = damageType;
            level = (uint)stats.Level;
            
            if (registered) return;
            stats.OnStatChanged += ((_, args) => SetStatData(args.Stats, TypeOfDamage));
            registered = true;
        }
    }
}