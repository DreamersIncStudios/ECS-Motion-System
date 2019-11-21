using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
namespace MotionSystem.System {

   
    public class InputSystem : ComponentSystem
    {
       
        const float k_Half = 0.5f;
        bool m_Crouching;

        EntityQueryDesc GroundChecker = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(CharControllerE), typeof(Transform), typeof(Animator), typeof(Rigidbody) }
        };
        Transform m_mainCam;

        protected override void OnUpdate()
        {
            if ( m_mainCam == null) {
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
                bool m_Crouching = new bool();
                Control.H = CrossPlatformInputManager.GetAxis("Horizontal");
                Control.V = CrossPlatformInputManager.GetAxis("Vertical");
               m_Crouching = Input.GetKey(KeyCode.C);


   
                if (!Control.Jump)
                    Control.Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        
                Control.Walk = Input.GetKey(KeyCode.LeftShift);



                if (Control.IsGrounded && m_Crouching)
                {
                    if (Control.Crouch)
                    { return; }
                    Control.CapsuleHeight = Control.CapsuleHeight / 2f;
                    Control.CapsuleCenter = Control.CapsuleCenter / 2f;
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
                    Control.Move = float3.zero;
                    Control.Crouch = false;
                    Control.Jump = false;
                }

            });

            Vector3 m_CamForward;             // The current forward direction of the camera
            Entities.ForEach((ref CharControllerE Control, Transform transform) =>
            {
                if (m_mainCam != null)
                {

                    m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                    Control.Move = Control.V * m_CamForward + Control.H * Camera.main.transform.right;
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




             

            });

      





            }

        


    }
}


