using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamersInc.ComboSystem.NPC

{
    public class NPCCombos : ScriptableObject, ICombos
    {
        public List<AnimationCombo> _comboList;
        public List<AnimationCombo> ComboList { get { return _comboList; } }

        List<ComboInfo> comboInfos;
        List<ComboNames> comboNames;

        public List<ComboInfo> ComboInfoList
        {
            get {
                comboInfos = new List<ComboInfo>();
                comboNames = new List<ComboNames>();

                foreach (AnimationCombo comboOption in ComboList)
                {
                    if (comboNames.Count == 0 || !comboNames.Contains(comboOption.LightAttack.Name))
                    {
                        comboNames.Add(comboOption.LightAttack.Name);
                        comboInfos.Add(new ComboInfo() { name = comboOption.LightAttack.Name, Unlocked = comboOption.LightAttack.Unlocked });

                    }
                    if (comboNames.Count == 0 || !comboNames.Contains(comboOption.HeavyAttack.Name))
                    {
                        comboNames.Add(comboOption.HeavyAttack.Name);
                        comboInfos.Add(new ComboInfo() { name = comboOption.HeavyAttack.Name, Unlocked = comboOption.HeavyAttack.Unlocked });

                    }
                    if (comboNames.Count == 0 || !comboNames.Contains(comboOption.ChargedLightAttack.Name))
                    {
                        comboNames.Add(comboOption.ChargedLightAttack.Name);
                        comboInfos.Add(new ComboInfo() { name = comboOption.ChargedLightAttack.Name, Unlocked = comboOption.ChargedLightAttack.Unlocked });

                    }
                    if (comboNames.Count == 0 || !comboNames.Contains(comboOption.ChargedHeavyAttack.Name))
                    {
                        comboNames.Add(comboOption.ChargedHeavyAttack.Name);
                        comboInfos.Add(new ComboInfo() { name = comboOption.ChargedHeavyAttack.Name, Unlocked = comboOption.ChargedHeavyAttack.Unlocked });

                    }
                    if (comboNames.Count == 0 || !comboNames.Contains(comboOption.Projectile.Name))
                    {
                        comboNames.Add(comboOption.Projectile.Name);
                        comboInfos.Add(new ComboInfo() { name = comboOption.Projectile.Name, Unlocked = comboOption.Projectile.Unlocked });

                    }
                }

                return comboInfos;
            }
        }

        public GameObject DisplayCombo()
        {
            throw new System.NotImplementedException();
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
    public struct ComboInfo
    {
        public ComboNames name;
        public bool Unlocked;
        public float Chance;
        public int LevelUnlocked;
    }
}