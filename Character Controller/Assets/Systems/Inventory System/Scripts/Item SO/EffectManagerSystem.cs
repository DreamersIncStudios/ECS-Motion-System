using Dreamers.InventorySystem;
using Stats.Entities;
using Unity.Entities;

// ReSharper disable Unity.BurstLoadingManagedType

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class EffectGroupSystem : ComponentSystemGroup
{
    public EffectGroupSystem()
    {
        RateManager = new RateUtils.VariableRateManager(1000);

    }
}
[UpdateInGroup(typeof(EffectGroupSystem))]
public partial class EffectManagerSystem: SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().WithChangeFilter<BaseCharacterComponent>()
            .ForEach((Entity entity, BaseCharacterComponent stats, CharacterInventory inventory) =>
        {
            if (stats.CurMana != 0) return;
            foreach (var weapon in inventory.Equipment.EquippedWeapons.Values)
            {
                weapon.activeSpell.Deactivate(weapon,stats, entity);
            }

        }).Run();
    }
}
