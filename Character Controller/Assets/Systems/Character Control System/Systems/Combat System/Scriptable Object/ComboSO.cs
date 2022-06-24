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
        [NonReorderable] [SerializeField] List<AnimationCombo> _comboList;
      [HideInInspector]  public List<AnimationCombo> ComboList { get { return _comboList; } }
        public TextAsset ComboNamesText;
        public int ComboListIndex; 
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

        public bool ShowMovesPanel = false;

        ComboSaveData ComboSave = new ComboSaveData();
        public SaveData GetSaveData()
        {

            throw new System.NotImplementedException();
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

   

    public List<string> GetListOfComboNames() { 
            List<string> list = new List<string>();
            var lines = ComboNamesText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var parts =  lines[ComboListIndex].Split(';');
            foreach (var part in parts) { 
                list.Add(part);
            }
            return list;
        }
        public List<ComboDefinition> GetComboDefinitions() {
            List<ComboDefinition> temp = new List<ComboDefinition>();
            List<string> comboNames = GetListOfComboNames();

            for (int i = 0; i < comboNames.Count-1; i++)
            {
                int index = i;

                var definationTemp = new ComboDefinition()
                {
                    name = comboNames[i],
                    ComboEnumName = (ComboNames)index + 1,
                    Unlocked = false,
                    test = new Queue<AttackType>()
                    //
                };
                foreach (var comboMove in ComboList)
                {
                    foreach (var trigger in comboMove.Triggers)
                    {
                        if (trigger.Name == (ComboNames)index + 1)
                        {
                            definationTemp.test.Enqueue(trigger.Type);

                            if (trigger.Unlocked)
                                definationTemp.Unlocked = true;
                        }
                    }
                }
                temp.Add(definationTemp);
            }
            return temp;
        }
        public void DisplayCombo()
        {
           List<ComboDefinition> comboDefinitions = GetComboDefinitions();
            // Launch Modal Window 
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
        [NonReorderable] public List<ComboInfo> SaveData;
    }

    [System.Serializable]
    public class ComboDefinition
    {
        public string name;
        public ComboNames ComboEnumName;
        public bool Unlocked { get; set; }
        [NonReorderable] public Queue<AttackType> test;
    }



}