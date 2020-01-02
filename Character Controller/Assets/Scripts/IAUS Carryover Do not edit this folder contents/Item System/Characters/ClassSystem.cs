namespace Stats {
    [System.Serializable]
    public struct CharacterClass {
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

        public float diffultyMod;
        public float LevelMod;

    }
    public enum ClassTitle{
        Grunt,Soldier, Ranger, Archer, Sorcer, Mage, Monk, Swordman, Thief, Knight, Bot, Generalist, Pugiblist
    }
}
