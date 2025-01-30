using System.Collections.Generic;
using Sirenix.Utilities;
using Streaming.SceneManagement.SectionMetadata;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Entities.SystemAPI;
using Hash128 = Unity.Entities.Hash128;

public class SectionBaker : Baker<Transform> 
{
    public override void Bake(Transform authoring)
    {
        // if (!authoring.gameObject.isStatic) return;
        // // Get the layer value from the GameObject
        //
         // Attach it as a component to the ECS entity
         DependsOn(authoring.gameObject);
         var entity = GetEntity(TransformUsageFlags.None);
        AddSharedComponent(entity, new SubsceneSectionBaking()
        {
            Layer = authoring.gameObject.layer,
            SceneGuid = GetSceneGUID()
        });
        
    }
}
//[TemporaryBakingType]
public struct SubsceneSectionBaking: ISharedComponentData
    {
       public int Layer;
       public Hash128 SceneGuid;
    
    }

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class SubSceneBakingSystem : SystemBase
{
    private Dictionary<Hash128, List<SectionData>> sectionDataDictionary;

    private int GetSectionID(float3 position, int layer)
    {
        if (layer == 6) return 0;

        var quadrantCellSize = layer switch
        {
            26 => 300,
            27 => 150,
            28 => 75,
            _ => 600
        };
        var quadrantYMultiplier = layer switch
        {
            28 => 10,
            27 => 100,
            26 => 1000,
            _ => 2000
        };

        return (int)(Mathf.FloorToInt(position.x / quadrantCellSize)
                     + (quadrantYMultiplier * Mathf.Floor(position.z / quadrantCellSize)));
    }

    protected override void OnCreate()
    {
        sectionDataDictionary = new Dictionary<Hash128, List<SectionData>>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach(
            (Entity entity,  SubsceneSectionBaking sectionBaking, in LocalToWorld transform) =>
            {
                if (sectionDataDictionary.TryGetValue(sectionBaking.SceneGuid, out var sectionData))
                {
                    if (TryUpdateExistingSection(sectionData, sectionBaking, transform, entity, ecb))
                        return;

                    AddNewSection(sectionData, sectionBaking, transform, entity, ecb);
                }
                else
                {
                    AddNewSceneSection(sectionBaking, transform, entity, ecb);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
     //  CleanupBakingComponents();
    }

    private bool TryUpdateExistingSection(List<SectionData> sectionData, SubsceneSectionBaking sectionBaking,
        LocalToWorld transform, Entity entity, EntityCommandBuffer ecb)
    {
        var positionHashKey = GetSectionID(transform.Position, sectionBaking.Layer);

        foreach (var data in sectionData)
        {
            if (data.Layer != sectionBaking.Layer || data.PositionHashKey != positionHashKey) continue;
            ecb.AddSharedComponent(entity, new SceneSection
            {
                SceneGUID = sectionBaking.SceneGuid,
                Section = data.Section
            });
            return true;
        }

        return false;
    }

    private void AddNewSection(List<SectionData> sectionData, SubsceneSectionBaking sectionBaking,
        LocalToWorld transform, Entity entity, EntityCommandBuffer ecb)
    {
        var positionHashKey = GetSectionID(transform.Position, sectionBaking.Layer);
        var newSectionCount = sectionData.IsNullOrEmpty() ? 1: sectionData.Count + 1;

        sectionData.Add(new SectionData
        {
            Layer = sectionBaking.Layer,
            PositionHashKey = positionHashKey,
            Section = newSectionCount
        });

        ecb.AddSharedComponent(entity, new SceneSection
        {
            SceneGUID = sectionBaking.SceneGuid,
            Section = newSectionCount
        });
        ecb.AddComponent(entity, new Circle()
        {
            Radius = sectionBaking.Layer switch
            {
                6 => 750,
                26 => 250,
                27 => 100,
                28 => 75,
                _ => 100
            },
            Center = GetSectionCenter(positionHashKey, sectionBaking.Layer)
        });
    }

    private void AddNewSceneSection(SubsceneSectionBaking sectionBaking, LocalToWorld transform, Entity entity,
        EntityCommandBuffer ecb)
    {
        var sectionData = new List<SectionData>();
        sectionDataDictionary.Add(sectionBaking.SceneGuid, sectionData);

        AddNewSection(sectionData, sectionBaking, transform, entity, ecb);
    }

    private void CleanupBakingComponents()
    {
        var query = QueryBuilder().WithAll<SubsceneSectionBaking>().Build();
        EntityManager.RemoveComponent<SubsceneSectionBaking>(query);
    }

    private float3 GetSectionCenter(int sectionID, int layer)
    {
        if (layer == 6)
            return new float3(0, 0, 0);

        var quadrantCellSize = layer switch
        {
            26 => 300,
            27 => 150,
            28 => 75,
            _ => 600
        };
        var quadrantYMultiplier = layer switch
        {
            28 => 10,
            27 => 100,
            26 => 1000,
            _ => 2000
        };

        // Reverse the ID calculation to find the center
        var xIndex = sectionID % quadrantYMultiplier;
        var zIndex = sectionID / quadrantYMultiplier;

        // Compute center position
        var centerX = (xIndex + 0.5f) * quadrantCellSize;
        var centerZ = (zIndex + 0.5f) * quadrantCellSize;

        return new float3(centerX, 0, centerZ); // Assuming Y is 0 for the center height
    }
}
    


public struct SectionData 
{
    public int Layer;
    public int PositionHashKey;
    public int Section;
    
}
