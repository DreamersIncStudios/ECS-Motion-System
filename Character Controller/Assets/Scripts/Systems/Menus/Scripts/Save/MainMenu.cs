using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.Global;
using UnityEngine.UI;
using DG.Tweening;
using Global.Menus;
using Core.SaveSystems;
namespace Core
{
    public class MainMenu
    {
        public static UIManager UIGM;
        public GameObject MainCanvas { get; private set; }
        GameObject mainMenu;
        Button LastButton;
        List<SaveDisplayData> GetSaveData => SaveSystem.Instance.Saves.MasterSaveList;
        public MainMenu(GameObject canvasRef) {
            MainCanvas = canvasRef;
            CreateMainMenu();
        
        }
        GameObject CreateMainMenu()
        {
            if (!UIGM)
            {
                UIGM = UIManager.instance;
            }
            if (!mainMenu)
            {
                Debug.Log("create Main menu");
                GameObject ButtonPanel = UIGM.GetPanel(MainCanvas.transform, new Vector2(400, 800), new Vector2(-300, -1540), new Vector2(1, 1), new Vector2(1, 1));
                //VerticalLayoutGroup VLG = ButtonPanel.AddComponent<VerticalLayoutGroup>();
                //VLG.padding.left = VLG.padding.right = VLG.padding.top = VLG.padding.bottom = 35;
                //VLG.spacing = 30;

                Button start = UIGM.UIButton(ButtonPanel.transform, "New Game", new Vector2(330, 125), new Vector2(200, -100));
                start.onClick.AddListener(() => { 
                    SetupCharacterSelect();
                    LastButton = start;
                });

                Button load = UIGM.UIButton(ButtonPanel.transform, "Load Save", new Vector2(330, 125), new Vector2(200, -250));
                load.onClick.AddListener(() =>
                {
                    SetupLoadSaveFiles();
                    LastButton = load;

                });
                Button option = UIGM.UIButton(ButtonPanel.transform, " Options", new Vector2(330, 125), new Vector2(200, -400));
                Button credits = UIGM.UIButton(ButtonPanel.transform, "Credit", new Vector2(330, 125), new Vector2(200, -550));
                Button End = UIGM.UIButton(ButtonPanel.transform, "End", new Vector2(330, 125), new Vector2(200, -700));
                End.onClick.AddListener(() =>
                    {
                        Application.Quit();
                        Debug.Log("Exit");
                    });
                start.gameObject.AddComponent<ButtonShift>();
                load.gameObject.AddComponent<ButtonShift>();
                option.gameObject.AddComponent<ButtonShift>();
                credits.gameObject.AddComponent<ButtonShift>();
                End.gameObject.AddComponent<ButtonShift>();

                ButtonPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-300, -540), .75f);
                // VLG.enabled = false;
                start.Select();
                mainMenu = ButtonPanel;
            }
            else { 

                mainMenu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-300, -540), .75f);
                LastButton.Select();

            }
            return mainMenu;

        }
        GameObject CharacterSelect;
       GameObject SetupCharacterSelect() {
            //swift to right then destoy
            mainMenu.GetComponent<RectTransform>().DOAnchorPosX(300, 1);

            if (!CharacterSelect)
            {
                //ToDo add another panel with a cancel button 
                GameObject panel = UIGM.GetPanel(MainCanvas.transform, new Vector2(500, 300), new Vector2(960, 230));
                panel.GetComponent<RectTransform>().DOAnchorPosY(-530, 1);
                panel.AddComponent<HorizontalLayoutGroup>();
                Button main = UIGM.UIButton(panel.transform, "left");
                main.onClick.AddListener(() =>
                {
                    //Make this an event or send message;
                    //GameMaster.Instance.SelectCharacter(0);
                    Object.Destroy(panel.gameObject);
                });
                main.Select();

                UIGM.UIButton(panel.transform, "right").onClick.AddListener(() =>
                {
                   // GameMaster.Instance.SelectCharacter(1);
                    Object.Destroy(panel.gameObject);

                });

                Button cancel = UIGM.UIButton(panel.transform, "Cancel");

                cancel.onClick.AddListener(() =>
                {
                    CreateMainMenu();
                    panel.GetComponent<RectTransform>().DOAnchorPosY(230, 1);
                });
                CharacterSelect = panel;
            }
            else
            {
                CharacterSelect.GetComponent<RectTransform>().DOAnchorPosY(-530, 1);

            }
            return CharacterSelect;
        }

        GameObject LoadFiles;
        public void SetupLoadSaveFiles() {
            //swift to right then destoy
            mainMenu.GetComponent<RectTransform>().DOAnchorPosX(300, 1);
            if (!LoadFiles)
            {
                LoadFiles = UIGM.GetPanel(MainCanvas.transform, new Vector2(1000, 300), new Vector2(960, 430) ,Dreamers.Global.LayoutGroup.Vertical);
                LoadFiles.GetComponent<RectTransform>().DOAnchorPosY(-530, 1);
                foreach (SaveDisplayData save in GetSaveData) 
                {
                   Button loadSave = UIGM.UIButton(LoadFiles.transform, save.DateOfSave);
                    loadSave.onClick.AddListener(() => {
                        SaveSystem.Instance.LoadGame(save.SaveNumber);
                    });
                }

                //add an if No save file set cancel to select
                Button cancel = UIGM.UIButton(LoadFiles.transform, "Cancel");
                cancel.Select();
                cancel.onClick.AddListener(() =>
                {
                    CreateMainMenu();
                    LoadFiles.GetComponent<RectTransform>().DOAnchorPosY(430, 1);
                    Object.Destroy(LoadFiles, 1.25f);
                });
            }
            else { 
                LoadFiles.GetComponent<RectTransform>().DOAnchorPosY(-530, 1);
            }

        }
        public void SetupOptions() {
            //swift to right then destoy
            mainMenu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-300, -540), .75f);
        }
        public void LoadCredits() {
            //swift to right then destoy
            mainMenu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-300, -540), .75f);
        }
 
    }
}