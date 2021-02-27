using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.SaveSystems
{
    public interface ISave 
    {
       SaveData Save();

        void Load( string jsonData);
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
        public int DayNumber;
        public PlayerChoice PlayerChoice;

    }

    [System.Serializable]
    public class CharacterSaveData {
        public uint PlayerLevel, CurHealth, MaxHealth, CurMana, MaxMana;
        public string CharacterName;
        public uint CharacterModelSelected; // Change to Enum later
    }

    public interface SaveData { }
}