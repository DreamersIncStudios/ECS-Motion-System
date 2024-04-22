using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Stats.Entities;

namespace DreamersInc.DamageSystem.Interfaces
{
    public interface IDamageable
    {
        Entity SelfEntityRef { get; }
        public Collider GetCollider { get; }
        void TakeDamage(int amount, TypeOfDamage typeOf, Element element);
        void ReactToHit(float impact, Vector3 hitPosition, Vector3 forward , TypeOfDamage typeOf = TypeOfDamage.Melee , Element element = Element.None);

        void SetData(Entity entity, BaseCharacterComponent character);
    }


    public enum TypeOfDamage {Melee, MagicAoE, Projectile, Magic, Recovery}
    public enum Element { None, Fire, Water, Earth, Wind, Ice, Holy, Dark}
    public struct AdjustHealth : IComponentData {
        public int Value;
    }
    public struct AdjustMana : IComponentData
    {
        public int Value;   
    }

    public struct EntityHasDiedTag: IComponentData { public int Value; }

    public struct Player : IComponentData { }
    public struct Enemy : IComponentData { }
    public struct NPC : IComponentData { }


}