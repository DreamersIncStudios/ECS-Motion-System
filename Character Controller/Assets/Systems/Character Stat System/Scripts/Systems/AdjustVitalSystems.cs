
using Unity.Entities;
using DreamersInc.DamageSystem.Interfaces;
using Stats.Entities;

// ReSharper disable Unity.BurstLoadingManagedType
namespace DreamersInc.DamageSystem
{
    public partial class AdjustVitalSystems : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity,BaseCharacterComponent character, in AdjustHealth mod) => {
                character.AdjustHealth(mod.Value);
                
                if (character.CurHealth <= 0)
                {
                    EntityManager.AddComponent<EntityHasDiedTag>(entity);
                    EntityManager.AddComponentData(mod.DamageDealtByEntity, new AddXP(character.ExpGiven(mod.Level)));
                }
                EntityManager.RemoveComponent<AdjustHealth>(entity);

            }).Run();


            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity,BaseCharacterComponent mana, in AdjustMana mod) => {
                mana.AdjustMana(mod.Value);

                EntityManager.RemoveComponent<AdjustMana>(entity);
            }).Run();

        }
    }
}