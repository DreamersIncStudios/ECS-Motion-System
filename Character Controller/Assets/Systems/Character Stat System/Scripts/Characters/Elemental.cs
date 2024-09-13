using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{   [System.Serializable]
    public class Elemental : BaseStat
    {
        public ElementName ElementNameType;
        public Elemental()
        {
            BaseValue = 100;
            ExpToLevel = 50;
            LevelModifier = 1.05f;
        }

    }
    public enum ElementName { None, Fire, Water, Earth, Wind, Ice, Holy, Dark}

}
