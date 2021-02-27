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
        GameSaveData gameData = new GameSaveData();
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

        public void SaveGame(int GameSave)
        {
            //Add inclusive true when upgrade to 2020 LTS
            foreach (var persist in FindObjectsOfType<MonoBehaviour>().OfType<ISave>())
            {
                persist.Save();
            }

            GameMaster gm = GameMaster.Instance;
            gameData.GetGameMasterSaveData.PlayerChoice = gm.GetPlayerChoice;
            gameData.GetGameMasterSaveData.DayNumber = gm.DayNumber;
            gameData.LastSaveTime = DateTime.Now.ToString();
            gm.ActiveSaveNumber = GameSave;

            using (StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + $"/SaveGame{GameSave}.json"))
            {
                var jsonGameData = JsonUtility.ToJson(gameData);
                var plainText = System.Text.Encoding.UTF8.GetBytes(jsonGameData);
                var b64 = Convert.ToBase64String(plainText);
                streamWriter.Write(b64);
            }

        }
        public void LoadGame(int GameSave)
        {
            using (StreamReader streamReader = new StreamReader(Application.persistentDataPath + $"/SaveGame{GameSave}.json"))
            {
                var b64 = streamReader.ReadToEnd();
                var plainText = Convert.FromBase64String(b64);
                var json = System.Text.Encoding.UTF8.GetString(plainText);

                gameData = JsonUtility.FromJson<GameSaveData>(json);
            }
            GameMaster gm = GameMaster.Instance;
            gm.GetPlayerChoice = gameData.GetGameMasterSaveData.PlayerChoice;
            gm.DayNumber = gameData.GetGameMasterSaveData.DayNumber;
            gm.ActiveSaveNumber = GameSave;
        }

        public void DeleteSave(int GameSave) {
            Saves.MasterSaveList.RemoveAt(GameSave);
        }
        public void AddNewSave()
        {
            GameMaster gm = GameMaster.Instance;
            Saves.MasterSaveList.Add(new SaveDisplayData()
            {
                SaveNumber = Saves.NextSaveCnt,
                CharacterName = "Tester", // Get from Player later 
                DateOfSave = DateTime.Now.ToString(),
                DayNumber = gm.DayNumber,
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
        public int DayNumber, Score;
    }

}
