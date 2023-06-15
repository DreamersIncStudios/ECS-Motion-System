using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject npcPrefab;

    class Baking : Baker<Spawner>
    {
        public override void Bake(Spawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new SpawnerComponent()
            {
                npcPrefab = GetEntity(authoring.npcPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }

}
public struct SpawnerComponent : IComponentData {
    public Entity npcPrefab;
}
