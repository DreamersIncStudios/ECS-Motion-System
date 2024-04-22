using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//using Core.SaveSystems;

namespace DreamersInc.ComboSystem
{
    [CreateAssetMenu(fileName = "Combo", menuName = "ComboSystem/Combo Data")]

    // ReSharper disable once InconsistentNaming
    public class ComboSO : ScriptableObject, ICombos
    {

        [FormerlySerializedAs("_comboLists")] [SerializeField]
        List<ComboSingle> comboLists;

        [FormerlySerializedAs("heavyComboLists")] [SerializeField] List<ComboSingle> AltAttackStyleComboList;


        [HideInInspector]
        public List<ComboSingle> ComboLists(bool index)
        {
            return index switch
            {
                false => comboLists,
                true => AltAttackStyleComboList,
            };
        }
    


    public TextAsset ComboNamesText;
        public int ComboListIndex;

        public void UnlockCombo(ComboNames name)
        {
            //TODO Implement Unlocking System
        }

        public bool GetAnimationTrigger(AnimatorStateInfo state, ComboInfo info, out AnimationTrigger trigger,
            out float endtime)
        {
            endtime = 0.0f;
            trigger = new AnimationTrigger();
            // TODO Implement
            return false;

        }

        public bool ShowMovesPanel = false;

        //ComboSaveData ComboSave = new ComboSaveData();
        //public SaveData GetSaveData()
        //{

        //    throw new System.NotImplementedException();
        //}

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

        }

        public AnimationTrigger GetTrigger(AnimatorStateInfo state, bool style = false)
        {
            foreach (ComboSingle combo in ComboLists(style))
            {
                foreach (AnimationCombo test in combo.ComboList)
                {
                    if (state.IsName(test.Trigger.TriggerString))
                        return test.Trigger;
                }
            }

            return new AnimationTrigger();
        }

        public VFX GetVFX(AnimatorStateInfo state, bool style = false)
        {
            return GetTrigger(state, style).AttackVFX;
        }

        public int GetAnimationComboIndex(AnimatorStateInfo state)
        {

            throw new ArgumentOutOfRangeException(nameof(state));

        }

        public int GetAnimationComboIndex(string state)
        {

            throw new ArgumentOutOfRangeException(nameof(state));
        }

        #endregion



        public List<string> GetListOfComboNames()
        {
            var lines = ComboNamesText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var parts = lines[ComboListIndex].Split(';');
            return parts.ToList();
        }

        public List<ComboDefinition> GetComboDefinitions()
        {
            List<ComboDefinition> temp = new();


            return temp;
        }

        public void DisplayCombo()
        {
            List<ComboDefinition> comboDefinitions = GetComboDefinitions();
            // Launch Modal Window 
        }


        public AnimationTrigger GetAttack(bool style = false)
        {
            var options = new List<AnimationTrigger>();
            foreach (var combo in ComboLists(style))
            {
                if(combo.Unlocked)
                    options.Add(combo.ComboList[0].Trigger);
            }
            var temp = new System.Random().Next(options.Count);
            
        
            
            return options[temp];
        }

    }


    //[System.Serializable]
    //public class ComboSaveData : SaveData
    //{
    //    [NonReorderable] public List<ComboInfo> SaveData;
    //}
}