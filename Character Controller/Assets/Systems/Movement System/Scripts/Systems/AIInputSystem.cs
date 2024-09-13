using Components.MovementSystem;
using DreamersInc.Global;
using MotionSystem.Components;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DreamersInc.MovementSys
{
    [UpdateBefore(typeof(InputSystem))]
    public partial class AIInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<Animator>().WithoutBurst().ForEach((ref AgentBody Agent, ref CharControllerE Control, ref Movement Mover) => 
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
            Entities.WithAll<Animator>().WithoutBurst().ForEach((ref AgentBody Agent, ref BeastControllerComponent Control, ref Movement Mover) =>
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
