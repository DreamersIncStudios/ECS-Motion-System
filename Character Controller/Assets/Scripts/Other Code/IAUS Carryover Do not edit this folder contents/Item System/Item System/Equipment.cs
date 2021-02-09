using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem;
namespace Stats
{
    public class Equipment : MonoBehaviour
    {
        PlayerCharacter PC;
        Animator anim;

        public Armor Helmet;
        public Armor Chest;
        public Armor Gloves;
        public Armor Boots;
        public Armor Shield;
        public Weapon Sword; //Need to rename for clarity
       // public Weapon Sword2; //Need to rename for clarity

        [HideInInspector] public GameObject HelmetGO;
        [HideInInspector] public GameObject ChestGO;
        [HideInInspector] public GameObject GlovesGO;
        [HideInInspector] public GameObject BootsGO;
        [HideInInspector] public GameObject ShieldGO;
        GameObject Wpn;

        private void Awake()
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();

            }

            if (PC == null)
            {
                PC = this.GetComponent<PlayerCharacter>();

            }
        }

        void Start()
        {

            //if (Helmet!=null) { Helmet.OnEquip(PC, ArmorType.Helmet); }
            //if (Chest != null) { Chest.OnEquip(PC, ArmorType.Chest); }
            //if (Gloves != null) { Gloves.OnEquip(PC, ArmorType.Gloves); }
            //if (Boots != null) { Boots.OnEquip(PC, ArmorType.Boots); }
            //if (Shield != null) { Shield.OnEquip(PC, ArmorType.Shield); }

            if (Sword != null)
            {
                Sword.OnEquip(PC, 0);
                Wpn = Sword.modelID;
            }
            
           // PC.StatUpdate();
        }

        private void Update()
        {

        }



        public void SetTrue()
        {
            anim.SetBool("CanDoDamage", true);
        }
        public void SetFalse()
        {
            anim.SetBool("CanDoDamage", false);
        }
        public void EquipWeaponAnim()
        {
            //   anim.SetBool("CanDoDamage", true);
            anim.SetBool("Weapon In Hand", true);
            Wpn.transform.SetParent( anim.GetBoneTransform(HumanBodyBones.RightHand));
            Wpn.transform.localPosition = Sword.HandPosition;
            Wpn.transform.localRotation = Quaternion.Euler(Sword.HandRotation);
        }

        public void UnequipWeaponAnim()
        {
            //   anim.SetBool("CanDoDamage", false);
            anim.SetBool("Weapon In Hand", false);
            Wpn.transform.parent = anim.GetBoneTransform(HumanBodyBones.Spine);
            Wpn.transform.localPosition = Sword.Position;
            Wpn.transform.localRotation = Quaternion.Euler(Sword.Rotation);
        }
    }
}