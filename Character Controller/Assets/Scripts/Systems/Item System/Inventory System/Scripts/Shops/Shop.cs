using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.Global;
using UnityEngine.UI;
using Dreamers.InventorySystem.Interfaces;
namespace Dreamers.InventorySystem.Generic
{
    [System.Serializable]
    public class Shop 
    {
        private string shopName;
        private List<ItemBaseSO> itemsToSell;
        private List<ItemBaseSO> itemsToBuyback;
        [Range(.5f, 1.0f)]
        public float Sell;
        [Range(.5f, 1.0f)]
        public float Buy;
        readonly UIManager manager;
        public bool Displayed { get { return (bool)MenuPanelParent; } }
        bool Buying = true;
        public Shop(string name = "", List<ItemBaseSO> itemToSell= default, uint SeedCapital = 1500)
        {
            this.shopName = name;
            //  this.storeWallet = SeedCapital;
            itemsToSell = new List<ItemBaseSO>();
            itemsToBuyback = new List<ItemBaseSO>();
            AddItemsToInventory(itemToSell);
            manager = UIManager.instance;
            Sell = 1;
            Buy = 1;
        }
        #region manage Inventory
        public void AddItemsToInventory(ItemBaseSO item)
        {
            itemsToSell.Add(item);
        }
        public void AddItemsToInventory(List<ItemBaseSO> items)
        {
            itemsToSell.AddRange(items);
        }

        public void SellItemToShop(ItemBaseSO item, out uint goldMod)
        {
            itemsToBuyback.Add(item);
            goldMod = item.Value;
        }
        public void SellItemToShop(List<ItemBaseSO> items, out uint goldMod)
        {
            itemsToSell.AddRange(items);
            goldMod = new uint();
            foreach (var item in items)
            {
                goldMod += item.Value;
            }
        }
        public void RemoveItemFromInventory(ItemBaseSO item)
        {
            itemsToSell.Remove(item);
        }

        public int GetItemIndex(ItemBaseSO item)
        {
            return itemsToSell.IndexOf(item);
        }
        public ItemBaseSO GetItem(int index)
        {
            return itemsToSell[index];
        }
        public Vector2 GetStoreMod(int luck, int Charsima) {

            // TODO Implement Logic for modifying buy and sell amount. Player with higher luck / charisma get larger discounts
            return new Vector2(Sell, Buy);
        }
        bool PurchaseItem(ItemBaseSO item, uint PlayerCashOnHand)
        {
            if (item.Value <= PlayerCashOnHand)
            {
             //   storeWallet += item.Value;
                return true;
            }
            else { return false; }
        }
        bool PurchaseItem(int itemIndex, uint PlayerCashOnHand)
        {
            return PurchaseItem(GetItem(itemIndex), PlayerCashOnHand);
        }
        #endregion

        #region Manage UI

        GameObject MenuPanelParent;
        GameObject ItemPanel;
        GameObject playerGold; // possible just work with player inventory

        public void OpenStore(CharacterInventory characterInventory)
        {
            MenuPanelParent = CreateStoreUI(new Vector2(0, 0),
         new Vector2(0, 0) , characterInventory);
        }

        public void CloseStore()
        {
            UnityEngine.Object.Destroy(MenuPanelParent);
        }
        GameObject CreateStoreUI(Vector2 Size, Vector2 Position, CharacterInventory characterInventory)
        {
            if (MenuPanelParent)
                UnityEngine.Object.Destroy(MenuPanelParent);

            GameObject Parent = manager.UICanvas();
            GameObject MainPanel = manager.GetPanel(Parent.transform, Size, Position);
            MainPanel.name = shopName;
            RectTransform PanelRect = MainPanel.GetComponent<RectTransform>();
            PanelRect.pivot = new Vector2(0.5f, .5f);
            PanelRect.anchorMax = new Vector2(1, 1);
            PanelRect.anchorMin = new Vector2(.0f, .0f);

            VerticalLayoutGroup VLG = MainPanel.AddComponent<VerticalLayoutGroup>();
            VLG.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            VLG.childAlignment = TextAnchor.UpperCenter;
            VLG.childControlHeight = true; VLG.childControlWidth = true;
            VLG.childForceExpandHeight = false; VLG.childForceExpandWidth = true;

            Text titleGO = manager.TextBox(MainPanel.transform, new Vector2(400, 50)).GetComponent<Text>();
            titleGO.alignment = TextAnchor.MiddleCenter;
            titleGO.text = shopName;
            titleGO.fontSize = 24;
            HorizontalLayoutGroup BuySell = manager.GetPanel(MainPanel.transform, new Vector2(1920, 60), Position).AddComponent<HorizontalLayoutGroup>();
            BuySell.name = "BuySell Header";
            BuySell.childControlHeight = false;
            BuySell.childForceExpandHeight = false;

            manager.UIButton(BuySell.transform, "Buy Items")
                .onClick.AddListener(() =>
                {
                    Buying = true;
                    ItemPanel = DisplayItems(ItemType.None, MainPanel.transform, characterInventory);
                });

            manager.UIButton(BuySell.transform, "Sell Items")
                .onClick.AddListener(() =>
                {
                    Buying = false;
                    ItemPanel = DisplayItems(ItemType.None, MainPanel.transform, characterInventory);
                });
            #region header
            HorizontalLayoutGroup ButtonHeader = manager.GetPanel(MainPanel.transform, new Vector2(1920, 60), Position).AddComponent<HorizontalLayoutGroup>();
            ButtonHeader.name = "Button Header";
            ButtonHeader.childControlHeight = false;
            ButtonHeader.childForceExpandHeight = false;
            manager.UIButton(ButtonHeader.transform, "All")
                .onClick.AddListener(() =>
                {
                    ItemPanel = DisplayItems(ItemType.None, MainPanel.transform, characterInventory);
                });


            for (int i = 1; i < 6; i++)
            {
                int index = i;
                Button Temp = manager.UIButton(ButtonHeader.transform, ((ItemType)i).ToString());
                Temp.onClick.AddListener(() =>
                {
                    ItemPanel = DisplayItems((ItemType)index, MainPanel.transform, characterInventory);
                });
                ;
                Temp.name = ((ItemType)i).ToString();

            }
            #endregion

            playerGold = DisplayPlayerGold(MainPanel.transform, characterInventory);
            ItemPanel = DisplayItems(ItemType.None, MainPanel.transform,characterInventory);
            return MainPanel;
        }
        Button ItemButton(Transform Parent, ItemBaseSO item)
        {
            Button temp = manager.UIButton(Parent, item.ItemName);
            temp.name = item.ItemName;
            Text texttemp = temp.GetComponentInChildren<Text>();
            texttemp.alignment = TextAnchor.LowerCenter;
            if (item.Stackable)
                texttemp.text += item.MaxStackCount;
            temp.GetComponentInChildren<Text>().alignment = TextAnchor.LowerCenter;
            temp.GetComponent<Image>().sprite = item.Icon;
            return temp;
        }
        List<GameObject> itemButton;
        GameObject DisplayItems(ItemType Filter, Transform Parent , CharacterInventory playerInventory)
        {
            if (ItemPanel)
                UnityEngine.Object.Destroy(ItemPanel);
            ItemPanel = manager.GetPanel(Parent.transform, new Vector2(1920, 0), new Vector2(0, 0));
                 GridLayoutGroup basePanel = ItemPanel.AddComponent<GridLayoutGroup>();
            basePanel.name = "Items Display";
            basePanel.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            basePanel.spacing = new Vector2(20, 20);
            List<ItemBaseSO> itemsToDisplay = itemsToSell;
            if (!Buying) {
                itemsToDisplay = new List<ItemBaseSO>();
                foreach (var slot in playerInventory.Inventory.ItemsInInventory)
                {
                    itemsToDisplay.Add(slot.Item);
                }
            }
            itemButton = new List<GameObject>();
            for (int i = 0; i < itemsToDisplay.Count; i++)
            {
                int index = i;
                if (itemsToDisplay[i].Type == Filter || Filter == ItemType.None)
                {
                    Button temp = ItemButton(basePanel.transform, itemsToDisplay[index]);
                    itemButton.Add(temp.gameObject);

                    temp.onClick.AddListener(() =>
                    {

                        GameObject pop = PopUpItemPanel(temp.GetComponent<RectTransform>().anchoredPosition
                             + new Vector2(575, -175)
                             , itemsToDisplay[index], playerInventory);
                        pop.AddComponent<PopUpMouseControl>();

                    });
                }
            }

                return ItemPanel;
            }
        public class OnWalletChangedEventArgs : EventArgs {
            public CharacterInventory inventory;
            public GameObject deleteThis;
            public bool soldItem;
        }
        EventHandler<OnWalletChangedEventArgs> OnWalletChanged;

        GameObject DisplayPlayerGold(Transform MainPanel, CharacterInventory playerInventory)
        {
            if (playerGold)
                UnityEngine.Object.Destroy(playerGold);

            Text goldInWallet = manager.TextBox(MainPanel, new Vector2(400, 50));
            playerGold = goldInWallet.gameObject;
            goldInWallet.alignment = TextAnchor.LowerRight;
            goldInWallet.text = playerInventory.Gold.ToString() + "G";
            goldInWallet.fontSize = 16;
            OnWalletChanged += (object sender, OnWalletChangedEventArgs eventArgs) =>
            {
                goldInWallet.text = eventArgs.inventory.Gold.ToString() + "G";
                if (eventArgs.soldItem)
                    UnityEngine.Object.Destroy(eventArgs.deleteThis);
            };

            return playerGold;
        }
        public EventHandler<OnPriceChangedEventArg> OnPriceChanged;
            public class OnPriceChangedEventArg : EventArgs {
            public int value;
        }
        GameObject PopUpItemPanel(Vector2 Pos, ItemBaseSO item, CharacterInventory playerInventory)
        {
            GameObject PopUp = manager.GetPanel(manager.UICanvas().transform, new Vector2(600, 400), Pos);
            Image temp = PopUp.GetComponent<Image>();
            Color color = temp.color; color.a = 1.0f;
            temp.color = color;

            HorizontalLayoutGroup group = PopUp.AddComponent<HorizontalLayoutGroup>();
            PopUp.AddComponent<PopUpMouseControl>();

            group.childControlWidth = false;
            GameObject descriptionPanel = manager.GetPanel(PopUp.transform, new Vector2(200, 400), Pos);
            descriptionPanel.AddComponent<VerticalLayoutGroup>();
            Text info = manager.TextBox(descriptionPanel.transform, new Vector2(250, 300));
            info.text = item.ItemName + "\n";
            info.text += item.Description + "\n";
            info.fontSize = 24;
            Text price = manager.TextBox(descriptionPanel.transform, new Vector2(250, 300));
            price.fontSize = 26;

            VerticalLayoutGroup buttonPanel = manager.GetPanel(PopUp.transform, new Vector2(150, 300), Pos).AddComponent<VerticalLayoutGroup>();
            buttonPanel.childControlHeight = buttonPanel.childForceExpandHeight = false;
            if (Buying)
            {
                Button buy = manager.UIButton(buttonPanel.transform, "Buy");
                Slider quantitySlider = manager.UISlider(buttonPanel.transform);
                quantitySlider.onValueChanged.AddListener(delegate {
                    if (OnPriceChanged != null)
                        OnPriceChanged(this, new OnPriceChangedEventArg {  value = (int)quantitySlider.value });

                });

                int sellQuantity = (int)quantitySlider.value;
            price.text = "Cost: " + Mathf.RoundToInt(item.Value * Sell)*quantitySlider.value + " gil";

                OnPriceChanged += (object sender, OnPriceChangedEventArg eventArg) => {
                    price.text = "Cost: " + Mathf.RoundToInt(item.Value * Sell) * eventArg.value + " gil";
                    sellQuantity = eventArg.value;
                };
                buy.onClick.AddListener(() =>
                {
                    for (int i = 0; i < sellQuantity; i++)
                    {
                        if (PurchaseItem(item, (uint)playerInventory.Gold))
                        {
                            Debug.Log(sellQuantity);
                            playerInventory.AdjustGold(-(int)item.Value);
                            playerInventory.Inventory.AddToInventory(item); 
                        }
                        else
                        {
                            //TODO Implement Can't Afford message
                            Debug.Log("Player has NSF");
                        }

                        if (OnWalletChanged != null) OnWalletChanged(this, new OnWalletChangedEventArgs { inventory = playerInventory,soldItem=false });
                    }
                    UnityEngine.Object.Destroy(PopUp);

                });
                switch (item.Type)
                {
                    case ItemType.Armor:
                    case ItemType.Weapon:
                    case ItemType.Blueprint_Recipes:
                       quantitySlider.gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                Button sell = manager.UIButton(buttonPanel.transform, "Sell");
                Slider quantitySlider = manager.UISlider(buttonPanel.transform);
                quantitySlider.onValueChanged.AddListener(delegate {
                    if (OnPriceChanged != null)
                        OnPriceChanged(this, new OnPriceChangedEventArg { value = (int)quantitySlider.value });

                });

                int buyQuantity = (int)quantitySlider.value;
                price.text = "Cost: " + Mathf.RoundToInt(item.Value * Sell) * quantitySlider.value + " gil";

                OnPriceChanged += (object sender, OnPriceChangedEventArg eventArg) => {
                    price.text = "Cost: " + Mathf.RoundToInt(item.Value * Buy) * eventArg.value + " gil";
                    buyQuantity = eventArg.value;
                };
                sell.onClick.AddListener(() =>
                {
                    for (int i = 0; i < buyQuantity; i++)
                    {
                        SellItemToShop(item, out uint goldMod);
                        playerInventory.AdjustGold((int)goldMod);
                        playerInventory.Inventory.RemoveFromInventory(item);
                    }
                    ItemPanel = DisplayItems(ItemType.None, MenuPanelParent.transform, playerInventory);
                    UnityEngine.Object.Destroy(PopUp);
                        if (OnWalletChanged != null) OnWalletChanged(this, new OnWalletChangedEventArgs { inventory = playerInventory, deleteThis=sell.gameObject, soldItem=true });
                });

                switch (item.Type)
                {
                    case ItemType.Armor:
                    case ItemType.Weapon:
                    case ItemType.Blueprint_Recipes:
                        quantitySlider.gameObject.SetActive(false);
                        break;
                }
            }
            return PopUp;
        }


        #endregion
    }

}