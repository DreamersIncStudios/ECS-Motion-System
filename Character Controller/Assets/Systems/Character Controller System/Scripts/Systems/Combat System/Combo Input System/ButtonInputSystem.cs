using System.Linq;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.MotionSystem;
using DreamersInc.ComboSystem;
using DreamersInc.InputSystems;
using MotionSystem.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable Unity.BurstLoadingManagedType

namespace DreamersInc.InputSystems
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class ButtonInputSystem : SystemBase
    {
        #region Variables
        private PlayerControls playerControls;
        private bool switchBlock;
        private bool block;

        private bool casting;
        private static readonly int Block = Animator.StringToHash("Block");
        private bool paused = false;
        Transform m_mainCam;
        private Entity controlManager;
        #endregion

        private void CreateInput()
        {
            playerControls = new PlayerControls();
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var data = new InputSingleton()
            {
                ControllerInput= playerControls
            };

          controlManager = manager.CreateSingleton(data);
            manager.AddComponent<RunningTag>(controlManager);
            
#if UNITY_EDITOR
            manager.SetName(controlManager, "Input");
#endif

        }
        
        protected override void OnCreate()
        {
            RequireForUpdate<Player_Control>();
            RequireForUpdate<RunningTag>();
           CreateInput();
           playerControls.PauseMenu.PauseGame.performed += OnTogglePause;

        }

        protected override void OnStartRunning()
        {
            block = false;
            playerControls.Enable();
            playerControls.PauseMenu.Disable();
            playerControls.PlayerController.Enable();
            playerControls.MagicController.Disable();
            
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

            
            playerControls.PlayerController.PauseGame.performed += OnTogglePause;
            playerControls.PlayerController.Jump.performed += OnPlayerJump;
            playerControls.PlayerController.StyleModPress.performed += OnStyleModPress;
            playerControls.PlayerController.StyleModRelease.performed += OnStyleModRelease;
            playerControls.PlayerController.AttackButtonHeld.performed += ButtonHelded;
            playerControls.PlayerController.SpellChange.performed += OpenSpellChangeMenu;
        }

        protected override void OnStopRunning()
        {
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
            playerControls.PlayerController.PauseGame.performed -= OnTogglePause;
            playerControls.PlayerController.Jump.performed -= OnPlayerJump;
            playerControls.PlayerController.StyleModPress.performed -= OnStyleModPress;
            playerControls.PlayerController.StyleModRelease.performed -= OnStyleModRelease;
            playerControls.PlayerController.AttackButtonHeld.performed -= ButtonHelded;
            playerControls.PlayerController.SpellChange.performed -= OpenSpellChangeMenu;

        }

        private void OnPlayerLightAttack(InputAction.CallbackContext obj)
        {

            Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                (Animator anim, PlayerComboComponent comboList, Command handler) =>
                {
                    if (anim.IsInTransition(0) || comboList.Combo.ShowMovesPanel) return;

                    foreach (ComboSingle comboTest in comboList.Combo.ComboLists(handler.StyleMod))
                    {
                        foreach (var comboOption in comboTest.comboSingleSO.ComboList)
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
                        foreach (var comboOption in comboTest.comboSingleSO.ComboList)
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
                        foreach (var comboOption in comboTest.comboSingleSO.ComboList)
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
                            TransitionDuration = .15f,
                        });
                    }
                    else
                    {
                        handler.InputQueue.Enqueue(new AnimationTrigger()
                        {
                            triggerAnimIndex = 1,
                            AttackType = AttackType.Dodge,
                            TransitionDuration = 0.05f,

                        });
                    }
                    handler.WeaponReset = 5.0f;

                    handler.HeldButton = false;

                }).Run();
        }


        private void OnPlayerBlock(InputAction.CallbackContext obj)
        {
            block = !block;
            switchBlock = true;

        }


        void ToggleMagicInput(InputAction.CallbackContext obj)
        {
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

        /// <summary>
        /// Handles the player jump action, allowing characters to jump under certain conditions.
        /// </summary>
        /// <param name="obj">
        /// The input callback context that contains information about the triggered jump input event.
        /// </param>
        void OnPlayerJump(InputAction.CallbackContext obj)
        {

            Entities.WithoutBurst().WithNone<CarryingTag>().ForEach(
                (ref CharControllerE control, in Player_Control pc) =>
                {
                    if (!control.CanJump) return;
                    if (!pc.InSafeZone && control is { Jump: false, IsGrounded: true })
                    {
                        control.Jump = true;
                    }
                }).Run();


            var interactableQuery = SystemAPI.QueryBuilder().WithAll<Interactable, LocalToWorld>().WithNone<CarriedTag>()
                .Build();
            var entities = interactableQuery.ToEntityArray(Allocator.TempJob);
            var positions = interactableQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
            var interactables = interactableQuery.ToComponentDataArray<Interactable>(Allocator.TempJob);
            Entities.WithStructuralChanges().WithAll<CharControllerE, Player_Control>().WithNone<CarryingTag>()
                .WithoutBurst().ForEach((Entity entity, in LocalToWorld transform) =>
                {
                    for (var index = 0; index < positions.Length; index++)
                    {
                        var position = positions[index];
                        var dist = Vector3.Distance(transform.Position, position.Position);
                        if (dist >= .75f) continue;
                        switch (interactables[index].Interaction)
                        {
                            case Interaction.Pickup:
                                EntityManager.AddComponentData(entities[index], new CarriedTag(entity));
                                EntityManager.AddComponentData(entities[index], new Parent() { Value = entity });
                                EntityManager.SetComponentData(entities[index], new LocalTransform()
                                {
                                    Scale = 1.0f,
                                    Rotation = Quaternion.identity,
                                    Position = new float3(0, .5f, 0)
                                });
                                EntityManager.AddComponent<CarryingTag>(entity);
                                break;
                        }
                        break;
                    }
                }).Run();
            var lookup = GetComponentLookup<Interactable>();
            
            Entities.WithStructuralChanges().WithAll<CharControllerE, Player_Control, CarryingTag>().WithoutBurst()
                .ForEach((Entity entity, DynamicBuffer<Child> children) =>
                {
                    foreach (var child in children)
                    {
                        if (!lookup.HasComponent(child.Value)) continue;
                        EntityManager.RemoveComponent<CarriedTag>(child.Value);
                        EntityManager.SetComponentData(child.Value, new Parent()
                        {
                            Value = Entity.Null
                        });
                        EntityManager.RemoveComponent<CarryingTag>(entity);
                        break;
                    }
                }).Run();

        }

        private ControllerOptions options;
        private void OnStyleModPress(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Command command, CharacterInventory inventory) =>
            {
                var weapon = inventory.Equipment.EquippedWeapons[WeaponSlot.Primary];
                switch (weapon.WeaponType)
                {
                    case WeaponType.Katana:
                    case WeaponType.Sword:
                    case WeaponType.H2BoardSword:
                        if(command.StyleModPressHold)
                            command.StyleMod = true;
                        else
                        {
                            command.StyleMod = !command.StyleMod;
                        } 
                        weapon.StyleChange(command.StyleMod);
                        break;
          
                }
            }).Run();
       
        }

        private void OnStyleModRelease(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Entity entity, Command command, 
                CharacterInventory inventory, PlayerComboComponent combo) =>
            {
                var weapon = inventory.Equipment.EquippedWeapons[WeaponSlot.Primary];

                switch (weapon.WeaponType)
                {
                    case WeaponType.Katana:
                    case WeaponType.H2BoardSword:
                    case WeaponType.Sword:
                        if (command.StyleModPressHold)
                            command.StyleMod = false;
                        inventory.Equipment.EquippedWeapons[WeaponSlot.Primary].StyleChange(command.StyleMod);
                        break;
                    case WeaponType.SpellBook:
                    case WeaponType.SpellBlade:
                        var spell = (SpellBookSO)weapon;
                        spell.SwapSpell(spell.CurIndex+1, entity);
                        combo.Combo = GetCombo(spell.CurComboID);
                        break;
                }
            }).Run();
        }

        public void OpenSpellChangeMenu(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Entity entity, Command command,
                CharacterInventory inventory, PlayerComboComponent combo) =>
            {
                var weapon = inventory.Equipment.EquippedWeapons[WeaponSlot.Primary];

                switch (weapon.WeaponType)
                {
                    case WeaponType.SpellBook:
                    case WeaponType.SpellBlade:
                        Debug.Log("Open UI Menu");
                        break;
                }
            }).Run();
        }

        void ButtonHelded(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((Command command) =>
            {
                command.HeldButton = true;

            }).Run();
                
        }
        void OnPlayerToggleWalkSprint(InputAction.CallbackContext obj)
        {
            //Todo add button press change speed 
            
        }

        public ComboSO GetCombo(int index)
        {
            var combos = Resources.LoadAll<ComboSO>("Combo Data");
            Debug.Log(combos.Length);

            var test = combos.FirstOrDefault(item => item.ID == index);
            return test;
        }


        protected override void OnUpdate()
        {
            
            if (m_mainCam == null)
            {
                if (Camera.main != null)
                {
                    m_mainCam = Camera.main.transform;
                }
                else
                {
                    Debug.LogWarning(
                        "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                    // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
                }
            }
            var dir = playerControls.PlayerController.Locomotion.ReadValue<Vector2>();
            var casting = playerControls.MagicController.enabled;
            Entities.ForEach((ref CharControllerE control, in Player_Control PC) =>
            {
                control.CastingInput = casting;

                if (control.block)
                {
                    control.H = 0.0f;
                    control.V = 0.0f;
                }
                else
                {
                    control.H = dir.x;
                    control.V = dir.y;
                    if (PC.InSafeZone)
                    {
                        control.Walk = true;
                    }
                    //todo add crouching input??? Is this needed
                }
            }).Schedule();


            Entities.WithoutBurst().WithAll<Player_Control>().ForEach(
                (Transform transform, ref CharControllerE Control) =>
                {

                    if (m_mainCam != null)
                    {
                        Vector3 m_CamForward = Vector3.Scale(m_mainCam.forward, new Vector3(1, 0, 1)).normalized;
                        Control.Move = Control.V * m_CamForward + Control.H * m_mainCam.right;
                    }
                    else
                    {
                        Control.Move = Control.V * Vector3.forward + Control.H * Vector3.right;
                    }

                    if (Control.Walk)
                        Control.Move *= 0.5f;
                    if (Control.Move.magnitude > 1.0f)
                        Control.Move.Normalize();
                    Control.Move = transform.InverseTransformDirection(Control.Move);

                    // This section of code can be moved to a  job??


                }).Run();

            
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
            var interactables = SystemAPI.QueryBuilder().WithAll<Interactable,LocalToWorld>().WithNone<CarriedTag>().Build();
            var positions = interactables.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((ref CharControllerE control, in LocalToWorld transform) =>
                {
                    control.CanJump = true;
                    foreach (var position in positions)
                    {
                        var dist = Vector3.Distance(transform.Position, position.Position);
                        if (dist >= 1.75f) continue;
                        control.CanJump = false;
                      
                    }
                }
            ).Schedule();
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
        
        private void OnTogglePause(InputAction.CallbackContext obj)
        {
            if(!paused)
            {
                Debug.Log("Pause Game open menu");
                playerControls.PauseMenu.Enable();
                playerControls.PlayerController.Disable();
                paused = true;
                World.DefaultGameObjectInjectionWorld.EntityManager.RemoveComponent<RunningTag>(controlManager);
            }
            else
            {
                Debug.Log("Start Game close menu");
                playerControls.PauseMenu.Disable();
                playerControls.PlayerController.Enable();
                paused = false;
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<RunningTag>(controlManager);

            }
        }
    }
}