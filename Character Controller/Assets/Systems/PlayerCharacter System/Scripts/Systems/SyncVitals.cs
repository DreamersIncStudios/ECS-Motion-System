using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Stats
{
    public class SyncVitals : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref PlayerStatComponent stat, PlayerCharacter StatGO) => {
                StatGO.CurHealth = stat.CurHealth;
                StatGO.CurMana = stat.CurMana;
            
            });

            Entities.ForEach((ref EnemyStats stat, EnemyCharacter StatGO) => {
                StatGO.CurHealth = stat.CurHealth;
                StatGO.CurMana = stat.CurMana;

            });
        }
    }
}