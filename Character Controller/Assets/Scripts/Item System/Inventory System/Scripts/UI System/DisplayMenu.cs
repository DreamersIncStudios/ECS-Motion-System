
using Dreamers.Global;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamers.InventorySystem.UISystem
{
    public class DisplayMenu 
    {
        readonly UIManager Manager;
        public bool Displayed { get { return (bool)MenuPanelParent; } }

        public DisplayMenu(BaseCharacter player,CharacterInventory characterInventory) {
            Manager = UIManager.instance;
        }

        public void OpenInventory(InventoryBase inventory) {

            MenuPanelParent = CreateMenu(
        new Vector2(0, 0),
        new Vector2(0, 0));
            playerStats = CreatePlayerPanel();
            itemPanel = CreateItemPanel();

        }



        public void CloseInventory() {
             Object.Destroy(MenuPanelParent);
        }
        private InventoryBase Inventory => CharacterInventory.Inventory;
        private EquipmentBase Equipment=> CharacterInventory.Equipment;
        private BaseCharacter Character => GameObject.FindGameObjectWithTag("Player").GetComponent<BaseCharacter>();
        private CharacterInventory CharacterInventory => Character.GetComponent<CharacterInventory>();
       // PlayerCharacter PC;

        GameObject MenuPanelParent { get; set; }
        GameObject CreateMenu(Vector2 Size, Vector2 Position) {
            if (MenuPanelParent) 
                Object.Destroy(MenuPanelParent);

            GameObject Parent = Manager.UICanvas();
            GameObject MainPanel = Manager.GetPanel(Parent.transform, Size, Position);
            MainPanel.transform.localScale = Vector3.one;
            RectTransform PanelRect = MainPanel.GetComponent<RectTransform>();
            PanelRect.pivot = new Vector2(0.5f, .5f);
            PanelRect.anchorMax = new Vector2(1, 1);
            PanelRect.anchorMin = new Vector2(.0f, .0f);

            HorizontalLayoutGroup HLG = MainPanel.AddComponent<HorizontalLayoutGroup>();
            DisplayItems = (ItemType)0; // change to zero when all tab is added


            HLG.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            HLG.spacing = 10;
            HLG.childAlignment = TextAnchor.UpperLeft;
            HLG.childControlHeight = true; HLG.childControlWidth = false;
            HLG.childForceExpandHeight = true; HLG.childForceExpandWidth = true;

            return MainPanel;
        }
        GameObject playerStats { get;  set; }
        GameObject CreatePlayerPanel() {
            if (playerStats)
                Object.Destroy(playerStats);

            GameObject MainPanel = Manager.GetPanel(MenuPanelParent.transform, new Vector2(400, 300), new Vector2(0, 150));
            MainPanel.name = "Player Window";
            MainPanel.transform.SetSiblingIndex(0);
            VerticalLayoutGroup VLG = MainPanel.AddComponent<VerticalLayoutGroup>();
            VLG.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            VLG.childAlignment = TextAnchor.UpperCenter;
            VLG.childControlHeight = false; VLG.childControlWidth = true;
            VLG.childForceExpandHeight = false; VLG.childForceExpandWidth = true;

            Text titleGO = Manager.TextBox(MainPanel.transform, new Vector2(400, 50)).GetComponent<Text>();
            titleGO.alignment = TextAnchor.MiddleCenter;
            titleGO.text = " Player";
            titleGO.fontSize = 24;
            VerticalLayoutGroup PlayerStatsWindow = Manager.GetPanel(MainPanel.transform, new Vector2(400, 450), new Vector2(0, 150)).AddComponent<VerticalLayoutGroup>();
            PlayerStatsWindow.name = "Player Stats Window";
            PlayerStatsWindow.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            PlayerStatsWindow.childAlignment = TextAnchor.UpperCenter;
            PlayerStatsWindow.childControlHeight = true; PlayerStatsWindow.childControlWidth = true;
            PlayerStatsWindow.childForceExpandHeight = true; PlayerStatsWindow.childForceExpandWidth = true;
            
            Text statsText= Manager.TextBox(PlayerStatsWindow.transform, new Vector2(400, 50)).GetComponent<Text>();
            statsText.alignment = TextAnchor.UpperLeft;
            statsText.text = " Player";
            statsText.fontSize = 24;

            statsText.text = Character.Name + " Lvl: " + Character.Level;
            statsText.text += "\nHealth:\t\t" + Character.CurHealth+"/" + Character.MaxHealth;
            statsText.text += "\nMana:\t\t\t" + Character.CurMana + "/" + Character.MaxMana+"\n";

            for (int i = 0; i < System.Enum.GetValues(typeof(AttributeName)).Length; i++)
            {
                statsText.text += "\n"+((AttributeName)i).ToString() + ":\t\t\t" + Character.GetPrimaryAttribute(i).BaseValue;
                statsText.text += " + "+ Character.GetPrimaryAttribute(i).BuffValue;
                statsText.text += " + " + Character.GetPrimaryAttribute(i).AdjustBaseValue;


            }
            currentEquipWindow = CurrentEquipWindow(MainPanel.transform);

            return MainPanel;


        }
        GameObject currentEquipWindow { get; set; }
        GameObject CurrentEquipWindow(Transform Parent) {
            if (currentEquipWindow) 
             { Object.Destroy(currentEquipWindow); }
            GridLayoutGroup CurrentEquips = Manager.GetPanel(Parent, new Vector2(400, 400), new Vector2(0, 150)).AddComponent<GridLayoutGroup>();
            CurrentEquips.transform.localScale = Vector3.one;
            CurrentEquips.padding = new RectOffset() { bottom = 15, top = 15, left = 15, right = 15 };
            CurrentEquips.childAlignment = TextAnchor.MiddleCenter;
            CurrentEquips.spacing = new Vector2(10, 10);


            for (int i = 0; i < System.Enum.GetValues(typeof(ArmorType)).Length; i++)
            {
                if (Equipment.EquippedArmor.TryGetValue((ArmorType)i, out ArmorSO value))
                {
                    ItemIconDisplay(CurrentEquips.transform, Equipment.EquippedArmor[(ArmorType)i]);

                }
                else
                {
                    ItemIconDisplay(CurrentEquips.transform, null);
                }
            }

            for (int i = 0; i < System.Enum.GetValues(typeof(WeaponSlot)).Length; i++)
            {
                if (Equipment.EquippedWeapons.TryGetValue((WeaponSlot)i, out WeaponSO value))
                {
                    ItemIconDisplay(CurrentEquips.transform, Equipment.EquippedWeapons[(WeaponSlot)i]);

                }
                else
                {
                    ItemIconDisplay(CurrentEquips.transform, null);
                }
            }

            return CurrentEquips.gameObject;
        }

        // Making a button to remove 
        Button ItemIconDisplay(Transform Parent, ItemBaseSO so) {
            Button temp = Manager.UIButton(Parent, "ItemName");
            if (so != null)
            {
                temp.image.sprite = so.Icon;
                if (!so.Icon)
                    temp.image.color = new Color() { a = 0.0f };

                temp.onClick.AddListener(() =>
                {
                    so.Unequip(CharacterInventory, Character);
                    playerStats = CreatePlayerPanel();
                    itemPanel = CreateItemPanel();

                });
            }
            return temp;
        }
        ItemType DisplayItems;
        GameObject itemPanel { get; set; }
        GameObject CreateItemPanel()
        {
            if (itemPanel)
                Object.Destroy(itemPanel);

            GameObject MainPanel = Manager.GetPanel(MenuPanelParent.transform, new Vector2(1400, 300), new Vector2(0, 150));
            MainPanel.transform.SetSiblingIndex(1);
            VerticalLayoutGroup VLG = MainPanel.AddComponent<VerticalLayoutGroup>();
            MainPanel.name = "Item Window";
            VLG.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            VLG.childAlignment = TextAnchor.UpperCenter;
            VLG.childControlHeight =true; VLG.childControlWidth = true;
            VLG.childForceExpandHeight = false; VLG.childForceExpandWidth = true;

            Text titleGO = Manager.TextBox(MainPanel.transform, new Vector2(400, 50)).GetComponent<Text>();
            titleGO.alignment = TextAnchor.MiddleCenter;
            titleGO.text = "Inventory";
            titleGO.fontSize = 24;
            titleGO.name = "Inventory Title TextBox";
            HorizontalLayoutGroup InventoryPanel = Manager.GetPanel(MainPanel.transform, new Vector2(400, 900), new Vector2(0, 150)).AddComponent<HorizontalLayoutGroup>();
            InventoryPanel.name = " Control Display Buttons";
            InventoryPanel.childControlHeight = false;
            InventoryPanel.childForceExpandHeight = false;
            MainPanel.name = "Items Window";

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
                        itemsDisplayerPanel = ItemsDisplayPanel(MainPanel.transform, Inventory, DisplayItems);
                    });
                
            }
            itemsDisplayerPanel = ItemsDisplayPanel(MainPanel.transform, Inventory, DisplayItems);
        
            return MainPanel;
        }

        GameObject itemsDisplayerPanel { get; set; }
        GameObject ItemsDisplayPanel(Transform Parent, InventoryBase inventory, ItemType Type) {
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

                if (DisplayItems == ItemType.None)
                {
                    Button temp = ItemButton(Main.transform, Slot);
                    temp.onClick.AddListener(() =>
                    {
                        GameObject pop = PopUpItemPanel(temp.GetComponent<RectTransform>().anchoredPosition
                             + new Vector2(575, -175)
                             , Slot, IndexOf);
                        // pop.AddComponent<PopUpMouseControl>();
                    });
                }
                else if (Slot.Item.Type == Type)
                {
                    Button temp = ItemButton(Main.transform, Slot);
                    temp.onClick.AddListener(() =>
                    {
                        GameObject pop = PopUpItemPanel(temp.GetComponent<RectTransform>().anchoredPosition
                             + new Vector2(575, -175)
                             , Slot, IndexOf);
                        // pop.AddComponent<PopUpMouseControl>();
                    });
                }
            }
            
            return Main.gameObject;
        }
        Button ItemButton(Transform Parent, ItemSlot Slot) {
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

        GameObject PopUpItemPanel(Vector2 Pos, ItemSlot Slot, int IndexOf)
        { 
            GameObject PopUp= Manager.GetPanel(Manager.UICanvas().transform, new Vector2(300, 300), Pos);
            HorizontalLayoutGroup group = PopUp.AddComponent<HorizontalLayoutGroup>();
            PopUp.AddComponent<PopUpMouseControl>();

            group.childControlWidth = false;

            Text info = Manager.TextBox(PopUp.transform, new Vector2(150,300));
            info.text = Slot.Item.ItemName + "\n";
            info.text += Slot.Item.Description;
            
                VerticalLayoutGroup ButtonPanel= Manager.GetPanel(PopUp.transform, new Vector2(150, 300), Pos).AddComponent<VerticalLayoutGroup>();

            switch (Slot.Item.Type) {
                case ItemType.General:
                    Button use = Manager.UIButton(ButtonPanel.transform, "Use Item");
                    use.onClick.AddListener(() =>
                    {
                        RecoveryItemSO temp = (RecoveryItemSO)Slot.Item;
                        temp.Use(CharacterInventory, IndexOf, Character);
                       
                        itemsDisplayerPanel = ItemsDisplayPanel(itemPanel.transform, Inventory, DisplayItems);
                        Object.Destroy(PopUp);

                    });
                    info.text += "\nQuantity: " + Slot.Count;
                    break;
                case ItemType.Armor:
                case ItemType.Weapon:
                    Button Equip = Manager.UIButton(ButtonPanel.transform, "Equip");
                    Equip.onClick.AddListener(() => 
                    {
                        switch (Slot.Item.Type) {
                            case ItemType.Armor:
                                ArmorSO Armor = (ArmorSO)Slot.Item;
                                Armor.EquipItem(CharacterInventory, IndexOf, Character);
                                break;
                            case ItemType.Weapon:
                                WeaponSO weapon = (WeaponSO)Slot.Item;
                                weapon.EquipItem(CharacterInventory, IndexOf, Character);

                                break;
                        }
                        itemsDisplayerPanel = ItemsDisplayPanel(itemPanel.transform, Inventory, DisplayItems); 
                        playerStats = CreatePlayerPanel();
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
                    Slot.Item.RemoveFromInventory(CharacterInventory, IndexOf);
                    Object.Destroy(PopUp);
                });
            }
           // Button Cancel = Manager.UIButton(ButtonPanel.transform, "Cancel");

            return PopUp;
        }
    }

 
}