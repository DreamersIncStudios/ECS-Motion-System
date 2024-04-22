using System.Collections.Generic;
using UnityEngine;

namespace DreamersInc.ComboSystem
{
    [System.Serializable]
    public class ComboSingle {
        [SerializeField] ComboNames name;
        public ComboNames Name { get { return name; } set { name = value; } } // Change To String ???????????
        public bool Unlocked;
        

        [SerializeField] List<AnimationCombo> comboList;
        [HideInInspector] public List<AnimationCombo> ComboList { get { return comboList; } }
    }
}