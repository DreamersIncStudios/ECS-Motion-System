using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using IAUS.ECS.Component;
using DreamersInc.ComboSystem.NPC;
using DreamersInc.ComboSystem;
public class NPCAttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref MeleeAttackTag tag, Command handler, NPCComboComponent AttackInfo, Animator anim) => {

            float picked = Random.Range(0, AttackInfo.combo.MaxProb);
            foreach (var combo in AttackInfo.combo.comboInfos) {
                if (combo.Picked(picked)) {
                    // add to input queue 
                    handler.InputQueue.Enqueue(AttackInfo.combo.GetAnimationTrigger(handler.StateInfo, combo));
                    PostUpdateCommands.RemoveComponent<MeleeAttackTag>(entity);
                }
            }
        });

        Entities.ForEach((Entity entity, ref MeleeMagicAttackTag tag, Command handler, NPCComboComponent AttackInfo, Animator anim) => {

            float picked = Random.Range(0, AttackInfo.combo.MaxProb);
            foreach (var combo in AttackInfo.combo.comboInfos)
            {
                if (combo.Picked(picked))
                {
                    // add to input queue 
                    handler.InputQueue.Enqueue(AttackInfo.combo.GetAnimationTrigger(handler.StateInfo, combo));
                    PostUpdateCommands.RemoveComponent<MeleeMagicAttackTag>(entity);
                }
            }
        });

        Entities.ForEach((Entity entity, ref RangeAttackTag tag, Command handler, NPCComboComponent AttackInfo, Animator anim) => {

            float picked = Random.Range(0, AttackInfo.combo.MaxProb);
            foreach (var combo in AttackInfo.combo.comboInfos)
            {
                if (combo.Picked(picked))
                {
                    // add to input queue 
                    handler.InputQueue.Enqueue(AttackInfo.combo.GetAnimationTrigger(handler.StateInfo, combo));
                    PostUpdateCommands.RemoveComponent<RangeAttackTag>(entity);
                }
            }
        });
        Entities.ForEach((Entity entity, ref RangeMagicAttackTag tag, Command handler, NPCComboComponent AttackInfo, Animator anim) => {

            float picked = Random.Range(0, AttackInfo.combo.MaxProb);
            foreach (var combo in AttackInfo.combo.comboInfos)
            {
                if (combo.Picked(picked))
                {
                    // add to input queue 
                    handler.InputQueue.Enqueue(AttackInfo.combo.GetAnimationTrigger(handler.StateInfo, combo));
                    PostUpdateCommands.RemoveComponent<RangeMagicAttackTag>(entity);
                }
            }
        });

    }
}
