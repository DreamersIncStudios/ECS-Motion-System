using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


namespace MotionSystem.Components
{
	public class AnimatorSpeedUpdate : MonoBehaviour
	{
		private AnimatorSpeedUpdateSystem AnimUpdate;
		public void OnAnimatorMove()
		{
			if (AnimUpdate == null)
			{
				AnimUpdate = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimatorSpeedUpdateSystem>();
			}
			AnimUpdate.Update();

		}


	}

	[DisableAutoCreation]
	public class AnimatorSpeedUpdateSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((ref CharControllerE controllerE, Animator m_Animator, Rigidbody m_Rigidbody) =>
			{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (controllerE.IsGrounded && Time.DeltaTime > 0)
				{
					Vector3 v = (m_Animator.deltaPosition * controllerE.m_MoveSpeedMultiplier) / Time.DeltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
					m_Rigidbody.velocity = v;
				}
			});
		}


	} 
}