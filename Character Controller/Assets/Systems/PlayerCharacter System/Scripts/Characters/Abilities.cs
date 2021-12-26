namespace Stats
{
    [System.Serializable]
    public class Abilities : ModifiedStat
    {
#pragma warning disable CS0414 // The field 'Abilities.Learned' is assigned but its value is never used
        bool Learned = false;
#pragma warning restore CS0414 // The field 'Abilities.Learned' is assigned but its value is never used
  

    }
    public enum AbilityName
    {
        Libra,
        Dispel,
        Detection
    }
}