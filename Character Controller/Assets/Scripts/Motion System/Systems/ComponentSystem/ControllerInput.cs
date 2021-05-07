
using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using DreamersStudio.CameraControlSystem;

namespace MotionSystem.System {

   
    public class InputSystem : ComponentSystem
    {
       
        const float k_Half = 0.5f;
        Transform m_mainCam;

        bool IsTargeting => CrossPlatformInputManager.GetAxis("Target Trigger") > .3f;

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

            Entities.ForEach(( Rigidbody RB, ref Player_Control PCC, ref CharControllerE Control) =>
            {
                 ControllerScheme InputSet = PCC.InputSet;

                bool m_Crouching = new bool();
                if (Control.block)
                {
                    Control.H = 0.0f;
                    Control.V = 0.0f;
                }
                else
                {
                    Control.H = CrossPlatformInputManager.GetAxis("Horizontal");
                    Control.V = CrossPlatformInputManager.GetAxis("Vertical");
                    m_Crouching = Input.GetKey(KeyCode.C);

                    if (!PCC.InSafeZone) {
                        if (!Control.Jump && Control.canInput && Control.IsGrounded && !Input.GetKey(InputSet.ActivateCADMenu))
                        {
                            Control.Jump = Input.GetKeyDown(InputSet.Jump);

                        }
                        if (Control.Jump)
                        {
                            Control.InputTimer = .2f;
                        }
                       // add controller toogle
                        Control.Walk = Input.GetKey(KeyCode.LeftShift);

                    }
                    else {
                        Control.Walk = true;
                    }

                }


                if (Control.IsGrounded && m_Crouching)
                {
                    if (Control.Crouch)
                    { return; }
                    Control.CapsuleHeight /= 2f;
                    Control.CapsuleCenter /= 2f;
                    Control.Crouch = true;
                    RB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }
                else
                {
                    Ray crouchRay = new Ray(RB.position + Vector3.up * Control.CapsuleRadius * k_Half, Vector3.up);
                    float crouchRayLength = Control.CapsuleHeight - Control.CapsuleRadius * k_Half;
                    if (Physics.SphereCast(crouchRay, Control.CapsuleRadius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                    {
                        Control.Crouch = true;
                        return;
                    }
                    //add orginial capsule stats
                    RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;


                    Control.CapsuleHeight = Control.OGCapsuleHeight;
                    Control.CapsuleCenter = Control.OGCapsuleCenter;
                    Control.Crouch = false;
                }

            });

            Entities.ForEach((NavMeshAgent agent, ref AI_Control ACC, ref CharControllerE Control, ref Movement mover) =>
            {

                if (mover.CanMove)
                {

                    Control.Move = agent.desiredVelocity;
                    Control.Crouch = false;
                    Control.Jump = false;
                }
                else
                {
                    if (!agent.isStopped) {
                        agent.isStopped = true;
                    }
                    Control.Move = float3.zero;
                    Control.Crouch = false;
                    Control.Jump = false;
                }

            });

            Vector3 m_CamForward;             // The current forward direction of the camera
            Entities.ForEach((ref CharControllerE Control, Transform transform) =>
            {
                if (!Control.AI) {
                    if (m_mainCam != null)
                    {
                        m_CamForward = Vector3.Scale(m_mainCam.forward, new Vector3(1, 0, 1)).normalized;
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
                Control.Move = transform.InverseTransformDirection(Control.Move);

                // This section of code can be moved to a  job??
             

                if (Control.TimerForEquipReset > 0.0f) {
                    Control.TimerForEquipReset -= Time.DeltaTime;
                }
                else { 
                    Control.TimerForEquipReset = 0.0f;
                }

            });
            }
    }
}


