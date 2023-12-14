using MotionSystem.Components;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Stats.Entities;
using UnityEngine.InputSystem;

namespace DreamersInc.Global
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class InputSystem : SystemBase
    {
        Transform m_mainCam;
        private PlayerControls playerControls;
        protected override void OnCreate()
        {
            RequireForUpdate<Player_Control>();
            RequireForUpdate<ControllerInfo>();
            playerControls = new PlayerControls();
        }

        protected override void OnStartRunning()
        {
            playerControls.Enable();
            playerControls.PlayerController.PauseGame.performed += OnTogglePause;
            playerControls.PauseMenu.Disable();
            playerControls.PauseMenu.PauseGame.performed += OnTogglePause;

        }

        protected override void OnStopRunning()
        {
            playerControls.Disable();
            playerControls.PlayerController.PauseGame.performed -= OnTogglePause;
            playerControls.PauseMenu.PauseGame.performed -= OnTogglePause;

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
            if (!SystemAPI.TryGetSingleton<ControllerInfo>(out var config))
                return;
            var dir = playerControls.PlayerController.Locomotion.ReadValue<Vector2>();
      
            Entities.WithoutBurst().ForEach((ref CharControllerE Control, in Player_Control PC) =>
            {
                Control.CastingInput = config.OpenCadInput;
                bool m_Crouching = new();
                if (!Control.CastingInput)
                {
                    if (Control.block)
                    {
                        Control.H = 0.0f;
                        Control.V = 0.0f;
                    }
                    else
                    {
                        Control.H = dir.x;
                        Control.V = dir.y;

                        m_Crouching = Input.GetKey(KeyCode.C);

                        if (!PC.InSafeZone)
                        {
                            if (!Control.Jump && Control.IsGrounded)
                            {
                                Control.Jump = config.Jumpb;

                            }

                            // add controller toogle
                            Control.Walk = Input.GetKey(KeyCode.LeftShift);

                        }
                        else
                        {
                            Control.Walk = true;
                        }

                    }
                }


            }).Run();


            Entities.WithoutBurst().ForEach((Animator Anim, ref CharControllerE Control) =>
            {
                if (!Control.AI)
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
                }
                if (Control.Walk)
                    Control.Move *= 0.5f;
                if (Control.Move.magnitude > 1.0f)
                    Control.Move.Normalize();
                Control.Move = Anim.transform.InverseTransformDirection(Control.Move);

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
    }

}
