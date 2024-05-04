using Components.MovementSystem;
using DreamersInc.Global;
using MotionSystem.Components;
using Stats.Entities;
using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace DreamersInc.MovementSys
{
    [UpdateBefore(typeof(InputSystem))]
    public partial class AIInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().ForEach(( Animator Anim, ref AgentBody Agent, ref CharControllerE Control, ref Movement Mover) => 
            {
                if (Mover.CanMove)
                {

                    Control.Move = Agent.Velocity;
                    Control.Crouch = false;
                    Control.Jump = false;
                }
                else
                {
                    if (!Agent.IsStopped)
                    {
                        Agent.IsStopped = true;
                    }
                    Control.Move = float3.zero;
                    Control.Crouch = false;
                    Control.Jump = false;
                }

            }).Run();
            Entities.WithoutBurst().ForEach(( Animator Anim,ref AgentBody Agent, ref BeastControllerComponent Control, ref Movement Mover) =>
            {
                if (Mover.CanMove)
                {

                    Control.Move = Agent.Velocity;
                    Control.Jump = false;
                }
                else
                {
                    if (!Agent.IsStopped)
                    {
                        Agent.IsStopped = true;
                    }
                    Control.Move = float3.zero;
                    Control.Jump = false;
                }

            }).Run();
        }
    }
}
