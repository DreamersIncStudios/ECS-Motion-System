using System.Collections;
using System.Collections.Generic;
using Stats;
using UnityEngine;




namespace ItemSystem
{
    public interface IConjuredWeapon
    {
        Material Shader { get; }
        float RevealRate { get; }
        float DissolveDelay { get; }
    }

    public class Weapon : EquipableItem, IWeapon, IRendered
    {
        [SerializeField] WeaponStyle _weaponType;
        [SerializeField] Vector3 _handPosition;
        [SerializeField] Vector3 _handRotation;
        [SerializeField] int _strengthMod;
        [SerializeField] int _vitalityMod;
        [SerializeField] int _speedMod;
        [SerializeField] int _skillMod;
        [SerializeField] int _concentrationMod;

        [SerializeField] GameObject _go;
        [SerializeField] HumanBodyBones _spawnBone;
        [SerializeField] Vector3 _pos;
        [SerializeField] Vector3 _rot;
        [SerializeField] Vector3 ColCenter;
        [SerializeField] Vector3 ColSize;
   

        public GameObject GO { get { return _go; } }
        public HumanBodyBones SpawnBone
        {get { _spawnBone = HumanBodyBones.Spine;
                  return _spawnBone;            }        }

        public Vector3 Position { get { return _pos; } }
        public Vector3 Rotation { get { return _rot; } }

        public WeaponStyle WType { get { return _weaponType; } }

        public Vector3 HandPosition { get { return _handPosition; } }

        public Vector3 HandRotation { get { return _handRotation; } }

        public int StrengthMod { get { return _strengthMod; } }

        public int VitalityMod        {          get { return _vitalityMod; } }
        public int SpeedMod { get { return _speedMod; } }

        public int SkillMod { get { return _skillMod; } }

        public int ConcentrationMod { get { return _concentrationMod; } }
        public GameObject modelID { get; set; } 
        public override void OnEquip(PlayerCharacter PC, ArmorType Check)
        {
            base.OnEquip(PC, Check);
            Animator Anim =PC.GetComponent<Animator>();
       
            GameObject Wpn= modelID = Instantiate(GO, Anim.GetBoneTransform(SpawnBone));

            Wpn.transform.localPosition= _pos;
            Wpn.transform.localRotation = Quaternion.Euler(Rotation);
            BoxCollider Col =Wpn.AddComponent<BoxCollider>();
            Col.isTrigger=true;
            Col.center = ColCenter;
            Col.size = ColSize;

          
          

        }
        public override void OnUnequip(PlayerCharacter PC)
        {
            base.OnUnequip(PC);
     
            Destroy(modelID);
        }



    }
}