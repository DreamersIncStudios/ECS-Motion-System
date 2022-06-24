using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Dreamers.ModalWindows
{
    public class ModalMenu : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] Transform headerArea;
        [SerializeField] TextMeshProUGUI titleField;
        [Header("Content")]
        [SerializeField] Transform contentArea;
        [SerializeField] Button SelectionButton;
        [SerializeField] Button alternativeButton;
        CanvasGroup group => GetComponent<CanvasGroup>();
        RectTransform windowRect => GetComponent<RectTransform>();

        public void DisplayMenu(string title, List<MenuButtons> buttons, string exitText = null, UnityEvent ExitAction = null) {

            headerArea.gameObject.SetActive(!string.IsNullOrEmpty(title));
            titleField.text = title;
            List<Button> selectionButtons = new List<Button>();
            selectionButtons.Add(SelectionButton);
            for (int i = 0; i < buttons.Count - 1; i++)
            {
                selectionButtons.Add(Instantiate(SelectionButton, SelectionButton.transform.parent));
            }
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                selectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = buttons[i].text;
                selectionButtons[i].onClick.AddListener(() => { buttons[index].actionToTake.Invoke(); });
            }
            selectionButtons[0].Select();
            alternativeButton.gameObject.SetActive(!string.IsNullOrEmpty(exitText));
            alternativeButton.transform.SetAsLastSibling();
            alternativeButton.GetComponentInChildren<TextMeshProUGUI>().text = exitText;
            alternativeButton.onClick.AddListener(() =>
            {
                ExitAction.Invoke();
            });
        }
    }
    [System.Serializable]
    public struct MenuButtons
    {
        public string text;
        public UnityEvent actionToTake;

    }
}
