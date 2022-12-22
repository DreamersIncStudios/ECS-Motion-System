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
            AddComponent(new SpawnerComponent()
            {
                npcPrefab = GetEntity(authoring.npcPrefab)
            });
        }
    }

}
public struct SpawnerComponent : IComponentData {
    public Entity npcPrefab;
}
