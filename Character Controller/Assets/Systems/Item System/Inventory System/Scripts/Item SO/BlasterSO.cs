using System.Collections;
using System.Linq;
using UnityEngine;
using Stats;
using Unity.Entities;
using Dreamers.InventorySystem.Interfaces;

namespace Dreamers.InventorySystem.SO
{
    public class BlasterSO :WeaponSO, IProjectile
    {


        #region Variable

        [SerializeField] int roundsPerMin;

        public int RoundsPerMin { get { return roundsPerMin; } }
        [SerializeField] int roundsPerShot;

        public int RoundsPerShot { get { return roundsPerShot; } }
        [SerializeField] float normalSpeed;

        public float NormalSpeed { get { return normalSpeed; } }

        [SerializeField] float offset;
        public float ShootLocationOffset { get { return offset; }  }
   
#endregion


        public override void Convert(Entity entity, EntityManager dstManager)
        {
            var ShootingData = new ShooterComponent
            {
                
                LastTimeShot = 0.0f,
                Offset = ShootLocationOffset,
                NormalSpeed = NormalSpeed,
                RateOfFire = RoundsPerMin,
                RoundsPerShot = RoundsPerShot
            };


            dstManager.AddComponentData(entity, ShootingData);

        }

        public override void Use(CharacterInventory characterInventory, BaseCharacter player)
        {
            //throw new System.NotImplementedException();
        }

        public  bool EquipItem(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
            bool temp = base.EquipItem(characterInventory,  player);
            if(temp)
                Convert(characterInventory.self, World.DefaultGameObjectInjectionWorld.EntityManager);
            return temp;
        }


        public new bool Equals(ItemBaseSO obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj.Type != Type)
                return false;

            // TODO: write your implementation of Equals() here

           BlasterSO Blaster = (BlasterSO)obj;

            return ItemID == Blaster.ItemID && ItemName == Blaster.ItemName && Value == Blaster.Value && Modifiers.SequenceEqual(Blaster.Modifiers) &&
                Exprience == Blaster.Exprience && LevelRqd == Blaster.LevelRqd;
        }


    }

}