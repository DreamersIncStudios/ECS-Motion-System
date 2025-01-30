using System.Collections.Generic;
using UnityEngine;

namespace DreamersInc.ComboSystem
{
   
    [CreateAssetMenu(fileName = "Combo Single Test", menuName = "ComboSystem/ComboSingle Data")]
    public class ComboSingleSO: ScriptableObject
    {
        [SerializeField] ComboNames name;
        [SerializeField] ComboNames PreviousCombo;
        public ComboNames Name { get { return name; } set { name = value; } } // Change To String ???????????
        [SerializeField] List<AnimationCombo> comboList;
        [HideInInspector] public List<AnimationCombo> ComboList { get { return comboList; } }
    }
    
    [System.Serializable]
    public class ComboSingle
    {
        public ComboNames Name => comboSingleSO.Name;
        public bool Unlocked;
        public ComboSingleSO comboSingleSO;

    }
}