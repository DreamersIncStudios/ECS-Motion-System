using System.Collections.Generic;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using System.Linq;
using Stats.Entities;
using System;

namespace Dreamers.InventorySystem
{
    [Serializable]
    public class ArmorSO : ItemBaseSO, IEquipable, IArmor
    {
        #region variables
        [SerializeField] private Quality quality;
        public Quality Quality => quality;

        [SerializeField] private GameObject model;
        public GameObject Model => model;
        [SerializeField] private bool equipToHuman;
        public bool EquipToHuman => equipToHuman;
        public bool Equipped { get; private set; }

        [SerializeField] private HumanBodyBones equipBone;
        public HumanBodyBones EquipBone => equipBone;
        [SerializeField] private ArmorType armorType;
        public ArmorType ArmorType => armorType;
        [SerializeField] private uint levelRqd;
        public uint LevelRqd => levelRqd;

        [SerializeField] private List<AttributeModifier> modifiers;
        public List<AttributeModifier> Modifiers => modifiers;

        [SerializeField] private float maxDurable;
        public float MaxDurability => maxDurable;
        public float CurrentDurability { get; set; }
        [SerializeField] private bool breakable;
        public bool Breakable => breakable;
        [SerializeField] private bool upgradeable;
        public bool Upgradeable => upgradeable;

        public int SkillPoints { get; set; }
        public int Experience { get; set; }
        GameObject armorModel;

        public bool Equip(BaseCharacterComponent player)
        {
            var anim = player.GORepresentative.GetComponent<Animator>();

            if (player.Level >= LevelRqd)
            {
                if (Model != null)
                {
                    armorModel = model = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = anim.GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            armorModel.transform.SetParent(bone);
                        }

                    }
                    else
                    {
                        armorModel.transform.SetParent(anim.transform);

                    }

                }
                player.ModCharacterAttributes(Modifiers);
                return Equipped = true;
            }
            else
            {
                Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level);
                return Equipped = false;
            }
        }
 
        #endregion

        /// <summary>
        /// Equip Item in Inventory to Another Character
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public  bool EquipItem(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            EquipmentBase equipment = characterInventory.Equipment;
            var anim = player.GORepresentative.GetComponent<Animator>();

            if (player.Level >= LevelRqd)
            {
                if (equipment.EquippedArmor.TryGetValue(this.ArmorType, out _))
                {
                    equipment.EquippedArmor[this.ArmorType].Unequip(characterInventory, player);
                }
                equipment.EquippedArmor[this.ArmorType] = this;

                if (Model != null)
                {
                    armorModel = model = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        var bone =anim.GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            armorModel.transform.SetParent(bone);
                        }

                    }
                    else
                    {
                        armorModel.transform.SetParent(anim.transform);

                    }

                }
                player.ModCharacterAttributes( Modifiers);

                characterInventory.Inventory.RemoveFromInventory(this);
                player.StatUpdate();
                return Equipped = true;
            }
            else { Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level);
                return Equipped =false;
            }
        }
    

        /// <summary>
        /// Unequipped item from character and return to target inventory
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public  bool Unequip(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            EquipmentBase equipment = characterInventory.Equipment;
            characterInventory.Inventory.AddToInventory(this);
            Destroy(armorModel);
           player.ModCharacterAttributes( Modifiers, false);
            equipment.EquippedArmor.Remove(this.ArmorType);
            Equipped = false;
            return true;
        }

        /// <summary>
        /// Unequip item from self and return inventory
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public override void Use(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            throw new NotImplementedException();
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

            ArmorSO armor = (ArmorSO)obj;

            return ItemID == armor.ItemID  && ItemName == armor.ItemName && Value == armor.Value && Modifiers.SequenceEqual( armor.Modifiers) &&
                Experience == armor.Experience && LevelRqd == armor.LevelRqd;
        }



    }


    
}