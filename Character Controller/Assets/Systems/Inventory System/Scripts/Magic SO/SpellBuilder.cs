using Dreamers.InventorySystem;
using DreamersInc.DamageSystem;
using DreamersInc.DamageSystem.Interfaces;
using DreamersInc.InventorySystem;
using MotionSystem;
using Stats.Entities;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DreamersInc.MagicSystem
{
    public class SpellBuilder
    {
        private readonly Entity entity;
        private EntityManager manager;
        private GameObject model;

        public SpellBuilder(string spellName)
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var baseEntityArch = manager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld)
            );
            var baseDataEntity = manager.CreateEntity(baseEntityArch);
            manager.SetName(baseDataEntity, spellName != string.Empty ? spellName : "Spell Data");
            manager.SetComponentData(baseDataEntity, new LocalTransform() { Scale = 1 });
            entity = baseDataEntity;
        }

        public SpellBuilder WithVFX(GameObject vfxPrefab, Transform spawn, int delay = 5)
        {
            model = Object.Instantiate(vfxPrefab);
           model.transform.position = spawn.position+spawn.forward*.5f;
           model.transform.rotation = spawn.rotation;
           model.GetComponent<DestroyAfterSecond>().DestroyAfterSeconds(delay);
        
            //model.tag = "VFX";
            if (entity == Entity.Null) return this;
            manager.AddComponentObject(entity, model.transform);
            manager.SetComponentData(entity, new LocalTransform()
            {
                Position = spawn.position+spawn.forward*.5f,
                Rotation = spawn.rotation,
                Scale = 1
            });


      
            return this;
        }

        public SpellBuilder WithTrajectory(float  speed)
        {
            if (entity == Entity.Null) return this;

            manager.AddComponentData(entity, new ParticleMovement(speed));
            return this;
        }

        public SpellBuilder WithDamage(BaseCharacterComponent caster)
        {
            var damage = model.GetComponent<IDamageDealer>();
            damage.SetStatData(caster,TypeOfDamage.Projectile);
            return this;
        }

        public SpellBuilder WithAreaOfEffect(Vector3 Position)
        {
            return this;
        }

        public Entity Build()
        {
            return entity;
        }
    }
}