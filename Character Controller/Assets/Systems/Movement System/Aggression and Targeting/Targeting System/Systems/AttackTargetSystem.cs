using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace AISenses.VisionSystems.Combat
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(TargetingSystem))]
    public partial class AttackTargetSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            
            Entities.ForEach((ref AttackTarget attackTarget, ref DynamicBuffer<ScanPositionBuffer> buffer) =>
            {
                if (attackTarget.isTargeting && buffer.Length >= attackTarget.AttackTargetIndex) 
                {
                    attackTarget.AttackTargetLocation = buffer[attackTarget.AttackTargetIndex].target.LastKnownPosition;
                }
                else {
                    NativeArray<ScanPositionBuffer> scans = buffer.ToNativeArray(Allocator.Temp);
                    if (buffer.Length > 0) { 
                        //Attack in direction of point target
                        var visibleTargetInArea =  buffer.ToNativeArray(Allocator.Temp);
                        visibleTargetInArea.Sort(new SortScanPositionByDistance());
                        for (int i = 0; i < visibleTargetInArea.Length; i++)
                        {

                        }
                        attackTarget.AttackTargetLocation = visibleTargetInArea[0].target.LastKnownPosition;
                    }
                     else
                    {
                        attackTarget.AttackTargetLocation = new float3(1,1,1);
                    }
                    scans.Dispose();
                }
            }).ScheduleParallel();
        }

    }

    public struct AttackTarget : IComponentData {
        public float3 AttackTargetLocation;
        public int AttackTargetIndex;
        public bool isTargeting;
        public float2 AttackDir;
        public float MoveRange;
        public float3 MoveTo(float3 curPos) {
            float dist = Vector3.Distance(curPos, AttackTargetLocation);
            if (AttackTargetLocation.Equals(new float3(1, 1, 1)))
                return new float3(1, 1, 1);

            if (dist < 10)
            {
                return Vector3.MoveTowards(curPos, AttackTargetLocation, .85f);
            }
            else { 
             float ratio  = MoveRange / dist;
                return Vector3.Lerp(curPos, AttackTargetLocation, ratio);
            }
        }
    }
}