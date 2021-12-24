using Dreamers.Global;
using System.Collections.Generic;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamers.InventorySystem.UISystem
{
    public partial class DisplayMenu
    {
        public class InventoryPanel : Panel {
            ItemType DisplayItems;
            GameObject itemsDisplayerPanel { get; set; }
            CharacterInventory CharacterInventory => Character.GetComponent<CharacterInventory>();
            public EquiqmentPanel equiqmentPanel;

            InventoryBase Inventory => CharacterInventory.Inventory;

            BaseCharacter Character;
            public InventoryPanel(Vector2 Size, Vector2 Position, BaseCharacter Character)
            {
                Setup(Size, Position);
                this.Character = Character;
                DisplayItems = (ItemType)0;
            }

            public override GameObject CreatePanel(Transform Parent)
            {
                if (Top)
                    Object.Destroy(Top);
                Top = Manager.GetPanel(Parent, Size, Position);
                Top.transform.SetSiblingIndex(1);
                VerticalLayoutGroup VLG = Top.AddComponent<VerticalLayoutGroup>();
                Top.name = "Item Window";
                VLG.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
                VLG.childAlignment = TextAnchor.UpperCenter;
                VLG.childControlHeight = true; VLG.childControlWidth = true;
                VLG.childForceExpandHeight = false; VLG.childForceExpandWidth = true;

                Text titleGO = Manager.TextBox(Top.transform, new Vector2(400, 50)).GetComponent<Text>();
                titleGO.alignment = TextAnchor.MiddleCenter;
                titleGO.text = "Inventory";
                titleGO.fontSize = 24;
                titleGO.name = "Inventory Title TextBox";
                HorizontalLayoutGroup InventoryPanel = Manager.GetPanel(Top.transform, new Vector2(400, 900), new Vector2(0, 150)).AddComponent<HorizontalLayoutGroup>();
                InventoryPanel.name = " Control Display Buttons";
                InventoryPanel.childControlHeight = false;
                InventoryPanel.childForceExpandHeight = false;
                Top.name = "Items Window";

                for (int i = 0; i < 7; i++)
                {
                    int test = i;
                    Button Temp;
                    if (i == 0)
                    {
                        Temp = Manager.UIButton(InventoryPanel.transform, "All");
                        Temp.name = "All";


                    }
                    else
                    {
                        Temp = Manager.UIButton(InventoryPanel.transform, ((ItemType)i).ToString());

                        Temp.name = ((ItemType)i).ToString();
                    }
                    Temp.onClick.AddListener(() =>
                    {
                        DisplayItems = (ItemType)test;
                        itemsDisplayerPanel = ItemsDisplayPanel(Top.transform, Inventory, DisplayItems);
                    });

                }
                itemsDisplayerPanel = ItemsDisplayPanel(Top.transform, Inventory, DisplayItems);

                return Top;
            }

            public override void DestoryPanel()
            {
                Object.Destroy(Top);

            }
            public override void Refresh()
            {
                //TODO have this Implementation Cleaned up so that we are just adding a new icon to the list. It work right now 
                if(Top)
                CreatePanel(Top.transform.parent);
            }
            ItemBaseSO itemToAdd;
            public void Refresh(ItemBaseSO item) {
                this.itemToAdd = item;
                Refresh();
            }
            GameObject ItemsDisplayPanel(Transform Parent, InventoryBase inventory, ItemType Type)
            {
                if (itemsDisplayerPanel)
                {
                    Object.Destroy(itemsDisplayerPanel);
                }

                GridLayoutGroup Main = Manager.GetPanel(Parent, new Vector2(1400, 300), new Vector2(0, 150)).AddComponent<GridLayoutGroup>();
                Main.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
                Main.spacing = new Vector2(20, 20);


                for (int i = 0; i < inventory.ItemsInInventory.Count; i++)
                {
                    ItemSlot Slot = inventory.ItemsInInventory[i];
                    int IndexOf = i;

                 if (Slot.Item.Type == Type|| Type == ItemType.None)
                    {
                        Button temp = ItemButton(Main.transform, Slot);
                        temp.onClick.AddListener(() =>
                        {
                            GameObject pop = PopUpItemPanel(temp.GetComponent<RectTransform>().anchoredPosition
                                 + new Vector2(575, -175)
                                 , Slot, IndexOf,temp);
                        });
                    }
                }

                return Main.gameObject;
            }
            Button ItemButton(Transform Parent, ItemSlot Slot)
            {
                Button temp = Manager.UIButton(Parent, Slot.Item.ItemName);
                temp.name = Slot.Item.ItemName;
                Text texttemp = temp.GetComponentInChildren<Text>();
                texttemp.alignment = TextAnchor.LowerCenter;
                if (Slot.Item.Stackable)
                    texttemp.text += Slot.Count;
                temp.GetComponentInChildren<Text>().alignment = TextAnchor.LowerCenter;
                temp.GetComponent<Image>().sprite = Slot.Item.Icon;
                return temp;
            }


            GameObject PopUpItemPanel(Vector2 Pos, ItemSlot Slot, int IndexOf, Button itemButton)
            {
                GameObject PopUp = Manager.GetPanel(Manager.UICanvas().transform, new Vector2(300, 300), Pos);
                HorizontalLayoutGroup group = PopUp.AddComponent<HorizontalLayoutGroup>();
                PopUp.AddComponent<PopUpMouseControl>();

                group.childControlWidth = false;

                Text info = Manager.TextBox(PopUp.transform, new Vector2(150, 300));
                info.text = Slot.Item.ItemName + "\n";
                info.text += Slot.Item.Description;

                VerticalLayoutGroup ButtonPanel = Manager.GetPanel(PopUp.transform, new Vector2(150, 300), Pos).AddComponent<VerticalLayoutGroup>();

                switch (Slot.Item.Type)
                {
                    case ItemType.General:
                        Button use = Manager.UIButton(ButtonPanel.transform, "Use Item");
                        use.onClick.AddListener(() =>
                        {
                            RecoveryItemSO temp = (RecoveryItemSO)Slot.Item;
                            temp.Use(CharacterInventory, Character);

                            itemsDisplayerPanel = ItemsDisplayPanel(Top.transform, Inventory, DisplayItems);
                            Object.Destroy(PopUp);

                        });
                        info.text += "\nQuantity: " + Slot.Count;
                        break;
                    case ItemType.Armor:
                    case ItemType.Weapon:
                        Button Equip = Manager.UIButton(ButtonPanel.transform, "Equip");
                        Equip.onClick.AddListener(() =>
                        {
                            bool equipedItem = false;
                            switch (Slot.Item.Type)
                            {
                                case ItemType.Armor:
                                    ArmorSO Armor = (ArmorSO)Slot.Item;
                                  equipedItem=  Armor.EquipItem(CharacterInventory,  Character);
                                    break;
                                case ItemType.Weapon:
                                    WeaponSO weapon = (WeaponSO)Slot.Item;
                                    equipedItem = weapon.EquipItem(CharacterInventory, Character);
                                    break;
                            }
                            //TODO Implement Change Event here
                            if (equipedItem)
                            {
                                equiqmentPanel.Refresh(Slot);
                                Object.Destroy(itemButton.gameObject);
                            }
                                    //itemsDisplayerPanel = ItemsDisplayPanel(Top.transform, Inventory, DisplayItems);
                            //TODO refresh PlayerPanel Event system;
                        //    playerStats = CreatePlayerPanel(MenuPanelParent.transform);
                            Object.Destroy(PopUp);
                        });
                        Button Mod = Manager.UIButton(ButtonPanel.transform, "Modify");
                        Mod.onClick.AddListener(() => Debug.LogWarning("Implentation to be added once Skill/Magic system designed"));
                        Button Dismantle = Manager.UIButton(ButtonPanel.transform, "Dismantle");

                        break;
                    case ItemType.Quest:
                        Button View = Manager.UIButton(ButtonPanel.transform, "View Item");

                        break;
                    case ItemType.Blueprint_Recipes:
                        break;
                }
                if (Slot.Item.Type != ItemType.Quest)
                {
                    Button Drop = Manager.UIButton(ButtonPanel.transform, "Drop");
                    Drop.onClick.AddListener(() => {
                        CharacterInventory.Inventory.RemoveFromInventory(Slot.Item);
                        Object.Destroy(PopUp);
                    });
                }

                return PopUp;
            }

        }
        public InventoryPanel GetInventoryPanel;

    }
}
