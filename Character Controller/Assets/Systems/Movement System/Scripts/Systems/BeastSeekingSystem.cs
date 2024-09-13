using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DreamersInc.MovementSys
{
    /// <summary>
    /// System that steers agent towards destination.
    /// </summary>
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AgentSeekingSystemGroup))]
    public partial struct BeastSeekingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new BeastSteeringJob().ScheduleParallel();
        }

        [BurstCompile]
        partial struct BeastSteeringJob : IJobEntity
        {
            private void Execute(ref AgentBody body, in BeastLocomotion locomotion, in LocalTransform transform)
            {
                if (body.IsStopped)
                    return;

                float3 towards = body.Destination - transform.Position;
                float distance = math.length(towards);
                float3 desiredDirection = distance > math.EPSILON ? towards / distance : float3.zero;
                body.Force = desiredDirection;
                body.RemainingDistance = distance;
            }
        }
    }
}