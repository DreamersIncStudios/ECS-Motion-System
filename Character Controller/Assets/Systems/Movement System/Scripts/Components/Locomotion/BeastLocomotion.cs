using Unity.Entities;
using Unity.Mathematics;

namespace DreamersInc.MovementSys
{
    public struct BeastLocomotion: IComponentData
    {
        /// <summary>
        /// Maximum movement speed when moving to destination.
        /// </summary>
        public float Speed;
        /// <summary>
        /// The maximum acceleration of an agent as it follows a path, given in units / sec^2.
        /// </summary>
        public float Acceleration;
        /// <summary>
        /// Maximum turning speed in (rad/s) while following a path.
        /// </summary>
        public float AngularSpeed;
        /// <summary>
        /// Stop within this distance from the target position.
        /// </summary>
        public float StoppingDistance;
        /// <summary>
        /// Should the agent brake automatically to avoid overshooting the destination point?
        /// </summary>
        public bool AutoBreaking;

        public BeastLocomotion(float speed, float acceleration, float angularSpeed, float stoppingDistance, bool autoBreaking)
        {
            Speed = speed;
            Acceleration = acceleration;
            AngularSpeed = angularSpeed;
            StoppingDistance = stoppingDistance;
            AutoBreaking = autoBreaking;
        }

        public BeastLocomotion DefaultLocomotion => new(speed: 3.5f, acceleration: 8, angularSpeed: math.radians(120),
            stoppingDistance: .5f, autoBreaking: true);
    }
}
