using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Dreamers.InventorySystem;
using UnityStandardAssets.CrossPlatformInput;

namespace Dreamers.ModalWindows
{
    public class QuickAccessMenu : MonoBehaviour
    {
        public RectTransform Base;
        public GameObject ContentArea;
        public GameObject ButtonPrefab;
        CharacterInventory character;
        bool casting => CrossPlatformInputManager.GetAxis("Target Trigger") > .3f; //TODO rename Target Trigger
        bool shown = false;
   

        private void Update()
        {
            if(!shown && casting)
                DisplayQuickAccessMenu();

            if (shown && !casting)
                HideQuickAccesMenu();
        }

        public void DisplayQuickAccessMenu() {
            if (!character)
                character = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterInventory>();
            Base.DOAnchorPosY(-250, .75f);
            shown = true;
        }
        public void HideQuickAccesMenu() { 
            Base.DOAnchorPosY(-700, .75f);
            shown = false;


        }
        public void DisplaySpells() {
            ClearContentArea();
        }
        public void DisplayItems() {
            ClearContentArea();

        }
        public void DisplayAbilities() {
            ClearContentArea();
        }
        public void DisplaySummons() { 
            ClearContentArea();
        }
        public void DisplayBase() { 
            ClearContentArea();
        }
        void ClearContentArea() { 

            foreach (Transform child in ContentArea.transform)
            {
                Destroy(child.gameObject);
            }
        
        }

    }
}
