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
            All = new ComponentType[] { typeof(CharController), typeof(Transform), typeof(Animator), typeof(Rigidbody) }
        };

        protected override void OnUpdate()
        {
            Entities.ForEach(( Rigidbody RB, ref Player_Control PCC, ref CharController Control) =>
            {
                Control.H = CrossPlatformInputManager.GetAxis("Horizontal");
                Control.V = CrossPlatformInputManager.GetAxis("Vertical");
                Control.Crouch = Input.GetKey(KeyCode.C);


   
                if (!Control.Jump)
                    Control.Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        
                Control.Walk = Input.GetKey(KeyCode.LeftShift);
                //    Debug.Log(Control.IsGrounded);

            });

            Entities.ForEach((NavMeshAgent agent, ref AI_Control ACC, ref CharController Control, ref Movement mover) =>
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
            Camera Main = Camera.main;
            Entities.ForEach((ref CharController Control, Transform transform) =>
            {
                if (Main == null)
                {
                    Debug.LogWarning(
                        "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                    // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
                    Control.Move = Control.V * Vector3.forward + Control.H * Vector3.right;
                }
                else {
                    m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                    Control.Move = Control.V * m_CamForward + Control.H * Camera.main.transform.right;

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


