
using Dreamers.Global;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Stats;
using UnityEngine;
using UnityEngine.UI;
using DreamersInc.UI;
using DreamersInc.MagicSkill;
namespace Dreamers.InventorySystem.UISystem
{
    public  partial class DisplayMenu 
    {
        readonly UIManager Manager;
     
        public enum PanelToRefresh { Inventory, CAD, PlayerStat, Equipment}
     
        public bool Displayed { get { return (bool)MenuPanelParent; } }

        public DisplayMenu(BaseCharacter player ) {
            Manager = UIManager.instance;
            this.Character = player;
        }

        public void OpenCharacterMenu(InventoryBase inventory) {
             GetInventoryPanel = new InventoryPanel(new Vector2(1400, 300), new Vector2(0, 150), Character);
            GetCADPanel = new CADPanel(new Vector2(1400, 1000), new Vector2(0, 150), Character.GetComponent<CastingDevice>());
            GetEquiqmentPanel = new EquiqmentPanel(new Vector2(400, 400), new Vector2(0, 150), Character, GetInventoryPanel);

            GetInventoryPanel.equiqmentPanel = GetEquiqmentPanel;
            MenuPanelParent = CreateMenu(
        new Vector2(0, 0),
        new Vector2(0, 0));

        }

        public void CloseCharacterMenu() {
             Object.Destroy(MenuPanelParent);
        }

        private BaseCharacter Character;

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
         //   DisplayItems = (ItemType)0; // change to zero when all tab is added


            HLG.padding = new RectOffset() { bottom = 20, top = 20, left = 20, right = 20 };
            HLG.spacing = 10;
            HLG.childAlignment = TextAnchor.UpperLeft;
            HLG.childControlHeight = true; HLG.childControlWidth = false;
            HLG.childForceExpandHeight = true; HLG.childForceExpandWidth = true;
            playerStats = CreatePlayerPanel(MainPanel.transform);
            TabView = CreateTabView(MainPanel.transform);
            return MainPanel;
        }

        GameObject TabView;
        public Sprite[] TabIcons;
        GameObject CreateTabView( Transform parent) {
            if (TabView)
                Object.Destroy(TabView);

            GameObject contextPanel = Manager.GetPanel(parent, new Vector2(1400, 0), Vector2.zero, Global.LayoutGroup.Vertical, "Context Panel");
            VerticalLayoutGroup verticalLayoutGroup = contextPanel.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlHeight = verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;
            GameObject tabGroup = Manager.GetPanel(contextPanel.transform, new Vector2(1400, 100), Vector2.zero, Global.LayoutGroup.Horizontal, "Tab Panel");
            TabGroup groupMaster = tabGroup.AddComponent<TabGroup>();
           TabButton inventory = Manager.TabButton (tabGroup.transform,"Inventory", "Inventory");
           
            inventory.OnTabSelected.
                AddListener(() => {
                 GetInventoryPanel.CreatePanel(contextPanel.transform);
            });
            inventory.OnTabDeslected.AddListener(() =>
               GetInventoryPanel.DestoryPanel());
            
            TabButton CAD = Manager.TabButton(tabGroup.transform,  "CAD", "CAD");
            CAD.OnTabSelected.AddListener(() => 
                GetCADPanel.CreatePanel(TabView.transform));
           
            CAD.OnTabDeslected.AddListener(() => GetCADPanel.DestoryPanel());
            TabButton save = Manager.TabButton(tabGroup.transform, "Save/Load", "Save/Load");

            groupMaster.TabIdle = Color.white;
            groupMaster.TabHovered = Color.red;
            groupMaster.TabSelected = Color.green;
            return contextPanel;
        }

    }

 
}