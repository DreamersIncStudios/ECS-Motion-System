using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using DreamersInc.DamageSystem.Interfaces;
namespace Stats
{
    public class EnemyCharacter : BaseCharacter
    {
        public uint EXPgained;
        public override void  Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            base.Convert(entity, dstManager, conversionSystem);
            var data = new EnemyStats() { MaxHealth = MaxHealth, MaxMana = MaxMana, CurHealth = CurHealth, CurMana = CurMana };
            dstManager.AddComponentData(entity, data);
            StatUpdate();

        }
        public override void TakeDamage(int Amount, TypeOfDamage typeOf, Element element)
        {
            //Todo Figure out element resistances, conditional mods, and possible affinity 
            float defense = typeOf switch { 
                TypeOfDamage.MagicAoE => MagicDef,
                _ => MeleeDef,
            };

            int damageToProcess = -Mathf.FloorToInt(Amount * defense * Random.Range(.92f, 1.08f));
            Debug.Log(damageToProcess + " HP of damage to target "+ Name);
            AdjustHealth health = new AdjustHealth() { Value = damageToProcess };
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(SelfEntityRef,health);
        }
    }
}
