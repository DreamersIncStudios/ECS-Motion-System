using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Dreamers.InventorySystem;
using SkillMagicSystem;
using TMPro;
using Stats;
using System;
using DreamersInc.ComboSystem;
using Unity.Entities;
using AttackType = DreamersInc.ComboSystem.AttackType;
using MotionSystem.CAD;

//using UnityStandardAssets.CrossPlatformInput;

namespace Dreamers.ModalWindows
{
    public class QuickAccessMenu : MonoBehaviour
    {
        public RectTransform Base;
        public GameObject ContentArea;
        public GameObject ButtonPrefab;
        CharacterInventory inventory;
        PlayerCharacter character;
        bool casted;

        bool casting => Input.GetAxis("Target Trigger") > .3f && !casted; //TODO rename Target Trigger
        bool shown = false;
        float resetTimer;
        bool reset => resetTimer > 0.0f;
        CastingTimeSystem timeSystem;
        private void Start()
        {
            timeSystem = CastingTimeSystem.instance;
        }

        private void Update()
        {
            if (!shown && casting && !reset && !timeSystem.Release)
                DisplayQuickAccessMenu();

            if (shown && !casting)
            {
                HideQuickAccesMenu();
            }
            
            if(shown && reset)
                HideQuickAccesMenu();

            if(reset)
                resetTimer -= Time.deltaTime;
        }

        public void DisplayQuickAccessMenu()
        {
            if (!inventory)
            {
                var Player = GameObject.FindGameObjectWithTag("Player");
                inventory = Player?.GetComponent<CharacterInventory>();
                character = Player?.GetComponent<PlayerCharacter>();
            }
            Base.DOAnchorPosY(-250, .75f);
            shown = true;
        }
        public void HideQuickAccesMenu()
        {
            Base.DOAnchorPosY(-800, .75f);
            shown = false;
            DisplayBase();

        }

        public void DisplaySpells()
        {
            ClearContentArea();
            foreach (Magic spell in inventory.magicSkillSystem.EquippedMagic)
            {
                Button buttonSpell = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
                TextMeshProUGUI spellText = buttonSpell.GetComponentInChildren<TextMeshProUGUI>();
                spellText.text = spell.Name;
                buttonSpell.onClick.AddListener(() =>
                {
                    if (spell.CanCast(character))
                    {
                        switch (spell.AbilityTarget)
                        {
                            case Targets.Self:
                                Command testing = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Command>(character.SelfEntityRef);
                                testing.InputQueue.Enqueue(new AnimationTrigger()
                                {
                                    attackType = (AttackType)spell.test.attackType,
                                    triggerAnimIndex = spell.test.TriggerAnimIndex,
                                    TransitionDuration = spell.test.TransitionDuration,
                                    TransitionOffset = spell.test.TransitionOffset,
                                    EndofCurrentAnim = spell.test.EndofCurrentAnim,
                                });

                                spell.Activate(character);
                                timeSystem.resetTimer = resetTimer = 3.0f;
                                timeSystem.Release = true;
                                break;
                            case Targets.Enemy:
                                DisplayTargerts(spell.AbilityTarget, spell);
                                break;
                            case Targets.Anyone:
                            case Targets.Projectile:
                                DisplayTargerts(spell.AbilityTarget, spell);
                                break;
                            case Targets.TeamMember:
                                DisplayTargerts(spell.AbilityTarget, spell);
                                break;
                            case Targets.AOE:
                                break;

                        }

                        Debug.Log($"Casting spell {spell.Name}");
                    }
                    else
                        Debug.Log($"Can not cast spell {spell.Name}");

                }); 
            }
            CreateBackButton();
            Debug.Log(inventory.magicSkillSystem.EquippedMagic.Count);
        }
        public void DisplayItems()
        {
            ClearContentArea();
            CreateBackButton();

        }
        public void DisplayAbilities()
        {
            ClearContentArea();
            foreach (Skill skill in inventory.magicSkillSystem.EquippedSkill)
            {
                Button buttonSpell = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
                TextMeshProUGUI spellText = buttonSpell.GetComponentInChildren<TextMeshProUGUI>();
                spellText.text = skill.Name;
                buttonSpell.onClick.AddListener(() =>
                {
                    if (skill.CanCast(character))
                    {
                        switch (skill.AbilityTarget)
                        {
                            case Targets.Self:
                                skill.Activate(character);
                                break;
                            case Targets.Enemy:
                                DisplayTargerts(skill.AbilityTarget, skill);
                                break;
                            case Targets.Anyone:
                            case Targets.Projectile:
                                DisplayTargerts(skill.AbilityTarget, skill);
                                break;
                            case Targets.TeamMember:
                                DisplayTargerts(skill.AbilityTarget, skill);
                                break;
                            case Targets.AOE:
                                break;
                        }

                        Debug.Log($"Casting spell {skill.Name}");
                    }
                    else
                        Debug.Log($"Can not cast spell {skill.Name}");

                });

            }
            Debug.Log(inventory.magicSkillSystem.EquippedSkill.Count);
            CreateBackButton();
        }
        public void DisplaySummons()
        {
            ClearContentArea();
            CreateBackButton();
        }
        public void DisplayBase()
        {
            ClearContentArea();
            Button spellsButton = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
            spellsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Spells";
            spellsButton.onClick.AddListener(DisplaySpells);

            Button itemsButton = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
            itemsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Items";
            itemsButton.onClick.AddListener(DisplayItems);

            Button skillsButton = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
            skillsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Abilities";
            skillsButton.onClick.AddListener(DisplayAbilities);

            Button summonsButton = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
            summonsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Summons";
            summonsButton.onClick.AddListener(DisplaySummons);

        }
        void ClearContentArea()
        {

            foreach (Transform child in ContentArea.transform)
            {
                Destroy(child.gameObject);
            }

        }
        void CreateBackButton()
        {
            Button backButton = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
            TextMeshProUGUI backText = backButton.GetComponentInChildren<TextMeshProUGUI>();
            backText.text = "Back";
            backButton.onClick.AddListener(DisplayBase);
        }
        List<GameObject> TargetInRange(bool IncludedFriends)
        {
            List<GameObject> temp = new List<GameObject>();



            return temp;
        }
        void DisplayTargerts(Targets target, BaseAbility ability)
        {
            ClearContentArea();
            //TODO replace with reference to Scanbuffer and Entites 
            BaseCharacter[] targets = target switch
            {
                Targets.Anyone => GameObject.FindObjectsOfType<BaseCharacter>(),
                Targets.Enemy => GameObject.FindObjectsOfType<EnemyCharacter>(),
                Targets.TeamMember => GameObject.FindObjectsOfType<PlayerCharacter>(),
                _ => throw new ArgumentOutOfRangeException(nameof(target), $"Not expected: ")
            };

            foreach (var item in targets)
            {
                Button characterButton = Instantiate(ButtonPrefab, ContentArea.transform).GetComponent<Button>();
                TextMeshProUGUI charText = characterButton.GetComponentInChildren<TextMeshProUGUI>();
                charText.text = item.Name;
                characterButton.onClick.AddListener(() => {
                    ability.Activate(character, item);
                });
            }
            CreateBackButton();
        }
    }
}
 