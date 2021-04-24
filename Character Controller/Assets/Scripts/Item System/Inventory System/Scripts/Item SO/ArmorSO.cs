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

    public class ArmorSO : ItemBaseSO, IEquipable, IArmor
    {
        #region variables
        [SerializeField] Quality quality;
        public Quality Quality { get { return quality; } }

        [SerializeField] private GameObject _model;
        public GameObject Model { get { return _model; } }
        [SerializeField] private bool _equipToHuman;
        public bool EquipToHuman { get { return _equipToHuman; } }
        public bool Equipped { get; private set; }

        [SerializeField] private HumanBodyBones _equipBone;
        public HumanBodyBones EquipBone { get { return _equipBone; } }
        [SerializeField] private ArmorType _armorType;
        public ArmorType ArmorType { get { return _armorType; } }
        [SerializeField] private uint _levelRqd;
        public uint LevelRqd { get { return _levelRqd; } }

        [SerializeField] private List<StatModifier> _modifiers;
        public List<StatModifier> Modifiers { get { return _modifiers; } }

        [SerializeField] private float maxDurable;
        public float MaxDurability { get { return maxDurable; } }
        public float CurrentDurablity { get; set; }
        [SerializeField] private bool breakable;
        public bool Breakable { get { return breakable; } }
        [SerializeField] private bool _upgradeable;
        public bool Upgradeable { get { return _upgradeable; } }

        public int SkillPoints { get; set; }
        public int Exprience { get; set; }
        GameObject armorModel;

        public void Equip(BaseCharacter player)
        {
            if (player.Level >= LevelRqd)
            {
                if (Model != null)
                {
                    armorModel = _model = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = player.GetComponent<Animator>().GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            armorModel.transform.SetParent(bone);
                        }

                    }
                    else
                    {
                        armorModel.transform.SetParent(player.transform);

                    }

                }
                EquipmentUtility.ModCharacterStats(player, Modifiers, true);

            }
        }

        #endregion
        public override void EquipItem(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
            if (Equipment.EquippedArmor.TryGetValue(this.ArmorType, out ArmorSO value))
            {
                Equipment.EquippedArmor[this.ArmorType].Unequip(characterInventory, player);
            }
            Equipment.EquippedArmor[this.ArmorType] = this;

            if (player.Level >= LevelRqd)
            {
                if (Model != null)
                {
                    armorModel=_model = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = player.GetComponent<Animator>().GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            armorModel.transform.SetParent(bone);
                        }

                    }
                    else {
                        armorModel.transform.SetParent(player.transform);
                    
                    }

                }
               EquipmentUtility.ModCharacterStats(player, Modifiers, true);

                RemoveFromInventory(characterInventory, IndexOf);
                Equipped = true;
                player.StatUpdate();
            }
            else { Debug.LogWarning("Level required to Equip is " + LevelRqd +". Character is currently level "+ player.Level); }
        }

        public override void Unequip(CharacterInventory characterInventory, BaseCharacter player)
        {
            EquipmentBase Equipment = characterInventory.Equipment;
            AddToInventory(characterInventory);
            Destroy(armorModel);
           EquipmentUtility.ModCharacterStats(player, Modifiers, false);
            Equipment.EquippedArmor.Remove(this.ArmorType);
        }
        public override void Convert(Entity entity, EntityManager dstManager)
        { }




        public override void Use(CharacterInventory characterInventory, int IndexOf, BaseCharacter player)
        {
            throw new System.NotImplementedException();
        }



        // override object.Equals
        public  bool Equals(ItemBaseSO obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj.Type != Type)
                return false;

            // TODO: write your implementation of Equals() here

            ArmorSO Armor = (ArmorSO)obj;

            return ItemID == Armor.ItemID  && ItemName == Armor.ItemName && Value == Armor.Value && Modifiers.SequenceEqual( Armor.Modifiers) &&
                Exprience == Armor.Exprience && LevelRqd == Armor.LevelRqd;
        }



    }


    
}