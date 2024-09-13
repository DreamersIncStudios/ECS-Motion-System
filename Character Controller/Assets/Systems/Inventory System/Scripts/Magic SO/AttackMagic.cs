using Dreamers.InventorySystem.AbilitySystem.Interfaces;
using Stats;
using Stats.Entities;
using DreamersInc.DamageSystem.Interfaces;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
namespace Dreamers.InventorySystem.AbilitySystem
{
    public class AttackMagic : AbilitySO, IAttackAbility
    {
        public uint DamageAmount => damageAmount;
        [SerializeField] private uint damageAmount;
        public uint ManaCost => manaCost;
        [SerializeField] uint manaCost;
        public GameObject VFX => vFX;
        [SerializeField] private GameObject vFX;
        public Vector3 Offset => offset;

        [SerializeField] private Vector2 offset;

        [SerializeField] private Vector3 size;


        public override void EquipAbility(Entity casterEntity)
        {
            base.EquipAbility(casterEntity);
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var stat = em.GetComponentData<BaseCharacterComponent>(casterEntity);
            damageAmount = (uint)stat.GetStat((int)StatName.MagicOffence).AdjustBaseValue * 10;
        }
        public override void Activate(Entity casterEntity)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var statData = em.GetComponentData<BaseCharacterComponent>(casterEntity);
           var transform = em.GetComponentData<LocalTransform>(casterEntity);
           if (statData.CurMana < ManaCost) return;
           Debug.Log($"Casting  {Name} for {damageAmount}. It cost {ManaCost} mana to cast");
            statData.AdjustMana(-(int)ManaCost);
            if (!VFX) return;
            var vfxGO = Instantiate(VFX, transform.Position + transform.Forward() * Offset.x+ transform.Up()*Offset.y,
                transform.Rotation);
            if(vfxGO.GetComponentInChildren<ParticleDamage>())
                vfxGO.GetComponentInChildren<ParticleDamage>().SetStatData(statData,TypeOfDamage.Magic);
        }

        public void DisplayInfo(Entity character)
        {
            throw new System.NotImplementedException();
        }
    }
}