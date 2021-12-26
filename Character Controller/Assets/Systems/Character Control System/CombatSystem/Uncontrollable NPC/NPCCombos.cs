using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DreamersInc.ComboSystem.NPC

{
    [CreateAssetMenu(fileName = "Combo", menuName = "ComboSystem/NPC")]
    public class NPCCombos : ScriptableObject, ICombos
    {
        public List<AnimationCombo> _comboList;
        public List<AnimationCombo> ComboList { get { return _comboList; } }
        public float MaxProb;
        public List<ComboPatternInfo> PatternInfo;
        List<ComboNames> comboNames;

       

        public GameObject DisplayCombo()
        {
            throw new System.NotImplementedException();
        }
        public void OnValidate()
        {
            UpdateTotalProbability();
        }

        public void UpdateTotalProbability() {
            float totalProb = 0f;
            for (int i = 0; i < PatternInfo.Count; i++) {
                ComboPatternInfo temp = PatternInfo[i];
               temp.probabilityRangeFrom = totalProb;
                totalProb += PatternInfo[i].Chance;
                PatternInfo[i] = temp;
            }
            MaxProb = totalProb;
            for (int i = 0; i < PatternInfo.Count; i++)
            {
                ComboPatternInfo temp = PatternInfo[i];
                temp.probabilityTotalWeight = totalProb;
                PatternInfo[i] = temp;
            }
        }
        public bool GetAnimationTrigger(AnimatorStateInfo State, ComboInfo info, out AnimationTrigger trigger, out float endtime) {
            endtime = 0.0f;
            foreach (var item in ComboList) {
                if (State.IsName(item.CurremtStateName.ToString()))
                {
                    endtime = item.AnimationEndTime;
                    if (item.LightAttack.Name.Equals(info.name) && item.LightAttack.Unlocked)
                    {
                        trigger= item.LightAttack;
                        return true;
                    }
                    else
                    if (item.HeavyAttack.Name.Equals(info.name) && item.HeavyAttack.Unlocked)
                    {
                        trigger = item.HeavyAttack;
                        return true;
                    }
                    else
                    if (item.ChargedLightAttack.Name.Equals(info.name) && item.ChargedLightAttack.Unlocked)
                    {
                        trigger = item.ChargedLightAttack;
                        return true;
                    }
                    else
                    if (item.ChargedHeavyAttack.Name.Equals(info.name) && item.ChargedHeavyAttack.Unlocked)
                    {
                        trigger = item.ChargedHeavyAttack;
                        return true;
                    }
                    else
                    if (item.Projectile.Name.Equals(info.name) && item.Projectile.Unlocked)
                    {
                        trigger = item.Projectile;
                        return true;
                    }
                    else
                    if (item.ChargedProjectile.Name.Equals(info.name) && item.ChargedProjectile.Unlocked)
                    {
                        trigger = item.ChargedProjectile;
                        return true;
                    }
                }
            }
            trigger = default;
            return false;

        }

        public void UnlockCombo(ComboNames Name)
        {
            // Add some logic about spending Exp point or whatever currency   
            for (int i = 0; i < ComboList.Count; i++)
            {
                AnimationCombo temp = _comboList[i];
                if (ComboList[i].LightAttack.Name == Name)
                {
                    temp.LightAttack.Unlocked = true;
                }
                if (ComboList[i].HeavyAttack.Name == Name)
                {
                    temp.HeavyAttack.Unlocked = true;
                }
                if (ComboList[i].ChargedLightAttack.Name == Name)
                {
                    temp.ChargedLightAttack.Unlocked = true;
                }
                if (ComboList[i].ChargedHeavyAttack.Name == Name)
                {
                    temp.ChargedHeavyAttack.Unlocked = true;
                }
                if (ComboList[i].Projectile.Name == Name)
                {
                    temp.Projectile.Unlocked = true;
                }
                _comboList[i] = temp;
            }
        }
    }

    [System.Serializable]
    public struct ComboPatternInfo
    {
        public ComboPattern Pattern;
        public float Chance;    
        public int LevelUnlocked;
        public float probabilityTotalWeight;

        [SerializeField] public float probabilityPercent => Chance / probabilityTotalWeight * 100;
        public float probabilityRangeFrom;
        public float probabilityRangeTo => probabilityRangeFrom + Chance;
        public void SetRangeFrom(float StartPoint) {
            probabilityRangeFrom = StartPoint;
        }
        public bool Picked(float picked) {
            return picked > probabilityRangeFrom && picked < probabilityRangeTo;
        }
    }
}