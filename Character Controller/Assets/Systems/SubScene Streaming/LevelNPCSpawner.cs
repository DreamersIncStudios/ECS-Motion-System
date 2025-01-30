using DreamersInc.BestiarySystem;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Utilities;
using System.Threading.Tasks;

[RequireComponent(typeof(BoxCollider))]
public class LevelNPCSpawner : MonoBehaviour
{
    public Vector3 SpawnPosition => spawnPoint.position;
    [SerializeField] Transform spawnPoint;
    List<GameObject> npcsInScene;
    private BoxCollider bounds;
    [SerializeField] private float YLimits =1.0f;
    private List<Entity> npcEntitiesInScene;

    [SerializeField] private UnityEngine.VFX.VisualEffect portal;
    [SerializeField] private List<Transform> portalSpawnPoints;
    
    // Start is called before the first frame update
     void Start()
    {
        bounds = GetComponent<BoxCollider>();
        bounds.isTrigger = true;
        npcsInScene = new List<GameObject>();
        npcEntitiesInScene = new List<Entity>();
 

        Invoke(nameof(Spawn), 10);

    }

    async void  SpawnWolvesViaPortal(int index)
    {
        var vfx  = Instantiate(portal, portalSpawnPoints[index]);
        await Task.Delay(2000);
        for (var i = 0; i < 5; i++)
        {
            BestiaryDB.SpawnNPC(5, portalSpawnPoints[0].position, out var go, out var entity);
            npcsInScene.Add(go);
            npcEntitiesInScene.Add(entity);
            await Task.Delay(150);
        }
        await Task.Delay(2755);
        vfx.Stop();
        await Task.Delay(3375);
        Destroy(vfx);
    }

    private async void Spawn()
    {
        var positions = new List<Vector3>();
        while (positions.Count < 20)
        {
            if (!GlobalFunctions.RandomPoint(spawnPoint.position, 125, out Vector3 pos)) continue;
            if (pos.y < YLimits)
            {
                positions.Add(pos);
            }

        }

        // for (int k = 0; k < 10; k++)
        // {
        //     for (var i = 0; i < 15; i++)
        //     {
        //         BestiaryDB.SpawnNPC(0, positions[i], out var go, out var entity);
        //         npcsInScene.Add(go);
        //         npcEntitiesInScene.Add(entity);
        //         await Task.Delay(10);
        //     }
        // }


        SpawnWolvesViaPortal(0);
    }

    public void DestroyAll()
    {    
        foreach (Entity entity in npcEntitiesInScene)
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
        }

        foreach (GameObject go in npcsInScene)
        {
            Destroy(go);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            DestroyAll();
    }

    private void OnDestroy()
    {
        if (ApplicationIsAboutToExitPlayMode) return;
        foreach (Entity entity in npcEntitiesInScene)
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
        }

        foreach (GameObject go in npcsInScene)
        {
            Destroy(go);
        }
    }

    private bool ApplicationIsAboutToExitPlayMode
    {
        get
        {
#if UNITY_EDITOR
            return EditorApplication.isPlayingOrWillChangePlaymode;
#else
             return false;
#endif
        }
    }

}