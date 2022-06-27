using Dreamers.Global;
using Dreamers.InventorySystem.Base;
using Dreamers.ModalWindows;
using Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dreamers.InventorySystem.UISystem
{
    public class DisplayCharacterUI : MonoBehaviour
    {
        UIManager Manager;
        [SerializeField] GameObject baseUI;
        bool OpenCloseMenu => Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.JoystickButton7);
        [SerializeField] Canvas getCanvas;
        public bool Displayed { get; private set; }
        InventoryBase Inventory => character.GetComponent<CharacterInventory>().Inventory;
        [SerializeField] List<MenuButtons> menuItems;
        private void Awake()
        {
            Manager = UIManager.instance;
        }
      
        // Update is called once per frame
        void Update()
        {
            if (OpenCloseMenu)
            {
                if (!character)
                    character = FindObjectOfType<PlayerCharacter>();
                if (!Displayed)
                {
                    OpenCharacterMenu(Inventory);
                    baseUI.SetActive(false); //TODO add fade out 
                }
                else
                {
                    CloseCharacterMenu();
                    baseUI.SetActive(true);//TODO add fade In

                }
            }

        }



        public void CloseCharacterMenu()
        {
            foreach (Transform child in getCanvas.transform)
            {
                Destroy(child.gameObject);
            }

            Displayed = false;
        }

       [SerializeField] BaseCharacter character;

        void CreateMenu()
        {
            Manager = UIManager.instance;
            CreateSideMenu();
            CharacterStatModal characterStat = Instantiate(Manager.StatsWindow, getCanvas.transform).GetComponent<CharacterStatModal>();
            characterStat.ShowAsCharacterStats(character);
            ItemModalWindow items = Instantiate(Manager.InventoryWindow, getCanvas.transform).GetComponent<ItemModalWindow>();
            items.ShowAsCharacterInventory(character.GetComponent<CharacterInventory>());
        }
        public void OpenCharacterMenu(InventoryBase inventory)
        {

            CreateMenu();
            Displayed = true;
        }

        void CreateSideMenu() {
            ModalMenu menu = Instantiate(Manager.ModalMenuPrefab, getCanvas.transform).GetComponent<ModalMenu>();
            menu.DisplayMenu("Still In Dev", menuItems);
        
        }
        public void OpenOptions() {
           // OptionUIPanel optionUI = Instantiate(Manager.optionsWindow, getCanvas.transform).GetComponent<OptionUIPanel>();
        }
    }
}