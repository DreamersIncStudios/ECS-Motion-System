
namespace EquipmentStats
{
    [System.Serializable]

    public class Attributes :BaseStat
    {
        public Attributes() {
            BaseValue = 0;
            BuffValue = 0;

        }
    }
    public enum AttributeNames { 
        Level,
        Durablity,
        Speed,
        Cooling_Factor, // 
    }
}