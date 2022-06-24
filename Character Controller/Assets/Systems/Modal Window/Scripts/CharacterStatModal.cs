using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.SO;

namespace Dreamers.InventorySystem.UISystem
{
    public class CharacterStatModal : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] Transform headerArea;
        [SerializeField] TextMeshProUGUI titleField;
        [Header("Body")]
        [SerializeField] Transform contentArea;
        [SerializeField] TextMeshProUGUI attributeNames;
        [SerializeField] TextMeshProUGUI attributeValues;
        [SerializeField] Button EquipmentButton;
        List<Button> curEquip;
        Sprite defaultButton;
        [Header("Footer")]
        [SerializeField] Transform footerArea;
        //TODO add on changesystem;
        private void Awake()
        {
            defaultButton = EquipmentButton.image.sprite;
        }
        public void ShowAsCharacterStats(BaseCharacter character)
        {
            CharacterInventory inventory = character.GetComponent<CharacterInventory>();
            EquipmentBase equipmentBase = inventory.Equipment;

            titleField.text = character.Name;
            UpdatePlayerStatsText(character);
            ShowEquipmentGrid(character, equipmentBase, inventory);
        }

        public void ShowEquipmentGrid(BaseCharacter character, EquipmentBase equipmentBase, CharacterInventory inventory)
        {
            curEquip = new List<Button>();
            curEquip.Add(EquipmentButton);
            for (int i = 0; i < 8; i++)
            {
                curEquip.Add(Instantiate(EquipmentButton, EquipmentButton.transform.parent));
            }
            UpdateEquipmentGrid(character, equipmentBase, inventory);

        }

        public void UpdateEquipmentGrid(BaseCharacter character, EquipmentBase equipmentBase, CharacterInventory inventory)
        {

            for (int i = 0; i < System.Enum.GetValues(typeof(WeaponSlot)).Length; i++)
            {
                int index = i;
                curEquip[index].onClick = new Button.ButtonClickedEvent();
                if (equipmentBase.EquippedWeapons.TryGetValue((WeaponSlot)i, out WeaponSO value))
                {
                    curEquip[index].image.sprite = value.Icon;
                    curEquip[i].GetComponentInChildren<TextMeshProUGUI>().text = value.ItemName;
                    curEquip[index].onClick.AddListener(() => {
                        value.Unequip(inventory);
                        curEquip[index].image.sprite = defaultButton;
                        curEquip[index].GetComponentInChildren<TextMeshProUGUI>().text = "";
                        UpdatePlayerStatsText(character);
                        var item = transform.root.GetComponentInChildren<ItemModalWindow>();
                        if (item)
                        {
                            item.Refresh();
                        }
                        curEquip[index].onClick = new Button.ButtonClickedEvent();
                    });
                }
                else
                {
                    curEquip[i].image.sprite = defaultButton;
                    curEquip[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                    ;
                }
            }

            for (int i = 0; i < System.Enum.GetValues(typeof(ArmorType)).Length; i++)
            {
                int index = i + 3;
                curEquip[index].onClick = new Button.ButtonClickedEvent();

                if (equipmentBase.EquippedArmor.TryGetValue((ArmorType)i, out ArmorSO value))
                {
                    curEquip[index].image.sprite = value.Icon;
                    curEquip[index].GetComponentInChildren<TextMeshProUGUI>().text = value.ItemName;
                    curEquip[index].onClick.AddListener(() => {
                        value.Unequip(inventory, character);
                        curEquip[index].image.sprite = defaultButton;
                        curEquip[index].GetComponentInChildren<TextMeshProUGUI>().text = "";
                        UpdatePlayerStatsText(character);
                        var item = transform.root.GetComponentInChildren<ItemModalWindow>();
                        if (item)
                        {
                            item.Refresh();
                        }
                        curEquip[index].onClick = new Button.ButtonClickedEvent();
                    });
                }
                else
                {
                    curEquip[index].image.sprite = defaultButton;
                    curEquip[index].GetComponentInChildren<TextMeshProUGUI>().text = "";

                }
            }



        }


        public void UpdatePlayerStatsText(BaseCharacter character)
        {
            attributeNames.text = "Lvl: ";
            attributeValues.text = character.Level.ToString() + "\n";
            attributeNames.text += "\nHealth:\t";
            attributeValues.text += character.CurHealth + "/" + character.MaxHealth;
            attributeNames.text += "\nMana:\t\n";
            attributeValues.text += "\n" + character.CurMana + "/" + character.MaxMana + "\n";

            for (int i = 1; i < System.Enum.GetValues(typeof(AttributeName)).Length; i++)
            {
                attributeNames.text += "\n" + ((AttributeName)i).ToString() + ":";
                attributeValues.text += "\n" + character.GetPrimaryAttribute(i).BaseValue;
                attributeValues.text += " + " + character.GetPrimaryAttribute(i).BuffValue;
                attributeValues.text += " + " + character.GetPrimaryAttribute(i).AdjustBaseValue;
            }

        }

    }
}