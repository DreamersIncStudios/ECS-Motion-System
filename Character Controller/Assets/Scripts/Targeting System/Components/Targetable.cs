using Unity.Entities;

namespace DreamersStudio.TargetingSystem
{
    [GenerateAuthoringComponent]
    public struct Targetable : IComponentData
    {
        public TargetType TargetType;
    }

    public enum TargetType {
        Ally, Enemy // More Types of be added 

    }
}