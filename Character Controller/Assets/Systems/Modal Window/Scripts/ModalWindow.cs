using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Dreamers.InventorySystem.Interfaces;

namespace Dreamers.ModalWindows
{
    public class ModalWindow : MonoBehaviour
    {

        [Header("Header")]
        [SerializeField] Transform headerArea;
        [SerializeField] TextMeshProUGUI titleField;
        [Header("Content")]
        [SerializeField] Transform contentArea;
        [SerializeField] Transform VerticalLayout;
        [SerializeField] Image heroImage;
        [SerializeField] TextMeshProUGUI heroText;
        [SerializeField] Transform horizontialLayout;
        [SerializeField] Transform iconContainer;
        [SerializeField] Image iconImage;
        [SerializeField] TextMeshProUGUI iconText;
        [SerializeField] Transform iconLayout;
        [SerializeField] Button selectionButton;
        [Header("Footer")]
        [SerializeField] Transform footerArea;
        [SerializeField] Button confirmButton;
        [SerializeField] Button declineButton;
        [SerializeField] Button alternativeButton;
        RectTransform windowRect => GetComponent<RectTransform>();
        CanvasGroup group => GetComponent<CanvasGroup>();
        #region Hero Window

        public void ShowAsHero(string title, Sprite imageToShow, string Message, UnityEvent confirmAction, UnityEvent declineAction = null, UnityEvent alternativeAction = null)
        {
            horizontialLayout.gameObject.SetActive(false);
            VerticalLayout.gameObject.SetActive(true);
            headerArea.gameObject.SetActive(!string.IsNullOrEmpty(title));
            titleField.text = title;

            heroImage.sprite = imageToShow;
            heroText.text = Message;

            confirmButton.onClick.AddListener(() =>
            {
                confirmAction.Invoke();
            });

            declineButton.gameObject.SetActive(declineAction != null);

            declineButton.onClick.AddListener(() =>
            {
                declineAction.Invoke();
            });

            alternativeButton.gameObject.SetActive(alternativeAction != null);
            alternativeButton.onClick.AddListener(() =>
            {
                alternativeAction.Invoke();
            });
        }

        public void ShowAsHero(string title, Sprite imageToShow, string Message, string confirmMessage, string declineMessage, string altMessage, UnityEvent confirmAction, UnityEvent declineAction = null, UnityEvent alternativeAction = null)
        {
            declineButton.GetComponentInChildren<TextMeshProUGUI>().text = declineMessage;
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmMessage;
            alternativeButton.GetComponentInChildren<TextMeshProUGUI>().text = altMessage;

            ShowAsHero(title, imageToShow, Message, confirmAction, declineAction, alternativeAction);
        }
        public void ShowAsHero(string title, Sprite imageToShow, string message, UnityEvent confirmAction)
        {
            ShowAsHero(title, imageToShow, message, "Continue", "", "", confirmAction);
        }
        public void ShowAsHero(string title, Sprite imageToShow, string message, UnityEvent confirmAction, UnityEvent declineAction)
        {
            ShowAsHero(title, imageToShow, message, "Continue", "Back", "", confirmAction, declineAction);
        }
        #endregion

        #region Selection Window
        public void ShowAsSelection(string title, List<SelectionIcons> buttons, UnityEvent cancel, bool GoBackEnabled = true) {
            horizontialLayout.gameObject.SetActive(false);
            VerticalLayout.gameObject.SetActive(false);
            iconLayout.gameObject.SetActive(true);

            headerArea.gameObject.SetActive(!string.IsNullOrEmpty(title));
            titleField.text = title;

            List<Button> selectionButtons = new List<Button>();
            selectionButtons.Add(selectionButton);
            for (int i = 0; i < buttons.Count - 1; i++)
            {
                selectionButtons.Add(Instantiate(selectionButton, selectionButton.transform.parent));
            }
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                selectionButtons[i].onClick.AddListener(() => {
                    buttons[index].Action.Invoke();
                    windowRect.DOAnchorPos(new Vector2(1200, 0), 3);
                    Destroy(this.gameObject, 4);
                });
                selectionButtons[i].GetComponent<Image>().sprite = buttons[i].Icon;
                selectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = buttons[index].IconName;
            }
            selectionButton.Select();
            //Footer 

            confirmButton.gameObject.SetActive(false);
            alternativeButton.gameObject.SetActive(false);
            if (GoBackEnabled) {
                declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Back";
                declineButton.onClick.AddListener(() =>
                {
                    windowRect.DOAnchorPos(new Vector2(1200, 0), 3);
                    cancel.Invoke();
                    Destroy(this.gameObject,4);
                });
            } else
            { declineButton.gameObject.SetActive(false); }
        }


        #endregion

        #region Prompt
        public void ShowAsPrompt(string title, Sprite imageToShow, string message, string confirmText, UnityEvent confirmAction, string declineText = null, UnityEvent declineAction = null)
        {
            group.alpha = 0.0f;
            group.DOFade(1.0f, 2.5f);
            horizontialLayout.gameObject.SetActive(true);
            VerticalLayout.gameObject.SetActive(false);
            headerArea.gameObject.SetActive(!string.IsNullOrEmpty(title));
            titleField.text = title;
            iconLayout.gameObject.SetActive(false);
            iconContainer.gameObject.SetActive(imageToShow != null);
            iconImage.sprite = imageToShow;
            iconText.fontSize = 24;
            iconText.text = message;
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmText;
            confirmButton.onClick.AddListener(() =>
            {
                group.DOFade(0.0f, 2.5f);
                confirmAction.Invoke();
                Destroy(gameObject,3);
            });

            declineButton.gameObject.SetActive(declineAction != null);
            declineButton.GetComponentInChildren<TextMeshProUGUI>().text = declineText;

            declineButton.onClick.AddListener(() =>
            {
                group.DOFade(0.0f, 2.5f);
                declineAction.Invoke();
                Destroy(gameObject,3);
            });

            alternativeButton.gameObject.SetActive(false);

        }
        public void ShowAsPrompt(string title, Sprite imageToShow, string message, UnityEvent confirmAction, UnityEvent declineAction = null)
        {
            ShowAsPrompt(title, imageToShow, message, "Yes", confirmAction, "No", declineAction);
        }
        #endregion

        public void ShowAsItemPrompt(string itemName, string itemDescription, Sprite itemIcon, ItemType type, UnityEvent UseItem, UnityEvent DropItem) {
            group.alpha = 0.0f;
            group.DOFade(1.0f, .5f);
            horizontialLayout.gameObject.SetActive(true);
            VerticalLayout.gameObject.SetActive(false);
            headerArea.gameObject.SetActive(!string.IsNullOrEmpty(itemName));
            titleField.text = itemName;
            iconLayout.gameObject.SetActive(false);
            iconContainer.gameObject.SetActive(itemIcon != null);
            iconImage.sprite = itemIcon;
            iconText.fontSize = 24;
            iconText.text = itemDescription;
            switch (type)
            {
                case ItemType.Armor:
                case ItemType.Weapon:

                    confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                    alternativeButton.gameObject.SetActive(true);
                    break;
                case ItemType.General:
                    confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
                    alternativeButton.gameObject.SetActive(true);
                    break;

                case ItemType.Crafting_Materials:
                    alternativeButton.gameObject.SetActive(true);
                    confirmButton.gameObject.SetActive(false);
                    break;
                case ItemType.Blueprint_Recipes:
                case ItemType.Quest:
                    alternativeButton.gameObject.SetActive(false);
                    confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";

                    break;
            }

            confirmButton.onClick.AddListener(() =>
            {
                group.DOFade(0.0f, .5f);
                UseItem.Invoke();
                Destroy(gameObject, .6f);
            });
            confirmButton.Select();
            alternativeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drop Item";
            alternativeButton.onClick.AddListener(() =>
            {
                group.DOFade(0.0f, .5f);
                DropItem.Invoke();
                Destroy(gameObject, .6f);
            });
            declineButton.gameObject.SetActive(true);
            declineButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";

            declineButton.onClick.AddListener(() =>
            {
                group.DOFade(0.0f, .5f);
                Destroy(gameObject, .6f);
            });
        }
        public void ShowAsItemPrompt(ItemBaseSO item, UnityEvent UseItem, UnityEvent DropItem) {
            ShowAsItemPrompt(item.ItemName, item.Description, item.Icon, item.Type, UseItem, DropItem);
        }


    }
    [System.Serializable]
    public struct SelectionIcons {
        public Sprite Icon;
        public string IconName;
        public string MouseOverText;
        public UnityEvent Action;
    }
}