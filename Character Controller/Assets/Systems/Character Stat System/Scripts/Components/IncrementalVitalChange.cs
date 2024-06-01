using DreamersInc.DamageSystem.Interfaces;
using Stats.Entities;
using Unity.Entities;

namespace Stats
{
    [InternalBufferCapacity(5)]
    public struct IncrementalVitalChange : IBufferElementData
    {
        public readonly VitalName Vital;
        /// <summary>
        /// How much vital should change per sec
        /// </summary>
        public readonly int ChangeRate;

        public readonly int ID;
        public bool Active; //?

        public IncrementalVitalChange(VitalName vital, int changeRate, int id)
        {
            ChangeRate = changeRate;
            Vital = vital;
            Active = true;
            ID = id;
        }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class StatGroupSystem : ComponentSystemGroup
    {
        public StatGroupSystem()
        {
            RateManager = new RateUtils.VariableRateManager(1000);

        }
    }
    [UpdateInGroup(typeof(StatGroupSystem))]
    public partial class IncrementalVitalChangeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithStructuralChanges().WithAll<BaseCharacterComponent>().ForEach(
                (Entity entity, DynamicBuffer<IncrementalVitalChange> changes) =>
                {
                    foreach (var vitalChange in changes)
                    {
                        switch (vitalChange.Vital)
                        {
                            case VitalName.Health:
                                EntityManager.AddComponentData(entity, new AdjustHealth(value: vitalChange.ChangeRate));
                                break;
                            case VitalName.Mana:
                                EntityManager.AddComponentData(entity, new AdjustMana(value: vitalChange.ChangeRate));
                                break;
                        }
                    }
                    
                }).Run();
      
        }
    }
}
