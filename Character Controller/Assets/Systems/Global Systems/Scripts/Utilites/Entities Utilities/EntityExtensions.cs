using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using System.Reflection;

namespace DreamersInc.EntityUtilities
{

    public static class EntityExtensions
    {
        public static void RemoveAllComponents(EntityManager entityManager, Entity entity)
        {
            // Retrieve the type of the EntityManager
            var entityManagerType = typeof(EntityManager);

            // Get all method info using reflection for "RemoveComponent" methods
            var methodInfo = entityManagerType.GetMethod("RemoveComponent", new[] { typeof(Entity) });

            // Get all components for the entity
            using (var componentTypes = entityManager.GetComponentTypes(entity, Allocator.Temp))
            {
                foreach (var componentType in componentTypes)
                {
                    // Call RemoveComponent dynamically using reflection
                    methodInfo.MakeGenericMethod(componentType.GetManagedType()).Invoke(entityManager, new object[] { entity });
                }
            }
        }
        public static void RemoveAllComponents(this EntityCommandBuffer ecb, Entity entity, EntityManager entityManager)
        {
            // Get all components for the entity
            using (var componentTypes = entityManager.GetComponentTypes(entity, Allocator.Temp))
            {
                // Iterate over each component type and schedule a removal command
                foreach (var componentType in componentTypes)
                {
                    // Dynamically remove each component type using generic method invocation
                    var methodInfo = typeof(EntityCommandBuffer).GetMethod("RemoveComponent", new[] { typeof(Entity) });
                    methodInfo.MakeGenericMethod(componentType.GetManagedType()).Invoke(ecb, new object[] { entity });
                }
            }
        }
    }
}