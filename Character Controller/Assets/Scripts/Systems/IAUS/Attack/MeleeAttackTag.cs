using Unity.Entities;

namespace IAUS.ECS.Component
{
    public interface iAttackTag : IComponentData {
        public bool Complete { get; }
        public uint NumOfAttacks { get; set; }
    }
    public struct MeleeAttackTag : iAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
    public struct MeleeMagicAttackTag : iAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
    public struct RangeAttackTag : iAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
    public struct RangeMagicAttackTag : iAttackTag
    {
        public bool Complete { get { return NumOfAttacks == 0; } }
        public uint NumOfAttacks { get; set; }
    }
}
