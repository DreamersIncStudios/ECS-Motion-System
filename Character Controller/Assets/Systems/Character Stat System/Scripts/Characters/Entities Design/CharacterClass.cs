namespace Stats.Entities
{

    [System.Serializable]
    public struct CharacterClass
    {
        public string Name;
        public ClassTitle title;
        public int Level;
        public int Strength;
        public int Vitality;
        public int Awareness;
        public int Speed;
        public int Skill;
        public int Resistance;
        public int Concentration;
        public int WillPower;
        public int Charisma;
        public int Luck;

        public float difficultyMod;
        public float LevelMod;

    }
    
}
