using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Stats
{
    public class DeathSystem : ComponentSystem
    {
        //TODO update 
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, Transform anim, ref EntityHasDiedTag tag) => {
                Debug.Log("Play Death Animation");
                Object.Destroy(anim.gameObject, 4);
                EntityManager.DestroyEntity(entity);
            });
            Entities.ForEach((ref EnemyStats stats, EnemyCharacter StatsGO) => {
                StatsGO.CurHealth = stats.CurHealth;
                StatsGO.MaxHealth = stats.MaxHealth;
                StatsGO.CurMana = stats.CurMana;
                StatsGO.MaxMana = stats.MaxMana;

            });

        }
    }

    public struct EntityHasDiedTag : IComponentData { 

    }
}