using System;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.DamageSystem
{

    public struct IncrementalEffect : IBufferElementData
    {
        private float duration;
        public readonly float EffectValue;
        public ElementName Element;
        public StatusEffects Effect;
        public float Repetitions;

        public IncrementalEffect(float effectValue, float duration, ElementName element)
        {
            EffectValue = effectValue;
            this.duration = duration;
            Element = element;
            Effect = element switch
            {
                ElementName.None => StatusEffects.None,
                ElementName.Fire => StatusEffects.Burnt,
                ElementName.Water => StatusEffects.None,
                ElementName.Earth => StatusEffects.None,
                ElementName.Wind => StatusEffects.None,
                ElementName.Ice => StatusEffects.Frostbite,
                ElementName.Holy => StatusEffects.Blessed,
                ElementName.Dark => StatusEffects.Cursed,
                _ => throw new ArgumentOutOfRangeException(nameof(element), element, null)
            };

            Effect = StatusEffects.None;
            Repetitions = 0;
        }

        public bool UpdateTime(float DeltaTime)
        {
            duration -= DeltaTime;
            return duration <= 0;
        }
    }


    public partial class IncrementalChangeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().ForEach((Entity entity, DynamicBuffer<IncrementalEffect> effects,
                     BaseCharacterComponent stat) =>
                {

                    for (var i = 0; i < effects.Length; i++ )
                    {
                        if (effects[i].UpdateTime(SystemAPI.Time.DeltaTime))
                        {
                            effects.RemoveAt(i);
                        }

                        switch (effects[i].Effect)
                        {
                            case StatusEffects.Burnt:
                            case StatusEffects.Frostbite:
                                IncrementalDamage(entity, (int)effects[i].EffectValue);
                                break;
                            case StatusEffects.Frozen:
                                break;
                            case StatusEffects.Confused:
                                break;
                            case StatusEffects.Frenzy:
                                break;
                            case StatusEffects.Electrocuted:
                                IncrementalDamage(entity, (int)effects[i].EffectValue);
Debug.Log("Shock interrupt");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            ).Run();
        }

        void IncrementalDamage(Entity entity, int damage)
        {
            EntityManager.AddComponentData(entity, new AdjustHealth() { Value = damage });
        }
    }
}