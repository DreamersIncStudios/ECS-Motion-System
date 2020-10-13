using Unity.Entities;

namespace DreamersStudio.TargetingSystem
{
    [GenerateAuthoringComponent]
    public struct Targetable : IComponentData
    {
        public TargetType TargetType;
    }

    public enum TargetType {
        Angel, Daemon, Human // More Types of be added 

    }
}