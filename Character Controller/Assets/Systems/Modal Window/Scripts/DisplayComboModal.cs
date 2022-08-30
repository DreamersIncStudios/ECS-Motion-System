using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DreamersInc.ComboSystem;


namespace Dreamers.ModalWindows
{
    public class DisplayComboModal : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] Transform headerArea;
        [SerializeField] TextMeshProUGUI titleField;
        [Header("Content")]
        [SerializeField] Transform contentArea;
        public GameObject ComboDisplay;
        public ComboSO test;

        public void DisplayCombo(ComboSO comboSO) {
            titleField.text = "Attack list for " + comboSO.name;
            ComboDisplay.SetActive(false);
            List<ComboDefinition> comboDefinitions = comboSO.GetComboDefinitions();
            foreach (ComboDefinition comboDefinition in comboDefinitions) {
                GameObject comboToDisplay = Instantiate(ComboDisplay, contentArea);
                comboToDisplay.SetActive(true);
                TextMeshProUGUI comboInfo = comboToDisplay.GetComponentInChildren<TextMeshProUGUI>();
                comboInfo.text = comboDefinition.name + ": " ;
                var comboQueue = comboDefinition.test.ToArray();
                for (int i = 0; i < comboQueue.Length; i++)
                 {
                    switch (comboQueue[i]) {
                        case AttackType.LightAttack:
                            comboInfo.text += "X";
                                break;
                        case AttackType.HeavyAttack:
                            comboInfo.text += "Y";
                            break;
                        case AttackType.ChargedLightAttack:
                            comboInfo.text += "Hold X";
                            break;
                        case AttackType.ChargedHeavyAttack:
                            comboInfo.text += "Hold Y";
                            break;
                        case AttackType.Projectile:
                            comboInfo.text += "B";
                            break;
                        case AttackType.ChargedProjectile:
                            comboInfo.text += "Hold B";
                            break;

                    }
                    if (i != comboQueue.Length-1)
                        comboInfo.text += " + ";
                }
                Button unlockButton = comboToDisplay.GetComponentInChildren<Button>();
                if (comboDefinition.Unlocked)
                {
                    unlockButton.interactable = false;
                    unlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlocked";
                }
                else
                {
                    unlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock add cost"; //Todo add unlock value;
                    unlockButton.onClick.AddListener(() =>
                    {
                        comboDefinition.Unlocked = true;

                    });
                }
            }

        }

    }
}