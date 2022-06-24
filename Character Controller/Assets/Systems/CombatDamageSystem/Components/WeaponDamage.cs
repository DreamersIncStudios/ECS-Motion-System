using System.Collections;
using UnityEngine;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using System;
using Random = UnityEngine.Random;

namespace DreamersInc.DamageSystem
{
   // [RequireComponent(typeof(MeshCollider))]
    public class WeaponDamage : MonoBehaviour, IDamageDealer
    {
        public int BaseDamage
        {
            get
            {
                //Todo Add mod value for Magic infused/Ennchanted weapon 
                int output = TypeOfDamage switch
                {
                    TypeOfDamage.MagicAoE => Stats.GetStat((int)StatName.Magic_Offence).AdjustBaseValue,
                    TypeOfDamage.Projectile => Stats.GetStat((int)StatName.Ranged_Offence).AdjustBaseValue,
                    _ => Stats.GetStat((int)StatName.Melee_Offence).AdjustBaseValue,
                };
                return output;
            }
        }
        public float CriticalHitMod => CriticalHit ? Random.Range(1.5f, 2.15f) : 1;
        private float randomMod => Random.Range(.85f, 1.15f)
; public bool CriticalHit
        {
            get
            {
                int prob = Mathf.RoundToInt(Random.Range(0, 255));
                int thresold = (Stats.GetStat((int)AttributeName.Skill).AdjustBaseValue + Stats.GetStat((int)AttributeName.Speed).AdjustBaseValue) / 2;
                return prob < thresold;
            }
        }
        public float MagicMod { get; private set; }
        public Element Element { get; private set; }

        public TypeOfDamage TypeOfDamage { get; private set; }

        public bool DoDamage { get; private set; }
        public BaseCharacter Stats { get { return GetComponentInParent<BaseCharacter>(); } }
        public int DamageAmount()
        {
            return Mathf.RoundToInt(BaseDamage * randomMod * CriticalHitMod);
        }

        public void SetDamageBool(bool value)
        {
            DoDamage = value;
        }

        public void SetDamageType()
        {
            throw new System.NotImplementedException();
        }

        public void SetElement(Element value)
        {
            Element = value;
            //TODO Balance 
            MagicMod = Element != Element.None ? Stats.GetStat((int)StatName.Magic_Offence).AdjustBaseValue / 10.0f : 1.0f;
        }
        IDamageable self;
        // Use this for initialization
        void Start()
        {
            if (GetComponent<Collider>())
            {
                TypeOfDamage = TypeOfDamage.Melee;
                GetComponent<Collider>().isTrigger = true;
                self = GetComponentInParent<IDamageable>();
            }
            else {
                throw new ArgumentNullException(nameof(gameObject),$"Collider has not been setup on equipped weapon. Please set up Collider in Editor; {gameObject.name}");
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            IDamageable hit = other.GetComponent<IDamageable>();
            //Todo add Friend filter.
            if (DoDamage && hit != null && hit != self)
            {
                hit.TakeDamage(DamageAmount(), TypeOfDamage, Element);
            }
        }
    }
}