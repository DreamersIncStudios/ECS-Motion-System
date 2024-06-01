using System.Linq;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.ComboSystem;
using DreamersInc.DamageSystem;
using DreamersInc.InputSystems;
using MotionSystem.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamersInc.Global
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(ButtonInputSystem))]
    public partial class InputSystem : SystemBase
    {
        Transform m_mainCam;
        private PlayerControls playerControls;
        protected override void OnCreate()
        {
            RequireForUpdate<Player_Control>();
            RequireForUpdate<InputSingleton>();
            if (SystemAPI.ManagedAPI.TryGetSingleton<InputSingleton>(out var inputSingle))
            {
                playerControls = inputSingle.ControllerInput;
            }

        }

        protected override void OnStartRunning()
        {
            if (playerControls == null)
            {
                if (SystemAPI.ManagedAPI.TryGetSingleton<InputSingleton>(out var inputSingle))
                {
                    playerControls = inputSingle.ControllerInput;
                }
            }

            playerControls.PlayerController.PauseGame.performed += OnTogglePause;
            playerControls.PauseMenu.Disable();
            playerControls.PauseMenu.PauseGame.performed += OnTogglePause;
            playerControls.PlayerController.Jump.performed += OnPlayerJump;
            playerControls.PlayerController.StyleModPress.performed += OnStyleModPress;
            playerControls.PlayerController.StyleModRelease.performed += OnStyleModRelease;
            playerControls.PlayerController.AttackButtonHeld.performed += ButtonHelded;
            playerControls.PlayerController.SpellChange.performed += OpenSpellChangeMenu;

        }

        protected override void OnStopRunning()
        {
            playerControls.Disable();
            playerControls.PlayerController.PauseGame.performed -= OnTogglePause;
            playerControls.PauseMenu.PauseGame.performed -= OnTogglePause;
            playerControls.PlayerController.Jump.performed -= OnPlayerJump;
            playerControls.PlayerController.StyleModPress.performed -= OnStyleModPress;
            playerControls.PlayerController.StyleModRelease.performed -= OnStyleModRelease;
            playerControls.PlayerController.AttackButtonHeld.performed -= ButtonHelded;
            playerControls.PlayerController.SpellChange.performed -= OpenSpellChangeMenu;

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
                bool m_Crouching = new();

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

        }

        private bool paused = false;
        private void OnTogglePause(InputAction.CallbackContext obj)
        {
            if(!paused)
            {
                Debug.Log("Pause Game open menu");
                playerControls.PauseMenu.Enable();
                playerControls.PlayerController.Disable();
                paused = true;
            }
            else
            {
                Debug.Log("Start Game close menu");
                playerControls.PauseMenu.Disable();
                playerControls.PlayerController.Enable();
                paused = false;
            }
        }

        void OnPlayerJump(InputAction.CallbackContext obj)
        {
            Entities.WithoutBurst().ForEach((ref CharControllerE control, in Player_Control pc) =>
            {
                if (!pc.InSafeZone && control is { Jump: false, IsGrounded: true })
                {
                    control.Jump = true;
                }
                
            }).Run();
        }

        public ControllerOptions options;
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
    }

}
