using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

namespace DreamersStudio.TargetingSystem
{
    public class TargetingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    public struct GetTargetsList : IJobChunk
    {
        public ArchetypeChunkBufferType<TargetBuffer> BufferChunk;
        [ReadOnly] public ArchetypeChunkComponentType<LocalToWorld> PositionChunk;
        [ReadOnly] public NativeArray<Targetable> TargetablesArray;
        [ReadOnly] public NativeArray<LocalToWorld> TargetPositions;
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
                        Target.Add(new TargetBuffer()
                        {
                            Target = new Target()
                            {
                                isFriendly = false
                            }
                        });
                        
                    
                    }
                }


            }
        }
    }
}
