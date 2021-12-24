
using Dreamers.Global;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamers.InventorySystem.UISystem
{
    public partial class DisplayMenu
    {
        GameObject playerStats { get; set; }
        GameObject CreatePlayerPanel(Transform parent)
        {
            if (playerStats)
                Object.Destroy(playerStats);

            GameObject MainPanel = Manager.GetPanel(parent, new Vector2(400, 300), new Vector2(0, 150));
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

            Text statsText = Manager.TextBox(PlayerStatsWindow.transform, new Vector2(400, 50)).GetComponent<Text>();
            statsText.alignment = TextAnchor.UpperLeft;
            statsText.text = " Player";
            statsText.fontSize = 24;

            statsText.text = Character.Name + " Lvl: " + Character.Level;
            statsText.text += "\nHealth:\t\t" + Character.CurHealth + "/" + Character.MaxHealth;
            statsText.text += "\nMana:\t\t\t" + Character.CurMana + "/" + Character.MaxMana + "\n";

            for (int i = 0; i < System.Enum.GetValues(typeof(AttributeName)).Length; i++)
            {
                statsText.text += "\n" + ((AttributeName)i).ToString() + ":\t\t\t" + Character.GetPrimaryAttribute(i).BaseValue;
                statsText.text += " + " + Character.GetPrimaryAttribute(i).BuffValue;
                statsText.text += " + " + Character.GetPrimaryAttribute(i).AdjustBaseValue;


            }
            GetEquiqmentPanel.CreatePanel(MainPanel.transform);

            return MainPanel;


        }


    }
}