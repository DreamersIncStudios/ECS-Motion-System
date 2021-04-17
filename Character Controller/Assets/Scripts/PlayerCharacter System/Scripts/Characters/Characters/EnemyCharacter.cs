using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace Stats
{
    public class EnemyCharacter : BaseCharacter,IConvertGameObjectToEntity
    {
        public uint EXPgained;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            selfEntityRef = entity;
            var data = new EnemyStats() { MaxHealth = MaxHealth, MaxMana = MaxMana, CurHealth = CurHealth, CurMana = CurMana };
            dstManager.AddComponentData(entity, data);
            dstManager.AddComponent<Unity.Transforms.CopyTransformFromGameObject>(entity);
            StatusBuffers = dstManager.AddBuffer<EffectStatusBuffer>(entity);
            StatUpdate();

        }
    }
}
