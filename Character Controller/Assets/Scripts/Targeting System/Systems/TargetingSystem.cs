using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using DreamersStudio.CameraControlSystem;

namespace DreamersStudio.TargetingSystem
{
    public class TargetingSystem : SystemBase
    {
        private EntityQuery Targetters;
        private EntityQuery Targets;

        protected override void OnCreate()
        {
            base.OnCreate();
            Targetters = GetEntityQuery(new EntityQueryDesc() { 
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(TargetBuffer)), ComponentType.ReadOnly(typeof(LocalToWorld)),
                ComponentType.ReadOnly(typeof(Player_Control))}
            });
            Targets = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadOnly(typeof(Targetable)), ComponentType.ReadOnly(typeof(LocalToWorld)) }
            });
        }
        protected override void OnUpdate()
        {
            JobHandle systemDeps = Dependency;
            systemDeps = new GetTargetsList()
            {
                BufferChunk = GetArchetypeChunkBufferType<TargetBuffer>(false),
                PositionChunk = GetArchetypeChunkComponentType<LocalToWorld>(true),
                TargetablesArray = Targets.ToComponentDataArray<Targetable>(Allocator.TempJob),
                TargetPositions = Targets.ToComponentDataArray<LocalToWorld>(Allocator.TempJob)
            }.ScheduleParallel(Targetters, systemDeps);

            Dependency = systemDeps;

            if (Input.GetAxis("Target Trigger")>.6f)
            {
                CameraControl.Instance.isTargeting = true;

            }
            if (Input.GetAxis("Target Trigger") < .6f && Input.GetAxis("Target Trigger") > .2f) { 
                CameraControl.Instance.isTargeting = false;


            }



        }
    }

    public struct GetTargetsList : IJobChunk
    {
        public ArchetypeChunkBufferType<TargetBuffer> BufferChunk;
        [ReadOnly] public ArchetypeChunkComponentType<LocalToWorld> PositionChunk;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Targetable> TargetablesArray;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<LocalToWorld> TargetPositions;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            BufferAccessor<TargetBuffer> Buffers = chunk.GetBufferAccessor(BufferChunk);
            NativeArray<LocalToWorld> Positions = chunk.GetNativeArray(PositionChunk);
            for (int i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<TargetBuffer> Target = Buffers[i];
                Target.Clear();
                LocalToWorld Pos = Positions[i];
                for (int j = 0; j < TargetablesArray.Length; j++)
                {
                    float dist = Vector3.Distance(Pos.Position, TargetPositions[j].Position);
                    if(dist<40) // Create a character skill/stat for range or determine a hardcode number
                    {

                        Vector3 dir = ((Vector3)TargetPositions[j].Position - (Vector3)Pos.Position).normalized;
                        float Output = new float();
                        if (dir.x >= 0)
                        {
                            Output = Vector3.Angle(Vector3.forward, dir);
                          
                        }
                        if (dir.x < 0)
                        {
                            Output = Vector3.Angle(Vector3.forward, dir);
                         
                        }
                        Target.Add(new TargetBuffer()
                        {
                            target = new Target()
                            {
                                isFriendly = isFriendly(TargetablesArray[j].TargetType, TargetType.Human),
                                CameraAngle = Output
                            }
                        }); ;
                        
                    
                    }
                }


            }
        }
        public bool isFriendly(TargetType targetType, TargetType Looker)
        {
            bool answer = false;
            switch (targetType)
            {
                case TargetType.Human:
                    switch (Looker)
                    {
                        case TargetType.Human:
                            answer = true;
                            break;
                        case TargetType.Angel:
                            answer = true;
                            break;
                        case TargetType.Daemon:
                            answer = false;
                            break;
                    }

                    break;
                case TargetType.Angel:
                    break;
                case TargetType.Daemon:
                    break;
            }
            return answer;
        }
    }

    public struct LookAtTarget : IComponentData {
        public int BufferIndex;
    }

}
