using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

/*Generic Player Character Class
 * Will be using BurgZergArcade Derived Player Character System  that is already in main project file
 */
namespace Stats
{



    // add safe checks
    public struct PlayerStatComponent: IComponentData {
        [HideInInspector] public Entity selfEntityRef;

        [Range(0, 999)]
        [SerializeField] int _curHealth;
        public int CurHealth { get { return _curHealth; } 
            set {

                if (value <= 0)
                    _curHealth = 0;
                else if (value > MaxHealth)
                    _curHealth = MaxHealth;
                else
                    _curHealth = value;
            } }
        [Range(0, 999)]
        [SerializeField] int _curMana;

        public int CurMana
        {
            get { return _curMana; }
            set
            {

                if (value <= 0)
                    _curMana= 0;
                else if (value > MaxMana)
                    _curMana = MaxMana;
                else
                    _curMana = value;
            }
        }
        [Range(0, 999)]
        public int MaxHealth;
        [Range(0, 999)]
        public int MaxMana;
        public float HealthRatio { get { return CurHealth /(float) MaxHealth; } }
        public float ManaRatio { get { return CurMana / (float)MaxMana; } }


        public float MagicDef;
        public float MeleeAttack;
        public float MeleeDef;

        public void AdjustHealth(int adj)
        {
            CurHealth += adj;
            if (CurHealth < 0) { CurHealth = 0; }
            if (CurHealth > MaxHealth) { CurHealth = MaxHealth; }

        }
        public void AdjustMana(int adj)
        {
            CurMana += adj;
            if (CurMana < 0) { CurMana = 0; }
            if (CurMana > MaxMana) { CurMana = MaxMana; }

        }

    }

    public struct EnemyStats : IComponentData {

        [HideInInspector] public Entity selfEntityRef;

        [Range(0, 999)]
        [SerializeField] int _curHealth;
        public int CurHealth
        {
            get { return _curHealth; }
            set
            {

                if (value <= 0)
                    _curHealth = 0;
                else if (value > MaxHealth)
                    _curHealth = MaxHealth;
                else
                    _curHealth = value;
            }
        }
        [Range(0, 999)]
        [SerializeField] int _curMana;

        public int CurMana
        {
            get { return _curMana; }
            set
            {

                if (value <= 0)
                    _curMana = 0;
                else if (value > MaxMana)
                    _curMana = MaxMana;
                else
                    _curMana = value;
            }
        }
        [Range(0, 999)]
        public int MaxHealth;
        [Range(0, 999)]
        public int MaxMana;
        public float HealthRatio { get { return CurHealth / (float)MaxHealth; } }
        public float ManaRatio { get { return CurMana / (float)MaxMana; } }


        public float MagicDef;
        public float MeleeAttack;
        public float MeleeDef;

        public void AdjustHealth(int adj)
        {
            CurHealth += adj;
            if (CurHealth < 0) { CurHealth = 0; }
            if (CurHealth > MaxHealth) { CurHealth = MaxHealth; }

        }
        public void AdjustMana(int adj)
        {
            CurMana += adj;
            if (CurMana < 0) { CurMana = 0; }
            if (CurMana > MaxMana) { CurMana = MaxMana; }

        }

    }

}