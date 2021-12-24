using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using Unity.Entities;
using System.Linq;
using DreamersInc.MagicSkill;



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


        public GameObject WeaponModel { get; set; }

        public void Equip(BaseCharacter player)
        {
            if (player.Level >= LevelRqd)
            {
                if (Model != null)
                {
                    WeaponModel = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = player.GetComponent<Animator>().GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            WeaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                        WeaponModel.transform.SetParent(player.transform);
                    }
                    WeaponModel.transform.localPosition = SheathedPos;
                    WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
                }
            }
        }
        //TODO Should this be a bool instead of Void

        public bool EquipItem(CharacterInventory characterInventory, BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
            if (player.Level >= LevelRqd)
            {
                if (Equipment.EquippedWeapons.TryGetValue(this.Slot, out _))
                {
                    Equipment.EquippedWeapons[this.Slot].Unequip(characterInventory, player);
                }
                Equipment.EquippedWeapons[this.Slot] = this;


                if (Model != null)
                {
                    WeaponModel = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = player.GetComponent<Animator>().GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            WeaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                        WeaponModel.transform.SetParent(player.transform);

                    }
                    WeaponModel.transform.localPosition = SheathedPos;
                    WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);

                }
                EquipmentUtility.ModCharacterStats(player, Modifiers, true);
                characterInventory.Inventory.RemoveFromInventory(this);

                
                player.StatUpdate();
                return Equipped = true; ;
            }
            else { Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level); 
                return Equipped = false;
            }

        }

        public bool Unequip(CharacterInventory characterInventory, BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
            characterInventory.Inventory.AddToInventory(this);
            Destroy(WeaponModel);

            EquipmentUtility.ModCharacterStats(player,Modifiers, false);
            Equipment.EquippedWeapons.Remove(this.Slot);
            Equipped = false;
            return true; ;
        }
        public override void Convert(Entity entity, EntityManager dstManager)
        { 
            //TODO Implement Convert at top level
        }

        public override void Use(CharacterInventory characterInventory, BaseCharacter player)
        {
            throw new System.NotImplementedException();
        }

        public void DrawWeapon(Animator anim) {
            WeaponModel.transform.SetParent(anim.GetBoneTransform(HeldBone));
            WeaponModel.transform.localPosition = HeldPos;
            WeaponModel.transform.localRotation = Quaternion.Euler(HeldRot);

        }
        public void StoreWeapon(Animator anim) {
            WeaponModel.transform.parent = anim.GetBoneTransform(EquipBone);
            WeaponModel.transform.localPosition = SheathedPos;
            WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
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