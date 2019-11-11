﻿using System.Collections;
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

    [UpdateBefore(typeof(InputSystem))]
    public class Grounded : ComponentSystem
    {
        EntityQueryDesc GroundChecker = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(CharController), typeof(Transform), typeof(Animator)}
        };

        protected override void OnUpdate()
        {

            NativeArray<CharController> chars = GetEntityQuery(GroundChecker).ToComponentDataArray<CharController>(Allocator.Persistent);
            Transform[] transforms = GetEntityQuery(GroundChecker).ToComponentArray<Transform>();
            Animator[] Anim = GetEntityQuery(GroundChecker).ToComponentArray<Animator>();
            NativeList<RaycastCommand> GroundCheck = new NativeList<RaycastCommand>(Allocator.Persistent);
            // GroundCheck using raycastCommand 
            for (int index = 0; index < chars.Length; index++)
            {
                //RaycastHit hitInfo;
                //#if UNITY_EDITOR
                //                // helper to visualise the ground check ray in the scene view
                //                Debug.DrawLine(transforms[index].position + (Vector3.up * 0.1f), transforms[index].position + (Vector3.up * 0.1f) + (Vector3.down * chars[index].GroundCheckDistance));
                //#endif


                GroundCheck.Add(new RaycastCommand()
                {
                    from = transforms[index].position + (Vector3.up * .01f),
                    direction = Vector3.down,
                    distance = chars[index].GroundCheckDistance,
                    layerMask = ~0 >> 6,
                    maxHits = 1
                });
            }

            NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(GroundCheck.Length, Allocator.Persistent);

            JobHandle Handle = RaycastCommand.ScheduleBatch(GroundCheck, results, 1);
            Handle.Complete();
            for (int index = 0; index < chars.Length; index++)
            {
               // Debug.Log(results[0].collider.name);
                CharController temp = chars[index];
                // if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))

                if (results[index].collider != null)
                {
                    temp.GroundNormal = results[0].normal;
                    temp.IsGrounded = true;
                 
                }
                else
                {
                    temp.GroundNormal = Vector3.up;

                    temp.IsGrounded = false;
                }
                chars[index] = temp;


            }
            Debug.Log(chars[0].IsGrounded);
            chars.Dispose();
            results.Dispose();
            GroundCheck.Dispose();


        }
    }

    public class InputSystem : ComponentSystem
    {

        const float k_Half = 0.5f;
        bool m_Crouching;

        protected override void OnUpdate()
        {
            Entities.ForEach((ref Player_Control PCC, ref CharController Control) =>
            {
                Control.H = CrossPlatformInputManager.GetAxis("Horizontal");
                Control.V = CrossPlatformInputManager.GetAxis("Vertical");
                Control.Crouch = Input.GetKey(KeyCode.C);

                if (!Control.Jump)
                {
                    Control.Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }
                Control.Walk = Input.GetKey(KeyCode.LeftShift);
                Debug.Log(Control.IsGrounded);

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

            Entities.ForEach((ref CharController control, Transform transform, Animator Anim, Rigidbody RB) =>
            {
                float m_TurnAmount;
                float m_ForwardAmount;
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
                //                control.IsGrounded = Anim.applyRootMotion = CheckGroundStatus(transform, control.GroundCheckDistance, out control.GroundNormal);
                Anim.applyRootMotion = control.IsGrounded;
                control.Move = Vector3.ProjectOnPlane(control.Move, control.GroundNormal);

                m_TurnAmount = Mathf.Atan2(control.Move.x, control.Move.z);
                m_ForwardAmount = control.Move.z;

                float turnSpeed = Mathf.Lerp(control.m_StationaryTurnSpeed, control.m_MovingTurnSpeed, m_ForwardAmount);
                transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);


                if (control.IsGrounded)
                {
                    if (control.Jump && !control.Crouch && Anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                    {
                        // jump!
                       RB.velocity = new Vector3(RB.velocity.x, control.m_JumpPower, RB.velocity.z);
                            control.IsGrounded = false;
                            Anim.applyRootMotion = false;
                        control.GroundCheckDistance = 0.1f;
                    }
                }
                else {
                    Vector3 extraGravityForce = (Physics.gravity * control.m_GravityMultiplier) - Physics.gravity;
                    RB.AddForce(extraGravityForce);

                   // control.GroundCheckDistance = RB.velocity.y < 0 ? control.m_OrigGroundCheckDistance : 0.01f;
                }

                //ScaleCapsules Collider

                //AutoCrouch 


                // Animator Updater
            
                    // update the animator parameters
                    Anim.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
                    Anim.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
                    Anim.SetBool("Crouch", m_Crouching);
                    Anim.SetBool("OnGround", control.IsGrounded);
                    if (!control.IsGrounded)
                    {
                        Anim.SetFloat("Jump", RB.velocity.y);
                    }

                    // calculate which leg is behind, so as to leave that leg trailing in the jump animation
                    // (This code is reliant on the specific run cycle offset in our animations,
                    // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
                    float runCycle =
                        Mathf.Repeat(
                            Anim.GetCurrentAnimatorStateInfo(0).normalizedTime + control.m_RunCycleLegOffset, 1);
                    float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
                    if (control.IsGrounded)
                    {
                        Anim.SetFloat("JumpLeg", jumpLeg);
                    }

                    // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
                    // which affects the movement speed because of the root motion.
                    if (control.IsGrounded && control.Move.magnitude > 0)
                    {
                        Anim.speed = control.m_AnimSpeedMultiplier;
                    }
                    else
                    {
                        // don't use that while airborne
                        Anim.speed = 1;
                    }

                control.Jump = false;

            });


        }

        bool CheckGroundStatus(Transform transform, float m_GroundCheckDistance, out Vector3 m_GroundNormal) {
                RaycastHit hitInfo;
#if UNITY_EDITOR
                // helper to visualise the ground check ray in the scene view
                Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            //NativeList<RaycastCommand> GroundCheck = new NativeList<RaycastCommand>(Allocator.Persistent);

            //GroundCheck.Add( new RaycastCommand()
            //{
            //    from = transform.position + (Vector3.up * .01f),
            //    direction = Vector3.down,
            //    distance = m_GroundCheckDistance,
            //    layerMask = ~0>>6,
            //    maxHits = 1
            //});

            //NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(1, Allocator.Persistent);

            //JobHandle Handle = RaycastCommand.ScheduleBatch(GroundCheck,results,10);
            //Handle.Complete();
            
           // Debug.Log(results[0].collider.name);

          if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
           // if (results[0].collider!=null)
            {
                    m_GroundNormal = hitInfo.normal;
                //   m_Animator.applyRootMotion = true;
                //GroundCheck.Dispose();
              //  results.Dispose();
                    return true;

                }
                else
                {
                    m_GroundNormal = Vector3.up;
                //   m_Animator.applyRootMotion = false;
               // GroundCheck.Dispose();
              //  results.Dispose();
                return false;
                }
        }


    }
}


