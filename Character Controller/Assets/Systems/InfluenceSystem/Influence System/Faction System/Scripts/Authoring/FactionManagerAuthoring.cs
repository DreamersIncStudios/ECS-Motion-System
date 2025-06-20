using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.FactionSystem.Authoring
{


    public class FactionManagerAuthoring : MonoBehaviour
    {
        [SerializeField] private List<FactionData> factionsList;
        [SerializeField] bool allowFriendlyFire = false;
         private class Baker : Baker<FactionManagerAuthoring>
        {
            public override void Bake(FactionManagerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FactionSingleton(authoring.allowFriendlyFire));
                
                var buffer = AddBuffer<Factions>(entity);
                foreach (var faction in authoring.factionsList)
                {
                    buffer.Add(new Factions(faction));
                }
            }
        }
        
    }

    public struct FactionSingleton : IComponentData
    {
        public bool AllowFriendlyFire;

        public FactionSingleton(bool authoringAllowFriendlyFire)
        {
         AllowFriendlyFire = authoringAllowFriendlyFire;
        }
    }
}
