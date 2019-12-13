using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats;


namespace ItemSystem {

    public class BaseItem : ScriptableObject, IBaseItem
    {

        [SerializeField] string _name;
        [SerializeField] Sprite icon;
        [SerializeField] string desc;
        [SerializeField] ItemType Type;
        [SerializeField] bool consumable;
        [SerializeField] bool stackable;
        [SerializeField] bool CanBeStored;
        [SerializeField] uint _gold;
        [SerializeField] uint Size;
        [SerializeField] int _itemID;
        [SerializeField] Quality _quality;


        public string Name { get { return _name; } set { _name = value; } }
        public int ItemID { get { return _itemID; } set { _itemID = value; } }
        public uint Value { get { return _gold; } set { _gold = value; } }
        public Sprite Icon { get { return icon; } set { icon = value; } }
        public string Description { get { return desc; } set { desc = value; } }
        public ItemType itemType { get { return Type; } }
        public uint size { get { return Size; } set { Size = value; } }
        public Quality Quality { get { return _quality; } }

        public bool Consumable { get {
                if (Type == ItemType.Curative) {
                    consumable = true;
                }
                return consumable; } }
        public bool Stackable { get
            {
                if (Type == ItemType.Curative)
                {
                    stackable= true;
                }
                return stackable; } }
        public bool canBeStored { get { return CanBeStored; } }
      
    }
    public abstract class EquipItem : BaseItem, DamageStatsMods, IEquipment
    {
        [SerializeField] int[] AttMod = new int[10];
        [SerializeField] int[] ElmtMod = new int[6];

        //??? List<List<Spell>> Spell_Protocols;
        [SerializeField] uint level;
        [SerializeField] Quality quality;
        [SerializeField] uint xp;
        [SerializeField] uint manaLoad;
        [SerializeField] int reqdCPUCore;
        [SerializeField] int animSet;

        [SerializeField] bool equipped;


        public int[] AttributeMod { get { return AttMod; } set { AttMod = value; } }
        public int[] ElementalMod { get { return ElmtMod; } set { ElmtMod = value; } }
        public uint Level { get { return level; } set { level = value; } }
        public uint Exp { get { return xp; } set { xp = value; } }
        public Quality Quality { get { return quality; } }
        public uint ManaLoad { get { return manaLoad; } set { manaLoad = value; } }
        public int ReqdCPUCore { get { return reqdCPUCore; } set { reqdCPUCore = value; } }
        public int AnimSet { get { return animSet; } set { animSet = value; } }


        public bool Equipped { get { return equipped; } set { equipped = value; } }

        public virtual void OnEquip(PlayerCharacter Stats)
        {
            Debug.Log("Primary Stat change ran");
            Stats.GetPrimaryAttribute((int)AttributeName.Strength).BuffValue += AttMod[0];
            Stats.GetPrimaryAttribute((int)AttributeName.Vitality).BuffValue += AttMod[1];
            Stats.GetPrimaryAttribute((int)AttributeName.Awareness).BuffValue += AttMod[2];
            Stats.GetPrimaryAttribute((int)AttributeName.Speed).BuffValue += AttMod[3];
            Stats.GetPrimaryAttribute((int)AttributeName.Skill).BuffValue += AttMod[4];
            Stats.GetPrimaryAttribute((int)AttributeName.Resistance).BuffValue += AttMod[5];
            Stats.GetPrimaryAttribute((int)AttributeName.Concentration).BuffValue += AttMod[6];
            Stats.GetPrimaryAttribute((int)AttributeName.WillPower).BuffValue += AttMod[7];
            Stats.GetPrimaryAttribute((int)AttributeName.Charisma).BuffValue += AttMod[8];
            Stats.GetPrimaryAttribute((int)AttributeName.Luck).BuffValue += AttMod[9];
            equipped = true;
            Stats.StatUpdate();
            Stats.CurHealth = Stats.MaxHealth = Stats.GetVital((int)VitalName.Health).AdjustBaseValue;
            Stats.CurMana = Stats.MaxMana = Stats.GetVital((int)VitalName.Mana).AdjustBaseValue;


            // GOI.transform.parent = Stats.gameObject.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Spine);
            // GOI.transform.position = Position;

        }

        public virtual void Unequip(PlayerCharacter Stats)
        {
            Stats.GetPrimaryAttribute((int)AttributeName.Strength).BuffValue -= AttMod[0];
            Stats.GetPrimaryAttribute((int)AttributeName.Vitality).BuffValue -= AttMod[1];
            Stats.GetPrimaryAttribute((int)AttributeName.Awareness).BuffValue -= AttMod[2];
            Stats.GetPrimaryAttribute((int)AttributeName.Speed).BuffValue -= AttMod[3];
            Stats.GetPrimaryAttribute((int)AttributeName.Skill).BuffValue -= AttMod[4];
            Stats.GetPrimaryAttribute((int)AttributeName.Resistance).BuffValue -= AttMod[5];
            Stats.GetPrimaryAttribute((int)AttributeName.Concentration).BuffValue -= AttMod[6];
            Stats.GetPrimaryAttribute((int)AttributeName.WillPower).BuffValue -= AttMod[7];
            Stats.GetPrimaryAttribute((int)AttributeName.Charisma).BuffValue -= AttMod[8];
            Stats.GetPrimaryAttribute((int)AttributeName.Luck).BuffValue -= AttMod[9];
            equipped = false;
            Stats.StatUpdate();
            Stats.CurHealth = Stats.MaxHealth = Stats.GetVital((int)VitalName.Health).AdjustBaseValue;
            Stats.CurMana = Stats.MaxMana = Stats.GetVital((int)VitalName.Mana).AdjustBaseValue;
        }

    }


}
