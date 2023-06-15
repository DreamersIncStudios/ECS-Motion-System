﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;  
using UnityEngine.Events;
using Core;
using Unity.Entities;
using Unity.Transforms;
using DreamersInc;


namespace DreamersIncStudios.MoonShot
{
    public sealed partial class GameMaster : MonoBehaviour
    { 
        public static GameMaster Instance;
        public GameStates State { get { return state; } set {
                if (value != state) {
                    state = value;

                }
            } }
        [SerializeField] GameStates state = GameStates.TitleScreen;

        [SerializeField] int SetEditorPlayerChoice;
        public int GetPlayerChoice { get; private set; }
        public bool SMTOverride;
        public CameraControls CamerasToControl;
        public int ActiveSaveNumber { get; set; }
        //    public InputSettings InputSettings = new InputSettings();
        public Quality Setting;

        Language GetLanguage;
        [SerializeField]
        public ControllerScheme controller;
        public ControllerScheme Controller { get; private set; }
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
            GetPlayerChoice = 1;
            DontDestroyOnLoad(this.gameObject);
#if UNITY_EDITOR
            GetPlayerChoice = SetEditorPlayerChoice;

#endif
#if !UNITY_EDITOR
            Application.targetFrameRate = 360;
#endif
            Controller = controller; // TODO contextual Change value ;
        }

        public uint DayNumber  = 0;
        private void Start()
        {

        }
        public bool CreateMenuMain => State == GameStates.TitleScreen && Input.GetButtonUp("Submit");


        public void Update()
        {
          
        }

        public void SetupNewGame()
        {
            State = GameStates.Playing;

        }
        public void GetSaves() { }
        public void LoadSaves(int SaveNumber) { }

        public void SelectCharacter(int Choice) {
            GetPlayerChoice = Choice;
        }
     

        void OnEnable()
        {
            //if (PlayerPrefs.HasKey("SMTOverride"))
            //{
            //    SMTOverride = PlayerPrefs.GetInt("SMTOverride") == 0;
            //}
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
        public bool GMEntityCreated { get {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                return manager.CreateEntityQuery((typeof(ControllerInfo))).TryGetSingletonEntity<ControllerInfo>(out _);
            } }
        public void CreateGMEntity()
        {

            if (!GMEntityCreated)
            {
                EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var data = new ControllerInfo();
                data.setup(controller);
                Entity gm = manager.CreateSingleton(data);
#if UNITY_EDITOR
                manager.SetName(gm, "Game Master");
#endif
                manager.CreateEntityQuery((typeof(ControllerInfo))).GetSingleton<ControllerInfo>().setup(controller);
            }
        }

    }
    public enum GameStates {
        TitleScreen,
        InMenu,
        Paused,
        Playing,
        InventoryMenu,
        Load,
        WaitingToStartLevel,
        Game_Over
    }

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
