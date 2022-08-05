﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using DG.Tweening;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Collections;
using Unity.Jobs;
using UnityStandardAssets.CrossPlatformInput;
using DreamersStudio.CameraControlSystem;
using DreamersInc.ComboSystem;

namespace MotionSystem.System
{

     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public class AnimatorUpdate : ComponentSystem
    {


        const float k_Half = 0.5f;
        bool IsNotTargeting => CrossPlatformInputManager.GetAxis("Target Trigger") < .3f;

        protected override void OnUpdate()
        {


            Entities.ForEach((ref CharControllerE control, Transform transform, Animator Anim, Rigidbody RB) =>
            {
                float m_TurnAmount;
                float m_ForwardAmount;


                //control.Move = Vector3.ProjectOnPlane(control.Move, control.GroundNormal);

                //  m_TurnAmount = control.Move.x;
                m_ForwardAmount = control.Move.z;
                m_TurnAmount = Mathf.Atan2(control.Move.x, control.Move.z);

                if (IsNotTargeting)
                {
                    float turnSpeed = Mathf.Lerp(control.m_StationaryTurnSpeed, control.m_MovingTurnSpeed, m_ForwardAmount);
                    transform.Rotate(0, m_TurnAmount * turnSpeed * Time.fixedDeltaTime, 0);
                }
                else
                {

                    m_TurnAmount = control.Move.x;
                    if (!control.AI)
                    {
                        if (CameraControl.Instance.TargetGroup.m_Targets[0].target!= null)
                            transform.DOLookAt(CameraControl.Instance.TargetGroup.m_Targets[0].target.position, .35f);
                    }
                }



                if (control.IsGrounded)
                {
                    HandleGroundedMovement(control, Anim, RB);
                }
                else
                {
                    HandleAirborneMovement(control, Anim, RB);
                }

                if (control.ApplyRootMotion)
                {
                    Anim.applyRootMotion = true;
                    control.ApplyRootMotion = false;
                }

                //ScaleCapsules Collider

                //AutoCrouch 


                // Animator Updater

                // update the animator parameters
                Anim.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.fixedDeltaTime);
                Anim.SetFloat("Turn", m_TurnAmount, 0.1f, Time.fixedDeltaTime);
                Anim.SetBool("Crouch", control.Crouch);
                Anim.SetBool("OnGround", control.IsGrounded);
                if (control.CombatCapable)
                {
                    Anim.SetBool("Weapon Drawn", control.EquipWeapon);
                    Anim.SetBool("IsTargeting", !IsNotTargeting);
                }
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



                control.TimerForEquipReset = Anim.GetBool("Weapon In Hand") && control.TimerForEquipReset <= 0.0f && !Anim.GetCurrentAnimatorStateInfo(0).IsName("Locomation_Grounded_Weapon")
                    ? control.EquipResetTimer : Anim.GetCurrentAnimatorStateInfo(0).IsTag("Combo") ? control.EquipResetTimer : control.TimerForEquipReset;

                if (control.TimerForEquipReset > 0.0f && Anim.GetCurrentAnimatorStateInfo(0).IsName("Locomation_Grounded_Weapon"))
                {
                    control.TimerForEquipReset -= 0.02f;
                    if (control.TimerForEquipReset < 0.0f)
                    {
                        control.TimerForEquipReset = 0.0f;
                        Anim.SetBool("Weapon In Hand",false);
                    }
                }
           



            });


            Entities.ForEach((ref CharControllerE Control, CapsuleCollider capsule) =>
            {

                capsule.center = Control.CapsuleCenter;
                capsule.height = Control.CapsuleHeight;

            });
        }
        void HandleGroundedMovement(CharControllerE control, Animator Anim, Rigidbody RB) {
            if (control.Jump && !control.Crouch)
            {
                if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded0")
                || Anim.GetCurrentAnimatorStateInfo(0).IsName("Locomation_Grounded_Weapon0")
                || Anim.GetCurrentAnimatorStateInfo(0).IsName("Targeted_Locomation0"))
                {
                    // jump!
                    Anim.applyRootMotion = false;
                    RB.velocity = new Vector3(RB.velocity.x, control.m_JumpPower, RB.velocity.z);
                    control.IsGrounded = false;
                    control.GroundCheckDistance = 0.1f;
                    control.SkipGroundCheck = true;
                }
            }
        }
        void HandleAirborneMovement(CharControllerE control, Animator Anim, Rigidbody RB)
        {
            Vector3 extraGravityForce = (Physics.gravity * control.m_GravityMultiplier) - Physics.gravity;
            RB.AddForce(extraGravityForce);

            control.SkipGroundCheck = RB.velocity.y > 0;
            control.GroundCheckDistance = RB.velocity.y < 0 ? control.m_OrigGroundCheckDistance : 0.1f;

            Anim.applyRootMotion =
            control.ApplyRootMotion;

        }

    }
}