using Unity.Entities;

namespace Dreamers.MotionSystem
{
    public struct Interactable : IComponentData
    {
        public Interaction Interaction;

        public Interactable(Interaction interaction)
        {
            Interaction = interaction;
        }
    }

    public enum Interaction
    {
        None,
        Pickup,
        Drop,
        Use,
        Interact
    }

    public struct CarriedTag : IComponentData
    {
        public Entity ParentToEntity;

        public CarriedTag(Entity entity)
        {
            ParentToEntity = entity;
        }
    }

    public struct CarryingTag : IComponentData
    {
    }


}