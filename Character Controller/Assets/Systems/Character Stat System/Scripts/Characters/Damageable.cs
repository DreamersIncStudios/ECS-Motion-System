using DreamersInc.CombatSystem.Animation;
using DreamersInc.DamageSystem.Interfaces;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Stats
{
    [RequireComponent(typeof(Collider))]
    public class Damageable : MonoBehaviour, IDamageable
    {

        public Entity SelfEntityRef { get; private set; }
        Stat Melee_Defense;
        Stat Magic_Defense;
        public Collider GetCollider => GetComponent<Collider>();
        public float MagicDef { get { return 1.0f / (float)(1.0f + ((float)Magic_Defense.AdjustBaseValue / 100.0f)); } }
        public float MeleeDef { get { return 1.0f / (float)(1.0f + ((float)Melee_Defense.AdjustBaseValue / 100.0f)); } }

        public void ReactToHit(float impact, Vector3 Test, Vector3 Forward, TypeOfDamage typeOf = TypeOfDamage.Melee, Element element = Element.None)
        {
            //Todo Figure out element resistances, conditional mods, and possible affinity 
            float defense = typeOf switch
            {
                TypeOfDamage.MagicAoE => MagicDef,
                TypeOfDamage.Melee => MeleeDef,
                _ => MeleeDef,
            };

            ReactToContact reactTo = new()
            {
                ForwardVector = Forward,
                positionVector = this.transform.position,
                RightVector = transform.right,
                HitIntensity = 4.45f,//Todo balance the mathe Mathf.FloorToInt(impact / (defense * 10.0f) * Random.Range(.92f, 1.08f)),
                HitContactPoint = Test
            };
            if (!World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<ReactToContact>(SelfEntityRef))
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(SelfEntityRef, reactTo);
        }

        public void TakeDamage(int Amount, TypeOfDamage typeOf, Element element)
        {
            //Todo Figure out element resistances, conditional mods, and possible affinity 
            float defense = typeOf switch
            {
                TypeOfDamage.MagicAoE => MagicDef,
                TypeOfDamage.Magic=> MagicDef,
                _ => MeleeDef,
            };

            int damageToProcess = -Mathf.FloorToInt(Amount * defense * Random.Range(.92f, 1.08f));
            Debug.Log(damageToProcess + " HP of damage to target "+ this.name);
            AdjustHealth health = new() { Value = damageToProcess };
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(SelfEntityRef, health);
        }
        public void SetData(Entity entity, BaseCharacterComponent character) {
            SelfEntityRef = entity;
            Magic_Defense = character.GetStat((int)StatName.Magic_Defense);
            Melee_Defense = character.GetStat((int)StatName.Melee_Defense);

        }

    }
}
