using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Entities;
using Dreamers.Global;
namespace Stats.UI
{


    public class PlayerStatUI : MonoBehaviour
    {
        public static PlayerStatUI instance;
        public PlayerCharacter Player;
        public Bar HealthBar { get; private set; }
        public Bar ManaBar { get; private set; }
        public Sprite BarSprite;

        public void Start()
        {
            if (!instance)
                instance = this;
            else
                Destroy(this);
            Player = FindObjectOfType<PlayerCharacter>();
            Setup();

        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Player.TakeDamage(110, DreamersInc.DamageSystem.Interfaces.TypeOfDamage.Melee, DreamersInc.DamageSystem.Interfaces.Element.None);
                Debug.Log("Test of UI");
            }
        }

        void Setup()
        {
            GameObject healthPanel = UIManager.instance.GetPanel(UIManager.instance.UICanvas().transform, new Vector2(250, 85), new Vector2(200, -75));
            VerticalLayoutGroup vertical = healthPanel.AddComponent<VerticalLayoutGroup>();
            vertical.childControlWidth = vertical.childForceExpandWidth = false;
            vertical.padding.top = vertical.padding.left = vertical.padding.bottom = 5;
            vertical.spacing = 5;

            HealthBar = new Bar(healthPanel.transform, BarSprite, 1.0f, 200, Color.red);
            ManaBar = new Bar(healthPanel.transform, BarSprite, 1.0f, 150, Color.blue);


        }

        public class Bar
        {
            private Color barColor;
            float ratio;
            private Sprite image;
            private Image BarImage;
            private Transform parent;
            int lengthOfBar;
            public Bar(Transform parent, Sprite image, float ratioAtStart, int length, Color barColor)
            {
                this.parent = parent;
                this.image = image;
                this.barColor = barColor;
                lengthOfBar = length;
                ratio = ratioAtStart;
                BarImage = UIManager.instance.GetImage(parent, image);
                BarImage.type = Image.Type.Filled;

                BarImage.fillMethod = Image.FillMethod.Horizontal;

                BarImage.fillAmount = ratio;
                BarImage.color = barColor;
                RectTransform PanelRect = BarImage.GetComponent<RectTransform>();
                PanelRect.sizeDelta = new Vector2(lengthOfBar, 30);
            }
            public void UpdateBarFillAmount(float cur)
            {
                ratio = cur;
                //Todo Lerp coroutine 
                BarImage.fillAmount = ratio;
            }
            public void UpdateBarLength(int lengthAdder)
            {
                lengthOfBar += lengthAdder;
                RectTransform PanelRect = BarImage.GetComponent<RectTransform>();
                PanelRect.sizeDelta = new Vector2(lengthOfBar, 30);
            }
            public int GetBarLength { get { return lengthOfBar; } }
        }


        public class PlayerStatUpdate : SystemBase
        //TODO find a way to only get the local entity;
        {
            protected override void OnUpdate()
            {
                Entities.WithoutBurst().WithChangeFilter<PlayerStatComponent>().ForEach((in PlayerStatComponent PC) =>
                {
                    if (PlayerStatUI.instance != null)
                    {

                        PlayerStatUI.instance.HealthBar.UpdateBarFillAmount(PC.HealthRatio);
                        PlayerStatUI.instance.ManaBar.UpdateBarFillAmount(PC.ManaRatio);
                    }
                }).Run();
            }
        }
    }
}