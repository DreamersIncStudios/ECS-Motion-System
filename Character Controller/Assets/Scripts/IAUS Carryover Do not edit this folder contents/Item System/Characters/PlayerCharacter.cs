using UnityEngine;
using System.Collections;
namespace Stats
{
    [System.Serializable]
    public class PlayerCharacter : BaseCharacter

    {
        public AnimStyle Fight;
        [Range(0, 9999)]
        public int CurHealth;
        [Range(0, 9999)]
        public int MaxHealth;
        [Range(0, 9999)]
        public int CurMana;
        [Range(0, 9999)]
        public int MaxMana;

        //public GameUI.GameUI Display;

        public float MagicDef;// why does this exist????? Should be using Stats
        public float MeleeAttack;
        public float MeleeDef;

        public override void StatUpdate() {
            base.StatUpdate();
            CurHealth = MaxHealth = GetVital((int)VitalName.Health).AdjustBaseValue;
            CurMana = MaxMana = GetVital((int)VitalName.Mana).AdjustBaseValue;
            MagicDef = 1.0f / (float)(1.0f + ((float)GetStat((int)StatName.Magic_Defence).AdjustBaseValue / 100.0f));// why is this divide by 100?????????????
            MeleeAttack = GetStat((int)StatName.Melee_Offence).AdjustBaseValue;
            MeleeDef = 1.0f / (float)(1.0f + ((float)GetStat((int)StatName.Melee_Defence).AdjustBaseValue / 100.0f));
        }
        private void Start()
        {

        }
        public void AdjustHealth(int adj)
        {
            CurHealth += adj;
            if (CurHealth < 0) { CurHealth = 0; }
            if (CurHealth > MaxHealth) { CurHealth = MaxHealth; }
           // Display.UpdateHealth(); set this to be call after change Player health only 
           
        }
        public void AdjustMana(int adj)
        {
            CurMana += adj;
            if (CurMana < 0) { CurMana = 0; }
            if (CurMana > MaxMana) { CurMana = MaxMana; }
           // Display.UpdateMana();
        }


    }

    public enum  TargetTypes{
        player,
        enemy,
    }
    public enum AnimStyle {
        Sword,Hand
    }
}
