using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

namespace Core.SaveSystems
{
    public class SaveSystem : MonoBehaviour
    {
       public GameSaveData gameData = new GameSaveData();
        [Serializable]
        public class SaveList
        {
           public  List<SaveDisplayData> MasterSaveList = new List<SaveDisplayData>();
            public int NextSaveCnt =>  MasterSaveList.Count ;
        }
        public SaveList Saves;
        public static SaveSystem Instance;

        public void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(this);
            GetAllSave();
        }
        public void GetAllSave() {
            using (StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/Master.json"))
            {
                var json = streamReader.ReadToEnd();
                Saves = JsonUtility.FromJson<SaveList>(json);
            }

        }
        public void WriteAllSaves()
        {

            using (StreamWriter streamWriteer = new StreamWriter(Application.persistentDataPath + "/Master.json"))
            {
                var json = JsonUtility.ToJson(Saves);
                streamWriteer.Write(json);
            }

        }

        public void SaveGame(int GameSave, int playerChoice, uint DayNumber)
        {
            //Add inclusive true when upgrade to 2020 LTS
            foreach (var persist in FindObjectsOfType<MonoBehaviour>(true))
            {
                persist.BroadcastMessage("Save");
            }

          
            gameData.GetGameMasterSaveData.PlayerChoice = playerChoice;
            gameData.GetGameMasterSaveData.DayNumber = DayNumber;
            gameData.LastSaveTime = DateTime.Now.ToString();
           // gameData.GetCharacterSaveData.PlayerCombos = GameObject.FindGameObjectWithTag("Player").GetComponent<combo>
            using (StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + $"/SaveGame{GameSave}.json"))
            {
                var jsonGameData = JsonUtility.ToJson(gameData);
                var plainText = System.Text.Encoding.UTF8.GetBytes(jsonGameData);
                var b64 = Convert.ToBase64String(plainText);
                streamWriter.Write(b64);
            }

        }
        public GameSaveData LoadGame(int GameSave)
        {
            using (StreamReader streamReader = new StreamReader(Application.persistentDataPath + $"/SaveGame{GameSave}.json"))
            {
                var b64 = streamReader.ReadToEnd();
                var plainText = Convert.FromBase64String(b64);
                var json = System.Text.Encoding.UTF8.GetString(plainText);

                gameData = JsonUtility.FromJson<GameSaveData>(json);
            }
            return gameData;
        }

        public void DeleteSave(int GameSave) {
            Saves.MasterSaveList.RemoveAt(GameSave);
        }
        public void AddNewSave( uint DayNumber )
        {
       
            Saves.MasterSaveList.Add(new SaveDisplayData()
            {
                SaveNumber = Saves.NextSaveCnt,
                CharacterName = "Tester", // Get from Player later 
                DateOfSave = DateTime.Now.ToString(),
                DayNumber =DayNumber,
                Score = 1000 // Add to GM Later
        });
            WriteAllSaves();

        }

        public void AppendSave(int GameSave)
        {


        }
        public void SaveMasterList() { 
        
        }

    }

    
    [Serializable]
    public class SaveDisplayData {
        public int SaveNumber;
        public string CharacterName, DateOfSave;
        public uint DayNumber, Score;
    }

}
