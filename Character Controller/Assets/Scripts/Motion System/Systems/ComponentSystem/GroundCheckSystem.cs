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
    //TODO UPdate using Unity Physics 
    public class GroundCheckSystem : ComponentSystem
    {

        EntityQueryDesc GroundChecker = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(CharControllerE), typeof(Transform), typeof(Animator), typeof(Rigidbody) }
        };
        protected override void OnUpdate()
        {

            Entities.With(GetEntityQuery(GroundChecker)).ForEach((Entity entity, ref CharControllerE Control, Transform transform) =>
            {


                NativeList<RaycastCommand> GroundCheck = new NativeList<RaycastCommand>(Allocator.Temp);


                GroundCheck.Add(new RaycastCommand()
                {
                    from = transform.position + (Vector3.up * .1f),
                    direction = Vector3.down,
                    distance = Control.GroundCheckDistance,
                    layerMask = Control.GroundCheckLayerMask,
                    maxHits = 1
                });
                GroundCheck.Add(new RaycastCommand()
                {
                    from = transform.position + (Vector3.up * .1f) + (Vector3.left * .25f),
                    direction = Vector3.down,
                    distance = Control.GroundCheckDistance,
                    layerMask = Control.GroundCheckLayerMask,
                    maxHits = 1
                });
                GroundCheck.Add(new RaycastCommand()
                {
                    from = transform.position + (Vector3.up * .1f) - (Vector3.left * .25f),
                    direction = Vector3.down,
                    distance = Control.GroundCheckDistance,
                    layerMask = Control.GroundCheckLayerMask,
                    maxHits = 1
                });
                GroundCheck.Add(new RaycastCommand()
                {
                    from = transform.position + (Vector3.up * .1f) + (Vector3.forward * .25f),
                    direction = Vector3.down,
                    distance = Control.GroundCheckDistance,
                    layerMask = Control.GroundCheckLayerMask,
                    maxHits = 1
                });
                GroundCheck.Add(new RaycastCommand()
                {
                    from = transform.position + (Vector3.up * .1f) - (Vector3.forward * .25f),
                    direction = Vector3.down,
                    distance = Control.GroundCheckDistance,
                    layerMask = Control.GroundCheckLayerMask,
                    maxHits = 1
                });

                NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(GroundCheck.Length, Allocator.Persistent);

                JobHandle Handle = RaycastCommand.ScheduleBatch(GroundCheck, results, 5);
                Handle.Complete();


                for (int i = 0; i < 5; i++)
                {
                    if (results[i].collider != null)
                    {
                        Control.GroundNormal = results[0].normal;
                        Control.IsGrounded = true;
                        goto end;
                    }
                    else
                    {
                        Control.GroundNormal = Vector3.up;
                        Control.IsGrounded = false;
                    }
                }
                end:
                //   chars.Dispose();
                results.Dispose();
                GroundCheck.Dispose();

            });
        }
    }


}