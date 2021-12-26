using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Unity.Entities;
using Unity.Transforms;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;

namespace Dreamers.InventorySystem
{
    public class BlasterSO :WeaponSO, IProjectile
    {


        #region Variable
        public GameObject ProjectilePrefab; //Move to SO later
        public GameObject ShootPoint; // may have to make this in code?????

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
                ProjectileGameObject = ProjectilePrefab,
                LastTimeShot = 0.0f,
                Offset = ShootLocationOffset,
                NormalSpeed = NormalSpeed,
                RoundsPerMin = RoundsPerMin,
                RoundsPerShot = RoundsPerShot
            };

            Entity point = dstManager.CreateEntity();
            var shootPoint = WeaponModel;
            shootPoint.transform.parent = WeaponModel.transform;
            dstManager.AddComponentObject(point, WeaponModel.transform);
            dstManager.AddComponentData(point, new Translation()); // Have to add all this stuff manually too
            dstManager.AddComponentData(point, new Rotation());
            dstManager.AddComponentData(point, new LocalToWorld());
            dstManager.AddComponentData(point, new CopyTransformFromGameObject()); // Or CopyTransformToGameObject - Only if you need to sync transforms

            // - Only if you want the parent child relationship
            dstManager.AddComponentData(point, new Parent { Value = entity });
            ShootingData.ShootFromHere = point;
            dstManager.AddComponentData(entity, ShootingData);

        }

        public override void Use(CharacterInventory characterInventory, BaseCharacter player)
        {
            //throw new System.NotImplementedException();
        }

#pragma warning disable CS0109 // The member 'BlasterSO.EquipItem(CharacterInventory, int, BaseCharacter)' does not hide an accessible member. The new keyword is not required.
        public new bool EquipItem(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
#pragma warning restore CS0109 // The member 'BlasterSO.EquipItem(CharacterInventory, int, BaseCharacter)' does not hide an accessible member. The new keyword is not required.
        {
            bool temp = base.EquipItem(characterInventory,  player);
            if(temp)
                Convert(characterInventory.self, World.DefaultGameObjectInjectionWorld.EntityManager);
            return temp;
        }


#pragma warning disable CS0108 // 'BlasterSO.Equals(ItemBaseSO)' hides inherited member 'WeaponSO.Equals(ItemBaseSO)'. Use the new keyword if hiding was intended.
        public bool Equals(ItemBaseSO obj)
#pragma warning restore CS0108 // 'BlasterSO.Equals(ItemBaseSO)' hides inherited member 'WeaponSO.Equals(ItemBaseSO)'. Use the new keyword if hiding was intended.
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

    public class Projectiles : ScriptableObject
    {
        public GameObject GO;
    }
}