using Unity.Entities;

namespace IAUS.ECS.Component
{
    public struct MeleeAttackTag : IComponentData { public bool test; }
    public struct MeleeMagicAttackTag : IComponentData { }
    public struct RangeAttackTag : IComponentData { }
    public struct RangeMagicAttackTag : IComponentData { }
}
