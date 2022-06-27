using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.SaveSystems
{
    public interface ISave 
    {
       SaveData GetSaveData();

        void Load( string jsonData);
        void Save();
    }
    [System.Serializable]
    public class GameSaveData {
        public string LastSaveTime;
        public GameMasterSaveData GetGameMasterSaveData = new GameMasterSaveData();
        public CharacterSaveData GetCharacterSaveData = new CharacterSaveData();

    }
    [System.Serializable]
    public class GameMasterSaveData {
        public uint Money;
        public uint DayNumber;
        public int PlayerChoice;

    }
  
    [System.Serializable]
    public partial class CharacterSaveData {
        public uint PlayerLevel, CurHealth, MaxHealth, CurMana, MaxMana, FreeExp;
        public string CharacterName;
        public uint CharacterModelSelected; // Change to Enum later
        public SaveData PlayerCombos;
        public int Level;
        public int[] PrimaryAttributeBase;

    }

    public interface SaveData { }
}