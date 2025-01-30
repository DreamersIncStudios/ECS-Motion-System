using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MotionSystem.Components;
using Unity.Transforms;

namespace AISenses.VisionSystems.Combat
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(TargetingSystem))]
    public partial class AttackTargetSystem : SystemBase
    {

        protected override void OnUpdate()
        {
//todo remove scan buffers that are not behind player
            Entities.ForEach((ref LocalToWorld transform, ref AttackTarget attackTarget, ref DynamicBuffer<ScanPositionBuffer> buffer,
                ref Vision vision, in CharControllerE Control) =>
            {
                if (buffer.IsEmpty)
                {
                    attackTarget = new AttackTarget();
                    return;

                }

                if (Control.Targetting) return;
                //Attack in direction of point target
                var visibleTargetInArea = buffer.ToNativeArray(Allocator.Temp);
                visibleTargetInArea.Sort(new SortScanPositionByDistance());

                foreach (var target in visibleTargetInArea)
                {
                    var dirToTarget = ((Vector3)target.target.LastKnownPosition -
                                       (Vector3)(transform.Position + new float3(0, 1, 0))).normalized;
                    if (!(Vector3.Angle(transform.Forward, dirToTarget) < vision.ViewAngle / 2.0f)) continue;

                    if (target.target.IsFriendly) continue;
                    if (!(vision.EngageRadius > target.dist)) continue;
                    attackTarget.AttackTargetLocation = target.target.LastKnownPosition;
                    attackTarget.TargetInRange = true;
                    return;
                }

                attackTarget.AttackTargetLocation = new float3();
                attackTarget.TargetInRange = false;

            }).ScheduleParallel();
        }

    }
}