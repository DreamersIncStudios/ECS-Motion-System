using Unity.Entities;

namespace IAUS.ECS.Component
{
    public interface IAttackTag : IComponentData {
        public bool Complete { get; }
        public uint NumOfAttacks { get; set; }
    }
    public struct MeleeAttackTag : IAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
    public struct MeleeMagicAttackTag : IAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
    public struct RangeAttackTag : IAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
    public struct RangeMagicAttackTag : IAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
}
