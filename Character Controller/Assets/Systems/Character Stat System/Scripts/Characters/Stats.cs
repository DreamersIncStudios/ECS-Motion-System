namespace Stats
{[System.Serializable]
    public class Stat : ModifiedStat
    {
        private bool known;

        public Stat()
        {
            known = false;
            ExpToLevel = 25;
            LevelModifier = 1.1f;
        }
        public bool Known
        {
            get => known;
            set => known = value;
        }
    }

    public enum StatName
    {
        MeleeOffence,
        MeleeDefense,
        RangedOffence,
        RangedDefence,
        MagicOffence,
        MagicDefense,
        RangeTarget,
        RangeMotion,
        StatusChange,
        ManaRecover
    }
    [System.Serializable]
    public struct StatModifier
    {
        public StatName Stat;
        public int BuffValue;

    }
}