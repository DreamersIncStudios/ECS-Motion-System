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
        private Stat meleeDefense;
        private Stat magicDefense;
        public Collider GetCollider => GetComponent<Collider>();
        private float MagicDef => 1.0f / (float)(1.0f + ((float)magicDefense.AdjustBaseValue / 100.0f));
        private float MeleeDef => 1.0f / (float)(1.0f + ((float)meleeDefense.AdjustBaseValue / 100.0f));

        /// <summary>
        /// Reacts to a hit received by the damageable object.
        /// </summary>
        /// <param name="impact">The impact force of the hit.</param>
        /// <param name="hitPosition">The test vector used for hit contact point.</param>
        /// <param name="forward">The forward vector of the damageable object.</param>
        /// <param name="typeOf">The type of damage inflicted (default: TypeOfDamage.Melee).</param>
        /// <param name="element">The element of the damage inflicted (default: Element.None).</param>
        public void ReactToHit(float impact, Vector3 hitPosition, Vector3 forward, TypeOfDamage typeOf = TypeOfDamage.Melee, Element element = Element.None)
        {
            //Todo Figure out element resistances, conditional mods, and possible affinity 
            var defense = typeOf switch
            {
                TypeOfDamage.MagicAoE => MagicDef,
                TypeOfDamage.Melee => MeleeDef,
                _ => MeleeDef,
            };

            ReactToContact reactTo = new()
            {
                ForwardVector = forward,
                positionVector = this.transform.position,
                RightVector = transform.right,
                HitIntensity = 4.45f,//Todo balance the mathe Mathf.FloorToInt(impact / (defense * 10.0f) * Random.Range(.92f, 1.08f)),
                HitContactPoint = hitPosition
            };
            if (!World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<ReactToContact>(SelfEntityRef))
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(SelfEntityRef, reactTo);
        }

        /// <summary>
        /// Takes a specific amount of damage based on the type of damage and element.
        /// </summary>
        /// <param name="Amount">The amount of damage to be taken.</param>
        /// <param name="typeOf">The type of damage.</param>
        /// <param name="element">The element of damage.</param>
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
            magicDefense = character.GetStat((int)StatName.Magic_Defense);
            meleeDefense = character.GetStat((int)StatName.Melee_Defense);

        }

    }
}
