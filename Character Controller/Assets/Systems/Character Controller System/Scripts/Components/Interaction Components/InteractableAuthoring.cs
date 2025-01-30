using Dreamers.MotionSystem;
using Unity.Entities;
using UnityEngine;

public class InteractableAuthoring : MonoBehaviour
{
    [SerializeField] Interaction interaction;
    private class InteractionBaker : Baker<InteractableAuthoring>
    {
        public override void Bake(InteractableAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Interactable(authoring.interaction));
        }
    }
}
