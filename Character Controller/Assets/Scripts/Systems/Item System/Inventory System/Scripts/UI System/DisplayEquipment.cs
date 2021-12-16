using Dreamers.InventorySystem.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem;


namespace Dreamers.InventorySystem.UISystem
{
    public partial class DisplayMenu
    {
        public class EquiqmentPanel : Panel
        {
            public BaseCharacter Character;
            private EquipmentBase Equipment => CharacterInventory.Equipment;
            private CharacterInventory CharacterInventory => Character.GetComponent<CharacterInventory>();

            InventoryPanel inventoryPanel;
            public EquiqmentPanel(Vector2 Size, Vector2 Position, BaseCharacter Character, InventoryPanel inventoryPanel)
            {
                Setup(Size, Position);
                this.Character = Character;
                this.inventoryPanel = inventoryPanel;
             
            }
            List<Button> armors;

            List<Button> weapons;
            public override GameObject CreatePanel(Transform Parent)
            {
               if (Top)
                    { Object.Destroy(Top); }

                armors = new List<Button>();
                weapons = new List<Button>();
                Top = Manager.GetPanel(Parent, new Vector2(400, 400), new Vector2(0, 150));
                GridLayoutGroup CurrentEquips = Top.AddComponent<GridLayoutGroup>();
                CurrentEquips.transform.localScale = Vector3.one;
                    CurrentEquips.padding = new RectOffset() { bottom = 15, top = 15, left = 15, right = 15 };
                    CurrentEquips.childAlignment = TextAnchor.MiddleCenter;
                    CurrentEquips.spacing = new Vector2(10, 10);


                    for (int i = 0; i < System.Enum.GetValues(typeof(ArmorType)).Length; i++)
                    {
                        if (Equipment.EquippedArmor.TryGetValue((ArmorType)i, out ArmorSO value))
                        {
                          armors.Add(  ItemIconDisplay(CurrentEquips.transform, value));
                        }
                        else
                        {
                          armors.Add(  ItemIconDisplay(CurrentEquips.transform, null));
                        }
                    }

                    for (int i = 0; i < System.Enum.GetValues(typeof(WeaponSlot)).Length; i++)
                    {
                        if (Equipment.EquippedWeapons.TryGetValue((WeaponSlot)i, out WeaponSO value))
                        {
                          weapons.Add(ItemIconDisplay(CurrentEquips.transform, value));
                        }
                        else
                        {
                           weapons.Add( ItemIconDisplay(CurrentEquips.transform, null));
                        }
                    }
                    return Top;
                
            }

            public override void DestoryPanel()
            {
                Object.Destroy(Top);
            }

            public override void Refresh()
            {

                GridLayoutGroup CurrentEquips = Top.GetComponent<GridLayoutGroup>();

                GameObject buttonToDelete;
                int index;
                Button buttonToAdd;
                switch (itemSlot.Item.Type)
                {
                    case ItemType.Armor:
                        ArmorSO armorTemp = (ArmorSO)itemSlot.Item;
                        if (Equipment.EquippedArmor.TryGetValue(armorTemp.ArmorType, out ArmorSO armorValue))
                        {
                            buttonToDelete = armors[(int)armorTemp.ArmorType].gameObject;
                            index = buttonToDelete.transform.GetSiblingIndex();
                            Object.Destroy(buttonToDelete);
                            armors.RemoveAt((int)armorTemp.ArmorType);
                            buttonToAdd = ItemIconDisplay(CurrentEquips.transform, armorValue);
                            buttonToAdd.transform.SetSiblingIndex(index);
                            armors.Insert((int)armorTemp.ArmorType, buttonToAdd);
                        }
                        break;
                    case ItemType.Weapon:
                        WeaponSO weaponTemp = (WeaponSO)itemSlot.Item;
                        if (Equipment.EquippedWeapons.TryGetValue(weaponTemp.Slot, out WeaponSO weaponValue))
                        {
                            buttonToDelete = weapons[(int)weaponTemp.Slot].gameObject;
                            index = buttonToDelete.transform.GetSiblingIndex();
                            Object.Destroy(buttonToDelete);
                            weapons.RemoveAt((int)weaponTemp.Slot);
                            buttonToAdd = ItemIconDisplay(CurrentEquips.transform, weaponValue);
                            buttonToAdd.transform.SetSiblingIndex(index);
                            weapons.Insert((int)weaponTemp.Slot, buttonToAdd);
                        }
                        break;
                }

                inventoryPanel.Refresh();
            }

            private ItemSlot itemSlot;
            public void Refresh(ItemType itemType, int index) {
                GridLayoutGroup CurrentEquips = Top.GetComponent<GridLayoutGroup>();

                GameObject buttonToDelete;
                int indexOF;
                Button buttonToAdd;

                switch (itemType)
                {
                    case ItemType.Armor:
                        buttonToDelete = armors[index].gameObject;
                        indexOF = buttonToDelete.transform.GetSiblingIndex();
                        Object.Destroy(buttonToDelete);
                        armors.RemoveAt(index);
                        buttonToAdd = ItemIconDisplay(CurrentEquips.transform, null);
                        buttonToAdd.transform.SetSiblingIndex(indexOF);
                        armors.Insert(index, buttonToAdd );

                        break;
                    case ItemType.Weapon:
                        buttonToDelete = weapons[index].gameObject;
                        indexOF = buttonToDelete.transform.GetSiblingIndex();
                        Object.Destroy(buttonToDelete);
                        weapons.RemoveAt(index);
                        buttonToAdd = ItemIconDisplay(CurrentEquips.transform, null);
                        buttonToAdd.transform.SetSiblingIndex(indexOF);
                        weapons.Insert(index, buttonToAdd);

                

                        break;

                }

            }

                public void Refresh(ItemSlot itemSlot)
            {
                this.itemSlot = itemSlot;
                Refresh();
            }

            Button ItemIconDisplay(Transform Parent, ItemBaseSO so)
            {
                Button temp = Manager.UIButton(Parent, "ItemName");
                if (so != null)
                {
                    temp.image.sprite = so.Icon;
                    if (!so.Icon)
                        temp.image.color = new Color() { a = 0.0f };
                    switch (so.Type)
                    {
                        case ItemType.Armor:

                            IEquipable equippedItem = (IEquipable)so;
                            temp.onClick.AddListener(() =>
                            {
                                ArmorSO armorToRemove = (ArmorSO)so;
                                //TODO Link To PlayerStat Panel
                                //playerStats = CreatePlayerPanel(MenuPanelParent.transform);
                                //TODO add inventory refresh
                               this.Refresh(so.Type, (int)armorToRemove.ArmorType);
                                equippedItem.Unequip(CharacterInventory, Character);
                                inventoryPanel.Refresh();

                            });
                            break;
                        case ItemType.Weapon:

                            IEquipable equippedWeapon = (IEquipable)so;
                            temp.onClick.AddListener(() =>
                            {
                                equippedWeapon.Unequip(CharacterInventory, Character);
                                //TODO Link To PlayerStat Panel
                               this.Refresh(so.Type,weapons.IndexOf(temp) );
                                inventoryPanel.Refresh();

                            });
                            break;
                    }
                }
                return temp;
            }
        }

        public static EquiqmentPanel GetEquiqmentPanel;
    }
}