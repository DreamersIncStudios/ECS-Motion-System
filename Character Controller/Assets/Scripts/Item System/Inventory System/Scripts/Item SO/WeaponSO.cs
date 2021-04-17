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
        #endregion


        public GameObject weaponModel { get; set; }
        public override void EquipItem(CharacterInventory characterInventory, int IndexOf,BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
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

                }
                        EquipmentUtility.ModCharacterStats(player,Modifiers, true);

                if (Equipment.EquippedWeapons.TryGetValue(this.Slot, out WeaponSO value))
                {
                    Equipment.EquippedWeapons[this.Slot].Unequip(characterInventory, player);
                }
                Equipment.EquippedWeapons[this.Slot] = this;

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