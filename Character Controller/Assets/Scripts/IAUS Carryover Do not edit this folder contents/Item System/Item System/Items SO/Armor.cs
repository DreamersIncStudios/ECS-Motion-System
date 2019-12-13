using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using UnityEditor;


namespace ItemSystem {

    public class EquipableItem : BaseItem {
        public uint LevelReq;

        public virtual void OnEquip(PlayerCharacter PC, ArmorType Check)
        {
            if (PC.Level < LevelReq) {

                        return;
            }
        }
        public virtual void OnUnequip(PlayerCharacter PC) { }
    }


   
    public class Armor : EquipableItem, IArmor,IRendered
    {
        [SerializeField] int _speedMod;
        [SerializeField] int _resistanceMod;
        [SerializeField] int _vitalityMod;
        [SerializeField] int _charismaMod;
        [SerializeField] int _awarenessMod;
      [SerializeField] ArmorType _armorType;
        [SerializeField] GameObject _go;
        [SerializeField] HumanBodyBones _spawnBone;
        [SerializeField] Vector3 _pos;
        [SerializeField] Vector3 _rot;
        int ModelID;

        public int SpeedMod { get { return _speedMod;} set { _speedMod = value; } }
        public int ResistanceMod { get { return _resistanceMod; } }
        public int VitalityMod { get { return _vitalityMod; } }
        public int CharismaMod { get { return _charismaMod; } }
        public int Awareness { get { return _awarenessMod; } }
        public ArmorType armor { get { return _armorType; } }

        public GameObject GO { get { return _go; } }
        public HumanBodyBones SpawnBone { get {
                                switch (_armorType)
                {
                    case ArmorType.Helmet:
                        _spawnBone = HumanBodyBones.Head;
                        break;
                    case ArmorType.Chest:
                        _spawnBone = HumanBodyBones.Chest;
                        break;
                    case ArmorType.Gloves:
                        _spawnBone = HumanBodyBones.RightHand;
                        break;
                    case ArmorType.Boots:
                        _spawnBone = HumanBodyBones.RightFoot;
                        break;
                    case ArmorType.Shield:
                        _spawnBone = HumanBodyBones.LeftHand;
                        break;
                }


               return _spawnBone; } }
        public Vector3 Position { get { return _pos; } }
        public Vector3 Rotation { get { return _rot; } }
    

        public override void OnEquip(PlayerCharacter PC, ArmorType Check )
        {
            Equipment E = PC.gameObject.GetComponent<Equipment>();
            if (Check != armor) {
                Debug.Log(" Can not equip");  // This should be replaced or made in valid by UI check
                switch (Check)
                {
                    case (ArmorType.Helmet):
                        E.Helmet = null;
                        break;
                    case (ArmorType.Chest):
                        E.Chest = null;
                        break;
                    case (ArmorType.Gloves):
                        E.Gloves = null;
                        break;
                    case (ArmorType.Boots):
                        E.Boots = null;
                        break;
                    case (ArmorType.Shield):
                        E.Shield = null;
                        break;
                }

                return;
            }
            base.OnEquip(PC, Check);
            PC.GetPrimaryAttribute((int)AttributeName.Speed).BuffValue += SpeedMod;
            PC.GetPrimaryAttribute((int)AttributeName.Resistance).BuffValue += ResistanceMod;
            PC.GetPrimaryAttribute((int)AttributeName.Vitality).BuffValue += VitalityMod;
            PC.GetPrimaryAttribute((int)AttributeName.Charisma).BuffValue += CharismaMod;
            PC.GetPrimaryAttribute((int)AttributeName.Awareness).BuffValue += _awarenessMod;
            // Add equipment Inventory Slot ... This code might be made null and void by UI Code
     //Instantiate model
            switch (_armorType) {
                case (ArmorType.Helmet):
                    break;
                case (ArmorType.Chest):
                    break;
                case (ArmorType.Gloves):
                    break;
                case (ArmorType.Boots):
                    break;
                case (ArmorType.Shield):
                    break;
            }

  
            PC.StatUpdate();

            //To Be Uncommented Later once Models are being used

            //Animator Anim = PC.GetComponent<Animator>();
            //GameObject Arm = Instantiate(GO, Anim.GetBoneTransform(SpawnBone));
            //Arm.transform.position = _rot;
            //Arm.transform.rotation = Quaternion.Euler(Rotation);
            //ModelID = Arm.gameObject.GetInstanceID();

        }
        public override void OnUnequip(PlayerCharacter PC)
        {
            PC.GetPrimaryAttribute((int)AttributeName.Speed).BuffValue -= SpeedMod;
            PC.GetPrimaryAttribute((int)AttributeName.Resistance).BuffValue -= ResistanceMod;
            PC.GetPrimaryAttribute((int)AttributeName.Vitality).BuffValue -= VitalityMod;
            PC.GetPrimaryAttribute((int)AttributeName.Charisma).BuffValue -= CharismaMod;
            PC.GetPrimaryAttribute((int)AttributeName.Awareness).BuffValue -= _awarenessMod;

            // Add equipment Inventory Slot ... This code might be made null and void by UI Code

            switch (_armorType)
            {
                case (ArmorType.Helmet):
                    break;
                case (ArmorType.Chest):
                    break;
                case (ArmorType.Gloves):
                    break;
                case (ArmorType.Boots):
                    break;
                case (ArmorType.Shield):
                    break;
            }
            //Destory Model
            PC.StatUpdate();
            //To Be Uncommented Later once Models are being used
            //GameObject Del = (GameObject)EditorUtility.InstanceIDToObject(ModelID);
            //Destroy(Del);
        }
    }
}