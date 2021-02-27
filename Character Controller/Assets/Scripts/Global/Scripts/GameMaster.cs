﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dreamers.Global;
using Global.Menus;
using Cinemachine;
using Core.SaveSystems;
using System.IO;
namespace Core

{
    public sealed class GameMaster : MonoBehaviour
    {
        public static GameMaster Instance;
        public GameStates State = GameStates.TitleScreen;
        public GameObject[] PlayerOptions;
        public GameObject Player;//{ get; private set; }
        public GameObject StartIcon;
        public PlayerChoice GetPlayerChoice = new PlayerChoice();
        public MainMenu mainMenu;
        public SaveSystem GetSaveSystem => GetComponent<SaveSystem>();
        public bool SMTOverride;
        public CameraControls CamerasToControl;
        public int ActiveSaveNumber { get; set; }
        public InputSettings InputSettings = new InputSettings();
        public Quality Setting;

        Language GetLanguage;
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
#if UNITY_STANDALONE_WIN

            InputSettings.TargetPlatform = PlatformOptions.PC;

#endif
#if UNITY_XBOXONE
        InputSettings.TargetPlatform = PlatformOptions.XBOX;
#endif
#if UNITY_PS4
       InputSettings.TargetPlatform = PlatformOptions.PC;
#endif

            InputSettings.Controller = true;
            InputSettings.SetUp();

        }

        public int DayNumber  = 0;
        private void Start()
        {
            //       PlayerOptions[0].GetComponent<Animator>().SetInteger("Idle State", 1);
            //PlayerOptions[1].GetComponent<Animator>().SetInteger("Idle State", 2);
            //PlayerOptions[0].GetComponent<Animator>().SetBool("Idle", true);
            //PlayerOptions[1].GetComponent<Animator>().SetBool("Idle", true);

        }
        public bool CreateMenuMain => State == GameStates.TitleScreen && Input.GetButtonUp("Submit");


        public void Update()
        {
            if (CreateMenuMain) {
                mainMenu = new MainMenu(StartIcon.transform.parent.gameObject);
                State = GameStates.InMenu;
                Destroy(StartIcon);
            }
        }

        public void SetupNewGame()
        {

            GetSaveSystem.SaveGame(GetSaveSystem.Saves.NextSaveCnt);
            GetSaveSystem.AddNewSave();

        }
        public void GetSaves() { }
        public void LoadSaves(int SaveNumber) { }

        public void SelectCharacter(int Choice) {
           // Player = PlayerOptions[Choice];
           // Player.BroadcastMessage("NewGame");
            //Player.GetComponent<Animator>().SetBool("Idle", false);
            //CamerasToControl.Main.gameObject.SetActive(false);
            //CamerasToControl.Follow.LookAt = Player.transform;
            //  CamerasToControl.Follow.Follow = Player.GetComponentInChildren<FollowPointRef>().transform;

         //   CamerasToControl.Target.LookAt = Player.transform;
         //   CamerasToControl.Target.Follow = Player.transform;
               State = GameStates.Playing;
            SetupNewGame();

        }


        void OnEnable()
        {
            if (PlayerPrefs.HasKey("SMTOverride"))
            {
                SMTOverride = PlayerPrefs.GetInt("SMTOverride") == 0;
            }
            if (PlayerPrefs.HasKey("Language"))
            {
                var json = PlayerPrefs.GetString("Language");
                GetLanguage = JsonUtility.FromJson<Language>(json);
            }
            new SMTOptions(SMTOverride);

        }

        void OnDisable()
        {

            PlayerPrefs.SetInt("SMTOverride", SMTOverride ? 0 : 1);

            var LangugaeSave = JsonUtility.ToJson(GetLanguage);
            PlayerPrefs.SetString("GetLanguage", LangugaeSave);
        }

        public void SetQualitySetting()
        {
            QualitySettings.SetQualityLevel(Setting.QualityLevel);
            QualitySettings.vSyncCount = Setting.VsyncCount;

        }

    }
    public enum GameStates {
        TitleScreen,
        InMenu,
        Paused,
        Playing,
        InventoryMenu
    }

    public enum PlayerChoice { a, b, c, d }
    public enum Language { English, Spanish }
    [System.Serializable]
    public struct CameraControls {
        public CinemachineVirtualCameraBase Main, Follow, Target;

    }
    [System.Serializable]
    public struct Quality
    {
        public int QualityLevel;
        public int VsyncCount;
        public Language _language;

    }

   

}
