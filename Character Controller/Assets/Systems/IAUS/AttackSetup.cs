using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using IAUS.ECS.Component;

namespace DreamersInc.ComboSystem.NPC
{
    [UpdateAfter(typeof(ComboInputSystem))]
    public class AttackSetup : ComponentSystem
    {

        protected override void OnUpdate()
        {
            BufferFromEntity<NPCAttackBuffer> bufferFromEntity = GetBufferFromEntity<NPCAttackBuffer>();

            Entities.ForEach((Entity entity,ref AttackActionTag tag,NPCComboComponent combo, Command handler) => {
                Debug.Log("run");
                DynamicBuffer<NPCAttackBuffer> buffer = bufferFromEntity[entity];
                int index = combo.combo.GetAnimationComboIndex(handler.StateInfo);
                float indexPicked = Random.Range(0, combo.combo.GetMaxProbAtCurrentState(index));
                Debug.Log(combo.combo.GetMaxProbAtCurrentState(index));

                foreach (var item in combo.combo.ComboList[index].Triggers) {
                    if (item.Picked(indexPicked)) {

                        buffer.Add(new NPCAttackBuffer()
                        {
                            Trigger = item
                        }) ;
                        
                    }
                }

                EntityManager.RemoveComponent<AttackActionTag>(entity);
            });

            Entities.ForEach((DynamicBuffer<NPCAttackBuffer> A, Command handler) => {
                if (!A.IsEmpty)
                {
                    if (A[0].Trigger.trigger)
                    {
                        //handler.InputQueue.Enqueue( A[0].Trigger);
                    }
                    else
                    {
                        A[0].Trigger.AdjustTime(Time.DeltaTime);
                    }
                }
            });

        }
    }
}