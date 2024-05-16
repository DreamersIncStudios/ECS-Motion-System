using System;
using System.Collections.Generic;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using Stats.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    [Serializable]
    public class WeaponSpellSO : SpellSO, IWeapon
    {
        #region Variables

        [SerializeField] private int comboDataIndex;
        [SerializeField] GameObject model;
        public GameObject Model => model;

        [Header("Weapon Info")] [SerializeField]
        private HumanBodyBones equipBone;

        public HumanBodyBones EquipBone => equipBone;
        [SerializeField] private List<StatModifier> modifiers;
        public List<StatModifier> Modifiers => modifiers;

        [SerializeField] private WeaponType weaponType;
        [SerializeField] TypeOfDamage typeOfDamage;
        [SerializeField] private HumanBodyBones heldBone;
        public HumanBodyBones HeldBone => heldBone;
        public WeaponType WeaponType => weaponType;
        [SerializeField] private WeaponSlot slot;
        public WeaponSlot Slot => slot;

        [SerializeField] private float maxDurable;
        public float MaxDurability => maxDurable;
        public float CurrentDurability { get; set; }
        [SerializeField] private bool breakable;
        public bool Breakable => true; // Breaks once Caster runs out of mana, Mana used on attack 
        public bool Upgradeable => false; // Might change later
        public int SkillPoints { get; set; }
        public int Experience { get; set; }
        public GameObject WeaponModel { get; set; }

        [SerializeField] Vector3 heldPos;
        public Vector3 SheathedPos => sheathedPos;
        [SerializeField] private Vector3 sheathedPos;  
        public Vector3 HeldPos => heldPos;

        [SerializeField] Vector3 _sheathedRot;
        public Vector3 SheathedRot => _sheathedRot;


        [SerializeField] Vector3 _heldRot;
        public Vector3 HeldRot => _heldRot;
        [SerializeField] Vector3 styleHeldPost;
        public Vector3 StyleHeldPost => styleHeldPost;

        [SerializeField] Vector3 styleHeldRot;

        public Vector3 StyleHeldRot => styleHeldRot;


        #endregion

        public override void Activate(SpellBookSO spellBookSo, BaseCharacterComponent player)
        {
            spellBookSo.CurHeldPos = HeldPos;
            spellBookSo.CurHeldRot = HeldRot;
            spellBookSo.CurSheathedPos = SheathedPos;
            spellBookSo.CurSheathedRot = SheathedRot;
            spellBookSo.CurComboID = comboDataIndex;
            spellBookSo.WeaponModel= WeaponModel = Instantiate(Model);
            var anim = player.GOrepresentative.GetComponent<Animator>();
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

        public bool EquipToHuman;

        public override void Use(CharacterInventory characterInventory, BaseCharacter player)
        {
            throw new System.NotImplementedException();
        }
    }
}