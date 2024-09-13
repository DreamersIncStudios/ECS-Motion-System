using System;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using System.Linq;
using DreamersInc.DamageSystem;
using Stats.Entities;
using DreamersInc.DamageSystem.Interfaces;
using Newtonsoft.Json;
using VisualEffect;

namespace Dreamers.InventorySystem
{
    public class WeaponSO : ItemBaseSO, IEquipable, IWeapon
    {
        #region Variables
        public new ItemType Type => ItemType.Weapon;
        [SerializeField] Quality quality;
        public Quality Quality => quality;

        [SerializeField] GameObject model;
        public GameObject Model => model;
        [SerializeField] private bool equipToHuman;
        public bool EquipToHuman => equipToHuman;
        [SerializeField] private HumanBodyBones heldBone;
        public HumanBodyBones HeldBone => heldBone;
        public bool Equipped { get; private set; }

        [SerializeField] private HumanBodyBones equipBone;
        public HumanBodyBones EquipBone => equipBone;
        [SerializeField] private List<AttributeModifier> modifiers;
        public List<AttributeModifier> Modifiers => modifiers;

        [SerializeField] private uint levelRqd;
        public uint LevelRqd => levelRqd;

        [SerializeField] private WeaponType weaponType;
        [SerializeField] TypeOfDamage typeOfDamage;
        public WeaponType WeaponType => weaponType;
        [SerializeField] private WeaponSlot slot;
        public WeaponSlot Slot => slot;
        [SerializeField] private float maxDurable;
        public float MaxDurability => maxDurable;
        public float CurrentDurability { get; set; }
        [SerializeField] private bool breakable;
        public bool Breakable => breakable;
        [SerializeField] private bool upgradeable;
        public bool Upgradeable => upgradeable;

        public bool AlwaysDrawn => alwaysDrawn;
        [SerializeField] bool alwaysDrawn;

        public int SkillPoints { get; set; }
        public int Experience { get; set; }
        [SerializeField] private Vector3 sheathedPos;
        public Vector3 SheathedPos => sheathedPos;
        [SerializeField] private Vector3 heldPos;
        public Vector3 HeldPos => heldPos;
        [SerializeField] private Vector3 sheathedRot;
        public Vector3 SheathedRot => sheathedRot;
        [SerializeField] private Vector3 heldRot;
        public Vector3 HeldRot => heldRot;
        [SerializeField] private Vector3 styleHeldPost;
        public Vector3 StyleHeldPost => styleHeldPost;
        [SerializeField] Vector3 styleHeldRot;
        public Vector3 StyleHeldRot => styleHeldRot;

        public List<Effects> PermWeaponEffects => permWeaponEffects;
        [SerializeField]
        private List<Effects> permWeaponEffects;
        
        public List<Effects> AdderWeaponEffect { get; private set; }
        [Range(0, 10)] public int MaxNumberOfEffects;
        [SerializeField] public ModifierSpellSO BaseModelState;

        public SpellSO ActiveSpell { get; private protected set; }

        #endregion


        public GameObject WeaponModel { get; set; }

        public class OnStatusEffectChangeArgs : EventArgs
        {
            public readonly bool Add;
            public Effects EffectChange;

            public OnStatusEffectChangeArgs(Effects effectChange, bool add = true)
            {
                EffectChange = effectChange;
                Add = add;
            }
        }

        public event EventHandler<OnStatusEffectChangeArgs> OnStatusEffectChange;

        public virtual bool Equip(BaseCharacterComponent player)
        {
            OnStatusEffectChange += (_, eventArgs) =>
            {
                if (eventArgs.Add)
                {
                    WeaponModel.GetComponent<WeaponDamage>().UpdateEffect(eventArgs.EffectChange);
                }else
                    WeaponModel.GetComponent<WeaponDamage>().UpdateEffect(eventArgs.EffectChange,true);
                    
            };
            
            var anim = player.GORepresentative.GetComponent<Animator>();
            if (player.Level >= LevelRqd)
            {
                AdderWeaponEffect = new List<Effects>();
                if (Model != null)
                {
                    WeaponModel = Instantiate(Model);
                    WeaponModel.GetComponent<IDamageDealer>().SetStatData(player, typeOfDamage);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = anim.GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            WeaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                        WeaponModel.transform.SetParent(anim.transform);
                    }
                    WeaponModel.transform.localPosition = SheathedPos;
                    WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
                }
                else
                {
                    WeaponModel = new GameObject();
                    WeaponModel.AddComponent<WeaponDamage>();
                    WeaponModel.transform.SetParent(anim.transform);
                }

                player.ModCharacterAttributes(Modifiers);
                if(alwaysDrawn )
                {
                    anim.SendMessage("EquipWeaponAnim");
                    DrawWeapon(anim);
                    if(EquipToHuman)
                        WeaponModel.AddComponent<DissolveSingle>();
                }

                foreach (var permEffect in PermWeaponEffects)
                {
                    OnStatusEffectChange?.Invoke(this, new OnStatusEffectChangeArgs(permEffect));
                }

                return Equipped = true; 
            }
            else
            {
                Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level);
                return Equipped = false;
            }
        }



        //TODO Should this be a bool instead of Void

        /// <summary>
        /// Equip Item in Inventory to Another Character
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool EquipItem(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            EquipmentBase equipment = characterInventory.Equipment;
            var anim = player.GORepresentative.GetComponent<Animator>();

            if (player.Level >= LevelRqd)
            {
                if (equipment.EquippedWeapons.TryGetValue(this.Slot, out _))
                {
                    equipment.EquippedWeapons[this.Slot].Unequip(characterInventory, player);
                }
                equipment.EquippedWeapons[this.Slot] = this;


                if (Model != null)
                {
                    WeaponModel = Instantiate(Model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = anim.GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            WeaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                        WeaponModel.transform.SetParent(anim.transform);

                    }
                    WeaponModel.transform.localPosition = SheathedPos;
                    WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);

                }  
                else
                {
                    WeaponModel = new GameObject();
                    WeaponModel.AddComponent<WeaponDamage>();
                    WeaponModel.transform.SetParent(anim.transform);
                }
                AdderWeaponEffect = new List<Effects>();

                player.ModCharacterAttributes(Modifiers);
                characterInventory.Inventory.RemoveFromInventory(this);
                if (alwaysDrawn)
                {
                    anim.SendMessage("EquipWeaponAnim");
                    DrawWeapon(anim);
                    WeaponModel.AddComponent<DissolveSingle>();
                }

                player.StatUpdate();
                return Equipped = true; 
            }
            else
            {
                Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level);
                return Equipped = false;
            }

        }

        /// <summary>
        /// Unequip item from character and return to target inventory
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Unequip(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            EquipmentBase equipment = characterInventory.Equipment;
            characterInventory.Inventory.AddToInventory(this);
            Destroy(WeaponModel);

            player.ModCharacterAttributes(Modifiers, false);
            equipment.EquippedWeapons.Remove(this.Slot);
            Equipped = false;
            return true; 
        }

        public override void Use(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            throw new NotImplementedException();
        }

        public void DrawWeapon(Animator anim)
        {
            if (!equipToHuman) return;
            WeaponModel.transform.SetParent(anim.GetBoneTransform(HeldBone));
            WeaponModel.transform.localPosition = HeldPos;
            WeaponModel.transform.localRotation = Quaternion.Euler(HeldRot);

        }
        public void StoreWeapon(Animator anim)
        {
            WeaponModel.transform.parent = anim.GetBoneTransform(EquipBone);
            WeaponModel.transform.localPosition = SheathedPos;
            WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
        }

        public virtual void StyleChange(bool check)
        {
            if (check)
            {
                WeaponModel.transform.localPosition = styleHeldPost;
                WeaponModel.transform.localRotation = Quaternion.Euler(styleHeldRot);
            }
            else
            {
                WeaponModel.transform.localPosition = HeldPos;
                WeaponModel.transform.localRotation = Quaternion.Euler(HeldRot);
            }
        }

        public bool SetEffect(Effects effect, bool overrideEffect = false)
        {
            if (overrideEffect)
            {
                foreach (var existingEffect in AdderWeaponEffect)
                {
                    OnStatusEffectChange?.Invoke(this, new OnStatusEffectChangeArgs(existingEffect,false));
                }
                AdderWeaponEffect = new List<Effects> { effect };
                OnStatusEffectChange?.Invoke(this, new OnStatusEffectChangeArgs(effect));

                return true;
            }

            if (AdderWeaponEffect.Count >= MaxNumberOfEffects)
            {
                return false;
            }

            AdderWeaponEffect.Add(effect);
            OnStatusEffectChange?.Invoke(this, new OnStatusEffectChangeArgs(effect));

            return true;
        }

        public bool SetEffect(List<Effects> effectsList, bool overrideEffect = false)
        {
            return effectsList.All(effect => SetEffect(effect));
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

            WeaponSO weapon = (WeaponSO)obj;

            return ItemID == weapon.ItemID && ItemName == weapon.ItemName && Value == weapon.Value && Modifiers.SequenceEqual(weapon.Modifiers) &&
                Experience == weapon.Experience && LevelRqd == weapon.LevelRqd;
        }
        public override string Serialize()
        {
            var serializeData = new SerializedWeaponData(itemID: ItemID, itemName: ItemName, description: Description,
                value: Value, type: Type, stackable: Stackable, questItem: QuestItem);
            string output = JsonConvert.SerializeObject(serializeData);


            return output;
        }
        
        class SerializedWeaponData : SerializedItemSO
        {     
            public SerializedWeaponData()
            {
            }

            public SerializedWeaponData(uint itemID, string itemName, string description, uint value, ItemType type, bool stackable, bool questItem)
            {
                ItemID = itemID;
                ItemName = itemName;
                Description = description;
                Value = value;
                Type = type;
                Stackable = stackable;
                QuestItem = questItem;
            }
        }
    }


}