using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Pawn;
    public List<Transform> SpawnPoints;
#pragma warning disable CS0649 // Field 'Spawner.SpawnCount' is never assigned to, and will always have its default value 0
    int SpawnCount;
#pragma warning restore CS0649 // Field 'Spawner.SpawnCount' is never assigned to, and will always have its default value 0
    public int MaxSpawnCount;
    public bool SpawnEnemies { get { return SpawnCount < MaxSpawnCount; } }

    public int EnemyStartLevel;
#pragma warning disable CS0414 // The field 'Spawner.Level' is assigned but its value is never used
    int Level=1;
#pragma warning restore CS0414 // The field 'Spawner.Level' is assigned but its value is never used

    public Transform Sphere;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
