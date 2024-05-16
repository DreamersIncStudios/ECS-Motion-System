using DreamersInc.ComboSystem;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable Unity.BurstLoadingManagedType

namespace DreamersInc.InputSystems
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class ButtonInputSystem : SystemBase
    {
        private PlayerControls playerControls;
        
        private void CreateInput()
        {
            playerControls = new PlayerControls();
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var data = new InputSingleton()
            {
                ControllerInput= playerControls
            };

            Entity gm = manager.CreateSingleton(data);
#if UNITY_EDITOR
            manager.SetName(gm, "Input");
#endif
             
        }
        
        protected override void OnCreate()
        {
            RequireForUpdate<Player_Control>();
           CreateInput();
        }

        protected override void OnStartRunning()
        {
            block = false;
            playerControls.Enable();
            playerControls.PlayerController.LightAttack.performed += OnPlayerLightAttack;
            playerControls.PlayerController.HeavyAttack.performed += OnPlayerHeavyAttack;
            playerControls.PlayerController.Projectile.performed += OnPlayerShoot;
            playerControls.PlayerController.Dodge.performed += OnPlayerDodge;
            playerControls.PlayerController.Block.performed += OnPlayerBlock;
            playerControls.PlayerController.OpenCadMenu.performed += ToggleMagicInput;
            playerControls.MagicController.OpenCadMenu.performed += ToggleMagicInput;
            playerControls.MagicController.LightAttack.performed += OnXMagicAttack;
            playerControls.MagicController.HeavyAttack.performed += OnYMagicAttack;
            playerControls.MagicController.Jump.performed += OnAMagicAttack;
            playerControls.MagicController.Dodge.performed += OnBMagicAttack;
            playerControls.PlayerController.Enable();
            playerControls.MagicController.Disable();
        }

        protected override void OnStopRunning()
        {
            playerControls.Disable();
            playerControls.PlayerController.LightAttack.performed -= OnPlayerLightAttack;
            playerControls.PlayerController.HeavyAttack.performed -= OnPlayerHeavyAttack;
            playerControls.PlayerController.Projectile.performed -= OnPlayerShoot;
            playerControls.PlayerController.Dodge.performed -= OnPlayerDodge;
            playerControls.PlayerController.Block.performed -= OnPlayerBlock;
            playerControls.PlayerController.OpenCadMenu.performed -= ToggleMagicInput;
            playerControls.MagicController.OpenCadMenu.performed -= ToggleMagicInput;
            playerControls.MagicController.LightAttack.performed -= OnXMagicAttack;
            playerControls.MagicController.HeavyAttack.performed -= OnYMagicAttack;
            playerControls.MagicController.Jump.performed -= OnAMagicAttack;
            playerControls.MagicController.Dodge.performed -= OnBMagicAttack;


        }

        private void OnPlayerLightAttack(InputAction.CallbackContext obj)
        {

            Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                (Animator anim, PlayerComboComponent comboList, Command handler) =>
                {
                    if (anim.IsInTransition(0) || comboList.Combo.ShowMovesPanel) return;

                    foreach (ComboSingle comboTest in comboList.Combo.ComboLists(handler.StyleMod))
                    {
                        foreach (var comboOption in comboTest.ComboList)
                        {
                            if (handler.StateInfo.IsName(comboOption.CurrentStateName))
                            {
                                handler.currentStateExitTime = comboOption.AnimationEndTime;
                                if (!comboOption.InputAllowed(handler.StateInfo.normalizedTime)) continue;
                                var trigger = comboOption.Trigger;
                                if (!comboTest.Unlocked || !handler.QueueIsEmpty) continue;
                                if (handler.HeldButton)
                                {
                                    if (trigger.AttackType == AttackType.ChargedLightAttack)
                                        handler.InputQueue.Enqueue(trigger);
                                }
                                else
                                {
                                    if (trigger.AttackType == AttackType.LightAttack)
                                        handler.InputQueue.Enqueue(trigger);
                                }

                                handler.WeaponReset = 5.0f;
                            }
                        }
                    }
                    handler.HeldButton = false;
                }).Run();

        }

        private void OnPlayerHeavyAttack(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                (Animator anim, PlayerComboComponent comboList, Command handler) =>
                {
                    if (anim.IsInTransition(0) || comboList.Combo.ShowMovesPanel) return;
                    foreach (ComboSingle comboTest in comboList.Combo.ComboLists(handler.StyleMod))
                    {
                        foreach (var comboOption in comboTest.ComboList)
                        {
                            if (handler.StateInfo.IsName(comboOption.CurrentStateName))
                            {
                                handler.currentStateExitTime = comboOption.AnimationEndTime;
                                if (!comboOption.InputAllowed(handler.StateInfo.normalizedTime)) continue;
                                var trigger = comboOption.Trigger;
                                if (!comboTest.Unlocked || !handler.QueueIsEmpty) continue;
                                if (handler.HeldButton)
                                {
                                    if (trigger.AttackType == AttackType.ChargedHeavyAttack)
                                        handler.InputQueue.Enqueue(trigger);
                                }
                                else
                                {
                                    if (trigger.AttackType == AttackType.HeavyAttack)
                                        handler.InputQueue.Enqueue(trigger);
                                }
                                handler.WeaponReset = 5.0f;

                            }
                        }
                    }
                    handler.HeldButton = false;

                }).Run();
        }

        private void OnPlayerShoot(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                (Animator anim, PlayerComboComponent comboList, Command handler) =>
                {
                    if (anim.IsInTransition(0) || comboList.Combo.ShowMovesPanel) return;
                    foreach (ComboSingle comboTest in comboList.Combo.ComboLists(handler.StyleMod))
                    {
                        foreach (var comboOption in comboTest.ComboList)
                        {
                            if (handler.StateInfo.IsName(comboOption.CurrentStateName))
                            {
                                handler.currentStateExitTime = comboOption.AnimationEndTime;
                                if (!comboOption.InputAllowed(handler.StateInfo.normalizedTime)) continue;
                                var trigger = comboOption.Trigger;
                                if (!comboTest.Unlocked || !handler.QueueIsEmpty) continue;
                                if (handler.HeldButton)
                                {
                                    if (trigger.AttackType == AttackType.ChargedProjectile)
                                        handler.InputQueue.Enqueue(trigger);
                                }
                                else
                                {
                                    if (trigger.AttackType == AttackType.Projectile)
                                        handler.InputQueue.Enqueue(trigger);
                                }
                                handler.WeaponReset = 5.0f;

                            }
                        }
                    }
                    handler.HeldButton = false;

                }).Run();
        }

        private void OnPlayerDodge(InputAction.CallbackContext obj)
        {
            var dir = playerControls.PlayerController.Locomotion.ReadValue<Vector2>();

            Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                (Animator anim, PlayerComboComponent comboList, Command handler) =>
                {
                    if (anim.IsInTransition(0) || comboList.Combo.ShowMovesPanel) return;
                    if (dir.y > .1)
                    {
                        handler.InputQueue.Enqueue(new AnimationTrigger()
                        {
                            triggerAnimIndex = 0,
                            AttackType = AttackType.Dodge,
                            TransitionDuration = .25f,
                        });
                    }
                    else
                    {
                        handler.InputQueue.Enqueue(new AnimationTrigger()
                        {
                            triggerAnimIndex = 1,
                            AttackType = AttackType.Dodge,
                            TransitionDuration = 0.25f,

                        });
                    }
                    handler.WeaponReset = 5.0f;

                    handler.HeldButton = false;

                }).Run();
        }

        private bool block;

        private void OnPlayerBlock(InputAction.CallbackContext obj)
        {
            block = !block;
            switchBlock = true;

        }

        private bool casting;
        private static readonly int Block = Animator.StringToHash("Block");

        void ToggleMagicInput(InputAction.CallbackContext obj)
        {
            Debug.Log("I casted light magic");
            if (casting)
            {
                casting = false;
                playerControls.PlayerController.Enable();
                Entities.WithoutBurst().WithAll<Player_Control>()
                    .ForEach(( Command handler) =>
                    {
                        handler.InputTimer = 0.0f;
                    }).Run();
                DisableSlowMoMode();
            }
            else
            {
                casting = true;
                playerControls.PlayerController.Disable();
                playerControls.MagicController.Enable();
                EnableSlowMoMode();
                Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                    (Command handler) =>
                    {
                        handler.InputTimer = 5.5f; //Todo make this value based on Stats
                    }).Run();
            }
        }

        void OnXMagicAttack(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Command handler) =>
            {
                handler.MagicInputQueue.Enqueue("X");
            }).Run();
        }

        void OnYMagicAttack(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Command handler) =>
            {
                handler.MagicInputQueue.Enqueue("Y");
            }).Run();
        }

        void OnBMagicAttack(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Command handler) =>
            {
                handler.MagicInputQueue.Enqueue("B");
            }).Run();
        }

        void OnAMagicAttack(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Command handler) =>
            {
                handler.MagicInputQueue.Enqueue("A");
            }).Run();
        }

        private bool switchBlock;

        protected override void OnUpdate()
        {
            if (block)
            {
                Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                    (Animator anim, Command handler) =>
                    {
                        anim.SetBool(Block, true);

                        handler.InputQueue.Enqueue(new AnimationTrigger()
                        {
                            AttackType = AttackType.Defend,

                        });
                    }).Run();
            }
            else if (switchBlock)
            {
                Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                    (Animator anim, Command handler) =>
                    {
                        anim.SetBool(Block, false);
                        handler.InputQueue.Clear();
                    }).Run();
                switchBlock = false;

            }
        }


        private void EnableSlowMoMode()
        {
            Entities.WithoutBurst().WithAll<Animator>().WithStructuralChanges().WithNone<AnimationSpeedMod>().ForEach(
                (Entity entity) =>
                {
                    //Todo add range limit;
                    EntityManager.AddComponentData(entity, new AnimationSpeedMod()
                    {
                        SpeedValue = .15f
                    });


                }).Run();

        }

        private void DisableSlowMoMode()
        {
            Entities.WithoutBurst().WithStructuralChanges().WithAll<AnimationSpeedMod, Animator>().ForEach(
                (Entity entity) => { EntityManager.RemoveComponent<AnimationSpeedMod>(entity); }).Run();

        }
    }
}