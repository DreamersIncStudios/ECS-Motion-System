using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class ElementDamageModStat
    {
        private float baseValue;
        private float buffValue;

        public ElementDamageModStat()
        {
            baseValue = 1;
            buffValue = 0;
        }

        public float BaseValue
        {
            get => baseValue;
            set => baseValue= value;
        }

        public float BuffValue
        {
            get => buffValue;
            set => buffValue = value;
        }

        public float AdjustBaseValue
        {
            get => baseValue + buffValue;
        }
    }
}
