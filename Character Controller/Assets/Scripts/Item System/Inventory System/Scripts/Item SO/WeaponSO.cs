using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Unity.Entities;
using System.Linq;

namespace Dreamers.InventorySystem
{
    public class WeaponSO : ItemBaseSO, IEquipable,IWeapon
    {
        #region Variables
        public new ItemType Type { get { return ItemType.Weapon; } }
        [SerializeField] Quality quality;
        public Quality Quality { get { return quality; } }

        [SerializeField] GameObject _model;
        public GameObject Model { get { return _model; } }
        [SerializeField] private bool _equipToHuman;
        public bool EquipToHuman { get { return _equipToHuman; } }
        [SerializeField] private HumanBodyBones _heldBone;
        public HumanBodyBones HeldBone { get { return _heldBone; } }
        public bool Equipped { get; private set; }
       
        [SerializeField] private HumanBodyBones _equipBone;
        public HumanBodyBones EquipBone { get { return _equipBone; } }
        [SerializeField] private List<StatModifier> _modifiers;
        public List<StatModifier> Modifiers { get { return _modifiers; } }

        [SerializeField] private uint _levelRQD;
        public uint LevelRqd { get { return _levelRQD; } }

        [SerializeField] private WeaponType _weaponType;
        public WeaponType WeaponType { get { return _weaponType; } }
        [SerializeField] private WeaponSlot slot;
        public WeaponSlot Slot { get { return slot; } }
        [SerializeField] private float maxDurable;
        public float MaxDurability { get { return maxDurable; } }
        public float CurrentDurablity { get; set; }
        [SerializeField] private bool breakable;
        public bool Breakable { get { return breakable; } }
        [SerializeField] private bool _upgradeable;
        public bool Upgradeable { get { return _upgradeable; } }

        public int SkillPoints { get; set; }
        public int Exprience { get; set; }
        [SerializeField] Vector3 _sheathedPos;
        public Vector3 SheathedPos { get { return _sheathedPos; } }


        [SerializeField] Vector3 _heldPos;
        public Vector3 HeldPos { get { return _heldPos; } }

        [SerializeField] Vector3 _sheathedRot;
        public Vector3 SheathedRot { get { return _sheathedRot; } }


        [SerializeField] Vector3 _heldRot;
        public Vector3 HeldRot { get { return _heldRot; } }
        #endregion


        public GameObject weaponModel { get; set; }

        public void Equip(BaseCharacter player)
        {
            if (player.Level >= LevelRqd)
            {
                if (Model != null)
                {
                    weaponModel = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = player.GetComponent<Animator>().GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            weaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                        weaponModel.transform.SetParent(player.transform);
                    }
                    weaponModel.transform.localPosition = SheathedPos;
                    weaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);

                }
            }
        }

        public override void EquipItem(CharacterInventory characterInventory, int IndexOf,BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
            if (Equipment.EquippedWeapons.TryGetValue(this.Slot, out WeaponSO value))
            {
                Equipment.EquippedWeapons[this.Slot].Unequip(characterInventory, player);
            }
            Equipment.EquippedWeapons[this.Slot] = this;

            if (player.Level >= LevelRqd)
            {
                if (Model != null)
                {
                 weaponModel = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = player.GetComponent<Animator>().GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            weaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                     weaponModel.transform.SetParent(player.transform);

                    }
                    weaponModel.transform.localPosition = SheathedPos;
                    weaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);

                }
                EquipmentUtility.ModCharacterStats(player,Modifiers, true);
                Equipped = true;
                player.StatUpdate();


                RemoveFromInventory(characterInventory, IndexOf);

            }
            else { Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level); }

        }

        public override void Unequip(CharacterInventory characterInventory, BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;

            EquipmentUtility.ModCharacterStats(player,Modifiers, false);
            AddToInventory(characterInventory);
            Equipment.EquippedWeapons.Remove(this.Slot);
            Destroy(weaponModel);

        }
        public override void Convert(Entity entity, EntityManager dstManager)
        { }

        public override void Use(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
            throw new System.NotImplementedException();
        }

        public void DrawWeapon(Animator anim) {
            weaponModel.transform.SetParent(anim.GetBoneTransform(HeldBone));
            weaponModel.transform.localPosition = HeldPos;
            weaponModel.transform.localRotation = Quaternion.Euler(HeldRot);

        }
        public void StoreWeapon(Animator anim) {
            weaponModel.transform.parent = anim.GetBoneTransform(EquipBone);
            weaponModel.transform.localPosition = SheathedPos;
            weaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
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

            WeaponSO Armor = (WeaponSO)obj;

            return ItemID == Armor.ItemID && ItemName == Armor.ItemName && Value == Armor.Value && Modifiers.SequenceEqual(Armor.Modifiers) &&
                Exprience == Armor.Exprience && LevelRqd == Armor.LevelRqd;
        }


    }


}