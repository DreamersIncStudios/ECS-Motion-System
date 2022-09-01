using UnityEngine;
using System.Collections;
using Unity.Entities;
using DreamersInc.DamageSystem.Interfaces;

namespace Stats
{
    [System.Serializable]
    public class PlayerCharacter : BaseCharacter

    {
        public void SetupDataEntity(Entity entity) {
           
            SelfEntityRef = entity;
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
           em.AddBuffer<EffectStatusBuffer>(entity);
            //Todo get level and stat data

            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new PlayerStatComponent { selfEntityRef = entity });
            StatUpdate();

        }


        public override void TakeDamage(int Amount, TypeOfDamage typeOf, Element element = 0)
        {
            //Todo Figure out element resistances, conditional mods, and possible affinity 
            float defense = typeOf switch
            {
                TypeOfDamage.MagicAoE => MagicDef,
                _ => MeleeDef,
            };

            int damageToProcess = -Mathf.FloorToInt(Amount * defense * Random.Range(.92f, 1.08f));
            AdjustHealth health = new AdjustHealth() { Value = damageToProcess };
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(SelfEntityRef, health);

        }

        public override void ReactToHit(float impact, Vector3 Test, Vector3 Forward, TypeOfDamage typeOf = TypeOfDamage.Melee, Element element = Element.None)
        {

        }

    }

}
