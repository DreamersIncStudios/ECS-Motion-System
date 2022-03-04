using DreamersInc.DamageSystem.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace Stats
{
    public class NPCChararacter : BaseCharacter
    {

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            base.Convert(entity, dstManager, conversionSystem);
            var data = new EnemyStats() { MaxHealth = MaxHealth, MaxMana = MaxMana, CurHealth = CurHealth, CurMana = CurMana };
            dstManager.AddComponentData(entity, data);
            StatUpdate();

        }

        public override void ReactToDamage(Vector3 DirOfAttack)
        {
            throw new System.NotImplementedException();
        }

        public override void TakeDamage(int Amount, TypeOfDamage typeOf, Element element)
        {
            base.TakeDamage(Amount, typeOf, element);
            //Todo Add system so that NPC can not hit 0 HP ?????

        
        }
    }
}
