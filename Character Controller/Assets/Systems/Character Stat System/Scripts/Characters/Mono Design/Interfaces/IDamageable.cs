using System.Collections;
using System.Collections.Generic;
using Stats;
using UnityEngine;
using Unity.Entities;
using Stats.Entities;

namespace DreamersInc.DamageSystem.Interfaces
{
    public interface IDamageable
    {
        Entity SelfEntityRef { get; }
        public Collider GetCollider { get; }
        void TakeDamage(int amount, TypeOfDamage typeOf, ElementName elementName);
        void ReactToHit(float impact, Vector3 hitPosition, Vector3 forward , TypeOfDamage typeOf = TypeOfDamage.Melee , ElementName elementName = ElementName.None);

        void SetData(Entity entity, BaseCharacterComponent character);
    }


    public enum TypeOfDamage {Melee, MagicAoE, Projectile, Magic, Recovery}

    public struct AdjustHealth : IComponentData {
        public int Value;

        public AdjustHealth(int value)
        {
            Value = value;
        }
    }
    public struct AdjustMana : IComponentData
    {
        public int Value;

        public AdjustMana(int value)
        {
            Value = value;
        }
    }

    public struct EntityHasDiedTag: IComponentData { public int Value; }

    public struct Player : IComponentData { }
    public struct Enemy : IComponentData { }
    public struct NPC : IComponentData { }


}