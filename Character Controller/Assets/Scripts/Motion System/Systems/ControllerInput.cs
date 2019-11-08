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

namespace MotionSystem.System {


    public class InputSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Player_Control PCC, ref CharController Control) => {
                Control.H = CrossPlatformInputManager.GetAxis("Horizontal");
                Control.V = CrossPlatformInputManager.GetAxis("Vertical");
                Control.Crouch = Input.GetKey(KeyCode.C);

                if (!Control.Jump)
                {
                    Control.Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }
                 Control.Walk=Input.GetKey(KeyCode.LeftShift);

            });

            Entities.ForEach((NavMeshAgent agent,ref AI_Control ACC, ref CharController Control, ref Movement mover) => {

                if (mover.CanMove)
                {
                    Control.Move = agent.desiredVelocity;
                    Control.Crouch = false;
                    Control.Jump = false;
                }
                else {
                    Control.Move = float3.zero;
                    Control.Crouch = false;
                    Control.Jump = false;
                }

            });


            Vector3 m_CamForward;             // The current forward direction of the camera

            Entities.ForEach((Transform transform,ref CharController control) => 
            {

                if (Camera.main == null)
                {
                    Debug.LogWarning(
                        "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                    // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
                }


                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                control.Move = control.V * m_CamForward + control.H * Camera.main.transform.right;



                if (control.Walk) control.Move *= 0.5f;

                if (control.Move.magnitude > 1.0f)
                    control.Move.Normalize();
                control.Move = transform.InverseTransformDirection(control.Move);

            });
            



            bool CheckGroundStatus(Transform transform, float m_GroundCheckDistance) {
                RaycastHit hitInfo;
#if UNITY_EDITOR
                // helper to visualise the ground check ray in the scene view
                Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
                if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
                {
                    m_GroundNormal = hitInfo.normal;
                    m_Animator.applyRootMotion = true;
                    return true;

                }
                else
                {
                    m_GroundNormal = Vector3.up;
                    m_Animator.applyRootMotion = false;
                    return false;
                }

           
            }

        }
    }
}

