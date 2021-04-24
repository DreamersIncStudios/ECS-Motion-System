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
    public class BlasterSO :WeaponSO,IEquipable ,IProjectile
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
            var ShootingData = new ShooterComponent();
            ShootingData.ProjectileGameObject = ProjectilePrefab;
            ShootingData.LastTimeShot = 0.0f;
            ShootingData.Offset = ShootLocationOffset;
            ShootingData.NormalSpeed = NormalSpeed;
            ShootingData.RoundsPerMin = RoundsPerMin;
            ShootingData.RoundsPerShot = RoundsPerShot;

            Entity point = dstManager.CreateEntity();
            var shootPoint = weaponModel;
            shootPoint.transform.parent = weaponModel.transform;
            dstManager.AddComponentObject(point, weaponModel.transform);
            dstManager.AddComponentData(point, new Translation()); // Have to add all this stuff manually too
            dstManager.AddComponentData(point, new Rotation());
            dstManager.AddComponentData(point, new LocalToWorld());
            dstManager.AddComponentData(point, new CopyTransformFromGameObject()); // Or CopyTransformToGameObject - Only if you need to sync transforms

            // - Only if you want the parent child relationship
            dstManager.AddComponentData(point, new Parent { Value = entity });
            ShootingData.ShootFromHere = point;
            dstManager.AddComponentData(entity, ShootingData);

        }

        public override void Use(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
            //throw new System.NotImplementedException();
        }

        public override void EquipItem(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
            base.EquipItem(characterInventory, IndexOf, player);
                Convert(characterInventory.self, World.DefaultGameObjectInjectionWorld.EntityManager);
            
        }

        public override void Unequip(CharacterInventory characterInventory, BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
            World.DefaultGameObjectInjectionWorld.EntityManager.RemoveComponent<ShooterComponent>(characterInventory.self);
            AddToInventory(characterInventory);
            Destroy(weaponModel);
            EquipmentUtility.ModCharacterStats(player, Modifiers, false);
            Equipment.EquippedWeapons.Remove(this.Slot);
        }

        public bool Equals(ItemBaseSO obj)
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