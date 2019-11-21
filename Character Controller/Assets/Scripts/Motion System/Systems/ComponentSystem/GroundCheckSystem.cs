using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MotionSystem.Components;
using Unity.Collections;
using Unity.Jobs;
namespace MotionSystem.System
{
    [UpdateAfter(typeof(InputSystem))]
    public class GroundCheckSystem : ComponentSystem
    {

        EntityQueryDesc GroundChecker = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(CharControllerE), typeof(Transform), typeof(Animator), typeof(Rigidbody) }
        };
        protected override void OnUpdate()
        {
            NativeArray<CharControllerE> chars = GetEntityQuery(GroundChecker).ToComponentDataArray<CharControllerE>(Allocator.TempJob);
            Transform[] transforms = GetEntityQuery(GroundChecker).ToComponentArray<Transform>();
            Animator[] Anims = GetEntityQuery(GroundChecker).ToComponentArray<Animator>();

            NativeList<RaycastCommand> GroundCheck = new NativeList<RaycastCommand>(Allocator.Persistent);

            for (int index = 0; index < chars.Length; index++)
            {
                GroundCheck.Add(new RaycastCommand()
                {
                    from = transforms[index].position + (Vector3.up * .1f),
                    direction = Vector3.down,
                    distance = chars[index].GroundCheckDistance,
                    layerMask = chars[index].Test,
                    maxHits = 1
                });
            }

            NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(GroundCheck.Length, Allocator.Persistent);

            JobHandle Handle = RaycastCommand.ScheduleBatch(GroundCheck, results, 1);
            Handle.Complete();
            //CharController temp;
            //for (int index = 0; index < chars.Length; index++)
            //{
            //    // Debug.Log(results[0].collider.name);
            //temp = chars[index];

            //    if (results[index].collider != null)
            //    {
            //        temp.GroundNormal = results[0].normal;
            //        temp.IsGrounded = true;

            //    }
            //    else
            //    {
            //        temp.GroundNormal = Vector3.up;

            //        temp.IsGrounded = false;
            //    }
          
            //    chars[index] = temp;
            //    //  Debug.Log(chars[index].IsGrounded);
     
            //}
            Entities.With(GetEntityQuery(GroundChecker)).ForEach((Entity entity, ref CharControllerE Control) => {
        


                if (results[entity.Index].collider != null)
                {
                    Control.GroundNormal = results[0].normal;
                    Control.IsGrounded = true;
                }
                else
                {
                    Control.GroundNormal = Vector3.up;

                    Control.IsGrounded = false;
                }
            });

            chars.Dispose();
            results.Dispose();
            GroundCheck.Dispose();
        }

    }
}