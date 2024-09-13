using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DreamersInc.MovementSys
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AgentLocomotionSystemGroup))]
    public partial struct BeastLocomotionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new BeastLocomotionJob
            {
                DeltaTime = state.WorldUnmanaged.Time.DeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct BeastLocomotionJob : IJobEntity
        {
            public float DeltaTime;

            private void Execute(ref LocalTransform transform, ref AgentBody body, in BeastLocomotion locomotion, in AgentShape shape)
            {
                if (body.IsStopped)
                    return;

                // Check, if we reached the destination
                float remainingDistance = body.RemainingDistance;
                if (remainingDistance <= locomotion.StoppingDistance + 1e-3f)
                {
                    body.Velocity = 0;
                    body.IsStopped = true;
                    return;
                }

                float maxSpeed = locomotion.Speed;

                // Start breaking if close to destination
                if (locomotion.AutoBreaking)
                {
                    float breakDistance = shape.Radius * 2 + locomotion.StoppingDistance;
                    if (remainingDistance <= breakDistance)
                    {
                        maxSpeed = math.lerp(locomotion.Speed * 0.25f, locomotion.Speed, remainingDistance / breakDistance);
                    }
                }

                // Force force to be maximum of unit length, but can be less
                float forceLength = math.length(body.Force);
                if (forceLength > 1)
                    body.Force = body.Force / forceLength;

                switch (shape.Type)
                {
                    // Update rotation
                    case ShapeType.Circle:
                    {
                        float angle = math.atan2(body.Velocity.x, body.Velocity.y);
                        transform.Rotation = math.slerp(transform.Rotation, quaternion.RotateZ(-angle), DeltaTime * locomotion.AngularSpeed);
                        break;
                    }
                    case ShapeType.Cylinder:
                    {
                        float angle = math.atan2(body.Velocity.x, body.Velocity.z);
                        transform.Rotation = math.slerp(transform.Rotation, quaternion.RotateY(angle), DeltaTime * locomotion.AngularSpeed);
                        break;
                    }
                }

                // Beast should only move, if facing direction and movement direction is within certain degrees
                float3 direction = math.normalizesafe(body.Velocity);
                float3 facing = math.mul(transform.Rotation, new float3(1, 0, 0));
                if (math.dot(direction, facing) > math.radians(10))
                {
                    maxSpeed = 0;
                }

                // Interpolate velocity
                body.Velocity = math.lerp(body.Velocity, body.Force * maxSpeed, DeltaTime * locomotion.Acceleration);

                float speed = math.length(body.Velocity);

                // Early out if steps is going to be very small
                if (speed < 1e-3f)
                    return;

                // Avoid over-stepping the destination
                if (speed * DeltaTime > remainingDistance)
                {
                    transform.Position += (body.Velocity / speed) * remainingDistance;
                    return;
                }

                // Update position
                transform.Position += DeltaTime * body.Velocity;
            }
        }
    }
}