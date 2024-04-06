using MotionSystem.Components;
using Unity.Entities;
using UnityEngine;
// ReSharper disable Unity.BurstLoadingManagedType

namespace MotionSystem.Systems
{
    public class AnimationSpeed : MonoBehaviour
    {
        public bool IsGrounded { get; set; }
        private Animator animator;
        private Rigidbody rb;
        public float moveSpeedMultiplier { get; set; }

        public void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }
        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (!IsGrounded || !(Time.deltaTime > 0)) return;
            var v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = rb.linearVelocity.y;
            rb.linearVelocity = v;
        }


    }
    public class AnimationSpeedLink : IComponentData {
        public AnimationSpeed Link;
    }

    public partial class AnimationSync : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((AnimationSpeedLink animLink, ref CharControllerE control) => {
                animLink.Link.IsGrounded = control.IsGrounded;
                animLink.Link.moveSpeedMultiplier = control.m_MoveSpeedMultiplier;
            
            }).WithoutBurst().Run();
        }
    }
}