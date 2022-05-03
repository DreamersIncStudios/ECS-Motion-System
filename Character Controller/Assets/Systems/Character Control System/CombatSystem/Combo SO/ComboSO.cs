using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.Global;
using Core.SaveSystems;
namespace DreamersInc.ComboSystem
{
    [CreateAssetMenu(fileName = "Combo", menuName = "ComboSystem/Combo Data")]

    public class ComboSO : ScriptableObject, ICombos
    {
        public List<AnimationCombo> _comboList;
        public List<AnimationCombo> ComboList { get { return _comboList; } }

        public void UnlockCombo(ComboNames Name)
        {
            //TODO Implement Unlocking System

        }

        public bool GetAnimationTrigger(AnimatorStateInfo State, ComboInfo info, out AnimationTrigger trigger, out float endtime)
        {
            endtime = 0.0f;
            trigger = new AnimationTrigger();
            // TODO Implement
            return false;

        }

        List<ComboInfo> comboInfos;
        List<ComboNames> comboNames;
        public List<ComboInfo> ComboInfoList
        {
            get
            {
                //TODO IDK 
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

        #region NPC Attack system
        public void OnValidate()
        {
            UpdateTotalProbability();

        }
        public void UpdateTotalProbability()
        {
            for (int i = 0; i < ComboList.Count; i++)
            {
                AnimationCombo tempCombo = _comboList[i];
                float totalProb = 0f;
                for (int j = 0; j < tempCombo.Triggers.Count; j++)
                {
                    if (tempCombo.Triggers[j].Unlocked)
                    {
                        AnimationTrigger temptrigger = tempCombo.Triggers[j];
                        temptrigger.probabilityRangeFrom = totalProb;
                        totalProb += temptrigger.Chance;
                        tempCombo.Triggers[j] = temptrigger;
                    }
                }
                float maxProb = tempCombo.MaxProb = totalProb;


                for (int j = 0; j < tempCombo.Triggers.Count; j++)
                {
                    AnimationTrigger temptrigger = tempCombo.Triggers[j];
                    temptrigger.probabilityTotalWeight = maxProb;
                    tempCombo.Triggers[j] = temptrigger;
                }
                _comboList[i] = tempCombo;
            }
        }

        public int GetAnimationComboIndex(AnimatorStateInfo state) {
            foreach (AnimationCombo combo in ComboList)
            {
                if (state.IsName(combo.CurrentStateName.ToString()))
                {
                    return ComboList.IndexOf(combo);
                }

            }
            throw new ArgumentOutOfRangeException("Animation not registered in Combo SO System");

        }
        public int GetAnimationComboIndex(ComboAnimNames state)
        {
            foreach (AnimationCombo combo in ComboList)
            {
                if (state == combo.CurrentStateName)
                {
                    return ComboList.IndexOf(combo);
                }

            }
            throw new ArgumentOutOfRangeException("Animation not registered in Combo SO System");
        }
        public float GetMaxProbAtCurrentState(AnimatorStateInfo state) {
            return ComboList[GetAnimationComboIndex(state)].MaxProb;
        }
        public float GetMaxProbAtCurrentState(int index)
        {
            return ComboList[index].MaxProb;
        }
        #endregion

        [System.Serializable]
        public class ComboDefinition {
            public string name;
            public ComboNames ComboEnumName;
            public bool Unlocked { get;  set; }
        }

        public bool GetCombo(ComboNames name, out ComboDefinition define) {
            define = null;
            foreach(ComboDefinition definition in ComboNameList){ 
                if(definition.ComboEnumName == name)
                    define= definition;
                return true;
            }
            return false;
        }
        public List<ComboDefinition> ComboNameList;
        public Dictionary<string, ComboDefinition> ComboInfoToDisplay { get; private set; }
        public void CreateComboList() {
            foreach (AnimationCombo animationCombo in ComboList) {
                foreach (AnimationTrigger trigger in animationCombo.Triggers) {
                    if (GetCombo(trigger.Name, out ComboDefinition define) && !ComboInfoToDisplay.TryGetValue(define.name, out _))
                    {
                        define.Unlocked = trigger.Unlocked;
                            ComboInfoToDisplay.Add(define.name,define);
                        
                    }
                }
            
            }
        
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