
namespace EquipmentStats
{
    [System.Serializable]
    public class ModAttributes : BaseStat
    {
        public ModAttributes()
        {
            BaseValue = 0;
            BuffValue = 0;

        }
    }
    public enum ModAttributeNames
    {
        Level,
        Durablity,
        Strength,
        Vitality,
        Awareness,
        Speed,
        Skill,
        Resistance,
        Concentration,
        WillPower,
        Charisma,
        Luck

    }
}