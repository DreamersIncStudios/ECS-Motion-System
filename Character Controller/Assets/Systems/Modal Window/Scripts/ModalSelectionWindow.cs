using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace Dreamers.ModalWindows
{
    public class ModalSelectionWindow : MonoBehaviour
    {

        [Header("Header")]
        [SerializeField] Transform headerArea;
        [SerializeField] TextMeshProUGUI titleField;

        [Header("Content")]
        [SerializeField] Transform contentArea;
        [SerializeField] Button selectionButton;

        [Header("Footer")]
        [SerializeField] Transform footerArea;
        [SerializeField] Button confirmButton;
        [SerializeField] Button declineButton;
        [SerializeField] Button alternativeButton;

        RectTransform windowRect => GetComponent<RectTransform>();
        CanvasGroup group => GetComponent<CanvasGroup>();
        #region Character Selection Window
        public void ShowAsCharacterSelection(string title, List<CharacterSelect> buttons,string confirm= null, UnityEvent confirmAction = null, string decline =null, UnityEvent declineAction = null, string alt = null, UnityEvent alternativeAction = null) {
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
                if(buttons[i].Icon != null)
                selectionButtons[i].GetComponent<Image>().sprite = buttons[i].Icon;
                selectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = buttons[index].CharacterName+"\n"+ buttons[index].CharacterWeapon;
            }
            selectionButtons[0].Select();
            confirmButton.gameObject.SetActive(confirmAction != null);
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirm;

            confirmButton.onClick.AddListener(() =>
            {
                confirmAction.Invoke();
            });

            declineButton.gameObject.SetActive(declineAction != null);
            declineButton.GetComponentInChildren<TextMeshProUGUI>().text = decline;

            declineButton.onClick.AddListener(() =>
            {
                declineAction.Invoke();
                group.DOFade(0.0f, 3.5f);
                Destroy(gameObject, 4);


            });

            alternativeButton.gameObject.SetActive(alternativeAction != null);
            alternativeButton.GetComponentInChildren<TextMeshProUGUI>().text = alt;

            alternativeButton.onClick.AddListener(() =>
            {
                alternativeAction.Invoke();
            });
        }
        public void ShowAsCharacterSelection(List<CharacterSelect> buttons, UnityEvent Back) {
           ShowAsCharacterSelection("Select Your Character", buttons , "", null, "Back", Back, "",null );
        }

        #endregion
    }

    [System.Serializable]
    public struct CharacterSelect {
        public string CharacterName, CharacterWeapon;
        public Sprite Icon;
        public UnityEvent Action;
    }
}