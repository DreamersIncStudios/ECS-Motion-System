using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.Global;
using Core.SaveSystems;
namespace DreamersInc.ComboSystem
{
    [CreateAssetMenu(fileName = "Combo", menuName = "ComboSystem/Player")]

    public class PlayerCombos : ScriptableObject, ICombos
    {
        public List<AnimationCombo> _comboList;
        public List<AnimationCombo> ComboList { get { return _comboList; } }


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
        List<ComboInfo> comboInfos;
        List<ComboNames> comboNames;
        public List<ComboInfo> ComboInfoList
        {
            get
            {
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
        public bool ShowMovesPanel = false;
        public GameObject DisplayCombo()
        {
            UIManager MGR = UIManager.instance;
            GameObject MovesPanel = MGR.GetPanel(MGR.UICanvas().transform, new Vector2(800, 400), new Vector2(Screen.width / 2, -Screen.height / 2), LayoutGroup.Vertical);
            foreach (ComboInfo Info in ComboInfoList)
            {
                MGR.UIButton(MovesPanel.transform, Info.name.ToString() + "Locked: " + !Info.Unlocked).onClick.AddListener(() // Need to write a text parser;
              =>
                {
                    if (!Info.Unlocked)
                    {
                        UnlockCombo(Info.name);

                    }
                });

            }
            return MovesPanel;

        }
        ComboSaveData ComboSave = new ComboSaveData();
        public SaveData GetSaveData()
        {
            ComboSave.SaveData = comboInfos;
            return ComboSave;
        }

        public void Load(string json)
        {
            throw new System.NotImplementedException();
        }
    }
    [System.Serializable]
    public struct ComboInfo
    {
        public ComboNames name;
        public bool Unlocked;
    }
    [System.Serializable]
    public class ComboSaveData : SaveData
    {
        public List<ComboInfo> SaveData;
    }

}