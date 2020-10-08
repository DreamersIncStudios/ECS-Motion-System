using Unity.Entities;

namespace DreamersStudio.TargetingSystem
{
    [GenerateAuthoringComponent]
    public struct TargetBuffer : IBufferElementData
    {
        public Target Target;
    }
    public struct Target {
        public bool isFriendly;

    }
}